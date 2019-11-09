using CommandLine;
using System.Collections.Generic;

namespace Neo.Compiler
{
    public class CmdOptions
    {
        [Option('f', "file", Required = true, HelpText = "Specify the file to compile to NEF file")]
        public string Filename { get; set; }

        [Option('r', "references", Required = false, HelpText = "Compilation references")]
        public IEnumerable<string> References { get; set; }
    }
}
