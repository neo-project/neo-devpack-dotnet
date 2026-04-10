using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace Neo.SmartContract.Template
{
    [DisplayName(nameof(Nep11Contract))]
    [ContractAuthor("<Your Name Or Company Here>", "<Your Public Email Here>")]
    [ContractDescription("<Description Here>")]
    [ContractVersion("<Version String Here>")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep11/Nep11Contract.cs")]
    [ContractPermission(Permission.Any, Method.OnNEP11Payment)]
    [SupportedStandards(NepStandard.Nep11)]
    public class Nep11Contract : Neo.SmartContract.Framework.Nep11Token<TokenState>
    {
        #region Owner

        private const byte Prefix_Owner = 0xff;

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner });
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160? previousOwner, UInt160? newOwner);

        [DisplayName("SetOwner")]
        public static event OnSetOwnerDelegate OnSetOwner = null!;

        public static void SetOwner(UInt160 newOwner)
        {
            if (!IsOwner())
                throw new InvalidOperationException("No Authorization!");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, "owner must be valid");

            UInt160? previousOwner = GetOwner();
            ExecutionEngine.Assert(previousOwner != newOwner, "owner must change");

            Storage.Put(new[] { Prefix_Owner }, newOwner);
            OnSetOwner(previousOwner, newOwner);
        }

        #endregion

        #region NEP11

        public override string Symbol { [Safe] get => "EXAMPLE"; }

        public static ByteString Mint(UInt160 to, string name, string description, string image)
        {
            if (!IsOwner())
                throw new InvalidOperationException("No Authorization!");

            ExecutionEngine.Assert(to.IsValid && !to.IsZero, "recipient must be valid");

            ByteString tokenId = NewTokenId();
            Nep11Token<TokenState>.Mint(tokenId, new TokenState
            {
                Owner = to,
                Name = name,
                Description = description,
                Image = image
            });

            return tokenId;
        }

        [Safe]
        public override Map<string, object> Properties(ByteString tokenId)
        {
            if (tokenId.Length >= 64) throw new Exception("The argument \"tokenId\" should be 64 or less bytes long.");

            var tokenMap = new LocalStorageMap(Prefix_Token);
            var tokenKey = tokenMap[tokenId] ?? throw new Exception("The token with given \"tokenId\" does not exist.");
            TokenState token = (TokenState)StdLib.Deserialize(tokenKey);

            return new Map<string, object>()
            {
                ["name"] = token.Name,
                ["description"] = token.Description,
                ["image"] = token.Image
            };
        }

        #endregion

        [Safe]
        public static bool Verify() => IsOwner();

        public static string MyMethod()
        {
            return Storage.Get("Hello");
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                return;
            }

            if (data is null) data = Runtime.Transaction.Sender;
            UInt160 initialOwner = (UInt160)data;

            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, "owner must exists");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            OnSetOwner(null, initialOwner);
            Storage.Put("Hello", "World");
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (!IsOwner())
                throw new InvalidOperationException("No authorization.");
            ContractManagement.Update(nefFile, manifest, data);
        }
    }

    public class TokenState : Nep11TokenState
    {
        public string Description = string.Empty;
        public string Image = string.Empty;
    }
}
