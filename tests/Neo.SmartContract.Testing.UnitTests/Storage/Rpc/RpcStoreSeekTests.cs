// Copyright (C) 2015-2026 The Neo Project.
//
// RpcStoreSeekTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.Persistence;
using Neo.Persistence.Providers;
using Neo.SmartContract.Testing.Storage.Rpc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Testing.UnitTests.Storage;

[TestClass]
public class RpcStoreSeekTests
{
    private static readonly byte[][] StorageKeys =
    [
        [], [0x00], [0x00, 0x00], [0x00, 0xff], [0x00, 0xff, 0x01],
        [0x10], [0x10, 0x01], [0x10, 0x02], [0x10, 0xff], [0x10, 0xff, 0x01],
        [0x11], [0x11, 0x01], [0xfe, 0xff], [0xfe, 0xff, 0x01],
        [0xff, 0x00], [0xff, 0x00, 0x01], [0xff, 0xff]
    ];

    [DataTestMethod]
    [DataRow("00", SeekDirection.Forward)]
    [DataRow("00", SeekDirection.Backward)]
    [DataRow("00FF", SeekDirection.Forward)]
    [DataRow("00FF", SeekDirection.Backward)]
    [DataRow("10", SeekDirection.Forward)]
    [DataRow("10", SeekDirection.Backward)]
    [DataRow("10FF", SeekDirection.Forward)]
    [DataRow("10FF", SeekDirection.Backward)]
    [DataRow("FEFF", SeekDirection.Forward)]
    [DataRow("FEFF", SeekDirection.Backward)]
    [DataRow("FF00", SeekDirection.Forward)]
    [DataRow("FF00", SeekDirection.Backward)]
    public void ContractPrefixQueriesMatchMemoryStore(string storagePrefix, SeekDirection direction)
    {
        using var server = new StorageRpcServer();
        using var memory = CreateMemoryStore();
        using var expectedCache = new StoreCache(memory);
        using var rpc = new RpcStore(server.Url);
        using var actualCache = new StoreCache(rpc);
        byte[] prefix = ContractKey(Convert.FromHexString(storagePrefix));

        var expected = expectedCache.Find(prefix, direction)
            .Select(entry => Format(entry.Key.ToArray(), entry.Value.ToArray())).ToArray();
        var actual = actualCache.Find(prefix, direction)
            .Select(entry => Format(entry.Key.ToArray(), entry.Value.ToArray())).ToArray();

        Assert.IsTrue(expected.Length >= 2);
        CollectionAssert.AreEqual(expected, actual);
        Assert.IsTrue(server.Requests.All(request => request.ContractId == 1));
        if (direction == SeekDirection.Backward)
        {
            Assert.IsTrue(server.Requests.All(request => request.Prefix.Length == 0));
            Assert.IsTrue(server.Requests.Any(request => request.Start > 0), "Backward scans must include later RPC pages.");
        }
        else
        {
            Assert.IsTrue(server.Requests.All(request => request.Prefix.AsSpan().SequenceEqual(Convert.FromHexString(storagePrefix))));
        }
    }

    [TestMethod]
    public void ForwardEmptyStoragePrefixReadsAllContractPages()
    {
        using var server = new StorageRpcServer();
        using var memory = CreateMemoryStore();
        using var expectedCache = new StoreCache(memory);
        using var rpc = new RpcStore(server.Url);
        using var actualCache = new StoreCache(rpc);
        byte[] prefix = ContractKey([]);

        var expected = expectedCache.Find(prefix).Select(entry => Format(entry.Key.ToArray(), entry.Value.ToArray())).ToArray();
        var actual = actualCache.Find(prefix).Select(entry => Format(entry.Key.ToArray(), entry.Value.ToArray())).ToArray();

        CollectionAssert.AreEqual(expected, actual);
        Assert.AreEqual(StorageKeys.Length, actual.Length);
        Assert.IsTrue(server.Requests.All(request => request.Prefix.Length == 0));
        CollectionAssert.AreEqual(Enumerable.Range(0, (StorageKeys.Length + 1) / 2).Select(page => page * 2).ToArray(),
            server.Requests.Select(request => request.Start).ToArray());
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("00")]
    [DataRow("1000")]
    [DataRow("10FF")]
    [DataRow("FFFF")]
    public void BackwardSeekUsesTheFullKeyAsAnUpperBound(string storageBoundary)
    {
        using var server = new StorageRpcServer();
        using var memory = CreateMemoryStore();
        using var rpc = new RpcStore(server.Url);
        byte[] boundary = ContractKey(Convert.FromHexString(storageBoundary));

        var expected = memory.Find(boundary, SeekDirection.Backward).Select(entry => Format(entry.Key, entry.Value)).ToArray();
        var actual = rpc.Find(boundary, SeekDirection.Backward).Select(entry => Format(entry.Key, entry.Value)).ToArray();

        CollectionAssert.AreEqual(expected, actual);
        Assert.IsTrue(server.Requests.All(request => request.Prefix.Length == 0));
    }

