using System;
using System.Text;

using YTS.Log;

namespace TranslationTemplateCommand
{
    public class Program
    {
        static int Main(string[] args)
        {
            Encoding encoding = Encoding.UTF8;

            var logFile = ILogExtend.GetLogFilePath("Program");
            ILog log = new FilePrintLog(logFile, encoding).Connect(new ConsolePrintLog());
            var logArgs = log.CreateArgDictionary();
            logArgs["CommandInputArgs"] = args;

            try
            {
                MainHelpr mainHelpr = new MainHelpr(log);
                CommandArgsParser commandArgsParser = new CommandArgsParser(log, mainHelpr);
                return commandArgsParser.OnParser(args);
            }
            catch (Exception ex)
            {
                log.Error("执行程序出错!", ex, logArgs);
                return 1;
            }
        }
    }
}
