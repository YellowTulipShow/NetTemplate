using System;

namespace CommandParameterParse.Attributes
{
    /// <summary>
    /// 参数配置: 描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct,
        AllowMultiple = false, Inherited = true)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// 注入: 描述
        /// </summary>
        public DescriptionAttribute(string Description)
        {
            this.Description = Description;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
