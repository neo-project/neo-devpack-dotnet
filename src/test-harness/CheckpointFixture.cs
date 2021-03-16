using System;
using System.IO;
using System.IO.Abstractions;
using Neo;
using Neo.BlockchainToolkit;
using Neo.BlockchainToolkit.Models;
using Neo.BlockchainToolkit.Persistence;

namespace NeoTestHarness
{
    public abstract class CheckpointFixture : IDisposable
    {
        readonly static Lazy<IFileSystem> defaultFileSystem = new Lazy<IFileSystem>(() => new FileSystem());
        readonly string checkpointTempPath;
        readonly RocksDbStorageProvider rocksProvider;
        public readonly ProtocolSettings ProtocolSettings;

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

            do
            {
                checkpointTempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }
            while (Directory.Exists(checkpointTempPath));

            var metadata = RocksDbStorageProvider.RestoreCheckpoint(checkpointPath, checkpointTempPath);
            this.ProtocolSettings = ProtocolSettings.Default with
            {
                Magic = metadata.magic,
                AddressVersion = metadata.addressVersion,
            };
            rocksProvider = RocksDbStorageProvider.OpenReadOnly(checkpointTempPath);
        }

        public void Dispose()
        {
            rocksProvider.Dispose();
            if (Directory.Exists(checkpointTempPath)) Directory.Delete(checkpointTempPath, true);
        }

        public CheckpointStorageProvider GetCheckpointStore()
        {
            return new CheckpointStorageProvider(rocksProvider);
        }

        public ExpressChain FindChain(string fileName = Constants.DEFAULT_EXPRESS_FILENAME, IFileSystem? fileSystem = null, string? searchFolder = null)
            => (fileSystem ?? defaultFileSystem.Value).FindChain(fileName, searchFolder);
    }
}

