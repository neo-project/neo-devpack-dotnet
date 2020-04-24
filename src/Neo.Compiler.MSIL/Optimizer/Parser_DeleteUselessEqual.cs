using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteUselessEqual : IOptimizeParser
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
            for (int x = items.Count - 1; x >= 2; x--)
            {
                if (!(items[x] is NefInstruction ins))
                {
                    continue;
                }

                switch (ins.OpCode)
                {
                    case OpCode.EQUAL:
                        {
                            if (items[x - 1] is NefInstruction p1 &&
                                items[x - 2] is NefInstruction p2
                                )
                            {
                                // Both are PUSHX

                                if (p1.OpCode >= OpCode.PUSHM1 &&
                                    p1.OpCode <= OpCode.PUSH16 &&
                                    p2.OpCode >= OpCode.PUSHM1 &&
                                    p2.OpCode <= OpCode.PUSH16)
                                {
                                    if (p1.OpCode == p2.OpCode)
                                    {
                                        // EQUAL true

                                        items.RemoveRange(x - 2, 2);
                                        OptimizedCount++;
                                        x -= 2;
                                        ins.SetOpCode(OpCode.PUSH1);
                                    }
                                    else
                                    {
                                        // EQUAL false

                                        items.RemoveRange(x - 2, 2);
                                        OptimizedCount++;
                                        x -= 2;
                                        ins.SetOpCode(OpCode.PUSH0);
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }
    }
}
