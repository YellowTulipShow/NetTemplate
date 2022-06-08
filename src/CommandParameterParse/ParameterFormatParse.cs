using System.Linq;
using System.Collections.Generic;

using CommandParameterParse.ParameterFormatHandles;

namespace CommandParameterParse
{
    /// <summary>
    /// 命令格式解析帮助类
    /// </summary>
    public class ParameterFormatParse
    {
        /// <summary>
        /// 格式处理队列
        /// </summary>
        protected readonly IList<IParameterFormatHandle> handles;

        /// <summary>
        /// 实例化 - 命令格式解析帮助类
        /// </summary>
        public ParameterFormatParse()
        {
        }
        /// <summary>
        /// 实例化 - 命令格式解析帮助类
        /// </summary>
        /// <param name="handles">格式处理队列</param>
        public ParameterFormatParse(IList<IParameterFormatHandle> handles)
        {
            this.handles = handles.ToList();
        }

        /// <summary>
        /// 增加处理方法
        /// </summary>
        /// <param name="handle">处理方法</param>
        public void AddHandle(IParameterFormatHandle handle) => handles.Add(handle);

        /// <summary>
        /// 解释执行参数
        /// </summary>
        /// <param name="args">原始调用命令输入参数队列</param>
        /// <returns>结果</returns>
        public ParameterFormatResult[] ExplainExecution(string[] args)
        {
            IList<ParameterFormatResult> rlist = new List<ParameterFormatResult>();
            string old_name = null;
            IList<string> contents = null;
            for (int i_arg = 0; i_arg < args.Length; i_arg++)
            {
                string arg_region = args[i_arg];
                if (IsParameterStart(arg_region, out string name, out IParameterFormatHandle handle))
                {
                    if (name != old_name)
                    {
                        if (!string.IsNullOrEmpty(old_name))
                        {
                            rlist.Add(new ParameterFormatResult()
                            {
                                Name = old_name,
                                Contents = contents.ToArray(),
                            });
                        }
                        old_name = name;
                        contents = new List<string>();
                    }
                }
                string content = handle?.ExtractContent(arg_region);
                if (content != null)
                {
                    contents?.Add(content);
                }
            }
            if (!string.IsNullOrEmpty(old_name))
            {
                rlist.Add(new ParameterFormatResult()
                {
                    Name = old_name,
                    Contents = contents.ToArray(),
                });
            }
            return rlist.ToArray();
        }

        private bool IsParameterStart(string region, out string name, out IParameterFormatHandle handle)
        {
            for (int i_handle = handles.Count - 1; i_handle >= 0; i_handle--)
            {
                var handle_item = handles[i_handle];
                if (handle_item.IsParameter(region, out name))
                {
                    handle = handle_item;
                    return true;
                }
            }
            name = string.Empty;
            handle = null;
            return false;
        }

        /// <summary>
        /// 静态执行方法
        /// </summary>
        /// <param name="args">原始调用命令输入参数队列</param>
        /// <param name="handles">格式处理队列</param>
        /// <returns>参数结果</returns>
        public static ParameterFormatResult[] Render(string[] args, IList<IParameterFormatHandle> handles = null)
        {
            ParameterFormatParse parse =
                handles == null ?
                new ParameterFormatParse():
                new ParameterFormatParse(handles);
            return parse.ExplainExecution(args);
        }
    }
}
