using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Native
    {
        public class NEO
        {
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string Name(string method = "name", object[] arguments = null);

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string Symbol(string method = "symbol", object[] arguments = null);

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern int Decimals(string method = "decimals", object[] arguments = null);

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern BigInteger TotalSupply(string method = "totalSupply", object[] arguments = null);

            /// <param name="method">Please input &quot;BalanceOf&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern BigInteger BalanceOf(string method, byte[] scriptHash);

            /// <param name="method">Please input &quot;Transfer&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern bool Transfer(string method, byte[] from, byte[] to, BigInteger amount);

            /// <param name="method">Please input &quot;RegisterCandidate&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern bool RegisterCandidate(string method, byte[] publicKey);

            /// <param name="method">Please input &quot;UnregisterCandidate&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern bool UnregisterCandidate(string method, byte[] publicKey);

            /// <param name="method">Please input &quot;Vote&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern bool Vote(string method, byte[] scriptHash, byte[][] candidatePublicKey);

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern byte[][] GetValidators(string method = "GetValidators", object[] arguments = null);

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern byte[][] GetCandidates(string method = "GetCandidates", object[] arguments = null);

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern byte[][] GetCommittee(string method = "GetCommittee", object[] arguments = null);

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern byte[][] GetNextBlockValidators(string method = "GetNextBlockValidators", object[] arguments = null);

            /// <param name="method">Please input &quot;UnclaimedGas&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern BigInteger UnclaimedGas(string method, byte[] scriptHash, uint endBlockIndex);
        }

        public class GAS
        {
            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern string Name(string method = "name", object[] arguments = null);

            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern string Symbol(string method = "symbol", object[] arguments = null);

            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern int Decimals(string method = "decimals", object[] arguments = null);

            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern BigInteger TotalSupply(string method = "totalSupply", object[] arguments = null);

            /// <param name="method">Please input &quot;BalanceOf&quot;</param>
            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern BigInteger BalanceOf(string method, byte[] scriptHash);

            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern bool Transfer(string method, byte[] from, byte[] to, BigInteger amount);
        }

        public class Policy
        {
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern BigInteger GetMaxTransactionsPerBlock(string method = "GetMaxTransactionsPerBlock", object[] arguments = null);

            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern BigInteger GetFeePerByte(string method = "GetFeePerByte", object[] arguments = null);

            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern byte[][] GetBlockedAccounts(string method = "GetBlockedAccounts", object[] arguments = null);

            /// <param name="method">Please input &quot;SetMaxBlockSize&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern bool SetMaxBlockSize(string method, BigInteger maxBlockSize);

            /// <param name="method">Please input &quot;SetMaxTransactionsPerBlock&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern bool SetMaxTransactionsPerBlock(string method, BigInteger maxTransactionsPerBlock);

            /// <param name="method">Please input &quot;SetFeePerByte&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern bool SetFeePerByte(string method, BigInteger feePerByte);

            /// <param name="method">Please input &quot;BlockAccount&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern bool BlockAccount(string method, byte[] scriptHash);

            /// <param name="method">Please input &quot;UnblockAccount&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern bool UnblockAccount(string method, byte[] scriptHash);
        }
    }
}
