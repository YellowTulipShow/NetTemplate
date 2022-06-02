using System;

namespace CommandParameterParse
{
    /// <summary>
    /// 命令解释
    /// </summary>
    public interface ICommandParse<T> where T : struct
    {
        /// <summary>
        /// 注册: 命令格式解释处理帮助
        /// </summary>
        /// <param name="parameterFormatHandle">命令格式解释处理帮助</param>
        void RegisterIParameterFormatHandle(IParameterFormatHandle parameterFormatHandle);

        /// <summary>
        /// 注册: 数据类型处理帮助
        /// </summary>
        /// <param name="typeHandle">数据类型处理帮助</param>
        void RegisterITypeHandle(ITypeHandle typeHandle);

        /// <summary>
        /// 执行解析操作, 运行命令程序进程内容
        /// </summary>
        /// <param name="args">命令行参数传入</param>
        /// <param name="runAction">后续执行程序</param>
        void OnExecute(string[] args, Action<T> runAction);
    }
}
