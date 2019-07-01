namespace Neo.Compiler.SourceMap
{
    public class SourceNodeOrSource
    {
        public SourceNode SourceNode { get; set; }
        public string Source { get; set; }

        public SourceNodeOrSource(SourceNode sourceNode)
        {
            SourceNode = sourceNode;
        }

        public SourceNodeOrSource(string source)
        {
            Source = source;
        }
    }
}