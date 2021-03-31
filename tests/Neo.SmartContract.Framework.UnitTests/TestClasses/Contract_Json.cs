using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Json : SmartContract
    {
        public static string Serialize(object obj)
        {
            return StdLib.JsonSerialize(obj);
        }

        public static object Deserialize(string json)
        {
            return StdLib.JsonDeserialize(json);
        }
    }
}
