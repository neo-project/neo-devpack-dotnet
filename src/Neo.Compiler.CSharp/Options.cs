namespace Neo.Compiler
{
    public class Options
    {
        public string? Output { get; set; }
        public bool Debug { get; set; }
        public bool NoOptimize { get; set; }
        public byte AddressVersion { get; set; }
    }
}
