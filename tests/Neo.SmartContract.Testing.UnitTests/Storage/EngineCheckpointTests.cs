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
using Neo.Persistence;
using Neo.Persistence.Providers;
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

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        [DataRow(11)]
        [DataRow(12)]
        [DataRow(13)]
        [DataRow(14)]
        [DataRow(15)]
        public void TruncatedRecordRejectsCheckpointBeforeRestore(int retainedRecordBytes)
        {
            using var complete = CreateTwoRecordStream();
            using var truncated = new MemoryStream(complete.ToArray()[..(16 + retainedRecordBytes)]);
            using var store = new MemoryStore();
            byte[] originalKey = [1, 0, 0, 0, 0x20];
            byte[] originalValue = [0x42];
            store.Put(originalKey, originalValue);
            using var snapshot = new StoreCache(store);

            Assert.ThrowsException<EndOfStreamException>(() => new EngineCheckpoint(truncated).Restore(snapshot));

            var entries = new EngineCheckpoint(snapshot).Data;
            Assert.AreEqual(1, entries.Length);
            CollectionAssert.AreEqual(originalKey, entries[0].key);
            CollectionAssert.AreEqual(originalValue, entries[0].value);
        }

        [TestMethod]
        public void EmptyCheckpointAndRecordBoundaryEofAreAccepted()
        {
            using var empty = new MemoryStream();
            Assert.AreEqual(0, new EngineCheckpoint(empty).Data.Length);

            using var complete = CreateTwoRecordStream();
            using var firstRecord = new MemoryStream(complete.ToArray()[..16]);
            var checkpoint = new EngineCheckpoint(firstRecord);
            Assert.AreEqual(1, checkpoint.Data.Length);
            CollectionAssert.AreEqual(new byte[] { 1, 0, 0, 0, 0x10 }, checkpoint.Data[0].key);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, checkpoint.Data[0].value);
        }

        [TestMethod]
        public void CheckpointSupportsZeroLengthFields()
        {
            using var stream = new MemoryStream(new byte[8]);
            var checkpoint = new EngineCheckpoint(stream);

            Assert.AreEqual(1, checkpoint.Data.Length);
            Assert.AreEqual(0, checkpoint.Data[0].key.Length);
            Assert.AreEqual(0, checkpoint.Data[0].value.Length);
        }

        [TestMethod]
        public void CheckpointReadsNonSeekableStreamInSmallSegments()
        {
            using var complete = CreateTwoRecordStream();
            using var segmented = new SegmentedReadStream(complete.ToArray());
            var checkpoint = new EngineCheckpoint(segmented);

            Assert.AreEqual(2, checkpoint.Data.Length);
            CollectionAssert.AreEqual(complete.ToArray(), checkpoint.ToArray());
        }

        private static MemoryStream CreateTwoRecordStream()
        {
            return CreateStream(writer =>
            {
                foreach (byte suffix in new byte[] { 0x10, 0x11 })
                {
                    WriteLength(writer, 5);
                    writer.Write(new byte[] { 1, 0, 0, 0, suffix });
                    WriteLength(writer, 3);
                    writer.Write(new byte[] { 1, 2, 3 });
                }
            });
        }

        private sealed class SegmentedReadStream(byte[] data) : Stream
        {
            private readonly MemoryStream _stream = new(data);
            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }
            public override int Read(byte[] buffer, int offset, int count) =>
                _stream.Read(buffer, offset, Math.Min(count, 2));
            public override int Read(Span<byte> buffer) => _stream.Read(buffer[..Math.Min(buffer.Length, 2)]);
            public override void Flush() => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            protected override void Dispose(bool disposing)
            {
                if (disposing) _stream.Dispose();
                base.Dispose(disposing);
            }
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
