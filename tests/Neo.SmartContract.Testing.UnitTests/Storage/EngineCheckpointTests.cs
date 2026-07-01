// Copyright (C) 2015-2026 The Neo Project.
//
// EngineCheckpointTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Storage;
using System;
using System.Buffers.Binary;
using System.IO;

namespace Neo.SmartContract.Testing.UnitTests.Storage
{
    [TestClass]
    public class EngineCheckpointTests
    {
        private const int MaxCheckpointKeyLength = 1024 * 1024;
        private const int MaxCheckpointValueLength = 16 * 1024 * 1024;

        [TestMethod]
        public void LoadCheckpointRejectsNegativeKeyLength()
        {
            using var stream = CreateStream(-1);

            var exception = Assert.ThrowsException<InvalidDataException>(() => new EngineCheckpoint(stream));
            StringAssert.Contains(exception.Message, "key");
        }

        [TestMethod]
        public void LoadCheckpointRejectsOversizedKeyLength()
        {
            using var stream = CreateStream(MaxCheckpointKeyLength + 1);

            var exception = Assert.ThrowsException<InvalidDataException>(() => new EngineCheckpoint(stream));
            StringAssert.Contains(exception.Message, "key");
        }

        [TestMethod]
        public void LoadCheckpointRejectsNegativeValueLength()
        {
            using var stream = CreateStream(writer =>
            {
                WriteLength(writer, 1);
                writer.WriteByte(0x01);
                WriteLength(writer, -1);
            });

            var exception = Assert.ThrowsException<InvalidDataException>(() => new EngineCheckpoint(stream));
            StringAssert.Contains(exception.Message, "value");
        }

        [TestMethod]
        public void LoadCheckpointRejectsOversizedValueLength()
        {
            using var stream = CreateStream(writer =>
            {
                WriteLength(writer, 1);
                writer.WriteByte(0x01);
                WriteLength(writer, MaxCheckpointValueLength + 1);
            });

            var exception = Assert.ThrowsException<InvalidDataException>(() => new EngineCheckpoint(stream));
            StringAssert.Contains(exception.Message, "value");
        }

        [TestMethod]
        public void LoadCheckpointReadsValidData()
        {
            byte[] key = [0x01, 0x02, 0x03];
            byte[] value = [0x04, 0x05];
            using var stream = CreateStream(writer =>
            {
                WriteLength(writer, key.Length);
                writer.Write(key);
                WriteLength(writer, value.Length);
                writer.Write(value);
            });

            var checkpoint = new EngineCheckpoint(stream);

            Assert.AreEqual(1, checkpoint.Data.Length);
            CollectionAssert.AreEqual(key, checkpoint.Data[0].key);
            CollectionAssert.AreEqual(value, checkpoint.Data[0].value);
        }

        private static MemoryStream CreateStream(int length)
        {
            return CreateStream(writer => WriteLength(writer, length));
        }

        private static MemoryStream CreateStream(Action<MemoryStream> write)
        {
            var stream = new MemoryStream();
            write(stream);
            stream.Position = 0;
            return stream;
        }

        private static void WriteLength(Stream stream, int length)
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, length);
            stream.Write(buffer);
        }
    }
}
