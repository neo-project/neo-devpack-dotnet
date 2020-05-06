namespace Neo.Compiler.Optimizer
{
    public interface INefItem
    {
        /// <summary>
        /// Nef Offset
        /// </summary>
        public int Offset { get; }

        public int OffsetInit { get; }

    }
}
