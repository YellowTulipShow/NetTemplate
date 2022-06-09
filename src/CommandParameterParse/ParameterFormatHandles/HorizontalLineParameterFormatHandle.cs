using System.Text.RegularExpressions;

namespace CommandParameterParse.ParameterFormatHandles
{
    /// <summary>
    /// 横线常规参数格式解析: --{参数名称}={内容}, 如: --name=张三
    /// </summary>
    public class HorizontalLineParameterFormatHandle : IParameterFormatHandle
    {
        private readonly Regex re_only_name;
        private readonly Regex re_is_name;
        private readonly Regex re_get_content;

        /// <summary>
        /// 实例化: 横线常规参数格式解析: --{参数名称}={内容}, 如: --name=张三
        /// </summary>
        public HorizontalLineParameterFormatHandle()
        {
            re_only_name = new Regex(@"^\-{2}([a-zA-Z]+)$");
            re_is_name = new Regex(@"^\-{2}([a-zA-Z]+)[^a-zA-Z]*");
            re_get_content = new Regex(@"^\-{2}([a-zA-Z]+)(=|\s+)['""]{0,1}([^\n\r'""]+)['""]{0,1}$");
        }

        /// <inheritdoc/>
        public virtual bool IsParameter(string region, out string name)
        {
            Match match = re_is_name.Match(region);
            name = match.Success ? match.Groups[1].Value : null;
            return match.Success;
        }

        /// <inheritdoc/>
        public virtual string ExtractContent(string region)
        {
            if (re_only_name.IsMatch(region))
                return null;
            Match match = re_get_content.Match(region);
            return match.Success ? match.Groups[3].Value :
                region?.Trim('\'').Trim('"');
        }

        /// <inheritdoc/>
        public virtual string[] HelpFormatPrint()
        {
            return new string[] {
                $"--<名称> <参数内容>",
                $"--<名称>=<参数内容>",
            };
        }
    }
}
