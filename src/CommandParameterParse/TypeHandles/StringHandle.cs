namespace CommandParameterParse.TypeHandles
{
    /// <summary>
    /// String 类型处理
    /// </summary>
    public class StringHandle : AbsTypeHandle<string>
    {
        /// <inheritdoc/>
        public override string To(string str) => str;
    }
}
