using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Neo.BuildTasks
{
    public class NeoContractInterface : Task
    {
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(ManifestFile))
            {
                Log.LogError("Invalid ManifestFile " + ManifestFile);
            }
            else
            {
                var manifest = NeoManifest.Load(ManifestFile);
                var source = ContractGenerator.GenerateContractInterface(manifest, RootNamespace);
                if (!string.IsNullOrEmpty(source))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(this.OutputFile));
                    FileOperationWithRetry(() => File.WriteAllText(this.OutputFile, source));
                }
            }
            return !Log.HasLoggedErrors;
        }

        [Required]
        public string OutputFile { get; set; } = "";

        [Required]
        public string ManifestFile { get; set; } = "";

        public string RootNamespace { get; set; } = "";

        static void FileOperationWithRetry(Action operation)
        {
            const int ProcessCannotAccessFileHR = unchecked((int)0x80070020);

            for (int retriesLeft = 6; retriesLeft > 0; retriesLeft--)
            {
                try
                {
                    operation();
                }
                catch (IOException ex) when (ex.HResult == ProcessCannotAccessFileHR && retriesLeft > 0)
                {
                    System.Threading.Tasks.Task.Delay(100).Wait();
                    continue;
                }
            }
        }
    }
}
