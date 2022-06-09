using System;

namespace CommandParameterParse
{
    /// <summary>
    /// 命令解释, 帮助文本输出
    /// </summary>
    public interface ICommandParseHelpPrint
    {
        /// <summary>
        /// 帮助文本输出执行
        /// </summary>
        void Prints(string[] help_content);
    }
}
