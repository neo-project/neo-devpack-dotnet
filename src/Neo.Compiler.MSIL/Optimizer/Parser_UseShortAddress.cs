using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    class Parser_UseShortAddress : IOptimizeParser
    {
        public bool NeedRightAddr => false;

        private bool Is8BitAddress(NefInstruction inst)
        {
            if (inst.AddressCountInData == 9)
                return false;

            for (var i = 0; i < inst.AddressCountInData; i++)
            {
                var addr = inst.GetAddressInData(i);
                if (addr >= sbyte.MinValue && addr <= sbyte.MaxValue)
                {
                    //smallAddr = true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void Parse(List<INefItem> Items)
        {
            for (int x = 0; x < Items.Count; x++)
            {
                if (!(Items[x] is NefInstruction inst))
                    continue;
                if (inst.OpCode == OpCode.PUSHA) //PUSHA 没有8bit对应指令
                    continue;
                if (inst.AddressSize != 4)
                    continue;

                if (Is8BitAddress(inst))
                {
                    var newcode = (OpCode)(((byte)inst.OpCode) - 1);
                    inst.SetOpCode(newcode);

                    // That's OK
                    //No need to do anything here.

                    //Link will fill right Address
                }
            }
        }
    }
}
