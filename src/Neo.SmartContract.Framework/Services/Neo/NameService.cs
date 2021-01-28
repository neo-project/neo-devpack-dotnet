#pragma warning disable CS0626

using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xa2b524b68dfe43a9d56af84f443c6b9843b8028c")]
    public class NameService
    {
        // NonfungibleToken

        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 owner);
        public static extern UInt160 OwnerOf(string name);
        public static extern Map<string, object> Properties(string name);
        public static extern Iterator<string> Tokens();
        public static extern Iterator<string> TokensOf(UInt160 owner);
        public static extern bool Transfer(UInt160 from, UInt160 to, string name);

        // NNS

        public static extern bool IsAvailable(string name);
        public static extern bool Register(string name, UInt160 owner);
        public static extern uint Renew(string name);
        public static extern void SetAdmin(string name, UInt160 admin);
        public static extern void SetRecord(string name, RecordType type, string data);
        public static extern string GetRecord(string name, RecordType type);
        public static extern void DeleteRecord(string name, RecordType type);
        public static extern string Resolve(string name, RecordType type);
        public static extern long GetPrice();
    }
}
