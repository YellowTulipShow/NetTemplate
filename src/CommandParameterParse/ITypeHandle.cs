using System;

namespace CommandParameterParse
{
    /// <summary>
    /// 接口: 数据类型处理
    /// </summary>
    public interface ITypeHandle
    {
        /// <summary>
        /// 得到需要标识的类型
        /// </summary>
        /// <returns>类型</returns>
        Type Identification();

        /// <summary>
        /// 转换传入的命令参数字符串内容
        /// </summary>
        /// <param name="strs">命令参数</param>
        /// <returns>结果类型值</returns>
        object To(string[] strs);
    }

    /// <summary>
    /// 接口: 数据类型处理泛型表达
    /// </summary>
    /// <typeparam name="T">需要处理的数据类型</typeparam>
    public interface ITypeHandle<T> : ITypeHandle
    {
        /// <inheritdoc/>
        new T To(string[] strs);
    }
}
