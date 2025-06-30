using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace Company.SmartContract
{
    [DisplayName("Nep11Contract")]
    [ManifestExtra("Author", "Your Name")]
    [ManifestExtra("Email", "your.email@example.com")]
    [ManifestExtra("Description", "A NEP-11 Non-Fungible Token")]
    [SupportedStandards("NEP-11")]
    [ContractPermission("*", "*")]
    public class Nep11Contract : Nep11Token<TokenState>
    {
        #region Owner

        private const byte Prefix_Owner = 0xff;

        [InitialValue("NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB", ContractParameterType.Hash160)]
        private static readonly UInt160 InitialOwner = default;

        [Safe]
        public static UInt160 GetOwner()
        {
            var currentOwner = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Owner });
            return currentOwner?.Length == 20 ? (UInt160)currentOwner : InitialOwner;
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public static void SetOwner(UInt160 newOwner)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No authorization.");
            if (newOwner != null && newOwner.IsValid)
            {
                Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Owner }, newOwner);
            }
        }

        #endregion

        #region Basic

        private const byte Prefix_TokenId = 0x02;

        [Safe]
        public static new string Symbol() => "NEP11";

        [Safe]
        public static new byte Decimals() => 0;

        public static void _deploy(object data, bool update)
        {
            if (update) return;
            
            var tx = (Transaction)Runtime.ScriptContainer;
            var owner = tx.Sender;
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Owner }, owner);
            
            // Initialize token counter
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TokenId }, 0);
        }

        #endregion

        #region NEP-11

        public static void Mint(UInt160 to, string name, string description, string image, Map<string, object> attributes)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No authorization.");
            
            if (to is null || !to.IsValid)
                throw new Exception("Invalid 'to' address");
            
            var tokenId = GenerateTokenId();
            var token = new TokenState
            {
                Owner = to,
                Name = name,
                Description = description,
                Image = image,
                Attributes = attributes
            };
            
            Mint(tokenId, token);
        }

        public static void Burn(ByteString tokenId)
        {
            var token = GetToken(tokenId);
            if (!Runtime.CheckWitness(token.Owner))
                throw new InvalidOperationException("No authorization.");
            
            Burn(tokenId);
        }

        [Safe]
        public static new Map<string, object> Properties(ByteString tokenId)
        {
            var token = GetToken(tokenId);
            var map = new Map<string, object>
            {
                ["name"] = token.Name,
                ["description"] = token.Description,
                ["image"] = token.Image
            };
            
            // Add custom attributes
            var attributeIterator = token.Attributes.Find();
            while (attributeIterator.Next())
            {
                map[attributeIterator.Key] = attributeIterator.Value;
            }
            
            return map;
        }

        private static ByteString GenerateTokenId()
        {
            var key = new byte[] { Prefix_TokenId };
            var id = Storage.Get(Storage.CurrentContext, key);
            var newId = (BigInteger)id + 1;
            Storage.Put(Storage.CurrentContext, key, newId);
            return Utility.StrictUTF8.Encode(newId.ToString());
        }

        #endregion

        #region Admin

        public static void Update(ByteString nefFile, string manifest)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No authorization.");
            ContractManagement.Update(nefFile, manifest, null);
        }

        public static void Destroy()
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No authorization.");
            ContractManagement.Destroy();
        }

        #endregion
    }

    public class TokenState : Nep11TokenState
    {
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public Map<string, object> Attributes { get; set; } = new();
    }
}