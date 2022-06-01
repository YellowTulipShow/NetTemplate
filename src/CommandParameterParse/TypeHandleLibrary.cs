using System;
using System.Collections.Generic;
using System.Text;

namespace CommandParameterParse
{
    public interface ITypeHandleLibrary
    {
        ITypeHandle GetHandle(Type type);
    }

    /// <summary>
    /// 类型处理库集合
    /// </summary>
    public class TypeHandleLibrary : ITypeHandleLibrary
    {
        public TypeHandleLibrary()
        {
        }

        public ITypeHandle GetHandle(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
