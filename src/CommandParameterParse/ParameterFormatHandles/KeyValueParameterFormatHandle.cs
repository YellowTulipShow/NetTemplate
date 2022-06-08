using System.Text.RegularExpressions;

namespace CommandParameterParse.ParameterFormatHandles
{
    /// <summary>
    /// 键值对格式: --{参数名称} {键}={值} {键}={值} ......
    /// </summary>
    public class KeyValueParameterFormatHandle : IParameterFormatHandle
    {
        private readonly Regex re_is_name;
        private readonly Regex re_get_content;

        /// <summary>
        /// 实例化: 键值对格式: --{参数名称} {键}={值} {键}={值} ......
        /// </summary>
        public KeyValueParameterFormatHandle()
        {
            re_is_name = new Regex(@"^\-{2}([a-zA-Z]+)$");
            re_get_content = new Regex(@"^([a-zA-Z]+)[=:]['""]{0,1}([^\n\r'""]+)['""]{0,1}$");
        }

        /// <inheritdoc/>
        public virtual bool IsParameter(string region, out string name)
        {
            Match match = re_is_name.Match(region);
            name = match.Success ? match.Groups[1].Value : null;
            return match.Success;
        }

        /// <inheritdoc/>
        public string ExtractContent(string region)
        {
            if (re_is_name.IsMatch(region))
            {
                return null;
            }
            Match match = re_get_content.Match(region);
            if (match.Success)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                return $"{key}={value}";
            }
            return region;
        }

        /// <inheritdoc/>
        public string[] HelpFormatPrint()
        {
            return new string[] {
                $"--<参数名称> <键>=<值> <键>=<值> ......",
            };
        }
    }
}
