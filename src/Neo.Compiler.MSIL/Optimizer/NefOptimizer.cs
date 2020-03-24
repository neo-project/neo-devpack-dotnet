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

        public Dictionary<string, uint> MapLabels = new Dictionary<string, uint>();
        public Dictionary<string, uint> MapLabelsOptimized = new Dictionary<string, uint>();
        
        public Dictionary<uint,uint> GetAddrConvertTable()
        {
            Dictionary<uint, uint> result = new Dictionary<uint, uint>();
            foreach (var key in MapLabels.Keys)
            {
                if(MapLabelsOptimized.ContainsKey(key))
                {
                    result[MapLabels[key]] = MapLabelsOptimized[key];
                }
            }
            return   result;
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
        
        public void LoadNef(Stream stream)
        {
            MapLabels.Clear();
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

                            MapLabels[labelname] = (uint)addr;
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
            }
        }

        /// <summary>
        /// Step03 Link
        /// </summary>
        void RefillAddr()
        {
            MapLabelsOptimized.Clear();

            //var mapLabel2Addr = new Dictionary<string, uint>();
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
                    //mapLabel2Addr[label.Name] = offset;
                    MapLabelsOptimized[label.Name] = offset;
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
                        var addr = (int)MapLabelsOptimized[label] - inst.Offset;
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
            foreach (var inst in this.Items)
            {
                if (inst is NefInstruction i)
                {
                    i.WriteTo(stream);
                }
            }

        }
    }
}
