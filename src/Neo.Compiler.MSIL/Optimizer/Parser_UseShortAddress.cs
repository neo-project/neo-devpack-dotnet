using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    class Parser_UseShortAddress : IOptimizeParser
    {
        private uint OptimizedCount = 0;

        public bool NeedRightAddress => false;
        public bool WillChangeAddress => OptimizedCount > 0;

        public void Init()
        {
            OptimizedCount = 0;
        }

        private bool Is8BitAddress(NefInstruction inst)
        {
            if (inst.AddressCountInData == 0)
                return false;

            for (var i = 0; i < inst.AddressCountInData; i++)
            {
                var addr = inst.GetAddressInData(i);
                if (addr < sbyte.MinValue || addr > sbyte.MaxValue)
                {
                    return false;
                }
            }

            return true;
        }

        public void Parse(List<INefItem> items)
        {
            for (int x = 0; x < items.Count; x++)
            {
                if (!(items[x] is NefInstruction inst)) continue;
                if (inst.OpCode == OpCode.PUSHA) continue; //PUSHA is not 8 bits
                if (inst.AddressSize != 4) continue;

                if (Is8BitAddress(inst))
                {
                    var newcode = (OpCode)(((byte)inst.OpCode) - 1); // Mind! The value of `inst_L` must equal the value of `inst` + 1
                    inst.SetOpCode(newcode);

                    // That's OK
                    //No need to do anything here.

                    //Link will fill right Address

                    OptimizedCount++;
                }
            }
        }
    }
}
