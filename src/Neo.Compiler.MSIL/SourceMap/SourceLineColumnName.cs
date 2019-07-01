namespace Neo.Compiler.SourceMap
{
    public class SourceLineColumnName
    {
        public string Source { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string Name { get; set; }

        public SourceLineColumnName()
        {
        }

        public SourceLineColumnName(string source, int line, int column, string name)
        {
            this.Source = source;
            this.Line = line;
            this.Column = column;
            this.Name = name;
        }

        public SourceLineColumnName(SourceLineColumnName other)
        {
            Source = other.Source;
            Line = other.Line;
            Column = other.Column;
            Name = other.Name;
        }
    }
}