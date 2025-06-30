using System.Collections.Generic;
using System.Linq;
using System;
using Neo.SmartContract.Init.Models;

namespace Neo.SmartContract.Init.Services;

public class TemplateService
{
    private readonly Dictionary<string, ProjectTemplate> _templates;

    public TemplateService()
    {
        _templates = InitializeTemplates();
    }

    public List<ProjectTemplate> GetAvailableTemplates()
    {
        return _templates.Values.ToList();
    }

    public ProjectTemplate GetTemplate(string id)
    {
        return _templates.TryGetValue(id, out var template)
            ? template
            : throw new ArgumentException($"Template '{id}' not found");
    }

    private Dictionary<string, ProjectTemplate> InitializeTemplates()
    {
        return new Dictionary<string, ProjectTemplate>
        {
            ["basic"] = new ProjectTemplate
            {
                Id = "basic",
                Description = "Basic smart contract with minimal setup",
                Features = new List<string> { "Storage", "Events", "Owner management" }
            },
            ["nep17"] = new ProjectTemplate
            {
                Id = "nep17",
                Description = "NEP-17 fungible token standard",
                Features = new List<string> { "Token transfers", "Balance tracking", "Events" }
            },
            ["nep11"] = new ProjectTemplate
            {
                Id = "nep11",
                Description = "NEP-11 non-fungible token (NFT) standard",
                Features = new List<string> { "NFT minting", "Ownership", "Metadata" }
            },
            ["oracle"] = new ProjectTemplate
            {
                Id = "oracle",
                Description = "Contract with Oracle service integration",
                Features = new List<string> { "External data", "HTTP requests", "Callbacks" }
            },
            ["multisig"] = new ProjectTemplate
            {
                Id = "multisig",
                Description = "Multi-signature wallet contract",
                Features = new List<string> { "Multiple owners", "Transaction approval", "Threshold" }
            },
            ["dao"] = new ProjectTemplate
            {
                Id = "dao",
                Description = "Decentralized Autonomous Organization",
                Features = new List<string> { "Proposals", "Voting", "Treasury", "Governance" }
            }
        };
    }

    public string GetTemplateContent(string templateId, string fileName, ProjectConfig config)
    {
        return templateId switch
        {
            "basic" => GetBasicTemplate(fileName, config),
            "nep17" => GetNep17Template(fileName, config),
            "nep11" => GetNep11Template(fileName, config),
            "oracle" => GetOracleTemplate(fileName, config),
            "multisig" => GetMultisigTemplate(fileName, config),
            "dao" => GetDaoTemplate(fileName, config),
            _ => throw new ArgumentException($"Unknown template: {templateId}")
        };
    }

    private string GetBasicTemplate(string fileName, ProjectConfig config)
    {
        return fileName switch
        {
            "Contract.cs" => $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {config.Name}
{{
    [DisplayName(""{config.Name}"")]
    [ManifestExtra(""Author"", ""{config.Author}"")]
    [ManifestExtra(""Email"", ""{config.Email}"")]
    [ManifestExtra(""Description"", ""{config.Description}"")]
    public class {config.Name} : SmartContract
    {{
        private const byte Prefix_Owner = 0x01;
        private const byte Prefix_Storage = 0x02;

        // Events
        [DisplayName(""Transfer"")]
        public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

        [DisplayName(""OwnerChanged"")]
        public static event Action<UInt160, UInt160> OnOwnerChanged;

        // Safe methods
        [Safe]
        public static UInt160 GetOwner()
        {{
            var key = new byte[] {{ Prefix_Owner }};
            var owner = Storage.Get(Storage.CurrentContext, key);
            return owner?.Length == 20 ? (UInt160)owner : UInt160.Zero;
        }}

        [Safe]
        public static string GetData(string key)
        {{
            var storageKey = Prefix_Storage.ToByteArray().Concat(key.ToByteArray());
            return Storage.Get(Storage.CurrentContext, storageKey);
        }}

        // Contract deployment
        public static void _deploy(object data, bool update)
        {{
            if (update) return;
            
            var tx = (Transaction)Runtime.ScriptContainer;
            var owner = tx.Sender;
            var key = new byte[] {{ Prefix_Owner }};
            Storage.Put(Storage.CurrentContext, key, owner);
        }}

        // Owner management
        public static bool SetOwner(UInt160 newOwner)
        {{
            var oldOwner = GetOwner();
            if (!Runtime.CheckWitness(oldOwner))
                throw new Exception(""Only owner can change ownership"");

            if (!newOwner.IsValid || newOwner.IsZero)
                throw new Exception(""Invalid new owner address"");

            var key = new byte[] {{ Prefix_Owner }};
            Storage.Put(Storage.CurrentContext, key, newOwner);
            OnOwnerChanged(oldOwner, newOwner);
            return true;
        }}

        // Storage operations
        public static bool SetData(string key, string value)
        {{
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
                throw new Exception(""Only owner can set data"");

            var storageKey = Prefix_Storage.ToByteArray().Concat(key.ToByteArray());
            Storage.Put(Storage.CurrentContext, storageKey, value);
            return true;
        }}

        public static bool DeleteData(string key)
        {{
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
                throw new Exception(""Only owner can delete data"");

            var storageKey = Prefix_Storage.ToByteArray().Concat(key.ToByteArray());
            Storage.Delete(Storage.CurrentContext, storageKey);
            return true;
        }}

        // Contract update
        public static bool Update(ByteString nefFile, string manifest)
        {{
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
                throw new Exception(""Only owner can update contract"");

            ContractManagement.Update(nefFile, manifest, null);
            return true;
        }}

        // Contract destruction
        public static bool Destroy()
        {{
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
                throw new Exception(""Only owner can destroy contract"");

            ContractManagement.Destroy();
            return true;
        }}
    }}
}}",
            ".csproj" => $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.6.2"" />
  </ItemGroup>

</Project>",
            _ => string.Empty
        };
    }

