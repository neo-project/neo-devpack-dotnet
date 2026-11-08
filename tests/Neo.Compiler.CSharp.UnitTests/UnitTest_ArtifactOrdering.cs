// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ArtifactOrdering.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory or
// http://www.opensource.org/licenses/mit-license.php for more details.
//
// Redistribution and use in source and binary forms, with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using System;
using System.Globalization;
using System.Linq;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ArtifactOrdering
{
    [TestMethod]
    public void GeneratedArtifactsUseCultureIndependentOrdering()
    {
        var manifest = CreateManifest();
        var nef = new NefFile
        {
            Compiler = "test",
            Source = "test.cs",
            Tokens = [],
            Script = new byte[] { (byte)OpCode.RET }
        };
        nef.CheckSum = NefFile.ComputeChecksum(nef);

        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            var english = CaptureArtifacts(manifest, nef);

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            var turkish = CaptureArtifacts(manifest, nef);

            Assert.AreEqual(english.Interface, turkish.Interface);
            Assert.AreEqual(english.TestingArtifacts, turkish.TestingArtifacts);
            Assert.AreEqual(english.Report, turkish.Report);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    private static (string Interface, string TestingArtifacts, string Report) CaptureArtifacts(ContractManifest manifest, NefFile nef)
    {
        var writer = new StringWriter();
        Neo.Compiler.AbiReporter.Print(nef, manifest, writer);
        return (
            Neo.Compiler.ContractInterfaceGenerator.GenerateInterface(manifest.Name, manifest, UInt160.Zero),
            manifest.GetArtifactsSource(),
            writer.ToString());
    }

    private static ContractManifest CreateManifest()
    {
        string[] names = ["I", "i", "İ", "ı"];
        return new ContractManifest
        {
            Name = "OrderingContract",
            Groups = [],
            SupportedStandards = [],
            Abi = new ContractAbi
            {
                Methods = names.Select(name => new ContractMethodDescriptor
                {
                    Name = name,
                    Parameters = [],
                    ReturnType = ContractParameterType.Integer,
                    Safe = true
                }).ToArray(),
                Events = names.Select(name => new ContractEventDescriptor
                {
                    Name = name,
                    Parameters = []
                }).ToArray()
            },
            Permissions = [],
            Trusts = WildcardContainer<ContractPermissionDescriptor>.Create()
        };
    }
}
