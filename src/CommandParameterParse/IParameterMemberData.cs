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
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="model">写入目标对象</param>
        /// <param name="value">写入值</param>
        /// <returns>写入目标对象赋值后返回</returns>
        object WriteValue(object model, object value);
    }
}
