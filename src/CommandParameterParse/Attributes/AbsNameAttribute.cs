using System;

namespace CommandParameterParse.Attributes
{
    /// <summary>
    /// 抽象类 - 参数配置: 名称配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false, Inherited = true)]
    public abstract class AbsNameAttribute : Attribute
    {
        /// <summary>
        /// 注入: 别名
        /// </summary>
        public AbsNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
}
