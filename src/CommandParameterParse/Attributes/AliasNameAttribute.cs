namespace CommandParameterParse.Attributes
{
    /// <summary>
    /// 参数配置: 别名
    /// </summary>
    public class AliasNameAttribute : AbsNameAttribute
    {
        /// <summary>
        /// 注入: 别名
        /// </summary>
        public AliasNameAttribute(string name) : base(name) { }
    }
}
