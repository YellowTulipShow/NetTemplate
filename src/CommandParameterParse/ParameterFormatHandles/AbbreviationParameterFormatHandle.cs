using System.Text.RegularExpressions;

namespace CommandParameterParse.ParameterFormatHandles
{
    /// <summary>
    /// 缩写参数格式解析: -{参数缩写} {内容}, 如: -n 张三
    /// </summary>
    public class AbbreviationParameterFormatHandle : IParameterFormatHandle
    {
        private readonly Regex re_is_name;
        private readonly Regex re_get_content;

        /// <summary>
        /// 实例化: 缩写参数格式解析: -{参数缩写} {内容}, 如: -n 张三
        /// </summary>
        public AbbreviationParameterFormatHandle()
        {
            re_is_name = new Regex(@"^\-([a-zA-Z])[^a-zA-Z]*");
            re_get_content = new Regex(@"^\-([a-zA-Z])(=|\s+)['""]{0,1}([^\n\r'""]+)['""]{0,1}$");
        }

        /// <inheritdoc/>
        public bool IsParameter(string region, out string name)
        {
            Match match = re_is_name.Match(region);
            name = match.Success ? match.Groups[1].Value : null;
            return match.Success;
        }

        /// <inheritdoc/>
        public string ExtractContent(string region)
        {
            Match match = re_get_content.Match(region);
            return match.Success ? match.Groups[3].Value : region;
        }

        /// <inheritdoc/>
        public string[] HelpFormatPrint()
        {
            return new string[] {
                $"-<缩写> <参数内容>",
                $"-<缩写>=<参数内容>",
            };
        }
    }
}
