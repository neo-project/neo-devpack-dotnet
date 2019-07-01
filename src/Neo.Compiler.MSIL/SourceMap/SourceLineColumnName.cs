namespace Neo.Compiler.SourceMap
{
    public class SourceLineColumnName
    {
        public string source { get; set; }
        public int line { get; set; }
        public int column { get; set; }
        public string name { get; set; }

        public SourceLineColumnName()
        {
        }

        public SourceLineColumnName(string source, int line, int column, string name)
        {
            this.source = source;
            this.line = line;
            this.column = column;
            this.name = name;
        }

        public SourceLineColumnName(SourceLineColumnName other)
        {
            source = other.source;
            line = other.line;
            column = other.column;
            name = other.name;
        }
    }
}