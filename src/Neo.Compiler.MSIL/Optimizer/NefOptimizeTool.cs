using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.Optimizer
{
    public static class NefOptimizeTool
    {
        private static NefOptimizer _optimizer;
        public static byte[] Optimize(byte[] script)
        {
            if (_optimizer == null)
            {
                _optimizer = new NefOptimizer();
                _optimizer.AddOptimizeParser(new Parser_DeleteNop());
                _optimizer.AddOptimizeParser(new Parser_UseShortAddress());
            }
            //step01 Load
            using (var ms = new System.IO.MemoryStream(script))
            {
                _optimizer.LoadNef(ms);
            }
            //step02 doOptimize
            _optimizer.Optimize();
            //step03 link
            using (var ms = new System.IO.MemoryStream())
            {
                _optimizer.LinkNef(ms);
                var bytes = ms.ToArray();
                return bytes;
            }
        }
    }
}
