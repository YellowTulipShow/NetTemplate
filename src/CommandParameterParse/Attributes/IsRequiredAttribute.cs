using System;

namespace CommandParameterParse.Attributes
{
    /// <summary>
    /// 参数配置: 是否必填
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false, Inherited = true)]
    public class IsRequiredAttribute : Attribute
    {
        /// <summary>
        /// 注入: 别名
        /// </summary>
        public IsRequiredAttribute(bool IsRequired = true)
        {
            this.IsRequired = IsRequired;
        }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }
    }
}
