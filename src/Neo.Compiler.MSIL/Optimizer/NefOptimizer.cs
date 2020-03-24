using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler.Optimizer
{
    public class NefOptimizer
    {
        /// <summary>
        /// Instructions
        /// </summary>
        private List<INefItem> Items;

        private readonly List<IOptimizeParser> OptimizeFunctions = new List<IOptimizeParser>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="script">Script</param>
        public NefOptimizer(byte[] script = null, Dictionary<string, int> methodEntry = null)
        {
            if (script != null)
            {
                using (var ms = new MemoryStream(script))
                {
                    LoadNef(ms, methodEntry);
                }
            }
        }

        public void AddOptimizeParser(IOptimizeParser function)
        {
            OptimizeFunctions.Add(function);
        }

        /// <summary>
        /// Optimize
        /// </summary>
        public void Optimize()
        {
            bool dirty = false;
            for (var i = 0; i < OptimizeFunctions.Count; i++)
            {
                var func = OptimizeFunctions[i];
                if (dirty && func.NeedRightAddress)
                {
                    RefillAddr();
                    dirty = false;
                }
                func.Init();
                func.Parse(Items);
                if (func.WillChangeAddress)
                    dirty = true;
            }
        }

        /// <summary>
        /// Step01 Load
        /// </summary>
        /// <param name="stream">Stream</param>
        public void LoadNef(Stream stream, Dictionary<string, int> methodEntry = null)
        {
            //read all Instruction to listInst
            var listInst = new List<NefInstruction>();
            //read all Address to listAddr
            var mapLabel = new Dictionary<int, NefLabel>();
            int labelindex = 1;

            NefInstruction inst;
            do
            {
                inst = NefInstruction.ReadFrom(stream);
                if (inst != null)
                {
                    listInst.Add(inst);
                    for (var i = 0; i < inst.AddressCountInData; i++)
                    {
                        // mapping addr to label
                        int addr = inst.GetAddressInData(i) + inst.Offset;
                        if (!mapLabel.ContainsKey(addr))
                        {
                            var labelname = "label" + labelindex.ToString("D06");
                            labelindex++;
                            var label = new NefLabel(labelname, addr);
                            mapLabel.Add(addr, label);
                        }

                        inst.Labels[i] = mapLabel[addr].Name;
                    }
                }
            } while (inst != null);

            //Add Labels
            Items = new List<INefItem>();
            foreach (var instruction in listInst)
            {
                var curOffset = instruction.Offset;
                if (mapLabel.ContainsKey(curOffset))
                {
                    Items.Add(mapLabel[curOffset]);
                }
                Items.Add(instruction);

                if (methodEntry.ContainsValue(instruction.Offset))
                {
                    var methodName = methodEntry.FirstOrDefault(q => q.Value == instruction.Offset).Key;
                    instruction.IsMethodEntry = true;
                    instruction.MethodName = methodName;
                }
            }
        }

        /// <summary>
        /// Step03 Link
        /// </summary>
        void RefillAddr()
        {
            var mapLabel2Addr = new Dictionary<string, uint>();
            //Recalc Address
            //collection Labels and Resort Offset
            uint offset = 0;
            foreach (var item in Items)
            {
                if (item is NefInstruction inst)
                {
                    inst.SetOffset((int)offset);
                    offset += inst.Size;
                }
                else if (item is NefLabel label)
                {
                    label.SetOffset((int)offset);
                    mapLabel2Addr[label.Name] = offset;
                }
            }

            //ChangeAddress
            foreach (var item in Items)
            {
                if (item is NefInstruction inst)
                {
                    for (var i = 0; i < inst.AddressCountInData; i++)
                    {
                        var label = inst.Labels[i];
                        var addr = (int)mapLabel2Addr[label] - inst.Offset;
                        inst.SetAddressInData(i, addr);
                    }
                }
            }
        }

        public void LinkNef(Stream stream, NeoModule module)
        {
            //Recalc Address
            //collection Labels and Resort Offset
            RefillAddr();

            //and Link
            //and find method entry offset
            Dictionary<string, int> methodEntry = new Dictionary<string, int>();
            foreach (var inst in this.Items)
            {
                if (inst is NefInstruction i)
                {
                    i.WriteTo(stream);
                    if (i.IsMethodEntry == true)
                        methodEntry.TryAdd(i.MethodName, i.Offset);
                }
            }

            if (module != null)
            {
                foreach (var function in module.mapMethods)
                {
                    if (methodEntry.ContainsKey(function.Value.displayName))
                    {
                        function.Value.funcaddr = methodEntry[function.Value.displayName];
                    }
                    else
                    {
                        function.Value.funcaddr = -1;
                    }
                }
            }
        }
    }
}
