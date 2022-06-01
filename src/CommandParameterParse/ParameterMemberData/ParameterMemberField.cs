using System;
using System.Reflection;

namespace CommandParameterParse.ParameterMemberData
{
    /// <summary>
    /// 字段成员
    /// </summary>
    public class ParameterMemberField : IParameterMemberData
    {
        private readonly FieldInfo field;

        /// <summary>
        /// 实例化 - 字段成员
        /// </summary>
        /// <param name="field">字段对象</param>
        public ParameterMemberField(FieldInfo field)
        {
            this.field = field;
        }

        /// <inheritdoc/>
        public Type GetDataType()
        {
            return field.FieldType;
        }

        /// <inheritdoc/>
        public MemberInfo GetMember()
        {
            return field;
        }

        /// <inheritdoc/>
        public void WriteValue(object model, object value)
        {
            field.SetValue(model, value);
        }
    }
}
