using System;
using System.Reflection;

namespace CommandParameterParse.ParameterMemberData
{
    /// <summary>
    /// 属性成员
    /// </summary>
    public class ParameterMemberProperty : IParameterMemberData
    {
        private readonly PropertyInfo property;

        /// <summary>
        /// 实例化 - 属性成员
        /// </summary>
        /// <param name="property">属性对象</param>
        public ParameterMemberProperty(PropertyInfo property)
        {
            this.property = property;
        }

        /// <inheritdoc/>
        public Type GetDataType()
        {
            return property.PropertyType;
        }

        /// <inheritdoc/>
        public MemberInfo GetMember()
        {
            return property;
        }

        /// <inheritdoc/>
        public object WriteValue(object model, object value)
        {
            property.SetValue(model, value, null);
            return model;
        }
    }
}
