using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.CommandLine;

using YTS.Log;
using System;

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

            RootCommand rootC = new RootCommand("基于开源项目 Fluid 解释 .liquid 模板, 用于生成文件命令程序");
            rootC.AddGlobalOption(rootDireOption);

            Command singleC = GetCommand_Single(rootDireOption);
            rootC.AddCommand(singleC);
            Command batchC = GetCommand_Batch(rootDireOption);
            rootC.AddCommand(batchC);

            return rootC.Invoke(args);
        }

        private Option<DirectoryInfo> GetOption_RootDire()
        {
            var option = new Option<DirectoryInfo>(
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
            option.Arity = ArgumentArity.ExactlyOne;
            return option;
        }

        private Command GetCommand_Single(Option<DirectoryInfo> rootDireOption)
        {
            var datasOption = GetOption_Datas();
            var templateOption = GetOption_Template();
            var outputOption = GetOption_Output();

            Command c = new Command("single", "单模板生成输出");
            c.AddOption(datasOption);
            c.AddOption(templateOption);
            c.AddOption(outputOption);

            c.SetHandler((context) =>
            {
                var logArgs = log.CreateArgDictionary();
                try
                {
                    var rootDire = context.ParseResult.GetValueForOption(rootDireOption);
                    logArgs["rootDire.FullName"] = rootDire.FullName;
                    var datas = context.ParseResult.GetValueForOption(datasOption);
                    logArgs["datas"] = datas;
                    var template = context.ParseResult.GetValueForOption(templateOption);
                    logArgs["template"] = template;
                    var output = context.ParseResult.GetValueForOption(outputOption);
                    logArgs["output"] = output;
                    mainHelpr.OnExecute(rootDire, template, output, datas);
                }
                catch (Exception ex)
                {
                    log.Error("单模板生成输出出错", ex, logArgs);
                    context.ExitCode = 1;
                }
            });
            return c;
        }
        private Option<string[]> GetOption_Datas()
        {
            var option = new Option<string[]>(
                name: "--data",
                description: $"数据内容定义, 可多个(<key>:<path>)",
                isDefault: true,
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        result.ErrorMessage = "数据内容定义不能为空!";
                        return null;
                    }
                    return result.Tokens.Select(b => b.Value).ToArray();
                });
            option.Arity = ArgumentArity.OneOrMore;
            return option;
        }
        private Option<string> GetOption_Template()
        {
            var option = new Option<string>(
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
            option.Arity = ArgumentArity.ExactlyOne;
            return option;
        }
        private Option<string> GetOption_Output()
        {
            var option = new Option<string>(
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
            option.Arity = ArgumentArity.ExactlyOne;
            return option;
        }

        private Command GetCommand_Batch(Option<DirectoryInfo> rootDireOption)
        {
            var configFileOption = new Option<string>(
                aliases: new string[] { "-c", "--config" },
                description: "批量生成配置文件路径定义" +
                    " 扩展名: .txt" +
                    " 每行格式: <template>.liquid | <output> | <key>:<path>.json <key>:<path>.json ...",
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
            c.AddOption(configFileOption);
            c.SetHandler((context) =>
            {
                var logArgs = log.CreateArgDictionary();
                try
                {
                    var rootDire = context.ParseResult.GetValueForOption(rootDireOption);
                    logArgs["rootDire.FullName"] = rootDire.FullName;
                    var filePath = context.ParseResult.GetValueForOption(configFileOption);
                    logArgs["filePath"] = filePath;
                    mainHelpr.OnExecute(rootDire, filePath);
                }
                catch (Exception ex)
                {
                    log.Error("批量生成输出出错", ex, logArgs);
                    context.ExitCode = 1;
                }
            });
            return c;
        }
    }
}
