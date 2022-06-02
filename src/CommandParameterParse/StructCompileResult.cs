using System;
using System.Collections.Generic;
using System.Reflection;

using CommandParameterParse.Attributes;
using CommandParameterParse.ParameterMemberData;

namespace CommandParameterParse
{
    /// <summary>
    /// 数据模型编译结果
    /// </summary>
    public struct StructCompileResult<T> where T : struct
    {
        /// <summary>
        /// 数据结构类型
        /// </summary>
        public Type StructType { get; set; }

        /// <summary>
        /// 配置存储 {属性/字段全名, 配置}
        /// </summary>
        public IDictionary<string, StructCompileResultOption> Options { get; set; }

        /// <summary>
        /// 属性队列 {全名, 数据值}
        /// </summary>
        public IDictionary<string, IParameterMemberData> Members { get; set; }

        /// <summary>
        /// 获取参数全名
        /// </summary>
        /// <param name="arg_name">参数名称:别名/缩写/全名</param>
        /// <returns>结果</returns>
        public string GetFullName(string arg_name)
        {
            foreach (var option in Options.Values)
            {
                if ((option.FullName ?? "") == arg_name ||
                    (option.AliasName ?? "") == arg_name ||
                    (option.AbbreviationName ?? "") == arg_name)
                    return option.FullName;
            }
            return null;
        }

        /// <summary>
        /// 对于数据进行编译
        /// </summary>
        /// <returns>编译结果</returns>
        public static StructCompileResult<T> Compile()
        {
            Type type = typeof(T);
            var r = new StructCompileResult<T>()
            {
                StructType = type,
                Options = new Dictionary<string, StructCompileResultOption>(),
                Members = new Dictionary<string, IParameterMemberData>(),
            };
            foreach (PropertyInfo item in type.GetProperties())
            {
                if (!item.CanRead)
                    continue;
                var option = CalcOption(item);
                r.Options[option.FullName] = option;
                r.Members[option.FullName] = new ParameterMemberProperty(item);
            }
            foreach (FieldInfo item in type.GetFields())
            {
                if (item.IsStatic)
                    continue;
                var option = CalcOption(item);
                r.Options[option.FullName] = option;
                r.Members[option.FullName] = new ParameterMemberField(item);
            }
            return r;
        }
        private static StructCompileResultOption CalcOption(MemberInfo member)
        {
            return new StructCompileResultOption()
            {
                FullName = member.Name,
                AliasName = member.GetCustomAttribute<AliasNameAttribute>()?.Name,
                AbbreviationName = member.GetCustomAttribute<AbbreviationNameAttribute>()?.Name,
                IsRequired = member.GetCustomAttribute<IsRequiredAttribute>()?.IsRequired ?? false,
                Description = member.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty,
            };
        }
    }

    /// <summary>
    /// 数据模型编译结果-配置型
    /// </summary>
    public struct StructCompileResultOption
    {
        /// <summary>
        /// 全名
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string AliasName { get; set; }
        /// <summary>
        /// 缩写名称
        /// </summary>
        public string AbbreviationName { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
