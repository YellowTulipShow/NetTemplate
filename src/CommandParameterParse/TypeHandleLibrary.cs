using System;
using System.Collections.Generic;
using System.Text;

using CommandParameterParse.TypeHandles;

namespace CommandParameterParse
{
    /// <summary>
    /// 类型处理库集合
    /// </summary>
    public class TypeHandleLibrary : ITypeHandleLibrary
    {
        private readonly IDictionary<string, ITypeHandle> dictHandle;

        /// <summary>
        /// 实例化 - 类型处理库集合
        /// </summary>
        public TypeHandleLibrary()
        {
            dictHandle = new Dictionary<string, ITypeHandle>();
            Register(new StringHandle());
        }

        private string GetSign(Type type)
        {
            return type.FullName;
        }

        /// <inheritdoc/>
        public void Register(ITypeHandle typeHandle)
        {
            if (typeHandle == null)
                return;
            string sign = GetSign(typeHandle.Identification());
            dictHandle[sign] = typeHandle;
        }

        /// <inheritdoc/>
        public ITypeHandle GetHandle(Type type)
        {
            string sign = GetSign(type);
            if (dictHandle.ContainsKey(sign))
            {
                return dictHandle[sign];
            };
            return null;
        }
    }
}
