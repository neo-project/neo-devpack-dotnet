// Copyright (C) 2015-2026 The Neo Project.
//
// RpcStoreHttpResponseTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Persistence;
using Neo.SmartContract.Testing.Storage.Rpc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Neo.SmartContract.Testing.UnitTests.Storage;

[TestClass]
public class RpcStoreHttpResponseTests
{
    [TestMethod]
    public void TryGetRejectsEmptyResponseWithMethodContext()
    {
        using var server = new RpcResponseServer(string.Empty);
        var store = new RpcStore(server.Url);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            store.TryGet([0, 0, 0, 1, 2], out _));

        StringAssert.Contains(exception.Message, "getstorage");
        StringAssert.Contains(exception.Message, "empty");
    }

    [TestMethod]
    public void TryGetRejectsInvalidJsonWithMethodContext()
    {
        using var server = new RpcResponseServer("not-json");
        var store = new RpcStore(server.Url);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            store.TryGet([0, 0, 0, 1, 2], out _));

        StringAssert.Contains(exception.Message, "getstorage");
        StringAssert.Contains(exception.Message, "invalid JSON");
        Assert.IsInstanceOfType<JsonException>(exception.InnerException);
    }

    [TestMethod]
    public void FindRejectsUnsuccessfulHttpStatusWithMethodContext()
    {
        using var server = new RpcResponseServer("""{"error":"temporarily unavailable"}""", "503 Service Unavailable");
        var store = new RpcStore(server.Url);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            store.Find([0, 0, 0, 1, 2], SeekDirection.Forward).ToArray());

        StringAssert.Contains(exception.Message, "findstorage");
        StringAssert.Contains(exception.Message, "503");
        StringAssert.Contains(exception.Message, "Service Unavailable");
    }

    private sealed class RpcResponseServer : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly Task _requestTask;

        public RpcResponseServer(string responseBody, string statusLine = "200 OK")
        {
            _listener = new TcpListener(IPAddress.Loopback, 0);
            _listener.Start();
            var port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            Url = new Uri($"http://localhost:{port}/");
            _requestTask = Task.Run(async () =>
            {
                using var client = await _listener.AcceptTcpClientAsync();
                await using var stream = client.GetStream();
                var buffer = new byte[4096];
                await stream.ReadAtLeastAsync(buffer, 1, throwOnEndOfStream: false);
                var bytes = Encoding.UTF8.GetBytes(responseBody);
                var headers = Encoding.ASCII.GetBytes(
                    $"HTTP/1.1 {statusLine}\r\nContent-Type: application/json\r\nContent-Length: {bytes.Length}\r\nConnection: close\r\n\r\n");
                await stream.WriteAsync(headers, 0, headers.Length);
                await stream.WriteAsync(bytes, 0, bytes.Length);
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
