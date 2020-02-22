using System;
using System.Collections.Generic;
using System.Text;
using Neo.VM;
namespace Neo.Compiler.Optimizer
{
    class Parser_UseShortAddress : IOptimizeParser
    {
        public void Parse(List<INefItem> Items)
        {
            for (int x = 0; x < Items.Count; x++)
            {
                var inst = Items[x] as NefInstruction;
                if (inst == null)
                    continue;
                if (inst.OpCode == OpCode.PUSHA)
                    continue;
                if (inst.AddressSize != 4)
                    continue;

                bool smallAddr = false;

                int[] oldaddr = new int[inst.AddressCountInData];
                for (var i = 0; i < inst.AddressCountInData; i++)
                {
                    var addr = inst.GetAddressInData(i);
                    oldaddr[i] = addr;
                    if (addr >= sbyte.MinValue || addr <= sbyte.MaxValue)
                    {
                        smallAddr = true;
                    }
                    else
                    {
                        smallAddr = false;
                        break;
                    }
                }
                if (smallAddr)
                {
                    var newcode = (OpCode)(((byte)inst.OpCode) - 1);
                    inst.SetOpCode(newcode);
                }// Remove _L

            }
        }
    }
}
