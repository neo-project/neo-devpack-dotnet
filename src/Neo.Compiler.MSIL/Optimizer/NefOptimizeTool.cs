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
        public static byte[] Optimize(byte[] script)
        {
            return Optimize(script, new string[] { "deletedeadcode", "useshortaddress" });
        }

        /// <summary>
        /// Optimize
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parsers">Optmize parser, currently, there are four parsers:
        /// <para> deletedeadcode -- delete dead code parser, default parser</para>
        /// <para> useshortaddress -- use short address parser. eg: JMP_L -> JMP, JMPIF_L -> JMPIF, default parser</para>
        /// <para> deletenop -- delete nop parser</para>
        /// <para> deleteuselessjmp -- delete useless jmp parser, eg: JPM 2</para></param>
        /// <returns>Optimized script</returns>
        public static byte[] Optimize(byte[] script, string[] parsers)
        {
            var optimizer = new NefOptimizer();

            if (parsers.Contains("deletedeadcode"))
            {
                optimizer.AddOptimizeParser(new Parser_DeleteDeadCode());
            }
            if (parsers.Contains("useshortaddress"))
            {
                optimizer.AddOptimizeParser(new Parser_UseShortAddress());
            }

            if (parsers.Contains("deletenop"))
            {
                optimizer.AddOptimizeParser(new Parser_DeleteNop());
            }
            if (parsers.Contains("deleteuselessjmp"))
            {
                optimizer.AddOptimizeParser(new Parser_DeleteUselessJmp());
            }

            //step01 Load
            using (var ms = new MemoryStream(script))
            {
                optimizer.LoadNef(ms);
            }
            //step02 doOptimize
            optimizer.Optimize();

            //step03 link
            using (var ms = new MemoryStream())
            {
                optimizer.LinkNef(ms);
                var bytes = ms.ToArray();
                return bytes;
            }
        }
    }
}
