using Neo.Json;

namespace Neo.Compiler
{
    /// <summary>
    /// Which file and method in the compiler generates the OpCode
    /// </summary>
    public class CompilerLocation
    {
        public int Line { get; set; }
        public required string Method { get; set; }
        public required string File { get; set; }

        /// <summary>
        /// Convert to Json
        /// </summary>
        /// <returns>Json object</returns>
        public JObject ToJson()
        {
            var compiler = new JObject();
            compiler["file"] = File;
            compiler["line"] = Line;
            compiler["method"] = Method;
            return compiler;
        }
    }
}
