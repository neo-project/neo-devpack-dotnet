using Neo.Cryptography;
using Neo.VM.Types;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Neo.SmartContract.Testing
{
    internal class DynamicArgumentSyscall
    {
        private readonly List<Func<StackItem>> _actions = new();

        /// <summary>
        /// Syscall Name
        /// </summary>
        public const string Name = $"{nameof(DynamicArgumentSyscall)}.Invoke";

        /// <summary>
        /// Syscall hash
        /// </summary>
        public static uint Hash => BinaryPrimitives.ReadUInt32LittleEndian(Encoding.ASCII.GetBytes(Name).Sha256());

        /// <summary>
        /// Add action
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>Sycall argument</returns>
        public int Add(Func<StackItem> action)
        {
            _actions.Add(action);
            return _actions.Count - 1;
        }

        /// <summary>
        /// Get item
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Stack item</returns>
        public StackItem Invoke(int index)
        {
            return _actions[index]();
        }
    }
}
