using Neo.Compiler;
using Neo.IO.Json;
using Neo.SmartContract;
using System.IO;

namespace Neo.TestingEngine
{
    public class BuildScript
    {
        public bool Success { get; internal set; }
        public NefFile Nef { get; protected set; }
        public JObject Manifest { get; protected set; }
        public JObject DebugInfo { get; protected set; }

        public BuildScript(NefFile nefFile, JObject manifestJson)
        {
            Nef = nefFile;
            Manifest = manifestJson;
        }

        internal static BuildScript Build(string filename)
        {
            BuildScript script;
            if (Path.GetExtension(filename).ToLowerInvariant() == ".nef")
            {
                var fileNameManifest = filename;
                using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
                {
                    NefFile neffile = new NefFile();
                    neffile.Deserialize(reader);
                    fileNameManifest = fileNameManifest.Replace(".nef", ".manifest.json");
                    string manifestFile = File.ReadAllText(fileNameManifest);
                    script = new BuildScript(neffile, JObject.Parse(manifestFile))
                    {
                        Success = true
                    };
                }
            }
            else
            {
                NefFile nef = null;
                JObject manifest = null;
                JObject debuginfo = null;
                CompilationContext context = CompilationContext.CompileSources(new[] { filename }, new Options
                {
                    AddressVersion = ProtocolSettings.Default.AddressVersion
                });

                if (context.Success)
                {
                    nef = context.CreateExecutable();
                    manifest = context.CreateManifest();
                    debuginfo = context.CreateDebugInformation();
                }

                script = new BuildScript(nef, manifest)
                {
                    Success = context.Success,
                    DebugInfo = debuginfo
                };
            }

            return script;
        }
    }
}
