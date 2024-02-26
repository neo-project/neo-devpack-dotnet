using Neo.IO;
using Neo.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Neo.SmartContract.Testing.Storage.Rpc;

public class RpcStore : IStore
{
    private int _id = 0;

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
    public ISnapshot GetSnapshot() => new RpcSnapshot(this);
    public bool Contains(byte[] key) => TryGet(key) != null;
    public void Dispose() { }

    #region Rpc calls

    public IEnumerable<(byte[] Key, byte[] Value)> Seek(byte[] key, SeekDirection direction)
    {
        if (direction is SeekDirection.Backward)
        {
            // Not implemented in RPC, we will query all the storage from the contract, and do it manually
            // it could return wrong results if we want to get data between contracts

            var prefix = key.Take(4).ToArray();
            ConcurrentDictionary<byte[], byte[]> data = new();

            // We ask for 5 bytes because the minimum prefix is one byte

            foreach (var entry in Seek(key.Take(key.Length == 4 ? 4 : 5).ToArray(), SeekDirection.Forward))
            {
                data.TryAdd(entry.Key, entry.Value);
            }

            foreach (var entry in new MemorySnapshot(data).Seek(key, direction))
            {
                yield return (entry.Key, entry.Value);
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
            var response = httpClient.PostAsync(Url, requestContent).GetAwaiter().GetResult().EnsureSuccessStatusCode();

            JObject jo = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            if (jo["result"]?.Value<JObject>() is JObject result && result["results"]?.Value<JArray>() is JArray results)
            {
                // iterate page

                var prefix = skey.ToArray().Take(4);

                foreach (JObject r in results)
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
                    errorCode == -100)
                {
                    yield break;
                }

                throw new Exception();
            }
        }

        throw new Exception();
    }

    public byte[]? TryGet(byte[] key)
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
        var response = httpClient.PostAsync(Url, requestContent).GetAwaiter().GetResult().EnsureSuccessStatusCode();

        JObject jo = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

        if (jo["result"]?.Value<string>() is string result)
        {
            // {\"jsonrpc\":\"2.0\",\"id\":1,\"result\":\"Aw==\"}

            return Convert.FromBase64String(result);
        }
        else
        {
            // {"jsonrpc":"2.0","id":3,"error":{"code":-100,"message":"Unknown storage","data":" ...

            if (jo["error"]?.Value<JObject>() is JObject error &&
                error["code"]?.Value<int>() is int errorCode &&
                errorCode == -100)
            {
                return null;
            }

            throw new Exception();
        }
    }

    #endregion
}
