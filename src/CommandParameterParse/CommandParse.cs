using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CommandParameterParse.Attributes;

namespace CommandParameterParse
{
    /// <inheritdoc/>
    public class CommandParse<T> : ICommandParse<T> where T : struct
    {
        /// <summary>
        /// 帮助输出处理接口
        /// </summary>
        protected readonly ICommandParseHelpPrint helpPrint;
        /// <summary>
        /// 数据模型编译结果
        /// </summary>
        protected readonly StructCompileResult<T> structCompile;
        /// <summary>
        /// 各字段属性数据类型处理方法库
        /// </summary>
        protected readonly ITypeHandleLibrary typeHandleLibrary;
        /// <summary>
        /// 命令参数格式处理集合
        /// </summary>
        protected readonly IList<IParameterFormatHandle> formatHandles;

        /// <summary>
        /// 实例化 - 命令解析实现类
        /// </summary>
        public CommandParse(ICommandParseHelpPrint helpPrint)
        {
            this.helpPrint = helpPrint;
            structCompile = StructCompileResult<T>.Compile();
            typeHandleLibrary = new TypeHandleLibrary();
            formatHandles = new List<IParameterFormatHandle>();
            InitDefaultConfigs();
        }

        /// <summary>
        /// 初始化默认相关配置
        /// </summary>
        protected virtual void InitDefaultConfigs()
        {
            typeHandleLibrary.Register(new TypeHandles.StringHandle());
            formatHandles.Add(new ParameterFormatHandles.AbbreviationParameterFormatHandle());
            formatHandles.Add(new ParameterFormatHandles.HorizontalLineParameterFormatHandle());
        }

        /// <inheritdoc/>
        public void RegisterIParameterFormatHandle(IParameterFormatHandle parameterFormatHandle)
        {
            formatHandles.Add(parameterFormatHandle);
        }

        /// <inheritdoc/>
        public void RegisterITypeHandle(ITypeHandle typeHandle)
        {
            typeHandleLibrary.Register(typeHandle);
        }

        /// <inheritdoc/>
        public void OnExecute(string[] args, Action<T> runAction)
        {
            try
            {
                ParameterFormatResult[] parameters = ParameterFormatParse.Render(args, formatHandles);
                if (CheckIsHelp(parameters))
                {
                    string[] textlines = PrintHelpDocument();
                    helpPrint.Prints(textlines);
                    return;
                }
                T m = GenerateDataStruct(parameters);
                runAction.Invoke(m);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                throw ex;
            }
        }

        private bool CheckIsHelp(ParameterFormatResult[] parameters)
        {
            return parameters.Any(b => b.Name == "h" || b.Name == "help");
        }

        /// <summary>
        /// 打印帮助文档内容
        /// </summary>
        private string[] PrintHelpDocument()
        {
            IList<string> lines = new List<string>();

            if ((structCompile.Descriptions?.Length ?? 0) > 0)
            {
                lines.Add($"命令描述:");
                foreach (string desc in structCompile.Descriptions)
                    lines.Add($"\t{desc}");
                lines.Add(string.Empty);
            }

            lines.Add($"参数:");
            foreach (var option in structCompile.Options.Values)
            {
                string names = string.Join(", ", new string[] {
                    option.AbbreviationName, option.AliasName, option.FullName
                }.Where(b => !string.IsNullOrEmpty(b)).ToArray());
                lines.Add($"\t名称: {names}");
                if (option.IsRequired)
                {
                    lines.Add($"\t[必填项]");
                }
                if ((option.Descriptions?.Length ?? 0) > 0)
                {
                    lines.Add($"\t命令描述:");
                    foreach (string desc in option.Descriptions)
                        lines.Add($"\t\t{desc}");
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
                string fullName = structCompile.GetFullName(r.Name);
                if (string.IsNullOrEmpty(fullName))
                    continue;
                if (!structCompile.Options.ContainsKey(fullName))
                    continue;
                StructCompileResultOption option = structCompile.Options[fullName];
                if (!structCompile.Members.ContainsKey(fullName))
                    throw new ArgumentOutOfRangeException($"参数全名: [{fullName}] 存在配置但没找到成员内容!");
                IParameterMemberData member = structCompile.Members[fullName];
                Type member_type = member.GetDataType();
                ITypeHandle handle = typeHandleLibrary.GetHandle(member_type);
                if (handle == null)
                    throw new NullReferenceException($"参数: [{fullName}] 所属的数据类型: {member_type.FullName} 无法识别!");
                object value = handle.To(r.Contents);
                if (value == null && option.IsRequired)
                    throw new ArgumentException($"参数: [{fullName}] 需要必填!");
                model = (T)member.WriteValue(model, value);
            }
            return model;
        }
    }
}