    private static byte[] ContractKey(byte[] storageKey) => [1, 0, 0, 0, .. storageKey];
    private static string Format(byte[] key, byte[] value) => $"{Convert.ToHexString(key)}:{Convert.ToHexString(value)}";

    private static MemoryStore CreateMemoryStore()
    {
        var store = new MemoryStore();
        foreach (byte[] key in StorageKeys)
            store.Put(ContractKey(key), [0x42, .. key]);
        return store;
    }

    private sealed class StorageRpcServer : IDisposable
    {
        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource _stop = new();
        private readonly Task _serverTask;
        public ConcurrentQueue<(int ContractId, byte[] Prefix, int Start)> Requests { get; } = new();
        public Uri Url { get; }

        public StorageRpcServer()
        {
            _listener.Start();
            Url = new Uri($"http://127.0.0.1:{((IPEndPoint)_listener.LocalEndpoint).Port}/");
            _serverTask = Task.Run(Serve);
        }

        private async Task Serve()
        {
            while (!_stop.IsCancellationRequested)
            {
                using var client = await _listener.AcceptTcpClientAsync(_stop.Token);
                using var stream = client.GetStream();
                using var headers = new MemoryStream();
                int terminator = 0;
                while (terminator != 0x0d0a0d0a)
                {
                    int value = stream.ReadByte();
                    if (value < 0) throw new EndOfStreamException();
                    headers.WriteByte((byte)value);
                    terminator = (terminator << 8) | value;
                }
                string lengthHeader = Encoding.ASCII.GetString(headers.ToArray()).Split("\r\n")
                    .Single(line => line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase));
                var body = new byte[int.Parse(lengthHeader.Split(':')[1].Trim())];
                await stream.ReadExactlyAsync(body, _stop.Token);
                var request = JObject.Parse(Encoding.UTF8.GetString(body));
                if (request["method"]?.Value<string>() != "findstorage")
                    throw new InvalidOperationException("Expected findstorage.");
                var parameters = (JArray)request["params"]!;
                int contractId = int.Parse(parameters[0]!.Value<string>()!);
                byte[] prefix = Convert.FromBase64String(parameters[1]!.Value<string>()!);
                int start = int.Parse(parameters[2]!.Value<string>()!);
                Requests.Enqueue((contractId, prefix, start));

                var matches = contractId == 1
                    ? StorageKeys.Where(key => key.AsSpan().StartsWith(prefix)).OrderBy(key => key, ByteArrayComparer.Default).ToArray()
                    : [];
                var page = matches.Skip(start).Take(2).ToArray();
                var response = new JObject
                {
                    ["jsonrpc"] = "2.0",
                    ["id"] = request["id"],
                    ["result"] = new JObject
                    {
                        ["results"] = new JArray(page.Select(key => new JObject
                        {
                            ["key"] = Convert.ToBase64String(key),
                            ["value"] = Convert.ToBase64String([0x42, .. key])
                        })),
                        ["truncated"] = start + page.Length < matches.Length,
                        ["next"] = start + page.Length
                    }
                };
                byte[] responseBody = Encoding.UTF8.GetBytes(response.ToString());
                byte[] responseHeaders = Encoding.ASCII.GetBytes(
                    $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {responseBody.Length}\r\nConnection: close\r\n\r\n");
                await stream.WriteAsync(responseHeaders, _stop.Token);
                await stream.WriteAsync(responseBody, _stop.Token);
            }
        }

        public void Dispose()
        {
            _stop.Cancel();
            _listener.Stop();
            try
            {
                _serverTask.GetAwaiter().GetResult();
            }
            catch (OperationCanceledException) { }
            finally
            {
                _stop.Dispose();
            }
        }
    }
}
