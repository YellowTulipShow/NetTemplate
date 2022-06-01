using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using CommandParameterParse.Attributes;

namespace CommandParameterParse
{
    /// <summary>
    /// 解析转化器
    /// </summary>
    public class ParseConverter
    {
        /// <summary>
        /// 类型处理库对象
        /// </summary>
        protected readonly ITypeHandleLibrary typeHandleLibrary;

        /// <summary>
        /// 初始化 - 解析转化器
        /// </summary>
        public ParseConverter(ITypeHandleLibrary typeHandleLibrary)
        {
            this.typeHandleLibrary = typeHandleLibrary;
        }

        /// <summary>
        /// 对于数据进行编译
        /// </summary>
        /// <typeparam name="T">需要解析的数据模型</typeparam>
        /// <returns>编译结果</returns>
        public ModelCompileResults Compile<T>() where T : struct
        {
            var r = new ModelCompileResults()
            {
                Options = new Dictionary<string, ModelCompileResultsOption>(),
                Propertys = new Dictionary<string, PropertyInfo>(),
                Fields = new Dictionary<string, FieldInfo>(),
            };
            Type type = typeof(T);
            foreach (PropertyInfo item in type.GetProperties())
            {
                if (!item.CanRead)
                    continue;
                var option = CalcOption(item);
                r.Options[option.FullName] = option;
                r.Propertys[option.FullName] = item;
            }
            foreach (FieldInfo item in type.GetFields())
            {
                if (item.IsStatic)
                    continue;
                var option = CalcOption(item);
                r.Options[option.FullName] = option;
                r.Fields[option.FullName] = item;
            }
            return r;
        }
        private ModelCompileResultsOption CalcOption(MemberInfo member)
        {
            return new ModelCompileResultsOption()
            {
                FullName = member.Name,
                AliasName = member.GetCustomAttribute<AliasNameAttribute>()?.Name,
                AbbreviationName = member.GetCustomAttribute<AbbreviationNameAttribute>()?.Name,
            };
        }

        /// <summary>
        /// 转为数据对象
        /// </summary>
        /// <typeparam name="T">数据结构</typeparam>
        /// <param name="args">命令行参数输入</param>
        /// <returns>结构数据结果</returns>
        public T ToData<T>(string[] args) where T : struct
        {
            T model = new T();
            var compiler = Compile<T>();
            for (int i = 0; i < args.Length; i++)
            {
                (string name, string[] contents) = ParseLine(args[i]);
                string fullName = GetFullName(compiler, name);
                if (string.IsNullOrEmpty(fullName))
                    continue;
                if (compiler.Propertys.ContainsKey(fullName))
                {
                    var prop = compiler.Propertys[fullName];
                    var handle = typeHandleLibrary.GetHandle(prop.PropertyType);
                    object value = handle.To(contents);
                    prop.SetValue(model, value, null);
                }
                else if (compiler.Fields.ContainsKey(fullName))
                {
                    var field = compiler.Fields[fullName];
                    var handle = typeHandleLibrary.GetHandle(field.FieldType);
                    object value = handle.To(contents);
                    field.SetValue(model, value);
                }
            }
            return model;
        }

        private string GetFullName(ModelCompileResults compiler, string arg_name)
        {
            foreach (var option in compiler.Options.Values)
            {
                if ((option.FullName ?? "") == arg_name ||
                    (option.AliasName ?? "") == arg_name ||
                    (option.AbbreviationName ?? "") == arg_name)
                    return option.FullName;
            }
            return null;
        }

        private (string name, string[] contents) ParseLine(string arg)
        {
            IList<string> name;

            Match m = Regex.Match(arg, @"\-+(\w+)");
            m = Regex.Match(arg, @"\-+(\w+)=([^\s]+)");
        }
    }


    /// <summary>
    /// 数据模型编译结果
    /// </summary>
    public struct ModelCompileResults
    {
        /// <summary>
        /// 配置存储 {属性/字段全名, 配置}
        /// </summary>
        public IDictionary<string, ModelCompileResultsOption> Options { get; set; }

        /// <summary>
        /// 属性队列 {全名, 属性信息}
        /// </summary>
        public IDictionary<string, PropertyInfo> Propertys { get; set; }

        /// <summary>
        /// 字段队列 {全名, 字段信息}
        /// </summary>
        public IDictionary<string, FieldInfo> Fields { get; set; }
    }
    /// <summary>
    /// 数据模型编译结果-配置型
    /// </summary>
    public struct ModelCompileResultsOption
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
    }
}
