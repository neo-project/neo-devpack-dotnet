using Neo.IO;
using Neo.Persistence;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.SmartContract.Testing.Storage
{
    public class EngineCheckpoint
    {
        /// <summary>
        /// Data
        /// </summary>
        public (byte[] key, byte[] value)[] Data { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="snapshot">Snapshot</param>
        public EngineCheckpoint(DataCache snapshot)
        {
            var list = new List<(byte[], byte[])>();

            foreach (var entry in snapshot.Seek(Array.Empty<byte>(), SeekDirection.Forward))
            {
                list.Add((entry.Key.ToArray(), entry.Value.ToArray()));
            }

            Data = list.ToArray();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
        public EngineCheckpoint(Stream stream)
        {
            var list = new List<(byte[], byte[])>();
            var buffer = new byte[sizeof(int)];

            while (stream.Read(buffer) == sizeof(int))
            {
                var length = BinaryPrimitives.ReadInt32LittleEndian(buffer);
                var key = new byte[length];

                if (stream.Read(key) != length) break;
                if (stream.Read(buffer) != sizeof(int)) break;

                length = BinaryPrimitives.ReadInt32LittleEndian(buffer);
                var data = new byte[length];

                if (stream.Read(data) != length) break;

                list.Add((key, data));
            }

            Data = list.ToArray();
        }

        /// <summary>
        /// Restore
        /// </summary>
        /// <param name="snapshot">Snapshot</param>
        public void Restore(DataCache snapshot)
        {
            // Clean snapshot

            foreach (var entry in snapshot.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray())
            {
                snapshot.Delete(entry.Key);
            }

            // Restore

            foreach (var entry in Data)
            {
                snapshot.Add(new StorageKey(entry.key), new StorageItem(entry.value));
            }
        }

        /// <summary>
        /// To Array
        /// </summary>
        /// <returns>binary data</returns>
        public byte[] ToArray()
        {
            using var ms = new MemoryStream();
            Write(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Write to Stream
        /// </summary>
        public void Write(Stream stream)
        {
            var buffer = new byte[sizeof(int)];

            foreach (var entry in Data)
            {
                BinaryPrimitives.WriteInt32LittleEndian(buffer, entry.key.Length);
                stream.Write(buffer);
                stream.Write(entry.key);

                BinaryPrimitives.WriteInt32LittleEndian(buffer, entry.value.Length);
                stream.Write(buffer);
                stream.Write(entry.value);
            }
        }
    }
}
