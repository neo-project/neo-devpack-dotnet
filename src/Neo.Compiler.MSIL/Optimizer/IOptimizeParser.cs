using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.Optimizer
{
    public interface IOptimizeParser
    {
        bool NeedRightAddr
        {
            get;
        }
        void Parse(List<INefItem> Items)
        {

        }
    }
}
