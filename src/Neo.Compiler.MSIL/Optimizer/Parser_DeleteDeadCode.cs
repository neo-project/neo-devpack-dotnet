using Neo.VM;
using System.Collections.Generic;
namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteDeadCode : IOptimizeParser
    {
        public bool NeedRightAddr => true;

        public void Parse(List<INefItem> Items)
        {
            List<int> reachableAddrs = new List<int>();
            Touch(Items, reachableAddrs, 0);
            reachableAddrs.Sort();

            // Remove useless instructions like JPMIF false xxx
            // If the previous instruction of JMPIF is PUSH, we can tell whether JMPIF is useful in advance

            for (var i = Items.Count - 1; i >= 0; i--)
            {
                if (!(Items[i] is NefInstruction inst))
                    continue;
                var addr = inst.Offset;
                if (!reachableAddrs.Contains(addr))
                    Items.RemoveAt(i);
            }
        }

        private static void Touch(List<INefItem> Items, List<int> reachableAddrs, int beginAddr)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (!(Items[i] is NefInstruction inst)) continue;
                if (inst.Offset < beginAddr) continue;
                if (inst.OpCode == OpCode.NOP) continue; // NOP never touch

                if (!reachableAddrs.Contains(inst.Offset))
                {
                    reachableAddrs.Add(inst.Offset);
                }

                if (inst.AddressCountInData > 0)// The instruction may contain jmp addess
                {
                    for (var j = 0; j < inst.AddressCountInData; j++)
                    {
                        var addr = inst.GetAddressInData(j) + inst.Offset;
                        if (!reachableAddrs.Contains(addr))
                        {
                            reachableAddrs.Add(addr);
                            Touch(Items, reachableAddrs, addr); // goto the JMP/call/... new address
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
