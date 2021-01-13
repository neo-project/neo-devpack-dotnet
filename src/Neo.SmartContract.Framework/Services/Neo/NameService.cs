#pragma warning disable CS0626

using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xa92fbe5bf164170a624474841485b20b45a26047")]
    public class NameService
    {
        // NonfungibleToken

        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 owner);
        public static extern UInt160 OwnerOf(byte[] tokenId);
        public static extern ByteString Properties(byte[] tokenId);
        public static extern Iterator<byte[]> TokensOf(UInt160 owner);
        public static extern bool Transfer(UInt160 from, UInt160 to, byte[] tokenId);

        // NNS

        public static extern bool IsAvailable(string name);
        public static extern bool Register(string name, UInt160 owner);
        public static extern uint Renew(string name);
        public static extern void SetAdmin(string name, UInt160 admin);
        public static extern void SetRecord(string name, RecordType type, string data);
        public static extern string GetRecord(string name, RecordType type);
        public static extern void DeleteRecord(string name, RecordType type);
        public static extern string Resolve(string name, RecordType type);
        public static extern void AddRoot(string root);
        public static extern long GetPrice();
        public static extern void SetPrice(long price);
    }
}