    private string GetNep17Template(string fileName, ProjectConfig config)
    {
        return fileName switch
        {
            "Contract.cs" => $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {config.Name}
{{
    [DisplayName(""{config.Name}"")]
    [ManifestExtra(""Author"", ""{config.Author}"")]
    [ManifestExtra(""Email"", ""{config.Email}"")]
    [ManifestExtra(""Description"", ""{config.Description}"")]
    [SupportedStandards(""NEP-17"")]
    [ContractPermission(""*"", ""*"")]
    public class {config.Name} : Nep17Token
    {{
        private const byte Prefix_Owner = 0xFF;

        [InitialValue(8, ContractParameterType.Integer)]
        private static readonly int InitialSupply = 100_000_000;

        [Safe]
        public static new string Symbol() => ""TKN"";

        [Safe]
        public static new byte Decimals() => 8;

        [Safe]
        public static UInt160 GetOwner()
        {{
            var key = new byte[] {{ Prefix_Owner }};
            var owner = Storage.Get(Storage.CurrentContext, key);
            return owner?.Length == 20 ? (UInt160)owner : UInt160.Zero;
        }}

        public static new void _deploy(object data, bool update)
        {{
            if (update) return;
            
            var tx = (Transaction)Runtime.ScriptContainer;
            var owner = tx.Sender;
            var key = new byte[] {{ Prefix_Owner }};
            Storage.Put(Storage.CurrentContext, key, owner);
            
            Mint(owner, InitialSupply);
        }}

        public static bool Mint(UInt160 to, BigInteger amount)
        {{
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
                throw new Exception(""Only owner can mint tokens"");

            if (amount <= 0)
                throw new Exception(""Invalid mint amount"");

            Nep17Token.Mint(to, amount);
            return true;
        }}

        public static bool Burn(BigInteger amount)
        {{
            var tx = (Transaction)Runtime.ScriptContainer;
            var account = tx.Sender;

            if (!Runtime.CheckWitness(account))
                throw new Exception(""Invalid witness"");

            if (amount <= 0)
                throw new Exception(""Invalid burn amount"");

            Nep17Token.Burn(account, amount);
            return true;
        }}

        public static bool Update(ByteString nefFile, string manifest)
        {{
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
                throw new Exception(""Only owner can update contract"");

            ContractManagement.Update(nefFile, manifest, null);
            return true;
        }}
    }}
}}",
            ".csproj" => $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.6.2"" />
  </ItemGroup>

</Project>",
            _ => string.Empty
        };
    }

