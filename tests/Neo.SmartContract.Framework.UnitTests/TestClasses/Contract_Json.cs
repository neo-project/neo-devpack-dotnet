using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Json : SmartContract.Framework.SmartContract
    {
        public static string Serialize(object obj)
        {
            return Json.Serialize(obj);
        }

        public static object Deserialize(string json)
        {
            return Json.Deserialize(json);
        }
    }
}
