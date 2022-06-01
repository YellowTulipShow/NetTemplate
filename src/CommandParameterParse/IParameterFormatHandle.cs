namespace CommandParameterParse
{
    /// <summary>
    /// 接口: 命令格式处理
    /// </summary>
    public interface IParameterFormatHandle
    {
        /// <summary>
        /// 判断是否是参数的开头部分
        /// </summary>
        /// <param name="region">参数'每部分'的内容</param>
        /// <param name="name">如果是则返回参数的名称</param>
        /// <returns>判断结果</returns>
        bool IsParameter(string region, out string name);

        /// <summary>
        /// 从行中提取需要的参数值内容
        /// </summary>
        /// <param name="region">参数'每部分'的内容</param>
        /// <returns>结果, 返回 NULL 则表示忽略</returns>
        string ExtractContent(string region);

        /// <summary>
        /// 输出格式内容的帮助文档
        /// </summary>
        /// <returns>格式内容</returns>
        string[] HelpFormatPrint();
    }
}
