namespace CommandParameterParse.Attributes
{
    /// <summary>
    /// 参数配置: 缩写名称
    /// </summary>
    public class AbbreviationNameAttribute : AbsNameAttribute
    {
        /// <summary>
        /// 注入: 缩写名称
        /// </summary>
        public AbbreviationNameAttribute(char name) : base(name.ToString()) { }
    }
}
