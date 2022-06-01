using System;
using System.Reflection;

namespace CommandParameterParse
{
    /// <summary>
    /// 参数成员数据
    /// </summary>
    public interface IParameterMemberData
    {
        /// <summary>
        /// 获取成员反射信息
        /// </summary>
        MemberInfo GetMember();

        /// <summary>
        /// 获取成员数据类型
        /// </summary>
        Type GetDataType();

        /// <summary>
        /// 写入数据值
        /// </summary>
        /// <param name="model">写入目标对象</param>
        /// <param name="value">写入值</param>
        void WriteValue(object model, object value);
    }
}
