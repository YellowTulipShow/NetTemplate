namespace CommandParameterParse
{
    /// <summary>
    /// 命令格式解析结果
    /// </summary>
    public struct ParameterFormatResult
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数附带得到的值
        /// </summary>
        public string[] Contents { get; set; }
    }
}
