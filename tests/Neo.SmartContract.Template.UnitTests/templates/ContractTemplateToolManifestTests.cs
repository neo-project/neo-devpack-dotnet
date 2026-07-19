// Copyright (C) 2015-2026 The Neo Project.
//
// ContractTemplateToolManifestTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace Neo.SmartContract.Template.UnitTests.templates;

[TestClass]
public class ContractTemplateToolManifestTests
{
    private static readonly string TemplateRoot = Path.GetFullPath(
        "../../../../../src/Neo.SmartContract.Template/templates");

    [DataTestMethod]
    [DataRow("neocontractnep11")]
    [DataRow("neocontractnep17")]
    [DataRow("neocontractoracle")]
    [DataRow("neocontractowner")]
    [DataRow("neocontractsolution")]
    public void ContractTemplate_IncludesCompilerToolManifest(string templateName)
    {
        var manifestPath = Path.Combine(TemplateRoot, templateName, ".config", "dotnet-tools.json");
        Assert.IsTrue(File.Exists(manifestPath), $"{templateName} is missing .config/dotnet-tools.json.");

        using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
        var root = document.RootElement;
        var compiler = root.GetProperty("tools").GetProperty("neo.compiler.csharp");

        Assert.AreEqual(1, root.GetProperty("version").GetInt32());
        Assert.IsTrue(root.GetProperty("isRoot").GetBoolean());
        Assert.AreEqual("TemplateNeoVersion", compiler.GetProperty("version").GetString());

        var commands = compiler.GetProperty("commands");
        Assert.AreEqual(1, commands.GetArrayLength());
        Assert.AreEqual("nccs", commands[0].GetString());
    }

    [DataTestMethod]
    [DataRow("neocontractnep11", "Nep11Contract.csproj")]
    [DataRow("neocontractnep17", "Nep17Contract.csproj")]
    [DataRow("neocontractoracle", "OracleRequest.csproj")]
    [DataRow("neocontractowner", "Ownable.csproj")]
    public void StandaloneContractTemplate_UsesLocalCompilerTool(
        string templateName,
        string projectName)
    {
        var project = XDocument.Load(Path.Combine(TemplateRoot, templateName, projectName));
        var commands = project.Descendants("Exec")
            .Select(element => (string?)element.Attribute("Command"))
            .Where(command => command is not null)
            .ToArray();

        CollectionAssert.Contains(commands, "dotnet tool restore");
        Assert.IsTrue(
            commands.Any(command => command!.StartsWith("dotnet tool run nccs ", StringComparison.Ordinal)),
            $"{projectName} must invoke nccs through the local tool manifest.");
    }
}
