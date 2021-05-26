using Neo.Cryptography.ECC;
using System.Numerics;

namespace Neo.SmartContract.Framework.Native
{
    public class NeoAccountState
    {
        public readonly BigInteger Balance;
        public readonly BigInteger Height;
        public readonly ECPoint VoteTo;
    }
}
