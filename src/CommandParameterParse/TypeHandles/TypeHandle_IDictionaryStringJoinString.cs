using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CommandParameterParse.TypeHandles
{
    /// <summary>
    /// 数据类型处理: IDictionary{string, string}
    /// </summary>
    public class TypeHandle_IDictionaryStringJoinString : ITypeHandle<IDictionary<string, string>>
    {
        /// <inheritdoc/>
        public Type Identification() => typeof(IDictionary<string, string>);

        /// <inheritdoc/>
        public IDictionary<string, string> To(string[] strs)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string str in strs)
            {
                var match = Regex.Match(str, @"^([a-zA-Z]+)=([^\n\r]+)$");
                if (!match.Success)
                    continue;
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                if (!string.IsNullOrEmpty(key))
                    dict[key] = value;
            }
            return dict;
        }

        /// <inheritdoc/>
        object ITypeHandle.To(string[] strs) => To(strs);
    }
}
