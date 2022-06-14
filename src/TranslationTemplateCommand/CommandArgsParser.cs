using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.CommandLine;

using YTS.Log;

namespace TranslationTemplateCommand
{
    public class CommandArgsParser
    {
        private readonly ILog log;
        private readonly MainHelpr mainHelpr;

        public CommandArgsParser(ILog log, MainHelpr mainHelpr)
        {
            this.log = log;
            this.mainHelpr = mainHelpr;
        }

        public int OnParser(string[] args)
        {
            Option<DirectoryInfo> rootDireOption = GetOption_RootDire();
            Option<IDictionary<string, string>> datasOption = GetOption_Datas();

            RootCommand rootC = new RootCommand("基于开源项目 Fluid 解释 .liquid 模板, 用于生成文件命令程序");
            rootC.AddGlobalOption(rootDireOption);
            rootC.AddGlobalOption(datasOption);

            Command singleC = GetCommand_Single(rootDireOption);
            rootC.AddCommand(singleC);
            Command batchC = GetCommand_Batch(rootDireOption);
            rootC.AddCommand(batchC);

            return rootC.Invoke(args);
        }

        private Option<DirectoryInfo> GetOption_RootDire()
        {
            return new Option<DirectoryInfo>(
                aliases: new string[] { "-r", "--root" },
                description: "根目录定义",
                isDefault: true,
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        result.ErrorMessage = "根目录定义是必填项!";
                        return null;
                    }
                    string direPath = result.Tokens.Single().Value;
                    if (!Directory.Exists(direPath))
                    {
                        result.ErrorMessage = $"获取到的根目录不存在: {direPath}";
                        return null;
                    }
                    return new DirectoryInfo(direPath);
                });
        }

        private Command GetCommand_Single(Option<DirectoryInfo> rootDireOption)
        {
            var datasOption = GetOption_Datas();
            var templateOption = GetOption_Template();
            var outputOption = GetOption_Output();

            Command c = new Command("single", "单模板生成输出");
            c.AddOption(datasOption);
            c.AddOption(outputOption);
            c.AddOption(outputOption);

            c.SetHandler((context) =>
            {
                var rootDire = context.ParseResult.GetValueForOption(rootDireOption);
                var datas = context.ParseResult.GetValueForOption(datasOption);
                var template = context.ParseResult.GetValueForOption(templateOption);
                var output = context.ParseResult.GetValueForOption(outputOption);
                mainHelpr.OnExecute(rootDire, datas, new TemplateOutputConfig()
                {
                    Template = template,
                    Output = output,
                });
            });
            return c;
        }
        private Option<IDictionary<string, string>> GetOption_Datas()
        {
            Regex lineRegex = new Regex(@"^(a-z):([^:]+\.json)$",
                RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            return new Option<IDictionary<string, string>>(
                name: "--data",
                description: $"数据内容定义, 单参数匹配正则: {lineRegex}",
                isDefault: true,
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        result.ErrorMessage = "数据内容定义不能为空!";
                        return null;
                    }
                    IDictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (var token in result.Tokens)
                    {
                        string line = token.Value;
                        Match match = lineRegex.Match(line);
                        if (!match.Success)
                        {
                            result.ErrorMessage = $"数据内容行参数无法识别: {line}";
                            return null;
                        }
                        string key = match.Groups[1].Value;
                        string value = match.Groups[2].Value;
                        dict[key] = value;
                    }
                    return dict;
                });
        }
        private Option<string> GetOption_Template()
        {
            return new Option<string>(
                aliases: new string[] { "-t", "--template" },
                description: "模板文件定义, 扩展名: .liquid",
                isDefault: true,
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        result.ErrorMessage = "模板文件定义需要传入值!";
                        return null;
                    }
                    string path = result.Tokens.Single().Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        result.ErrorMessage = "模板文件定义获取为空!";
                        return null;
                    }
                    if (!Regex.IsMatch(path, @"\.liquid$",
                        RegexOptions.ECMAScript | RegexOptions.IgnoreCase))
                    {
                        result.ErrorMessage = $"模板文件扩展名异常: {path}";
                        return null;
                    }
                    return path;
                });
        }
        private Option<string> GetOption_Output()
        {
            return new Option<string>(
                aliases: new string[] { "-o", "--output" },
                description: "输出文件路径定义",
                isDefault: true,
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        result.ErrorMessage = "输出文件路径定义需要传入值!";
                        return null;
                    }
                    string path = result.Tokens.Single().Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        result.ErrorMessage = "输出文件路径定义获取为空!";
                        return null;
                    }
                    return path;
                });
        }

        private Command GetCommand_Batch(Option<DirectoryInfo> rootDireOption)
        {
            var listFileOption = new Option<string>(
                aliases: new string[] { "-c", "--config" },
                description: "批量生成配置文件路径定义, 扩展名: .txt 每行格式: <template>.liquid | <output> | <key>:<path>.json <key>:<path>.json ...",
                isDefault: true,
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        result.ErrorMessage = "配置文件定义需要传入值!";
                        return null;
                    }
                    string path = result.Tokens.Single().Value;
                    if (string.IsNullOrEmpty(path))
                    {
                        result.ErrorMessage = "配置文件定义获取为空!";
                        return null;
                    }
                    if (!Regex.IsMatch(path, @"\.txt$",
                        RegexOptions.ECMAScript | RegexOptions.IgnoreCase))
                    {
                        result.ErrorMessage = $"配置文件扩展名异常: {path}";
                        return null;
                    }
                    return path;
                });

            Command c = new Command("batch", "配置文件批量生成输出");
            c.AddOption(listFileOption);
            c.SetHandler((context) =>
            {
                var rootDire = context.ParseResult.GetValueForOption(rootDireOption);
                var filePath = context.ParseResult.GetValueForOption(listFileOption);
                mainHelpr.OnExecute(rootDire, filePath);
            });
            return c;
        }
    }
}
