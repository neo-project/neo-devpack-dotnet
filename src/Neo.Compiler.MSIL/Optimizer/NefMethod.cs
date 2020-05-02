using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.Optimizer
{
    public class NefMethod
    {
        public string Name
        {
            get
            {
                return (Items[0] as NefLabel).Name;
            }
        }
        public int Offset
        {
            get
            {
                return (Items[0] as NefLabel).Offset;
            }
        }
        public List<INefItem> Items = new List<INefItem>();
    }
}
