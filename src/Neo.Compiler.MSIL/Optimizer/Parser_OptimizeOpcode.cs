using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    class Parser_OptimizeOpcode : IOptimizeParser
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
                if (!(items[x] is NefInstruction ins))
                {
                    continue;
                }

                switch (ins.OpCode)
                {
                    // Reduce unused data

                    case OpCode.STLOC:
                    case OpCode.LDLOC:
                    case OpCode.STARG:
                    case OpCode.LDARG:
                        {
                            if (items[x] is NefInstruction p1 && p1.Data.Length == 1)
                            {
                                if (p1.Data[0] >= 0 && p1.Data[0] <= 6)
                                {
                                    p1.SetOpCode((OpCode)(ins.OpCode - 7 + p1.Data[0]));
                                    p1.SetData(Array.Empty<byte>());
                                }
                            }
                            break;
                        }
                }
            }
        }
    }
}
