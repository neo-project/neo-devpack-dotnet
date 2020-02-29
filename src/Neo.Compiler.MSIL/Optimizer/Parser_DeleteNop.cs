using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteNop : IOptimizeParser
    {
        private uint OptimizedCount = 0;

        public bool NeedRightAddress => false;
        public bool WillChangeAddress => OptimizedCount > 0;

        public void Init()
        {
            OptimizedCount = 0;
        }

        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="items">Items</param>
        public void Parse(List<INefItem> items)
        {
            for (int x = items.Count - 1; x >= 0; x--)
            {
                if (items[x] is NefInstruction ins && ins.OpCode == OpCode.NOP)
                {
                    items.RemoveAt(x);
                    OptimizedCount++;
                }
            }
        }
    }
}
