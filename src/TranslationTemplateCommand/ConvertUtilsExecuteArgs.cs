using System.Collections.Generic;

using CommandParameterParse.Attributes;

namespace TranslationTemplateCommand
{
    [CommandParameterDescription("解释模板文件用于生成文件")]
    public struct ConvertUtilsExecuteArgs
    {
        [AbbreviationName('r')]
        [AliasName("root")]
        [CommandParameterDescription("-r --root <根目录地址>")]
        public string Root { get; set; }

        [AbbreviationName('t')]
        [AliasName("template")]
        [IsRequired()]
        [CommandParameterDescription("-t -template <模板地址>")]
        public string Template { get; set; }

        [AbbreviationName('o')]
        [AliasName("output")]
        [IsRequired()]
        [CommandParameterDescription("-o --output <输出路径>")]
        public string Output { get; set; }

        [AliasName("data")]
        [IsRequired()]
        [CommandParameterDescription("--data <数据JSON路径配置...>")]
        [CommandParameterDescription("例如: --data key1=value1 key2=value2 ...")]
        public IDictionary<string, string> Data { get; set; }
    }
}
