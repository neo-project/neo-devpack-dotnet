using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler.Optimizer
{
    public static class NefOptimizeTool
    {
        /// <summary>
        /// Optimize
        /// </summary>
        /// <param name="script">Script</param>
        /// <returns>Optimized script</returns>
        public static byte[] Optimize(byte[] script, int[] EntryPoints)
        {
            return Optimize(script, EntryPoints, out _);
        }

        public static byte[] Optimize(byte[] script, int[] EntryPoints, out Dictionary<int, int> addrConvertTable)
        {
            return Optimize(script, EntryPoints,
                OptimizeParserType.DELETE_DEAD_CODE |
                OptimizeParserType.USE_SHORT_ADDRESS |
                OptimizeParserType.DELETE_CONST_EXECUTION |
                OptimizeParserType.DELETE_USELESS_EQUAL
            , out addrConvertTable);
        }

        public static byte[] Optimize(byte[] script, int[] EntryPoints, OptimizeParserType parserTypes)
        {
            return Optimize(script, EntryPoints, parserTypes, out _);
        }

        /// <summary>
        /// Optimize
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parserTypes">Optmize parser, currently, there are four parsers:
        /// <para> DELETE_DEAD_CODDE -- delete dead code parser, default parser</para>
        /// <para> USE_SHORT_ADDRESS -- use short address parser. eg: JMP_L -> JMP, JMPIF_L -> JMPIF, default parser</para>
        /// <para> DELETE_NOP -- delete nop parser</para>
        /// <para> DELETE_USELESS_JMP -- delete useless jmp parser, eg: JPM 2</para>
        /// <para> DELETE_USELESS_EQUAL -- delete useless equal parser, eg: EQUAL 01 01 </para></param>
        /// <returns>Optimized script</returns>
        public static byte[] Optimize(byte[] script, int[] EntryPoints, OptimizeParserType parserTypes, out Dictionary<int, int> addrConvertTable)
        {
            var optimizer = new NefOptimizer();

            foreach (OptimizeParserType e in Enum.GetValues(typeof(OptimizeParserType)))
            {
                if (e == OptimizeParserType.NONE)
                    continue;
                if ((e & parserTypes) > 0)
                {
                    object[] objAttrs = typeof(OptimizeParserType).GetField(e.ToString()).GetCustomAttributes(typeof(OptimizeParserAttribute), false);
                    if (objAttrs is null || objAttrs.Length == 0) continue;
                    var attribute = (OptimizeParserAttribute)objAttrs[0];
                    var obj = Activator.CreateInstance(attribute.Type);
                    if (obj is null) continue;
                    IOptimizeParser parser = (IOptimizeParser)obj;
                    optimizer.AddOptimizeParser(parser);
                }
            }

            addrConvertTable = null;

            //优化是一个管线，一次就应该解决问题，这个循环有些蠢，另开issue讨论
            // 10 iterations max
            for (int x = 0; x < 10; x++)
            {
                //step01 Load
                using (var ms = new MemoryStream(script))
                {
                    optimizer.LoadNef(ms, EntryPoints);
                }

                //step02 doOptimize
                optimizer.Optimize();

                //step03 link
                using (var ms = new MemoryStream())
                {
                    optimizer.LinkNef(ms);

                    if (addrConvertTable is null)
                        addrConvertTable = optimizer.GetAddrConvertTable();
                    else
                    {
                        Dictionary<int, int> addrConvertTableTemp = optimizer.GetAddrConvertTable();
                        addrConvertTable = optimizer.RebuildAddrConvertTable(addrConvertTable, addrConvertTableTemp);
                    }

                    //updateEntryPoints for next loop
                    EntryPoints = optimizer.GetEntryPoint();


                    var bytes = ms.ToArray();
                    if (bytes.SequenceEqual(script))
                    {
                        // Sometimes the script could be bigger but more efficient
                        // int.Max+INC (6 bytes) => PUSHInt64 (9 bytes)
                        break;
                    }
                    script = bytes;
                }

                // Execute it while decrease the size
            }

            return script;

        }
    }
}
