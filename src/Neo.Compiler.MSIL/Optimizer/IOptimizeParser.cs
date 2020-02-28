using System.Collections.Generic;

namespace Neo.Compiler.Optimizer
{
    public interface IOptimizeParser
    {
        bool NeedRightAddress { get; }
        bool WillChangeAddress { get; }

        /// <summary>
        /// Parse items
        /// </summary>
        /// <param name="Items">Items</param>
        void Parse(List<INefItem> Items);
    }
}
