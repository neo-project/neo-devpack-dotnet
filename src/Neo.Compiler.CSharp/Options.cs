namespace Neo.Compiler
{
    public class Options
    {
        public string? Output { get; set; }
        public bool Debug { get; set; }
        public bool Assembly { get; set; }
        public bool NoOptimize { get; set; }
        public bool NoInline { get; set; }
        public byte AddressVersion { get; set; }
    }
}
