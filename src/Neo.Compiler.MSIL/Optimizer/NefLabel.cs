using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Neo.Compiler.Optimizer
{
    [DebuggerDisplay("Label:Name={Name}: = {Offset}")]

    class NefLabel :INefItem
    {
        public NefLabel(string name,int Offset)
        {
            this.Name = name;
            this.Offset = Offset;
        }
        public string Name
        {
            get;
            set;
        }
        public int Offset { get; private set; }
        public void SetOffset(int offset)
        {
            this.Offset = offset;
        }
    }
}
