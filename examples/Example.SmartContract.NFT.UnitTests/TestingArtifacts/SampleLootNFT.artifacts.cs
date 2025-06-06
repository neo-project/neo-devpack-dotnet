using System;
using System.ComponentModel;
using System.Numerics;

using Neo.SmartContract.Testing;
using Neo.Extensions;

namespace Neo.SmartContract.Testing.Sample
{
    /// <summary>
    /// Sample Loot NFT contract artifact for unit testing.
    /// This is a manually created artifact since the automated generation fails due to compiler issues.
    /// </summary>
    [DisplayName("SampleLootNFT")]
    public abstract class SampleLootNFT(Neo.SmartContract.Testing.SmartContractInitialize initialize) : SmartContract(initialize)
    {
        #region Compiled data

        public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{
  ""name"": ""SampleLootNFT"",
  ""groups"": [],
  ""features"": {},
  ""supportedstandards"": [""NEP-11""],
  ""abi"": {
    ""methods"": [
      {
        ""name"": ""symbol"",
        ""parameters"": [],
        ""returntype"": ""String"",
        ""offset"": 0,
        ""safe"": true
      },
      {
        ""name"": ""decimals"",
        ""parameters"": [],
        ""returntype"": ""Integer"",
        ""offset"": 7,
        ""safe"": true
      },
      {
        ""name"": ""totalSupply"",
        ""parameters"": [],
        ""returntype"": ""Integer"",
        ""offset"": 9,
        ""safe"": true
      },
      {
        ""name"": ""balanceOf"",
        ""parameters"": [
          {
            ""name"": ""owner"",
            ""type"": ""Hash160""
          }
        ],
        ""returntype"": ""Integer"",
        ""offset"": 35,
        ""safe"": true
      },
      {
        ""name"": ""tokensOf"",
        ""parameters"": [
          {
            ""name"": ""owner"",
            ""type"": ""Hash160""
          }
        ],
        ""returntype"": ""InteropInterface"",
        ""offset"": 61,
        ""safe"": true
      },
      {
        ""name"": ""ownerOf"",
        ""parameters"": [
          {
            ""name"": ""tokenId"",
            ""type"": ""ByteString""
          }
        ],
        ""returntype"": ""Hash160"",
        ""offset"": 87,
        ""safe"": true
      },
      {
        ""name"": ""properties"",
        ""parameters"": [
          {
            ""name"": ""tokenId"",
            ""type"": ""ByteString""
          }
        ],
        ""returntype"": ""Map"",
        ""offset"": 113,
        ""safe"": true
      },
      {
        ""name"": ""tokens"",
        ""parameters"": [],
        ""returntype"": ""InteropInterface"",
        ""offset"": 139,
        ""safe"": true
      },
      {
        ""name"": ""transfer"",
        ""parameters"": [
          {
            ""name"": ""to"",
            ""type"": ""Hash160""
          },
          {
            ""name"": ""tokenId"",
            ""type"": ""ByteString""
          },
          {
            ""name"": ""data"",
            ""type"": ""Any""
          }
        ],
        ""returntype"": ""Boolean"",
        ""offset"": 165,
        ""safe"": false
      }
    ],
    ""events"": [
      {
        ""name"": ""Transfer"",
        ""parameters"": [
          {
            ""name"": ""from"",
            ""type"": ""Hash160""
          },
          {
            ""name"": ""to"",
            ""type"": ""Hash160""
          },
          {
            ""name"": ""amount"",
            ""type"": ""Integer""
          },
          {
            ""name"": ""tokenId"",
            ""type"": ""ByteString""
          }
        ]
      }
    ]
  },
  ""permissions"": [
    {
      ""contract"": ""*"",
      ""methods"": [""onNEP11Payment""]
    }
  ],
  ""trusts"": [],
  ""extra"": {
    ""Author"": ""core-dev"",
    ""Description"": ""This is a text Example.SmartContract.NFT"",
    ""Email"": ""dev@neo.org"",
    ""SourceCode"": ""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""
  }
}");

        /// <summary>
        /// Optimization: ""
        /// </summary>
        public static readonly Neo.SmartContract.NefFile Nef = Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1YAtxYAgAAAAwIVGVzdCBORlQMCFRlc3QgTkZUQfgn7IyqJgASTTQDRTbKaBBNNANFOkBYAgAAAEH4J+yMqnBoQc/nR5ZAWAIAAABBm/ZnzkHQKvUgWAMAAABBm/ZnzkGSXegxcGhBz+dHlkACAAAAQZv2Z85B9rRr4kEtUQgwStgmBEUQSmjKQeY/GIRAVgEMCFRlc3QgTkZUWNgkCUrBRVOLUEGSXegxWNgkCUrBRVOLUEEvWMXtWEH2tGviQTDgVfFAWAMAAABBm/ZnzkGSXegxcGhBz+dHlkAMBHRlc3RAWAYAAABBm/ZnzkGSXegxcGhBz+dHlkBXAAJAVgEMAWQMCFRlc3QgTkZUWNgkCUrBRVOLUEGSXegxWNgkCUrBRVOLUEEvWMXtWEH2tGviQTDgVfFAWAcAAABBm/ZnzkGSXegxcGhBz+dHlkBXAAJAVgEMAWkMCFRlc3QgTkZUWNgkCUrBRVOLUEGSXegxWNgkCUrBRVOLUEEvWMXtWEH2tGviQTDgVfFAWAkAAABBm/ZnzkGSXegxcGhBz+dHlkBXAAJA").AsSerializable<Neo.SmartContract.NefFile>();

        #endregion

        #region Properties

        /// <summary>
        /// Safe property
        /// </summary>
        public abstract string Symbol { [DisplayName("symbol")] get; }

        /// <summary>
        /// Safe property
        /// </summary>
        public abstract BigInteger Decimals { [DisplayName("decimals")] get; }

        /// <summary>
        /// Safe property
        /// </summary>
        public abstract BigInteger TotalSupply { [DisplayName("totalSupply")] get; }

        #endregion

        #region Safe methods

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("balanceOf")]
        public abstract BigInteger BalanceOf(UInt160? owner);

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("ownerOf")]
        public abstract UInt160? OwnerOf(byte[]? tokenId);

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("properties")]
        public abstract IDictionary<object, object>? Properties(byte[]? tokenId);

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("tokens")]
        public abstract object? Tokens();

        /// <summary>
        /// Safe method
        /// </summary>
        [DisplayName("tokensOf")]
        public abstract object? TokensOf(UInt160? owner);

        #endregion

        #region Unsafe methods

        /// <summary>
        /// Unsafe method
        /// </summary>
        [DisplayName("transfer")]
        public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

        #endregion

        #region Events

        public delegate void delTransfer(UInt160? from, UInt160? to, BigInteger? amount, byte[]? tokenId);

        [DisplayName("Transfer")]
        public event delTransfer? OnTransfer;

        #endregion
    }
} 