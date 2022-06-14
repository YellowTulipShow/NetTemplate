using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using CommandParameterParse;
using CommandParameterParse.TypeHandles;
using CommandParameterParse.ParameterFormatHandles;

using System.CommandLine;

using YTS.Log;
using System.Collections.Generic;

namespace TranslationTemplateCommand
{
    public class MainHelpr
    {
        private readonly ILog log;

        public MainHelpr(ILog log)
        {
            this.log = log;
        }

        public void OnExecute(DirectoryInfo rootDire, IDictionary<string, string> datas, TemplateOutputConfig templateOutputConfig)
        {
            throw new NotImplementedException();
        }
        public void OnExecute(DirectoryInfo rootDire, IDictionary<string, string> datas, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
