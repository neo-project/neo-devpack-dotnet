// Copyright (C) 2015-2026 The Neo Project.
//
// ContractTemplateAnalyzerPackageTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Neo.SmartContract.Template.UnitTests.templates
{
    [TestClass]
    public class ContractTemplateAnalyzerPackageTests
    {
        private const string AnalyzerAssets = "runtime; build; native; contentfiles; analyzers; buildtransitive";
        private static readonly string TemplateRoot = Path.GetFullPath("../../../../../src/Neo.SmartContract.Template/templates");

        public static IEnumerable<object[]> ContractProjects()
        {
            yield return ["neocontractnep11/Nep11Contract.csproj"];
            yield return ["neocontractnep17/Nep17Contract.csproj"];
            yield return ["neocontractoracle/OracleRequest.csproj"];
            yield return ["neocontractowner/Ownable.csproj"];
            yield return ["neocontractsolution/NeoContractSolution/NeoContractSolution.csproj"];
        }

        [DataTestMethod]
        [DynamicData(nameof(ContractProjects), DynamicDataSourceType.Method)]
        public void ContractTemplateIncludesPrivateAnalyzerPackage(string relativeProjectPath)
        {
            var project = XDocument.Load(Path.Combine(TemplateRoot, relativeProjectPath));
            var analyzerReference = project
                .Descendants("PackageReference")
                .SingleOrDefault(element => (string?)element.Attribute("Include") == "Neo.SmartContract.Analyzer");

            Assert.IsNotNull(analyzerReference, $"{relativeProjectPath} must reference Neo.SmartContract.Analyzer.");
            Assert.AreEqual("TemplateNeoVersion", (string?)analyzerReference.Attribute("Version"));
            Assert.AreEqual("all", (string?)analyzerReference.Element("PrivateAssets"));
            Assert.AreEqual(AnalyzerAssets, (string?)analyzerReference.Element("IncludeAssets"));
        }
    }
}
