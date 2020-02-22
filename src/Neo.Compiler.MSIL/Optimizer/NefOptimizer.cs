using System.Collections.Generic;
using System.IO;

namespace Neo.Compiler.Optimizer
{
    public class NefOptimizer
    {
        /// <summary>
        /// Instructions
        /// </summary>
        private List<INefItem> Items;

        private List<IOptimizeParser> OptimizeFunctions = new List<IOptimizeParser>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="script">Script</param>
        public NefOptimizer(byte[] script = null)
        {
            if (script != null)
            {
                using (var ms = new MemoryStream(script))
                {
                    LoadNef(ms);
                }
            }
        }

        public void AddOptimizeParser(IOptimizeParser function)
        {
            if (OptimizeFunctions == null)
                OptimizeFunctions = new List<IOptimizeParser>();
            OptimizeFunctions.Add(function);
        }

        /// <summary>
        /// Optimize
        /// </summary>
        public void Optimize()
        {
            if (OptimizeFunctions == null || OptimizeFunctions.Count == 0)
                return;
            for (var i = 0; i < OptimizeFunctions.Count; i++)
            {
                var func = OptimizeFunctions[i];
                if (i > 0 && func.NeedRightAddr)
                    RefillAddr();
                func.Parse(this.Items);
            }
        }

        /// <summary>
        /// Step01 Load
        /// </summary>
        /// <param name="stream">Stream</param>
        public void LoadNef(Stream stream)
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
                    if (inst.AddressCountInData > 0)
                    {
                        for (var i = 0; i < inst.AddressCountInData; i++)
                        {
                            var addr = inst.GetAddressInData(i) + inst.Offset;
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
                }
            } while (inst != null);

            //Add Labels
            if (Items == null)
                Items = new List<INefItem>();
            else
                Items.Clear();
            foreach (var instruction in listInst)
            {
                var curOffset = instruction.Offset;
                if (mapLabel.ContainsKey(curOffset))
                {
                    Items.Add(mapLabel[curOffset]);
                }
                Items.Add(instruction);
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
            uint Offset = 0;
            foreach (var item in this.Items)
            {
                if (item is NefInstruction inst)
                {
                    inst.SetOffset((int)Offset);
                    Offset += inst.CalcTotalSize;
                }
                else if (item is NefLabel label)
                {
                    label.SetOffset((int)Offset);
                    mapLabel2Addr[label.Name] = Offset;
                }
            }

            //ChangeAddress
            foreach (var item in this.Items)
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

        public void LinkNef(Stream stream)
        {
            //Recalc Address
            //collection Labels and Resort Offset
            RefillAddr();

            //and Link
            foreach (var _inst in this.Items)
            {
                if (_inst is NefInstruction)
                    NefInstruction.WriteTo(_inst as NefInstruction, stream);
            }
        }
    }
}
