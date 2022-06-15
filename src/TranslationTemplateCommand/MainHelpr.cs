using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using YTS.Log;
using Newtonsoft.Json;
using Fluid;
using System.Text.RegularExpressions;

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
                return null;
            }
            cache_FluidTemplate[templateFile.FullName] = template;
            return template;
        }

        private IDictionary<string, dynamic> ToModelDatas(IDictionary<string, string> datas, DirectoryInfo rootDire)
        {
            var logArgs = log.CreateArgDictionary();
            logArgs["rootDire.FullName"] = rootDire.FullName;
            IDictionary<string, dynamic> modelconfigs = new Dictionary<string, dynamic>();
            foreach (string key in datas.Keys)
            {
                logArgs["key"] = key;
                string path = datas[key];
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
                    continue;
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

        public void OnExecute(DirectoryInfo rootDire, string templatePath, string outputPath, IDictionary<string, string> datas)
        {
            var logArgs = log.CreateArgDictionary();
            logArgs["rootDire.FullName"] = rootDire.FullName;
            logArgs["datas"] = datas;
            logArgs["templatePath"] = templatePath;
            logArgs["outputPath"] = outputPath;
            try
            {
                FileInfo templateFile = ToFile(templatePath, rootDire.FullName);
                if (!templateFile.Exists)
                {
                    log.Error("模板文件查找失败!", logArgs);
                    return;
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
                    return;
                }
                object dataAggregate = ToModelDatas(datas, rootDire);
                if (dataAggregate == null)
                {
                    log.Error("数据集合(dataAggregate) 获取为空!", logArgs);
                    return;
                }
                //string model_json = JsonConvert.SerializeObject(modelconfigs);
                //dynamic model = JsonConvert.DeserializeObject<dynamic>(model_json);

                var context = new TemplateContext(dataAggregate);
                string result_content = fluidTemplate.Render(context);
                File.WriteAllText(traget_file.FullName, result_content, encoding);
                log.Info($"Generate File Success: {traget_file.FullName}");
            }
            catch (Exception ex)
            {
                log.Error("单条执行转换出错!", ex, logArgs);
            }
        }

        public void OnExecute(DirectoryInfo rootDire, string configPath)
        {
            var logArgs = log.CreateArgDictionary();
            logArgs["rootDire.FullName"] = rootDire.FullName;
            logArgs["configPath"] = configPath;
            try
            {
                FileInfo configFile = ToFile(configPath, rootDire.FullName);
                logArgs["configFile"] = configFile;
                if (!configFile.Exists)
                {
                    log.Error("配置文件查找失败!", logArgs);
                    return;
                }
                Regex lineRegex = new Regex(@"^([^\|\s]+\.liquid)\s+\|\s+([^\|\s]+\.[a-z]+)\s+\|([^\|\s]+)$",
                    RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
                Regex dataPathRegex = new Regex(@"^(\w+):([^:\s]+.json)$",
                    RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
                logArgs["lineRegex"] = lineRegex.ToString();
                logArgs["dataPathRegex"] = dataPathRegex.ToString();
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
                        continue;
                    }
                    string templatePath = lineMatch.Groups[1].Value;
                    logArgs["templatePath"] = line;
                    string outputPath = lineMatch.Groups[2].Value;
                    logArgs["outputPath"] = line;
                    string dataPathSetString = lineMatch.Groups[3].Value;
                    logArgs["dataSetString"] = line;
                    string[] dataPathSet = Regex.Split(dataPathSetString, @"\s+");
                    IDictionary<string, string> datas = new Dictionary<string, string>();
                    for (int index_dataPath = 0; index_dataPath < dataPathSet.Length; index_dataPath++)
                    {
                        string dataPath = dataPathSet[index_dataPath];
                        logArgs["dataPath"] = dataPath;
                        logArgs["index_dataPath"] = index_dataPath;
                        Match dataPathMatch = dataPathRegex.Match(dataPath);
                        if (!dataPathMatch.Success)
                        {
                            log.Error("JSON数据文件路径无法识别!", logArgs);
                            continue;
                        }
                        string key = dataPathMatch.Groups[1].Value;
                        logArgs["dataPath.key"] = dataPath;
                        string path = dataPathMatch.Groups[2].Value;
                        logArgs["dataPath.path"] = index_dataPath;
                        datas[key] = path;
                    }
                    OnExecute(rootDire, templatePath, outputPath, datas);
                }
            }
            catch (Exception ex)
            {
                log.Error("批量模板执行转换出错!", ex, logArgs);
            }
        }
    }
}
