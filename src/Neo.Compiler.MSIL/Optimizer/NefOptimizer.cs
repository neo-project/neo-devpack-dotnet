using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler.Optimizer
{
    public class NefOptimizer
    {
        private List<NefMethod> Methods;

        private readonly List<IOptimizeParser> OptimizeFunctions = new List<IOptimizeParser>();

        public int[] GetEntryPoint()
        {
            int[] methodoffset = new int[Methods.Count];
            for (var i = 0; i < Methods.Count; i++)
            {
                methodoffset[i] = Methods[i].Offset;
            }
            return methodoffset;
        }

        public Dictionary<int, int> GetAddrConvertTable()
        {
            var addrConvertTable = new Dictionary<int, int>();
            foreach (var m in Methods)
            {
                foreach (var item in m.Items)
                {
                    addrConvertTable[item.OffsetInit] = item.Offset;
                }
            }
            return addrConvertTable;
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
                foreach (var m in Methods)
                {
                    func.Parse(m.Items);
                }
                if (func.WillChangeAddress)
                    dirty = true;
            }
        }

        /// <summary>
        /// Step01 Load
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="entryPoints">Entry points</param>
        public void LoadNef(Stream stream, int[] entryPoints)
        {
            // Read all Instruction to listInst.
            var listInst = new List<NefInstruction>();
            // Read all Address to listAddr.
            var mapLabel = new Dictionary<int, NefLabel>();
            int labelindex = 1;

            // Insert EntryPoint Label.
            for (var i = 0; i < entryPoints.Length; i++)
            {
                if (i > 0 && entryPoints[i - 1] == entryPoints[i])
                {
                    // Same EntryPoints are not allowed
                    throw new Exception("Same EntryPoints are not allowed.");
                }
                if (!mapLabel.ContainsKey(entryPoints[i]))
                {
                    var labelname = "method" + i.ToString("D04");
                    var addr = entryPoints[i];
                    var label = new NefLabel(labelname, addr, true);
                    mapLabel.Add(addr, label);
                }
            }
            if (!mapLabel.ContainsKey(0))
            {
                var labelname = "method_zero";
                var label = new NefLabel(labelname, 0, true);
                mapLabel.Add(0, label);
            }

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

            // Add Labels and split to Methods
            Methods = new List<NefMethod>();

            var curMethod = new NefMethod();
            if (listInst.Count == 0)//空也要插一个标签
            {
                curMethod.Items.Add(mapLabel[0]);
            }
            else
            {
                foreach (var instruction in listInst)
                {
                    var curOffset = instruction.Offset;
                    if (mapLabel.ContainsKey(curOffset))
                    {
                        var label = mapLabel[curOffset];
                        if (label.IsEntryPoint && curMethod.Items.Count > 0)
                        {
                            Methods.Add(curMethod);
                            curMethod = new NefMethod();
                        }
                        curMethod.Items.Add(mapLabel[curOffset]);
                    }
                    curMethod.Items.Add(instruction);
                }
            }
            Methods.Add(curMethod);
        }

        internal Dictionary<int, int> RebuildAddrConvertTable(Dictionary<int, int> addrConvertTable, Dictionary<int, int> addrConvertTableTemp)
        {
            for (int i = 0; i < addrConvertTable.Count; i++)
            {
                var findFlag = false;
                var kvp = addrConvertTable.ElementAt(i);
                foreach (var kvpTemp in addrConvertTableTemp)
                {
                    if (kvp.Value == kvpTemp.Key)
                    {
                        addrConvertTable[kvp.Key] = addrConvertTableTemp[kvpTemp.Key];
                        findFlag = true;
                        break;
                    }
                }
                if (!findFlag)
                    addrConvertTable.Remove(kvp.Key);
            }
            return addrConvertTable;
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
            foreach (var m in Methods)
            {
                foreach (var item in m.Items)
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
            }

            //ChangeAddress
            foreach (var m in Methods)
            {
                foreach (var item in m.Items)
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
        }

        public void LinkNef(Stream stream)
        {
            //Recalc Address
            //collection Labels and Resort Offset
            RefillAddr();

            //and Link
            foreach (var m in Methods)
            {
                foreach (var inst in m.Items)
                {
                    if (inst is NefInstruction i)
                    {
                        i.WriteTo(stream);
                    }
                }
            }
        }
    }
}
