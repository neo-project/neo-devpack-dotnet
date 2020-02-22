using System;
using System.Collections.Generic;
using System.Text;
using Neo.VM;
namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteNop : IOptimizeParser
    {
        public void Parse(List<INefItem> Items)
        {
            for (int x = Items.Count - 1; x >= 0; x--)
            {
                var ins = Items[x] as NefInstruction;
                if (ins != null && ins.OpCode == OpCode.NOP)
                {
                    Items.RemoveAt(x);
                }
            }
        }
    }
}
