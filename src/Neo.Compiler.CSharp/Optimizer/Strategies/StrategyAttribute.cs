using System;

namespace Neo.Optimizer
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StrategyAttribute : Attribute
    {
        /// <summary>
        /// Strategy name
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Greater num to be executed first
        /// </summary>
        public int Priority = 0;
    }
}
