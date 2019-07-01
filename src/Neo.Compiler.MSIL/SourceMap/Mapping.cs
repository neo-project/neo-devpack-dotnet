namespace Neo.Compiler.SourceMap
{
    public class Mapping
    {
        public int? OriginalLine { get; set; }
        public int? OriginalColumn { get; set; }
        public int GeneratedLine { get; set; }
        public int GeneratedColumn { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
    }
}