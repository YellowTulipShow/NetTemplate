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

        public ConvertUtils(ILog log, Encoding encoding)
        {
            this.log = log;
            this.encoding = encoding;
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

                string template_content = File.ReadAllText(template_file.FullName, encoding);
                logArgs["Output"] = m.Output;
                FileInfo traget_file = ToFile(m.Output, m.Root);

                logArgs["DataFilePaths"] = m.Data;
                IDictionary<string, dynamic> modelconfigs = new Dictionary<string, dynamic>();
                foreach (string key in m.Data.Keys)
                {
                    string value_path = m.Data[key];
                    FileInfo value_file = ToFile(value_path, m.Root);
                    string value_content = File.ReadAllText(value_file.FullName, encoding);
                    modelconfigs[key] = JsonConvert.DeserializeObject<dynamic>(value_content);
                }
                string model_json = JsonConvert.SerializeObject(modelconfigs);
                dynamic model = JsonConvert.DeserializeObject<dynamic>(model_json);
                logArgs["IsGetData"] = true;

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
