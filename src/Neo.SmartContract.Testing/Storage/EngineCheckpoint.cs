// Copyright (C) 2015-2026 The Neo Project.
//
// EngineCheckpoint.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Extensions;
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
        private const int MaxCheckpointKeyLength = 1024 * 1024;
        private const int MaxCheckpointValueLength = 16 * 1024 * 1024;

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

            while (TryReadExactly(stream, buffer))
            {
                var key = ReadCheckpointField(stream, buffer, MaxCheckpointKeyLength, "key");
                stream.ReadExactly(buffer);

                var data = ReadCheckpointField(stream, buffer, MaxCheckpointValueLength, "value");

                list.Add((key, data));
            }

            Data = list.ToArray();
        }

        /// <summary>
        /// Load checkpoint from file
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>EngineCheckpoint</returns>
        public static EngineCheckpoint Load(string path)
        {
            using var stream = File.OpenRead(path);
            return new EngineCheckpoint(stream);
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
        /// Save checkpoint to file
        /// </summary>
        /// <param name="path">File path</param>
        public void Save(string path)
        {
            using var stream = File.Create(path);
            Write(stream);
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

        private static byte[] ReadCheckpointField(Stream stream, byte[] lengthBuffer, int maxLength, string fieldName)
        {
            var length = BinaryPrimitives.ReadInt32LittleEndian(lengthBuffer);
            if (length < 0 || length > maxLength)
                throw new InvalidDataException($"Invalid checkpoint {fieldName} length: {length}.");

            var data = new byte[length];
            stream.ReadExactly(data);
            return data;
        }

        private static bool TryReadExactly(Stream stream, Span<byte> buffer)
        {
            var offset = 0;
            while (offset < buffer.Length)
            {
                var read = stream.Read(buffer[offset..]);
                if (read == 0)
                {
                    if (offset == 0) return false;
                    throw new EndOfStreamException("Incomplete checkpoint key length.");
                }
                offset += read;
            }

            return true;
        }
    }
}
