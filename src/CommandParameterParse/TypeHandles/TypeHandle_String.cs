namespace CommandParameterParse.TypeHandles
{
    /// <summary>
    /// 数据类型处理: string
    /// </summary>
    public class TypeHandle_String : AbsTypeHandle<string>
    {
        /// <inheritdoc/>
        public override string To(string[] strs)
        {
            return string.Join("", strs);
        }
    }
}
