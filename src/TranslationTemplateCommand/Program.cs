using System;
using System.IO;
using System.Text;

using CommandParameterParse;
using CommandParameterParse.TypeHandles;
using CommandParameterParse.ParameterFormatHandles;

using YTS.Log;

namespace TranslationTemplateCommand
{
    public class Program : ICommandParseHelpPrint
    {
        private static string GetLogFilePath(string name)
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"logs/{name}/{DateTime.Now:yyyy_MM_dd}.log");
        }

        static void Main(string[] args)
        {
            Encoding encoding = Encoding.UTF8;

            var logFile = new FileInfo(GetLogFilePath("TranslationTemplateCommand"));
            ILog log = new FilePrintLog(logFile.FullName, encoding)
                .Connect(new ConsolePrintLog());
            var logArgs = log.CreateArgDictionary();
            logArgs["CommandInputArgs"] = args;

            try
            {
                ICommandParseHelpPrint helpPrint = new Program();
                ICommandParse<ConvertUtilsExecuteArgs> commandParse = new CommandParse<ConvertUtilsExecuteArgs>(helpPrint);
                commandParse.RegisterITypeHandle(new TypeHandle_IDictionaryStringJoinString());
                commandParse.RegisterIParameterFormatHandle(new KeyValueParameterFormatHandle());
                commandParse.OnExecute(args, m =>
                {
                    var util = new ConvertUtils(log, encoding);
                    util.OnExecut(m);
                });
            }
            catch (Exception ex)
            {
                log.Error("解释命令行参数出错!", ex, logArgs);
            }
        }

        public void Prints(string[] help_content)
        {
            foreach (string line in help_content)
            {
                Console.WriteLine(line);
            }
        }
    }
}
