// Copyright (C) 2015-2025 The Neo Project.
//
// RpcStore.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Extensions;
using Neo.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Neo.SmartContract.Testing.Storage.Rpc;

public class RpcStore : IStore
{
    private int _id = 0;

    /// <summary>
    /// Event raised when a new snapshot is created
    /// </summary>
    public event IStore.OnNewSnapshotDelegate? OnNewSnapshot;

    /// <summary>
    /// Url
    /// </summary>
    public Uri Url { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="url">Url</param>
    public RpcStore(Uri url)
    {
        Url = url;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="url">Url</param>
    public RpcStore(string url) : this(new Uri(url)) { }

    public void Delete(byte[] key) => throw new NotImplementedException();
    public void Put(byte[] key, byte[] value) => throw new NotImplementedException();
    public IStoreSnapshot GetSnapshot()
    {
        var snapshot = new RpcSnapshot(this);
        OnNewSnapshot?.Invoke(this, snapshot);
        return snapshot;
    }
    public bool Contains(byte[] key) => TryGet(key) != null;
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #region Rpc calls

    // Same logic as MemorySnapshot
    private static IEnumerable<(byte[] Key, byte[] Value)> Find(ConcurrentDictionary<byte[], byte[]> innerData, byte[]? keyOrPrefix, SeekDirection direction = SeekDirection.Forward)
    {
        keyOrPrefix ??= [];
        if (direction == SeekDirection.Backward && keyOrPrefix.Length == 0) yield break;

        var comparer = direction == SeekDirection.Forward ? ByteArrayComparer.Default : ByteArrayComparer.Reverse;
        IEnumerable<KeyValuePair<byte[], byte[]>> records = innerData;
        if (keyOrPrefix.Length > 0)
            records = records
                .Where(p => comparer.Compare(p.Key, keyOrPrefix) >= 0);
        records = records.OrderBy(p => p.Key, comparer);
        foreach (var pair in records)
            yield return (pair.Key[..], pair.Value[..]);
    }

    public IEnumerable<(byte[] Key, byte[] Value)> Find(byte[]? key, SeekDirection direction)
    {
        // This(IStore.Seek) is different from LevelDbStore, RocksDbStore and MemoryStore.
        ArgumentNullException.ThrowIfNull(key);

        // This(IStore.Seek) is different from LevelDbStore, RocksDbStore and MemoryStore.
        // The following logic has this requirement.
        if (key.Length < 4)
            throw new ArgumentException("Key must be at least 4 bytes(the first 4 bytes are the contract id)", nameof(key));

        if (direction is SeekDirection.Backward)
        {
            // Not implemented in RPC, we will query all the storage from the contract, and do it manually
            // it could return wrong results if we want to get data between contracts
            ConcurrentDictionary<byte[], byte[]> data = new();

            // We ask for 5 bytes because the minimum prefix is one byte
            foreach (var (Key, Value) in Find([.. key.Take(key.Length == 4 ? 4 : 5)], SeekDirection.Forward))
            {
                data.TryAdd(Key, Value);
            }

            foreach (var (Key, Value) in Find(data, key, direction))
            {
                yield return (Key, Value);
            }

            yield break;
        }

        var skey = new StorageKey(key);
        var start = 0;

        while (true)
        {
            var requestBody = new
            {
                jsonrpc = "2.0",
                method = "findstorage",
                @params = new string[] { skey.Id.ToString(), Convert.ToBase64String(skey.Key.ToArray()), start.ToString() },
                id = _id = Interlocked.Increment(ref _id),
            };

            using var httpClient = new HttpClient();
            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(Url, requestContent).GetAwaiter().GetResult();

            JObject jo = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            if (jo["result"]?.Value<JObject>() is JObject result && result["results"]?.Value<JArray>() is JArray results)
            {
                // iterate page

                var prefix = skey.ToArray().Take(4);

                foreach (JObject r in results.Cast<JObject>())
                {
                    if (r["key"]?.Value<string>() is string jkey &&
                        r["value"]?.Value<string>() is string kvalue)
                    {
                        yield return (prefix.Concat(Convert.FromBase64String(jkey)).ToArray(), Convert.FromBase64String(kvalue));
                    }
                }

                if (result["truncated"]?.Value<bool>() == true &&
                    result["next"]?.Value<int>() is int next)
                {
                    start = next;
                }
                else
                {
                    yield break;
                }
            }
            else
            {
                // {"jsonrpc":"2.0","id":3,"error":{"code":-100,"message":"Unknown storage","data":" ...

                if (jo["error"]?.Value<JObject>() is JObject error &&
                    error["code"]?.Value<int>() is int errorCode &&
                    (errorCode == -100 || errorCode == -104))
                {
                    yield break;
                }

                throw new Exception();
            }
        }

        throw new Exception();
    }

    public bool TryGet(byte[] key, [NotNullWhen(true)] out byte[]? value)
    {
        var skey = new StorageKey(key);
        var requestBody = new
        {
            jsonrpc = "2.0",
            method = "getstorage",
            @params = new string[] { skey.Id.ToString(), Convert.ToBase64String(skey.Key.ToArray()) },
            id = _id = Interlocked.Increment(ref _id),
        };

        using var httpClient = new HttpClient();
        var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = httpClient.PostAsync(Url, requestContent).GetAwaiter().GetResult();

        JObject jo = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

        if (jo["result"]?.Value<string>() is string result)
        {
            // {\"jsonrpc\":\"2.0\",\"id\":1,\"result\":\"Aw==\"}

            value = Convert.FromBase64String(result);
            return true;
        }
        else
        {
            // {"jsonrpc":"2.0","id":3,"error":{"code":-100,"message":"Unknown storage","data":" ...

            if (jo["error"]?.Value<JObject>() is JObject error &&
                error["code"]?.Value<int>() is int errorCode &&
                (errorCode == -100 || errorCode == -104))
            {
                value = null;
                return false;
            }

            throw new Exception();
        }
    }

    public byte[]? TryGet(byte[] key)
    {
        if (TryGet(key, out var value))
        {
            return value;
        }

        return null;
    }

    #endregion
}
