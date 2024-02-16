using Neo.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            // TODO: not implemented in RPC

            throw new NotImplementedException();
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

            if (jo["result"]?["results"]?.Value<JArray>() is JArray results)
            {
                // iterate page

                foreach (JObject result in results)
                {
                    if (result["key"]?.Value<string>() is string jkey &&
                        result["value"]?.Value<string>() is string kvalue)
                    {
                        yield return (Convert.FromBase64String(jkey), Convert.FromBase64String(kvalue));
                    }
                }

                if (jo["truncated"]?.Value<bool>() == true &&
                    jo["next"]?.Value<int>() is int next)
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
