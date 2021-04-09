namespace Neo.SmartContract.Framework
{
    public class Nep11TokenState
    {
        public UInt160 Owner;
        public string Name;

        public virtual Map<string, object> GetProperties()
        {
            Map<string, object> map = new();
            map["name"] = Name;
            return map;
        }
    }
}
