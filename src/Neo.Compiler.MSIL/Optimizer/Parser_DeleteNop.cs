using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteNop : IOptimizeParser
    {
        public bool NeedRightAddr => false;

        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="Items">Items</param>
        public void Parse(List<INefItem> Items)
        {
            for (int x = Items.Count - 1; x >= 0; x--)
            {
                if (Items[x] is NefInstruction ins && ins.OpCode == OpCode.NOP)
                {
                    Items.RemoveAt(x);
                }
            }
        }
    }
}
