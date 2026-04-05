// Copyright (C) 2015-2026 The Neo Project.
//
// RpcStoreTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Persistence;
using Neo.SmartContract.Native;
using Neo.SmartContract.Testing.Storage;
using Neo.SmartContract.Testing.Storage.Rpc;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using StorageKey = Neo.SmartContract.StorageKey;

namespace Neo.SmartContract.Testing.UnitTests.Storage
{
    [TestClass]
    public class RpcStoreTests
    {
        private readonly record struct RpcCall(string Method, string[] Parameters);

        private sealed class FakeRpcHandler(Func<RpcCall, object> responder) : HttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                string body = request.Content is null
                    ? string.Empty
                    : await request.Content.ReadAsStringAsync(cancellationToken);

                using var document = JsonDocument.Parse(body);
                string method = document.RootElement.GetProperty("method").GetString()
                    ?? throw new AssertFailedException("RPC request method is missing.");
                string[] parameters =
                [
                    ..document.RootElement.GetProperty("params")
                        .EnumerateArray()
                        .Select(static p => p.GetString() ?? string.Empty)
                ];

                var payload = responder(new RpcCall(method, parameters));
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = content
                };
            }
        }

        private static byte[] FullStorageKey(int id, params byte[] key)
        {
            return new StorageKey { Id = id, Key = key }.ToArray();
        }

        [TestMethod]
        public void TestTryGetAndContains()
        {
            const int contractId = 123;
            byte[] existingKey = FullStorageKey(contractId, 0xAA, 0xBB);
            byte[] missingKey = FullStorageKey(contractId, 0xCC);
            byte[] expectedValue = [0x01, 0x02, 0x03];

            using var client = new HttpClient(new FakeRpcHandler(call =>
            {
                Assert.AreEqual("getstorage", call.Method);
                Assert.AreEqual(contractId.ToString(), call.Parameters[0]);

                if (call.Parameters[1] == Convert.ToBase64String([0xAA, 0xBB]))
                {
                    return new { jsonrpc = "2.0", id = 1, result = Convert.ToBase64String(expectedValue) };
                }

                if (call.Parameters[1] == Convert.ToBase64String([0xCC]))
                {
                    return new
                    {
                        jsonrpc = "2.0",
                        id = 1,
                        error = new { code = -100, message = "Unknown storage" }
                    };
                }

                throw new AssertFailedException($"Unexpected key lookup: {call.Parameters[1]}");
            }));

            using var store = new RpcStore("http://localhost:20332", client);

            Assert.IsTrue(store.TryGet(existingKey, out var actualValue));
            CollectionAssert.AreEqual(expectedValue, actualValue);
            Assert.IsTrue(store.Contains(existingKey));

            Assert.IsFalse(store.TryGet(missingKey, out var missingValue));
            Assert.IsNull(missingValue);
            Assert.IsFalse(store.Contains(missingKey));
        }

        [TestMethod]
        public void TestFindSupportsPaginationAndBackwardSeek()
        {
            const int contractId = 456;
            byte[] prefix = [0x10];
            byte[] keyA = [0x10, 0x01];
            byte[] keyB = [0x10, 0x02];
            byte[] keyC = [0x10, 0x03];

            object ForwardPage(string start) => start switch
            {
                "0" => new
                {
                    jsonrpc = "2.0",
                    id = 1,
                    result = new
                    {
                        results = new[]
                        {
                            new { key = Convert.ToBase64String(keyA), value = Convert.ToBase64String(new byte[] { 0xA1 }) },
                            new { key = Convert.ToBase64String(keyB), value = Convert.ToBase64String(new byte[] { 0xB2 }) }
                        },
                        truncated = true,
                        next = 2
                    }
                },
                "2" => new
                {
                    jsonrpc = "2.0",
                    id = 1,
                    result = new
                    {
                        results = new[]
                        {
                            new { key = Convert.ToBase64String(keyC), value = Convert.ToBase64String(new byte[] { 0xC3 }) }
                        },
                        truncated = false
                    }
                },
                _ => throw new AssertFailedException($"Unexpected pagination token: {start}")
            };

            using var client = new HttpClient(new FakeRpcHandler(call =>
            {
                Assert.AreEqual("findstorage", call.Method);
                Assert.AreEqual(contractId.ToString(), call.Parameters[0]);
                Assert.AreEqual(Convert.ToBase64String(prefix), call.Parameters[1]);
                return ForwardPage(call.Parameters[2]);
            }));

            using var store = new RpcStore("http://localhost:20332", client);

            byte[] forwardPrefix = FullStorageKey(contractId, prefix);
            var forward = store.Find(forwardPrefix, SeekDirection.Forward).ToArray();
            Assert.AreEqual(3, forward.Length);
            CollectionAssert.AreEqual(FullStorageKey(contractId, keyA), forward[0].Key);
            CollectionAssert.AreEqual(new byte[] { 0xA1 }, forward[0].Value);
            CollectionAssert.AreEqual(FullStorageKey(contractId, keyB), forward[1].Key);
            CollectionAssert.AreEqual(new byte[] { 0xB2 }, forward[1].Value);
            CollectionAssert.AreEqual(FullStorageKey(contractId, keyC), forward[2].Key);
            CollectionAssert.AreEqual(new byte[] { 0xC3 }, forward[2].Value);

            byte[] backwardStart = FullStorageKey(contractId, keyB);
            var backward = store.Find(backwardStart, SeekDirection.Backward).ToArray();
            Assert.AreEqual(2, backward.Length);
            CollectionAssert.AreEqual(FullStorageKey(contractId, keyB), backward[0].Key);
            CollectionAssert.AreEqual(FullStorageKey(contractId, keyA), backward[1].Key);
        }

        [TestMethod]
        public void TestFindRangeSupportsForwardAndBackwardSeek()
        {
            const int contractId = 789;
            byte[] keyA = [0x10, 0x01];
            byte[] keyB = [0x10, 0x02];
            byte[] keyC = [0x10, 0x03];
            byte[] rangeEnd = [0x10, 0x04];

            using var client = new HttpClient(new FakeRpcHandler(call =>
            {
                Assert.AreEqual("findstorage", call.Method);
                Assert.AreEqual(contractId.ToString(), call.Parameters[0]);
                Assert.AreEqual(string.Empty, call.Parameters[1]);
                Assert.AreEqual("0", call.Parameters[2]);

                return new
                {
                    jsonrpc = "2.0",
                    id = 1,
                    result = new
                    {
                        results = new[]
                        {
                            new { key = Convert.ToBase64String(keyA), value = Convert.ToBase64String(new byte[] { 0xA1 }) },
                            new { key = Convert.ToBase64String(keyB), value = Convert.ToBase64String(new byte[] { 0xB2 }) },
                            new { key = Convert.ToBase64String(keyC), value = Convert.ToBase64String(new byte[] { 0xC3 }) }
                        },
                        truncated = false
                    }
                };
            }));

            using var store = new RpcStore("http://localhost:20332", client);

            byte[] start = FullStorageKey(contractId, keyB);
            byte[] end = FullStorageKey(contractId, rangeEnd);

            var forward = store.FindRange(start, end, SeekDirection.Forward).ToArray();
            Assert.AreEqual(2, forward.Length);
            CollectionAssert.AreEqual(FullStorageKey(contractId, keyB), forward[0].Key);
            CollectionAssert.AreEqual(new byte[] { 0xB2 }, forward[0].Value);
            CollectionAssert.AreEqual(FullStorageKey(contractId, keyC), forward[1].Key);
            CollectionAssert.AreEqual(new byte[] { 0xC3 }, forward[1].Value);

            var backward = store.FindRange(start, end, SeekDirection.Backward).ToArray();
            Assert.AreEqual(2, backward.Length);
            CollectionAssert.AreEqual(FullStorageKey(contractId, keyC), backward[0].Key);
            CollectionAssert.AreEqual(FullStorageKey(contractId, keyB), backward[1].Key);
        }

        [TestMethod]
        public void TestEngineStorageInitializationCheckUsesRpcStore()
        {
            byte[] initKey = FullStorageKey(NativeContract.ContractManagement.Id, 15);

            using var client = new HttpClient(new FakeRpcHandler(call =>
            {
                Assert.AreEqual("getstorage", call.Method);
                Assert.AreEqual(NativeContract.ContractManagement.Id.ToString(), call.Parameters[0]);
                Assert.AreEqual(Convert.ToBase64String(new byte[] { 15 }), call.Parameters[1]);

                return new { jsonrpc = "2.0", id = 1, result = Convert.ToBase64String(new byte[] { 0x01 }) };
            }));

            using var store = new RpcStore("http://localhost:20332", client);
            var engineStorage = new EngineStorage(store);

            Assert.IsTrue(engineStorage.IsInitialized);
            Assert.IsTrue(store.Contains(initKey));
        }
    }
}
