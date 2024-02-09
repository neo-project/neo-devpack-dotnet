using System;

namespace Neo.Optimizer
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StrategyAttribute : Attribute
    {
        public string? Name { get; init; }
        public int Priority = 0;  // greater num to be executed first
    }
    }
}
