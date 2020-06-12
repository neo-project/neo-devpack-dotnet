using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Native
    {
        public static class NEO
        {
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string Name(string method = "name");

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string Symbol(string method = "symbol");

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string Decimals(string method = "decimals");

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string TotalSupply(string method = "totalSupply");

            /// <param name="method">Please input &quot;BalanceOf&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string BalanceOf(string method, byte[] scriptHash);

            /// <param name="method">Please input &quot;Transfer&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string Transfer(string method, byte[] from, byte[] to, BigInteger amount);

            /// <param name="method">Please input &quot;RegisterCandidate&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string RegisterCandidate(string method, byte[] publicKey);

            /// <param name="method">Please input &quot;UnregisterCandidate&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string UnregisterCandidate(string method, byte[] publicKey);

            /// <param name="method">Please input &quot;Vote&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string Vote(string method, byte[] scriptHash, byte[][] candidatePublicKey);

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string GetValidators(string method = "GetValidators");

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string GetCandidates(string method = "GetCandidates");

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string GetCommittee(string method = "GetCommittee");

            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string GetNextBlockValidators(string method = "GetNextBlockValidators");

            /// <param name="method">Please input &quot;UnclaimedGas&quot;</param>
            [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
            public static extern string UnclaimedGas(string method, byte[] scriptHash, uint endBlockIndex);
        }

        public static class GAS
        {
            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern string Name(string method = "name");

            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern string Symbol(string method = "symbol");

            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern string Decimals(string method = "decimals");

            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern string TotalSupply(string method = "totalSupply");

            /// <param name="method">Please input &quot;BalanceOf&quot;</param>
            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern string BalanceOf(string method, byte[] scriptHash);

            [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
            public static extern string Transfer(string method, byte[] from, byte[] to, BigInteger amount);
        }

        public static class Policy
        {
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern string GetMaxTransactionsPerBlock(string method = "GetMaxTransactionsPerBlock");

            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern string GetFeePerByte(string method = "GetFeePerByte");

            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern string GetBlockedAccounts(string method = "GetBlockedAccounts");

            /// <param name="method">Please input &quot;SetMaxBlockSize&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern string SetMaxBlockSize(string method, BigInteger maxBlockSize);

            /// <param name="method">Please input &quot;SetMaxTransactionsPerBlock&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern string SetMaxTransactionsPerBlock(string method, BigInteger maxTransactionsPerBlock);

            /// <param name="method">Please input &quot;SetFeePerByte&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern string SetFeePerByte(string method, BigInteger feePerByte);

            /// <param name="method">Please input &quot;BlockAccount&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern string BlockAccount(string method, byte[] scriptHash);

            /// <param name="method">Please input &quot;UnblockAccount&quot;</param>
            [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
            public static extern string UnblockAccount(string method, byte[] scriptHash);
        }
    }
}
