using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Template.OracleDemo.CSharp
{
    public class OracleDemo : SmartContract
    {
        public static void DoRequest()
        {
            string url = "http://127.0.0.1:8080/test";
            string filter = "$.value"; // JSONPath, { "value": "hello world" }
            string callback = "callback";
            object userdata = new byte[] { 0x00 };
            long gasForResponse = 10000000; // minimum fee 

            Oracle.Request(url, filter, callback, userdata, gasForResponse);
        }

        public static void Callback(string url, object userdata, int code, string result)
        {
            object ret = Json.Deserialize(result); // [ "hello world" ]
            object[] arr = (object[])ret;
            string content = (string)arr[0];
            Runtime.Log(content);
        }
    }
}
