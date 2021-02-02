#pragma warning disable CS0626

using System.Numerics;
using Neo.Cryptography.ECC;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xf61eebf573ea36593fd43aa150c055ad7906ab83")]
    public class NEO
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 account);
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data = null);

        public static extern BigInteger GetGasPerBlock();
        public static extern BigInteger UnclaimedGas(UInt160 account, uint end);

        public static extern bool RegisterCandidate(ECPoint pubkey);
        public static extern bool UnRegisterCandidate(ECPoint pubkey);
        public static extern bool Vote(UInt160 account, ECPoint voteTo);
        public static extern (ECPoint, BigInteger)[] GetCandidates();
        public static extern ECPoint[] GetCommittee();
        public static extern ECPoint[] GetNextBlockValidators();
    }
}
