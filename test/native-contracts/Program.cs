using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Neo.BuildTasks;
using SimpleJSON;

namespace Neo
{
    class UInt160 {}
    class UInt256 {}

    namespace Cryptography.ECC
    {
        class ECPoint {}
    }
}

namespace native_contracts
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var requestJson = new JSONObject();
            requestJson.Add("jsonrpc", "2.0");
            requestJson.Add("id", 1);
            requestJson.Add("method", "getnativecontracts");
            requestJson.Add("params", new JSONArray());
            var requestContent = new ByteArrayContent(Encoding.UTF8.GetBytes(requestJson.ToString()));

            var client = new HttpClient();
            var response = await client.PostAsync("http://seed5t.neo.org:20332", requestContent);
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            var responseJson = SimpleJSON.JSONObject.Parse(Encoding.UTF8.GetString(responseBytes));
            
            if (!System.IO.Directory.Exists("./out")) System.IO.Directory.CreateDirectory("./out");
            var results = (JSONArray)responseJson["result"];
            foreach (var result in results)
            {
                var manifest = NeoManifest.FromManifestJson((JSONObject)result.Value["manifest"]);
                var source = ContractGenerator.GenerateContractInterface(manifest, "Neo.Native");
                await System.IO.File.WriteAllTextAsync($"./out/{manifest.Name}.cs", source);
            }
        }
    }
}
