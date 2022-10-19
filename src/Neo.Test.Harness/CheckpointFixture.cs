using System;
using System.IO;
using System.IO.Abstractions;
using Neo;
using Neo.BlockchainToolkit;
using Neo.BlockchainToolkit.Models;
using Neo.BlockchainToolkit.Persistence;
using Neo.Persistence;

namespace NeoTestHarness
{
    public abstract class CheckpointFixture : IDisposable
    {
        readonly static Lazy<IFileSystem> defaultFileSystem = new Lazy<IFileSystem>(() => new FileSystem());
        readonly CheckpointStore checkpointStore;

        public IReadOnlyStore CheckpointStore => checkpointStore;
        public ProtocolSettings ProtocolSettings => checkpointStore.Settings;

        public CheckpointFixture(string checkpointPath)
        {
            if (Path.IsPathFullyQualified(checkpointPath))
            {
                if (!File.Exists(checkpointPath)) throw new FileNotFoundException("couldn't find checkpoint", checkpointPath);
            }
            else
            {
                var directory = Path.GetFullPath(".");
                var tempPath = Path.GetFullPath(checkpointPath, directory);
                while (!File.Exists(tempPath))
                {
                    directory = Path.GetDirectoryName(directory);
                    tempPath = Path.GetFullPath(checkpointPath, directory!);
                }
                checkpointPath = tempPath;
            }

            checkpointStore = new CheckpointStore(checkpointPath);
        }

        public void Dispose()
        {
            checkpointStore.Dispose();
        }

        public ExpressChain FindChain(string fileName = Constants.DEFAULT_EXPRESS_FILENAME, IFileSystem? fileSystem = null, string? searchFolder = null)
            => (fileSystem ?? defaultFileSystem.Value).FindChain(fileName, searchFolder);
    }
}

