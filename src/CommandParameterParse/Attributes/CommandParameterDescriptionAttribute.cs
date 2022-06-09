using System;

namespace CommandParameterParse.Attributes
{
    /// <summary>
    /// 参数配置: 描述 (允许多行配置)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct,
        AllowMultiple = true, Inherited = true)]
    public class CommandParameterDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 注入: 描述
        /// </summary>
        public CommandParameterDescriptionAttribute(string Description)
        {
            this.Description = Description;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
