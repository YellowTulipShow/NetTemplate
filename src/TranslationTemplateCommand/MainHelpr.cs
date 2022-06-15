using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Fluid;

using YTS.Log;

namespace TranslationTemplateCommand
{
    public class MainHelpr
    {
        private readonly ILog log;
        private readonly Encoding encoding;
        private readonly FluidParser parser;
        private readonly IDictionary<string, IFluidTemplate> cache_FluidTemplate;
        private readonly IDictionary<string, dynamic> cache_DataJSON;

        public MainHelpr(ILog log)
        {
            this.log = log;
            encoding = Encoding.UTF8;
            parser = new FluidParser();
            cache_FluidTemplate = new Dictionary<string, IFluidTemplate>();
            cache_DataJSON = new Dictionary<string, dynamic>();
        }

        private IFluidTemplate ToFluidTemplate(FileInfo templateFile)
        {
            if (cache_FluidTemplate.ContainsKey(templateFile.FullName))
            {
                return cache_FluidTemplate[templateFile.FullName];
            }
            var logArgs = log.CreateArgDictionary();
            logArgs["templateFile.FullName"] = templateFile.FullName;
            string template_content = File.ReadAllText(templateFile.FullName, encoding);
            if (!parser.TryParse(template_content, out IFluidTemplate template, out string error))
            {
                logArgs["FluidParser.ErrorMessage"] = error;
                log.Error("调用: FluidParser 解释模板与数据失败", logArgs);
                throw new ArgumentException($"调用: FluidParser 解释模板与数据失败: {templateFile.FullName}");
            }
            cache_FluidTemplate[templateFile.FullName] = template;
            return template;
        }

        private IDictionary<string, dynamic> ToModelDatas(string[] datas, DirectoryInfo rootDire)
        {
            var logArgs = log.CreateArgDictionary();
            logArgs["datas"] = datas;
            logArgs["rootDire.FullName"] = rootDire.FullName;

            Regex dataLineRegex = new Regex(@"^([a-z]+):([^:\s]+\.json)$",
                RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            IDictionary<string, dynamic> modelconfigs = new Dictionary<string, dynamic>();
            for (int index_data = 0; index_data < datas.Length; index_data++)
            {
                string dataLine = datas[index_data];
                logArgs["dataLine"] = dataLine;
                logArgs["index_data"] = index_data;
                Match dataLineMatch = dataLineRegex.Match(dataLine);
                if (!dataLineMatch.Success)
                {
                    log.Error($"JSON数据文件路径无法识别", logArgs);
                    throw new ArgumentException($"JSON数据文件路径无法识别: {dataLine}");
                }
                string key = dataLineMatch.Groups[1].Value;
                logArgs["key"] = key;
                string path = dataLineMatch.Groups[2].Value;
                logArgs["path"] = path;
                if (cache_DataJSON.ContainsKey(path))
                {
                    modelconfigs[key] = cache_DataJSON[path];
                    continue;
                }
                FileInfo value_file = ToFile(path, rootDire.FullName);
                string value_content = File.ReadAllText(value_file.FullName, encoding);
                dynamic model = JsonConvert.DeserializeObject<dynamic>(value_content);
                if (model == null)
                {
                    log.Error("获取JSON数据为空", logArgs);
                    throw new FileNotFoundException($"JSON数据文件路径无法识别: {dataLine}");
                }
                modelconfigs[key] = model;
                cache_DataJSON[path] = model;
            }
            return modelconfigs;
        }

        private FileInfo ToFile(string path, string root = null)
        {
            path = path?.Trim();
            root = root?.Trim();
            FileInfo info;
            if (Path.IsPathRooted(path))
            {
                info = new FileInfo(path);
            }
            else
            {
                path = Path.Combine(root, path);
                info = new FileInfo(path);
            }
            DirectoryInfo dire = info.Directory;
            if (!dire.Exists)
            {
                dire.Create();
            }
            return info;
        }

        public void OnExecute(DirectoryInfo rootDire, string templatePath, string outputPath, string[] datas)
        {
            var logArgs = log.CreateArgDictionary();
            logArgs["rootDire.FullName"] = rootDire.FullName;
            logArgs["datas"] = datas;
            logArgs["templatePath"] = templatePath;
            logArgs["outputPath"] = outputPath;

            FileInfo templateFile = ToFile(templatePath, rootDire.FullName);
            if (!templateFile.Exists)
            {
                log.Error("模板文件查找失败!", logArgs);
                throw new FileNotFoundException($"模板文件查找失败: {templateFile.FullName}");
            }
            FileInfo traget_file = ToFile(outputPath, rootDire.FullName);
            if (!traget_file.Exists)
            {
                traget_file.Create().Close();
            }
            IFluidTemplate fluidTemplate = ToFluidTemplate(templateFile);
            if (fluidTemplate == null)
            {
                log.Error("模板编译结果(FluidTemplate) 获取为空!", logArgs);
                throw new ArgumentNullException($"模板编译结果(FluidTemplate) 获取为空: {templateFile.FullName}");
            }
            object dataAggregate = ToModelDatas(datas, rootDire);
            if (dataAggregate == null)
            {
                log.Error("数据集合(dataAggregate) 获取为空!", logArgs);
                throw new ArgumentNullException($"数据集合(dataAggregate) 获取为空!");
            }
            var context = new TemplateContext(dataAggregate);
            string result_content = fluidTemplate.Render(context);
            File.WriteAllText(traget_file.FullName, result_content, encoding);
            log.Info($"Generate File Success: {traget_file.FullName}");
        }

        public void OnExecute(DirectoryInfo rootDire, string configPath)
        {
            var logArgs = log.CreateArgDictionary();
            logArgs["rootDire.FullName"] = rootDire.FullName;
            logArgs["configPath"] = configPath;

            FileInfo configFile = ToFile(configPath, rootDire.FullName);
            logArgs["configFile"] = configFile.FullName;
            if (!configFile.Exists)
            {
                log.Error("配置文件查找失败!", logArgs);
                throw new FileNotFoundException($"配置文件查找失败: {configFile.FullName}");
            }
            Regex lineRegex = new Regex(@"^([^\|\s]+\.liquid)\s+\|\s+([^\|\s]+\.[a-z]+)\s+\|\s+([^\|]+)$",
                RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
            logArgs["lineRegex"] = lineRegex.ToString();
            string[] lines = File.ReadAllLines(configFile.FullName, encoding);
            for (int index_line = 0; index_line < lines.Length; index_line++)
            {
                string line = lines[index_line];
                logArgs["line"] = line;
                logArgs["index_line"] = index_line;
                Match lineMatch = lineRegex.Match(line);
                if (!lineMatch.Success)
                {
                    log.Error("单行无法识别!", logArgs);
                    throw new ArgumentException($"单行无法识别: {line}");
                }
                string templatePath = lineMatch.Groups[1].Value;
                logArgs["templatePath"] = line;
                string outputPath = lineMatch.Groups[2].Value;
                logArgs["outputPath"] = line;
                string dataPathSetString = lineMatch.Groups[3].Value;
                logArgs["dataSetString"] = line;
                string[] dataPathSet = Regex.Split(dataPathSetString, @"\s+");
                OnExecute(rootDire, templatePath, outputPath, dataPathSet);
            }
        }
    }
}
