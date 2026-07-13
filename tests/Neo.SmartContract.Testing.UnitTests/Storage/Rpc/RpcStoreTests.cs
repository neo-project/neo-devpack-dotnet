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
using Neo.SmartContract.Testing.Storage;
using Neo.SmartContract.Testing.Storage.Rpc;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Neo.SmartContract.Testing.UnitTests.Storage
{
    [TestClass]
    public class RpcStoreTests
    {
        private const string RpcUrlEnvironmentVariable = "NEO_RPC_TEST_URL";

        [TestMethod]
        public void RpcStoreMutationsExplainReadOnlyBehavior()
        {
            var store = new RpcStore("http://localhost:10332");

            var deleteException = Assert.ThrowsException<NotImplementedException>(() => store.Delete(new byte[] { 1 }));
            StringAssert.Contains(deleteException.Message, "read-only");

            var putException = Assert.ThrowsException<NotImplementedException>(() => store.Put(new byte[] { 1 }, new byte[] { 2 }));
            StringAssert.Contains(putException.Message, "read-only");
        }

        [TestMethod]
        public void RpcSnapshotCommitExplainsReadOnlyBehaviorWhenDirty()
        {
            var store = new RpcStore("http://localhost:10332");
            var snapshot = store.GetSnapshot();
            snapshot.Put(new byte[] { 1 }, new byte[] { 2 });

            var exception = Assert.ThrowsException<NotImplementedException>(() => snapshot.Commit());

            StringAssert.Contains(exception.Message, "read-only");
        }

        [TestMethod]
        public void RpcStoreTryGetUnexpectedRpcResponseIncludesErrorDetails()
        {
            using var server = new RpcResponseServer("""{"error":{"code":-500,"message":"boom","data":"details"}}""");
            var store = new RpcStore(server.Url);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => store.TryGet([0, 0, 0, 1, 2], out _));

            StringAssert.Contains(exception.Message, "getstorage");
            StringAssert.Contains(exception.Message, "code=-500");
            StringAssert.Contains(exception.Message, "message=boom");
            StringAssert.Contains(exception.Message, "data=details");
        }

        [TestMethod]
        public void RpcStoreUnexpectedRpcErrorHandlesMissingFields()
        {
            using var server = new RpcResponseServer("""{"error":{}}""");
            var store = new RpcStore(server.Url);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => store.TryGet([0, 0, 0, 1, 2], out _));

            StringAssert.Contains(exception.Message, "code=<missing>");
            StringAssert.Contains(exception.Message, "message=<missing>");
            Assert.IsFalse(exception.Message.Contains("data="));
        }

        [TestMethod]
        public void RpcStoreFindUnexpectedRpcResponseIncludesRawResponse()
        {
            using var server = new RpcResponseServer("""{"result":{"unexpected":true}}""");
            var store = new RpcStore(server.Url);

            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
                store.Find([0, 0, 0, 1, 2], SeekDirection.Forward).ToArray());

            StringAssert.Contains(exception.Message, "findstorage");
            StringAssert.Contains(exception.Message, "unexpected");
        }

        [TestMethod]
        public void RpcStoreReadsAndFindsUsingDeterministicResponses()
        {
            using var server = new RpcResponseServer(
                """{"result":"AQI="}""",
                """{"error":{"code":-100,"message":"Unknown storage"}}""",
                """{"result":"Aw=="}""",
                """{"result":{"results":[{"key":"Ag==","value":"BA=="}],"truncated":true,"next":1}}""",
                """{"result":{"results":[{"key":"Aw==","value":"BQ=="}],"truncated":false}}""",
                """{"error":{"code":-104,"message":"Unknown contract"}}""",
                """{"result":{"results":[{"key":"Ag==","value":"BA=="},{"key":"Aw==","value":"BQ=="}],"truncated":false}}""");
            var store = new RpcStore(server.Url);
            byte[] key = [0, 0, 0, 1, 2];

            Assert.IsTrue(store.TryGet(key, out var value));
            CollectionAssert.AreEqual(new byte[] { 1, 2 }, value);

            Assert.IsFalse(store.TryGet(key, out var missing));
            Assert.IsNull(missing);
            Assert.IsTrue(store.Contains(key));

            var forward = store.Find(key, SeekDirection.Forward).ToArray();
            Assert.AreEqual(2, forward.Length);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 1, 2 }, forward[0].Key);
            CollectionAssert.AreEqual(new byte[] { 4 }, forward[0].Value);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 1, 3 }, forward[1].Key);
            CollectionAssert.AreEqual(new byte[] { 5 }, forward[1].Value);

            Assert.AreEqual(0, store.Find(key, SeekDirection.Forward).Count());

            var backward = store.Find([0, 0, 0, 1, 3], SeekDirection.Backward).ToArray();
            Assert.AreEqual(2, backward.Length);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 1, 3 }, backward[0].Key);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 1, 2 }, backward[1].Key);
        }

        [TestMethod]
        public void RpcSnapshotDelegatesReadsAndTracksState()
        {
            using var server = new RpcResponseServer(
                """{"result":"AQI="}""",
                """{"result":"Aw=="}""",
                """{"error":{"code":-100,"message":"Unknown storage"}}""",
                """{"result":{"results":[{"key":"Ag==","value":"BA=="}],"truncated":false}}""");
            var store = new RpcStore(server.Url);
            IStoreSnapshot? capturedSnapshot = null;
            store.OnNewSnapshot += (_, snapshot) => capturedSnapshot = snapshot;

            using var snapshot = store.GetSnapshot();
            Assert.AreSame(snapshot, capturedSnapshot);
            snapshot.Commit();

            byte[] key = [0, 0, 0, 1, 2];
            Assert.IsTrue(snapshot.TryGet(key, out var value));
            CollectionAssert.AreEqual(new byte[] { 1, 2 }, value);
            Assert.IsTrue(snapshot.Contains(key));
            Assert.IsFalse(snapshot.Contains(key));

            var records = snapshot.Find(key, SeekDirection.Forward).ToArray();
            Assert.AreEqual(1, records.Length);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 1, 2 }, records[0].Key);
            CollectionAssert.AreEqual(new byte[] { 4 }, records[0].Value);

            snapshot.Delete(key);
            var exception = Assert.ThrowsException<NotImplementedException>(() => snapshot.Commit());
            StringAssert.Contains(exception.Message, "read-only");
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void TestRpcStore()
        {
            var rpcUrl = Environment.GetEnvironmentVariable(RpcUrlEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(rpcUrl))
            {
                Assert.Inconclusive($"Set {RpcUrlEnvironmentVariable} to a reachable Neo N3 JSON-RPC endpoint to run this integration test.");
                return;
            }

            var engine = new TestEngine(new EngineStorage(new RpcStore(rpcUrl)), false);

            // check network values

            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);
            Assert.IsTrue(engine.Native.Ledger.CurrentIndex > 3_510_270);

            // check with Seek (RPC doesn't support Backward, it could be slow)

            Assert.IsTrue(engine.Native.NEO.GasPerBlock > 0, $"Unexpected GasPerBlock: {engine.Native.NEO.GasPerBlock}");

            // check contract state round-trip through RPC-backed storage

            var state = engine.Native.ContractManagement.GetContract(engine.Native.NEO.Hash);
            Assert.IsNotNull(state);
            Assert.AreEqual(engine.Native.NEO.Hash, state!.Hash);
            Assert.AreEqual("NeoToken", state.Manifest.Name);

            var roundTrip = engine.Native.ContractManagement.GetContractById(state.Id);
            Assert.IsNotNull(roundTrip);
            Assert.AreEqual(state.Hash, roundTrip!.Hash);

            Assert.IsTrue(engine.Native.ContractManagement.HasMethod(engine.Native.NEO.Hash, "getCandidateVote", 1));
        }

        private sealed class RpcResponseServer : IDisposable
        {
            private readonly TcpListener _listener;
            private readonly Task _requestTask;

            public RpcResponseServer(params string[] responseBodies)
            {
                _listener = new TcpListener(IPAddress.Loopback, 0);
                _listener.Start();
                var port = ((IPEndPoint)_listener.LocalEndpoint).Port;
                Url = new Uri($"http://localhost:{port}/");
                _requestTask = Task.Run(async () =>
                {
                    foreach (var responseBody in responseBodies)
                    {
                        using var client = await _listener.AcceptTcpClientAsync();
                        await using var stream = client.GetStream();
                        var buffer = new byte[4096];
                        await stream.ReadAtLeastAsync(buffer, 1, throwOnEndOfStream: false);
                        var bytes = Encoding.UTF8.GetBytes(responseBody);
                        var headers = Encoding.ASCII.GetBytes(
                            $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {bytes.Length}\r\nConnection: close\r\n\r\n");
                        await stream.WriteAsync(headers, 0, headers.Length);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                    }
                });
            }

            public Uri Url { get; }

            public void Dispose()
            {
                _listener.Stop();
                try
                {
                    _requestTask.GetAwaiter().GetResult();
                }
                catch (ObjectDisposedException)
                {
                }
                catch (SocketException)
                {
                }
            }
        }
    }
}
