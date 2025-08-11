// Copyright (C) 2015-2025 The Neo Project.
//
// TemplateManager.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Neo.Compiler
{
    public enum ContractTemplate
    {
        Basic,
        NEP17,
        NEP11,
        Ownable,
        Oracle
    }

    public class TemplateManager
    {
        private readonly Dictionary<ContractTemplate, TemplateInfo> templates;

        public TemplateManager()
        {
            templates = new Dictionary<ContractTemplate, TemplateInfo>
            {
                [ContractTemplate.Basic] = new TemplateInfo
                {
                    Name = "Basic Contract",
                    Description = "A simple smart contract with basic functionality",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetBasicContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                },
                [ContractTemplate.NEP17] = new TemplateInfo
                {
                    Name = "NEP-17 Token",
                    Description = "NEP-17 fungible token standard implementation",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetNep17ContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                },
                [ContractTemplate.NEP11] = new TemplateInfo
                {
                    Name = "NEP-11 NFT",
                    Description = "NEP-11 non-fungible token standard implementation",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetNep11ContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                },
                [ContractTemplate.Ownable] = new TemplateInfo
                {
                    Name = "Ownable Contract",
                    Description = "Contract with owner management functionality",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetOwnableContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                },
                [ContractTemplate.Oracle] = new TemplateInfo
                {
                    Name = "Oracle Contract",
                    Description = "Contract that interacts with Oracle services",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetOracleContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                }
            };
        }

        public class TemplateInfo
        {
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();
        }

        public void GenerateContract(ContractTemplate template, string projectName, string outputPath, Dictionary<string, string>? additionalReplacements = null)
        {
            if (!templates.ContainsKey(template))
                throw new ArgumentException($"Unknown template: {template}");

            var templateInfo = templates[template];
            var replacements = new Dictionary<string, string>
            {
                { "{{ProjectName}}", projectName },
                { "{{Namespace}}", projectName },
                { "{{ClassName}}", projectName },
                { "{{Author}}", Environment.UserName },
                { "{{Email}}", $"{Environment.UserName}@example.com" },
                { "{{Description}}", $"{projectName} Smart Contract" },
                { "{{Version}}", "1.0.0" },
                { "{{Year}}", DateTime.Now.Year.ToString() }
            };

            if (additionalReplacements != null)
            {
                foreach (var kvp in additionalReplacements)
                {
                    replacements[kvp.Key] = kvp.Value;
                }
            }

            string projectPath = Path.Combine(outputPath, projectName);
            Directory.CreateDirectory(projectPath);

            foreach (var file in templateInfo.Files)
            {
                string fileName = ReplaceTokens(file.Key, replacements);
                string fileContent = ReplaceTokens(file.Value, replacements);
                string filePath = Path.Combine(projectPath, fileName);

                File.WriteAllText(filePath, fileContent);
                Console.WriteLine($"Created: {filePath}");
            }

            Console.WriteLine($"\nSuccessfully created {template} contract '{projectName}' in {projectPath}");
            Console.WriteLine("\nTo build your contract:");
            Console.WriteLine($"  dotnet build {Path.Combine(projectPath, projectName + ".csproj")}");
            Console.WriteLine("\nTo compile to NEF:");
            Console.WriteLine($"  dotnet run --project src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- {Path.Combine(projectPath, projectName + ".csproj")}");
        }

        private string ReplaceTokens(string content, Dictionary<string, string> replacements)
        {
            foreach (var replacement in replacements)
            {
                content = content.Replace(replacement.Key, replacement.Value);
            }
            return content;
        }

        public IEnumerable<(ContractTemplate template, string name, string description)> GetAvailableTemplates()
        {
            return templates.Select(t => (t.Key, t.Value.Name, t.Value.Description));
        }

        private static string GetProjectFileTemplate()
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""*"" />
  </ItemGroup>

  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Message Text=""Smart contract project {{ProjectName}} built successfully"" Importance=""high"" />
  </Target>

</Project>";
        }

        private static string GetBasicContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract
    {
        private const string HelloPrefix = ""Hello, "";

        [Safe]
        public static string GetMessage(string name)
        {
            return HelloPrefix + name + ""!"";
        }

        public static void SetMessage(string key, string value)
        {
            Storage.Put(Storage.CurrentContext, key, value);
        }

        [Safe]
        public static string GetStoredMessage(string key)
        {
            return Storage.Get(Storage.CurrentReadOnlyContext, key);
        }

        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Storage.Put(Storage.CurrentContext, ""Deployed"", Runtime.Time);
            }
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            ContractManagement.Update(nefFile, manifest, data);
        }

        public static void Destroy()
        {
            ContractManagement.Destroy();
        }
    }
}";
        }

        private static string GetNep17ContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep17)]
    public class {{ClassName}} : Neo.SmartContract.Framework.Nep17Token
    {
        private const byte Prefix_Owner = 0xff;

        public override string Symbol { [Safe] get => ""{{ProjectName}}""; }
        public override byte Decimals { [Safe] get => 8; }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner });
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 previousOwner, UInt160 newOwner);

        [DisplayName(""SetOwner"")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160 newOwner)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""owner must be valid"");

            UInt160 previous = GetOwner();
            Storage.Put(new[] { Prefix_Owner }, newOwner);
            OnSetOwner(previous, newOwner);
        }

        public static new void Burn(UInt160 account, BigInteger amount)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");
            Nep17Token.Burn(account, amount);
        }

        public static new void Mint(UInt160 to, BigInteger amount)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");
            Nep17Token.Mint(to, amount);
        }

        [Safe]
        public static bool Verify() => IsOwner();

        public static void _deploy(object data, bool update)
        {
            if (update) return;

            if (data is null) data = Runtime.Transaction.Sender;
            UInt160 initialOwner = (UInt160)data;

            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, ""owner must exists"");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            OnSetOwner(null, initialOwner);

            // Mint initial supply to owner
            Mint(initialOwner, 1000000_00000000); // 1,000,000 tokens with 8 decimals
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }
    }
}";
        }

        private static string GetNep11ContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep11)]
    public class {{ClassName}} : Nep11Token<TokenState>
    {
        private const byte Prefix_Owner = 0xff;
        private const byte Prefix_TokenId = 0xfe;

        public override string Symbol { [Safe] get => ""{{ProjectName}}""; }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner });
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public static void Mint(string tokenId, TokenState tokenState)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            Nep11Token<TokenState>.Mint(tokenId, tokenState);
        }

        public static void _deploy(object data, bool update)
        {
            if (update) return;

            if (data is null) data = Runtime.Transaction.Sender;
            UInt160 initialOwner = (UInt160)data;

            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, ""owner must exists"");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            Storage.Put(new[] { Prefix_TokenId }, 0);
        }

        protected override byte[] GetKey(string tokenId) =>
            ConcatKey(Prefix_TokenId, tokenId);

        private static byte[] ConcatKey(byte prefix, string tokenId)
        {
            return Helper.Concat((byte[])new[] { prefix }, tokenId);
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }
    }

    public class TokenState : Nep11TokenState
    {
        public string? Description;
        public string? Image;
        public Map<string, object>? Properties;
    }
}";
        }

        private static string GetOwnableContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract
    {
        private const byte Prefix_Owner = 0xff;

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner });
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 previousOwner, UInt160 newOwner);

        [DisplayName(""SetOwner"")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160 newOwner)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""owner must be valid"");

            UInt160 previous = GetOwner();
            Storage.Put(new[] { Prefix_Owner }, newOwner);
            OnSetOwner(previous, newOwner);
        }

        public static void DoSomething()
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            // Owner-only logic here
            Storage.Put(Storage.CurrentContext, ""LastAction"", Runtime.Time);
        }

        [Safe]
        public static object GetData(string key)
        {
            return Storage.Get(Storage.CurrentReadOnlyContext, key);
        }

        public static void SetData(string key, object value)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            Storage.Put(Storage.CurrentContext, key, value);
        }

        [Safe]
        public static bool Verify() => IsOwner();

        public static void _deploy(object data, bool update)
        {
            if (update) return;

            if (data is null) data = Runtime.Transaction.Sender;
            UInt160 initialOwner = (UInt160)data;

            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, ""owner must exists"");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            OnSetOwner(null, initialOwner);
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }

        public static void Destroy()
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Destroy();
        }
    }
}";
        }

        private static string GetOracleContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract
    {
        private const byte Prefix_RequestId = 0x01;
        private const byte Prefix_Response = 0x02;

        public static void RequestData(string url, string filter, string callback, object userData, long gasForResponse)
        {
            Oracle.Request(url, filter, callback, userData, gasForResponse);
        }

        // This method is called by the Oracle service when a response is received
        // The method name must match the callback parameter in RequestData
        public static void OnOracleResponse(string requestedUrl, object userData, OracleResponseCode responseCode, string result)
        {
            if (responseCode != OracleResponseCode.Success)
            {
                Runtime.Log(""Oracle response failed with code: "" + (byte)responseCode);
                return;
            }

            // Store the response
            StorageContext context = Storage.CurrentContext;
            byte[] key = Helper.Concat(new byte[] { Prefix_Response }, StdLib.Serialize(userData));
            Storage.Put(context, key, result);
            
            Runtime.Log(""Oracle response received: "" + result);
            
            // Trigger an event
            OnResponseReceived(requestedUrl, userData, result);
        }

        public delegate void OnResponseReceivedDelegate(string url, object userData, string response);
        
        [DisplayName(""ResponseReceived"")]
        public static event OnResponseReceivedDelegate OnResponseReceived = default!;

        [Safe]
        public static string GetLastResponse(object userData)
        {
            StorageContext context = Storage.CurrentReadOnlyContext;
            byte[] key = Helper.Concat(new byte[] { Prefix_Response }, StdLib.Serialize(userData));
            ByteString data = Storage.Get(context, key);
            return data;
        }

        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Storage.Put(Storage.CurrentContext, ""Deployed"", Runtime.Time);
            }
        }

        public static void Update(ByteString nefFile, string manifest, object data)
        {
            ContractManagement.Update(nefFile, manifest, data);
        }

        public static void Destroy()
        {
            ContractManagement.Destroy();
        }
    }
}";
        }
    }
}
