using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Compiler.Optimizer
{
    public static class NefOptimizeTool
    {
        /// <summary>
        /// Optimize
        /// </summary>
        /// <param name="script">Script</param>
        /// <returns>Optimized script</returns>
        public static byte[] Optimize(byte[] script, NeoModule module = null)
        {
            return Optimize(script, new OptimizeParserType[] { OptimizeParserType.DELETE_DEAD_CODDE, OptimizeParserType.USE_SHORT_ADDRESS }, module);
        }

        /// <summary>
        /// Optimize
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parsers">Optmize parser, currently, there are four parsers:
        /// <para> DELETE_DEAD_CODDE -- delete dead code parser, default parser</para>
        /// <para> USE_SHORT_ADDRESS -- use short address parser. eg: JMP_L -> JMP, JMPIF_L -> JMPIF, default parser</para>
        /// <para> DELETE_NOP -- delete nop parser</para>
        /// <para> DELETE_USELESS_JMP -- delete useless jmp parser, eg: JPM 2</para></param>
        /// <returns>Optimized script</returns>
        public static byte[] Optimize(byte[] script, OptimizeParserType[] parserTypes, NeoModule module = null)
        {
            var optimizer = new NefOptimizer();

            foreach (var parserType in parserTypes)
            {
                object[] objAttrs = parserType.GetType().GetField(parserType.ToString()).GetCustomAttributes(typeof(OptimizeParserAttribute), false);
                if (objAttrs is null || objAttrs.Length == 0) continue;

                var attribute = (OptimizeParserAttribute)objAttrs[0];
                var obj = Activator.CreateInstance(attribute.Type);
                if (obj is null) continue;
                IOptimizeParser parser = (IOptimizeParser)obj;
                optimizer.AddOptimizeParser(parser);
            }

            //find method entry
            Dictionary<string, int> methodEntry = new Dictionary<string, int>();
            if (module != null)
            {
                foreach (var function in module.mapMethods)
                {
                    var mm = function.Value;
                    if (mm.inSmartContract == false || mm.isPublic == false)
                        continue;
                    if (methodEntry.ContainsKey(function.Value.displayName))
                        throw new Exception("not allow same name functions");

                    methodEntry.Add(function.Value.displayName, function.Value.funcaddr);
                }
            }

            //step01 Load
            using (var ms = new MemoryStream(script))
            {
                optimizer.LoadNef(ms, methodEntry);
            }
            //step02 doOptimize
            optimizer.Optimize();

            //step03 link
            using (var ms = new MemoryStream())
            {
                optimizer.LinkNef(ms, module);
                var bytes = ms.ToArray();
                return bytes;
            }
        }
    }
}
