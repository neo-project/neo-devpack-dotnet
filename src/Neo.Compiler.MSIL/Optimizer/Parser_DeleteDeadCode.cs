using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    class Parser_DeleteDeadCode : IOptimizeParser
    {
        private uint OptimizedCount = 0;

        public bool NeedRightAddress => true;
        public bool WillChangeAddress => OptimizedCount > 0;

        public void Init()
        {
            OptimizedCount = 0;
        }

        public void Parse(List<INefItem> items)
        {
            var reachableAddrs = new HashSet<int>();
            Touch(items, reachableAddrs, 0);

            // Remove useless instructions like JPMIF false xxx
            // If the previous instruction of JMPIF is PUSH, we can tell whether JMPIF is useful in advance

            for (var i = items.Count - 1; i >= 0; i--)
            {
                if (!(items[i] is NefInstruction inst))
                    continue;
                if (!reachableAddrs.Contains(inst.Offset))
                {
                    items.RemoveAt(i);
                    OptimizedCount++;
                }
            }
        }

        private static void Touch(List<INefItem> items, HashSet<int> reachableAddrs, int beginAddr)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!(items[i] is NefInstruction inst)) continue;
                if (inst.Offset < beginAddr) continue;
                if (inst.OpCode == OpCode.NOP) continue; // NOP never touch

                reachableAddrs.Add(inst.Offset);

                // Try is not linear. If encounter a try, skip to the catch and finally segments to scan.
                // If encounter endtry, will also skip to finally segment to scan

                if (inst.AddressCountInData > 0) // The instruction may contain jmp addess
                {
                    for (var j = 0; j < inst.AddressCountInData; j++)
                    {
                        var addr = inst.GetAddressInData(j) + inst.Offset;
                        if (reachableAddrs.Add(addr))
                        {
                            Touch(items, reachableAddrs, addr); // goto the JMP/call/... new address
                        }
                    }
                }

                // The next instructions of them can't be executed unless other instructions jmp to here

                if (inst.OpCode == OpCode.JMP ||
                    inst.OpCode == OpCode.JMP_L ||
                    inst.OpCode == OpCode.RET ||
                    inst.OpCode == OpCode.THROW ||
                    inst.OpCode == OpCode.ENDTRY ||
                    inst.OpCode == OpCode.ENDTRY_L ||
                    inst.OpCode == OpCode.ENDFINALLY)
                {
                    return;
                }
            }
        }
    }
}
