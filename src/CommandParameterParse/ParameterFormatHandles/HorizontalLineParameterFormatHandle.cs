using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandParameterParse.ParameterFormatHandles
{
    /// <summary>
    /// 横线格式: -p XXX or --path=XXXX
    /// </summary>
    public class HorizontalLineParameterFormatHandle : IParameterFormatHandle
    {
        /// <inheritdoc/>
        public bool IsParameter(string region, out string name)
        {
            Match match = Regex.Match(region, @"^\-+([a-zA-Z]+)");
            if (match.Success)
            {
                name = match.Groups[1].Value;
                return true;
            }
            match = Regex.Match(region, @"^\-+([a-zA-Z]+)=([^\s]+)$");
            if (match.Success)
            {
                name = match.Groups[1].Value;
                return true;
            }
            name = null;
            return false;
        }

        /// <inheritdoc/>
        public string ExtractContent(string region)
        {
            Match match = Regex.Match(region, @"^\-+([a-zA-Z]+)");
            if (match.Success)
                return null;
            match = Regex.Match(region, @"^\-+([a-zA-Z]+)=([^\s]+)$");
            if (match.Success)
                return match.Groups[2].Value;
            return region;
        }

        /// <inheritdoc/>
        public string[] HelpFormatPrint()
        {
            return new string[] {
                $"-<n>, --<name> [<parameter> <parameter> ...]",
                $"-<n>, --<name>=[<parameter>]",
                $"<n>: 缩写, <name>: 命令名称, <parameter>: 参数内容",
            };
        }
    }
}
