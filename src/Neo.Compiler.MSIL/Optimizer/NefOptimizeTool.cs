using System.IO;

namespace Neo.Compiler.Optimizer
{
    public static class NefOptimizeTool
    {
        private static NefOptimizer _optimizer;

        /// <summary>
        /// Optimize
        /// </summary>
        /// <param name="script">Script</param>
        /// <returns>Optimized script</returns>
        public static byte[] Optimize(byte[] script)
        {
            if (_optimizer == null)
            {
                _optimizer = new NefOptimizer();

                _optimizer.AddOptimizeParser(new Parser_DeleteNop());
                _optimizer.AddOptimizeParser(new Parser_DeleteDeadCode());
                _optimizer.AddOptimizeParser(new Parser_UseShortAddress());
            }

            //step01 Load
            using (var ms = new MemoryStream(script))
            {
                _optimizer.LoadNef(ms);
            }
            //step02 doOptimize
            _optimizer.Optimize();

            //step03 link
            using (var ms = new MemoryStream())
            {
                _optimizer.LinkNef(ms);
                var bytes = ms.ToArray();
                return bytes;
            }
        }
    }
}
