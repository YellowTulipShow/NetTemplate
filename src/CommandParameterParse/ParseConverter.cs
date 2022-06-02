using System;
using System.Linq;

namespace CommandParameterParse
{
    /// <summary>
    /// 解析转化器
    /// </summary>
    public class ParseConverter<T> where T : struct
    {
        /// <summary>
        /// 类型处理库对象
        /// </summary>
        protected readonly ITypeHandleLibrary typeHandleLibrary;

        /// <summary>
        /// 数据结构编译结果
        /// </summary>
        protected readonly StructCompileResult<T> compiler;

        /// <summary>
        /// 初始化 - 解析转化器
        /// </summary>
        public ParseConverter(ITypeHandleLibrary typeHandleLibrary)
        {
            this.typeHandleLibrary = typeHandleLibrary;
            compiler = StructCompileResult<T>.Compile();
        }

        /// <summary>
        /// 是否需要帮助
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool IsNeedHelp(ParameterFormatResult[] parameter)
        {
            return parameter.Any(b => b.Name == "h" || b.Name == "help");
        }

        /// <summary>
        /// 生成数据结构结果值
        /// </summary>
        /// <param name="parameter">参数格式结果内容</param>
        /// <returns>结构数据结果</returns>
        public T GenerateDataStruct(ParameterFormatResult[] parameter)
        {
            T model = new T();
            for (int i = 0; i < parameter.Length; i++)
            {
                var r = parameter[i];
                string fullName = compiler.GetFullName(r.Name);
                if (string.IsNullOrEmpty(fullName))
                    continue;
                if (!compiler.Options.ContainsKey(fullName))
                    continue;
                StructCompileResultOption option = compiler.Options[fullName];
                if (!compiler.Members.ContainsKey(fullName))
                    throw new ArgumentOutOfRangeException($"参数全名: [{fullName}] 存在配置但没找到成员内容!");
                IParameterMemberData member = compiler.Members[fullName];
                Type member_type = member.GetDataType();
                ITypeHandle handle = typeHandleLibrary.GetHandle(member_type);
                object value = handle.To(r.Contents);
                if (value == null && option.IsRequired)
                    throw new ArgumentException($"参数: [{fullName}] 需要必填!");
                member.WriteValue(model, value);
            }
            return model;
        }
    }
}
