using Neo.VM;
using System.Collections.Generic;
namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteDeadCode : IOptimizeParser
    {
        public bool NeedRightAddr => true;

        public void Parse(List<INefItem> Items)
        {
            List<int> touchCodes = new List<int>();
            Touch(Items, touchCodes, 0);
            touchCodes.Sort();

            // Remove useless instructions like JPMIF false xxx
            // If the previous instruction of JMPIF is PUSH, we can tell whether JMPIF is useful in advance

            for (var i = Items.Count - 1; i >= 0; i--)
            {
                if (!(Items[i] is NefInstruction inst))
                    continue;
                var addr = Items[i].Offset;
                if (!touchCodes.Contains(addr))
                    Items.RemoveAt(i);
            }
        }

        private static void Touch(List<INefItem> Items, List<int> touchCodes, int beginaddr)
        {
            //bool bTouchMode = true;

            bool findbegin = false;
            for (int x = 0; x < Items.Count; x++)
            {
                if (!(Items[x] is NefInstruction inst))
                    continue;
                if (!findbegin)
                {
                    if (inst.Offset >= beginaddr)
                        findbegin = true;
                    else
                        continue;
                }

                if (inst.OpCode == OpCode.NOP) // NOP never touch
                    continue;

                if (touchCodes.Contains(inst.Offset) == false)
                {
                    touchCodes.Add(inst.Offset);
                }

                if (inst.AddressCountInData > 0)// The instruction may contain jmp addess
                {
                    for (var i = 0; i < inst.AddressCountInData; i++)
                    {
                        var addr = inst.GetAddressInData(i) + inst.Offset;
                        if (touchCodes.Contains(addr) == false)
                        {
                            touchCodes.Add(addr);
                            Touch(Items, touchCodes, addr);
                        }
                    }
                }

                // The next instructions of them can't be executed unless other instructions jmp to here
                if (inst.OpCode == OpCode.JMP ||
                    inst.OpCode == OpCode.JMP_L ||
                    inst.OpCode == OpCode.RET ||
                    inst.OpCode == OpCode.THROW)
                {
                    return;
                }
            }
        }
    }
}
