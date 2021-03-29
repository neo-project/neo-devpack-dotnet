using Neo.IO.Json;
using Neo.SmartContract;

namespace Neo.Compiler.CSharp.UnitTests.Utils
{
    class BuildScript
    {
        public CompilationContext context;
        public NefFile nef;
        public JObject manifest;
        public void Build(string filename)
        {
            Options op = new Options();
            context = Neo.Compiler.CompilationContext.CompileSources(new string[] { filename }, op);
            manifest = context.CreateManifest();
            nef = context.CreateExecutable();
        }
    }
}
