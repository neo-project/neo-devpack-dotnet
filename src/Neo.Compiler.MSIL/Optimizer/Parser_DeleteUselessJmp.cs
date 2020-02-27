using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteUselessJmp : IOptimizeParser
    {
        public bool NeedRightAddr => false;

        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="items">Items</param>
        public void Parse(List<INefItem> items)
        {
            for (int x = items.Count - 1; x >= 0; x--)
            {
                if (!(items[x] is NefInstruction ins))
                {
                    continue;
                }

                switch (ins.OpCode)
                {
                    case OpCode.JMP:
                        {
                            // Jump always to the next instruction
                            if (ins.Data[0] == 2) items.RemoveAt(x);
                            break;
                        }
                    case OpCode.JMP_L:
                        {
                            // Jump always to the next instruction
                            if (BitConverter.ToInt32(ins.Data) == 5) items.RemoveAt(x);
                            break;
                        }
                }
            }
        }
    }
}
