using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteUselessJmp : IOptimizeParser
    {
        private uint OptimizedCount = 0;

        public bool NeedRightAddress => true;
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
                if (!(items[x] is NefInstruction ins))
                {
                    continue;
                }

                switch (ins.OpCode)
                {
                    case OpCode.JMP:
                        {
                            // Jump always to the next instruction
                            if (ins.Data[0] == 2)
                            {
                                items.RemoveAt(x);
                                OptimizedCount++;
                            }
                            break;
                        }
                    case OpCode.JMP_L:
                        {
                            // Jump always to the next instruction
                            if (BitConverter.ToInt32(ins.Data) == 5)
                            {
                                items.RemoveAt(x);
                                OptimizedCount++;
                            }
                            break;
                        }
                }
            }
        }
    }
}
