using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Fluid;

using Newtonsoft.Json;

using YTS.Log;

namespace TranslationTemplateCommand
{
    public class ConvertUtils
    {
        private readonly ILog log;
        private readonly Encoding encoding;
        private readonly FluidParser parser;
        private readonly IDictionary<string, IFluidTemplate> cache_FluidTemplate;

        public ConvertUtils(ILog log, Encoding encoding)
        {
            this.log = log;
            this.encoding = encoding;
            parser = new FluidParser();
            cache_FluidTemplate = new Dictionary<string, IFluidTemplate>();
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

        public void OnExecut(ConvertUtilsExecuteArgs m)
        {
            var logArgs = log.CreateArgDictionary();
            logArgs["Root"] = m.Root;
            logArgs["Template"] = m.Template;
            try
            {
                FileInfo template_file = ToFile(m.Template, m.Root);
                if (!template_file.Exists)
                {
                    log.Error("模板文件查找失败!", logArgs);
                    return;
                }
                IFluidTemplate fluidTemplate = ToFluidTemplate(template_file);


                string template_content = File.ReadAllText(template_file.FullName, encoding);
                logArgs["Output"] = m.Output;
                FileInfo traget_file = ToFile(m.Output, m.Root);

                logArgs["DataFilePaths"] = m.Data;

                var parser = new FluidParser();
                if (!parser.TryParse(template_content, out IFluidTemplate template, out string error))
                {
                    logArgs["FluidParser.ErrorMessage"] = error;
                    log.Error("调用: FluidParser 解释模板与数据失败", logArgs);
                    return;
                }
                var context = new TemplateContext(model);
                string result_content = template.Render(context);
                logArgs["Result"] = result_content;
                if (!traget_file.Exists)
                {
                    traget_file.Create().Close();
                }
                File.WriteAllText(traget_file.FullName, result_content, encoding);
                log.Info($"Generate File Success: {traget_file.FullName}");
            }
            catch (Exception ex)
            {
                log.Error("执行转换出错!", ex, logArgs);
            }
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
    }
}
