using System.Diagnostics;

namespace Neo.Compiler.Optimizer
{
    [DebuggerDisplay("FuncName={FuncName}, Offset={Offset}")]
    class NefFuncAddr : INefItem
    {
        /// <summary>
        /// Name
        /// </summary>
        public string FuncName { get; set; }

        /// <summary>
        /// Offset
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="funcName">funcName</param>
        /// <param name="Offset">Offset</param>
        public NefFuncAddr(string funcName, int Offset)
        {
            this.FuncName = funcName;
            this.Offset = Offset;
        }

        /// <summary>
        /// Set offset
        /// </summary>
        /// <param name="offset">Offset</param>
        public void SetOffset(int offset)
        {
            this.Offset = offset;
        }
    }
}