    private string GetNep11Template(string fileName, ProjectConfig config)
    {
        // NEP-11 NFT template implementation
        return fileName switch
        {
            "Contract.cs" => $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {config.Name}
{{
    [DisplayName(""{config.Name}"")]
    [ManifestExtra(""Author"", ""{config.Author}"")]
    [ManifestExtra(""Email"", ""{config.Email}"")]
    [ManifestExtra(""Description"", ""{config.Description}"")]
    [SupportedStandards(""NEP-11"")]
    [ContractPermission(""*"", ""*"")]
    public class {config.Name} : Nep11Token<TokenState>
    {{
        private const byte Prefix_Owner = 0xFF;
        private const byte Prefix_TokenId = 0x02;

        [Safe]
        public static new string Symbol() => ""NFT"";

        [Safe]
        public static UInt160 GetOwner()
        {{
            var key = new byte[] {{ Prefix_Owner }};
            var owner = Storage.Get(Storage.CurrentContext, key);
            return owner?.Length == 20 ? (UInt160)owner : UInt160.Zero;
        }}

        public static new void _deploy(object data, bool update)
        {{
            if (update) return;
            
            var tx = (Transaction)Runtime.ScriptContainer;
            var owner = tx.Sender;
            var key = new byte[] {{ Prefix_Owner }};
            Storage.Put(Storage.CurrentContext, key, owner);
        }}

        public static bool Mint(UInt160 to, string name, string description, string image, Map<string, object> attributes)
        {{
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
                throw new Exception(""Only owner can mint NFTs"");

            var tokenId = GenerateTokenId();
            var token = new TokenState
            {{
                Owner = to,
                Name = name,
                Description = description,
                Image = image,
                Attributes = attributes
            }};

            Mint(tokenId, token);
            return true;
        }}

        private static ByteString GenerateTokenId()
        {{
            var key = new byte[] {{ Prefix_TokenId }};
            var currentId = Storage.Get(Storage.CurrentContext, key);
            var newId = currentId?.ToBigInteger() + 1 ?? 1;
            Storage.Put(Storage.CurrentContext, key, newId);
            return newId.ToByteArray();
        }}

        public static new Map<string, object> Properties(ByteString tokenId)
        {{
            var token = GetToken(tokenId);
            var properties = new Map<string, object>
            {{
                [""name""] = token.Name,
                [""description""] = token.Description,
                [""image""] = token.Image,
                [""attributes""] = token.Attributes
            }};
            return properties;
        }}

        public static bool Update(ByteString nefFile, string manifest)
        {{
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
                throw new Exception(""Only owner can update contract"");

            ContractManagement.Update(nefFile, manifest, null);
            return true;
        }}
    }}

    public class TokenState : Nep11TokenState
    {{
        public string Description {{ get; set; }} = string.Empty;
        public string Image {{ get; set; }} = string.Empty;
        public Map<string, object> Attributes {{ get; set; }} = new();
    }}
}}",
            ".csproj" => $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.6.2"" />
  </ItemGroup>

</Project>",
            _ => string.Empty
        };
    }

    private string GetOracleTemplate(string fileName, ProjectConfig config)
    {
        // Oracle template implementation
        return GetBasicTemplate(fileName, config); // Simplified for now
    }

    private string GetMultisigTemplate(string fileName, ProjectConfig config)
    {
        return fileName switch
        {
            "Contract.cs" => $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {config.Name}
{{
    [DisplayName(""{config.Name}"")]
    [ManifestExtra(""Author"", ""{config.Author}"")]
    [ManifestExtra(""Email"", ""{config.Email}"")]
    [ManifestExtra(""Description"", ""{config.Description}"")]
    [ContractPermission(""*"", ""*"")]
    public class {config.Name} : SmartContract
    {{
        // Multi-signature wallet implementation
        // See full template in Neo.SmartContract.Template
        
        private const byte Prefix_Owner = 0x01;
        private const byte Prefix_Transaction = 0x02;
        private const byte Prefix_Confirmation = 0x03;
        
        [Safe]
        public static bool IsOwner(UInt160 address)
        {{
            var ownerKey = Prefix_Owner.ToByteArray().Concat(address);
            return Storage.Get(Storage.CurrentContext, ownerKey) != null;
        }}
        
        public static BigInteger SubmitTransaction(UInt160 to, BigInteger value, ByteString data)
        {{
            // Implementation details in full template
            return 0;
        }}
        
        public static void ConfirmTransaction(BigInteger transactionId)
        {{
            // Implementation details in full template
        }}
    }}
}}",
            ".csproj" => $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.6.2"" />
  </ItemGroup>

</Project>",
            _ => string.Empty
        };
    }

    private string GetDaoTemplate(string fileName, ProjectConfig config)
    {
        return fileName switch
        {
            "Contract.cs" => $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {config.Name}
{{
    [DisplayName(""{config.Name}"")]
    [ManifestExtra(""Author"", ""{config.Author}"")]
    [ManifestExtra(""Email"", ""{config.Email}"")]
    [ManifestExtra(""Description"", ""{config.Description}"")]
    [ContractPermission(""*"", ""*"")]
    public class {config.Name} : SmartContract
    {{
        // DAO implementation
        // See full template in Neo.SmartContract.Template
        
        private const byte Prefix_Member = 0x02;
        private const byte Prefix_Proposal = 0x03;
        private const byte Prefix_Vote = 0x04;
        
        [Safe]
        public static bool IsMember(UInt160 address)
        {{
            var memberKey = Prefix_Member.ToByteArray().Concat(address);
            return Storage.Get(Storage.CurrentContext, memberKey) != null;
        }}
        
        public static BigInteger CreateProposal(string title, string description, ByteString callData, UInt160 target)
        {{
            // Implementation details in full template
            return 0;
        }}
        
        public static void Vote(BigInteger proposalId, bool support)
        {{
            // Implementation details in full template
        }}
        
        public static void ExecuteProposal(BigInteger proposalId)
        {{
            // Implementation details in full template
        }}
    }}
}}",
            ".csproj" => $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.6.2"" />
  </ItemGroup>

</Project>",
            _ => string.Empty
        };
    }
}
