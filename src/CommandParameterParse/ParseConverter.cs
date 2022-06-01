using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CommandParameterParse.Attributes;

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
        protected readonly DataStructCompileResult<T> compiler;

        /// <summary>
        /// 初始化 - 解析转化器
        /// </summary>
        public ParseConverter(ITypeHandleLibrary typeHandleLibrary)
        {
            this.typeHandleLibrary = typeHandleLibrary;
            compiler = DataStructCompileResult<T>.Compile();
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
        /// 打印帮助文档内容
        /// </summary>
        /// <param name="formatHandles">参数格式处理</param>
        /// <returns>文档内容</returns>
        public string[] PrintHelpDocument(IParameterFormatHandle[] formatHandles)
        {
            IList<string> lines = new List<string>();

            string command_desc = compiler.StructType.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (!string.IsNullOrEmpty(command_desc))
            {
                lines.Add($"命令描述:");
                lines.Add($"\t{command_desc}");
                lines.Add(string.Empty);
            }

            lines.Add($"参数:");
            foreach (var option in compiler.Options.Values)
            {
                string names = string.Join(", ", new string[] {
                    option.AbbreviationName, option.AliasName, option.FullName
                }.Where(b => string.IsNullOrEmpty(b)).ToArray());
                lines.Add($"\t名称: {names}");
                if (option.IsRequired)
                {
                    lines.Add($"\t[必填项]");
                }
                if (!string.IsNullOrEmpty(option.Description))
                {
                    lines.Add($"\t描述: {option.Description}");
                }
                lines.Add(string.Empty);
            }

            lines.Add($"格式描写:");
            foreach (IParameterFormatHandle formatHandle in formatHandles)
            {
                foreach (var line in formatHandle.HelpFormatPrint())
                {
                    lines.Add($"\t{line}");
                }
                lines.Add(string.Empty);
            }

            return lines.ToArray();
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
                DataStructCompileResultOption option = compiler.Options[fullName];
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
