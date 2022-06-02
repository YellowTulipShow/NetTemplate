using System;

namespace CommandParameterParse
{
    /// <summary>
    /// 类型处理库
    /// </summary>
    public interface ITypeHandleLibrary
    {
        /// <summary>
        /// 注册类型处理帮助
        /// </summary>
        /// <param name="typeHandle">处理帮助</param>
        void Register(ITypeHandle typeHandle);

        /// <summary>
        /// 根据数据类型获取类型处理帮助
        /// </summary>
        /// <param name="type">属性/字段数据类型</param>
        /// <returns>处理帮助</returns>
        ITypeHandle GetHandle(Type type);
    }
}
