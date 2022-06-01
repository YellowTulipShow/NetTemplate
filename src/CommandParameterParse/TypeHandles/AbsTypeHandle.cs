using System;

namespace CommandParameterParse.TypeHandles
{
    /// <inheritdoc/>
    public abstract class AbsTypeHandle<T> : ITypeHandle<T>
    {
        /// <inheritdoc/>
        public Type Identification() => typeof(T);

        /// <inheritdoc/>
        public abstract T To(string[] strs);

        /// <inheritdoc/>
        object ITypeHandle.To(string[] strs) => To(strs);
    }
}
