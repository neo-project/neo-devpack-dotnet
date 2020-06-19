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
                                items[x - 2] is NefInstruction p2 &&
                                p1.IsPush(out var v1) &&
                                p2.IsPush(out var v2)
                                )
                            {
                                if (v1 == v2)
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
                            break;
                        }
                    case OpCode.NOTEQUAL:
                        {
                            if (items[x - 1] is NefInstruction p1 &&
                                items[x - 2] is NefInstruction p2 &&
                                p1.IsPush(out var v1) &&
                                p2.IsPush(out var v2)
                                )
                            {
                                if (v1 != v2)
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
                            break;
                        }
                }
            }
        }
    }
}
