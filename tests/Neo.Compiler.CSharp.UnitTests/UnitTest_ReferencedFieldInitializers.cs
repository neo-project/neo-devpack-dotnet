// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ReferencedFieldInitializers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ReferencedFieldInitializers
{
    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void InheritedFieldsUseTheirDeclaringProjectSemanticModel(CompilationOptions.OptimizationType optimization)
    {
        string directory = Directory.CreateTempSubdirectory().FullName;
        try
        {
            string libraryDirectory = Directory.CreateDirectory(Path.Combine(directory, "Library")).FullName;
            string contractDirectory = Directory.CreateDirectory(Path.Combine(directory, "Contract")).FullName;
            string libraryProject = Path.Combine(libraryDirectory, "Library.csproj");
            File.WriteAllText(libraryProject, """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
                </Project>
                """);
            File.WriteAllText(Path.Combine(libraryDirectory, "BaseData.cs"), """
                namespace Library;

                public class BaseData
                {
                    public int Number = Values.GetNumber();
                    public int Property { get; set; } = 17;
                    public int DefaultValue;
                }

                public static class Values
                {
                    public static int GetNumber() => 23;
                }
                """);

            string frameworkProject = Path.Combine(Syntax.SyntaxProbeLoader.GetRepositoryRoot(),
                "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
            string contractProject = Path.Combine(contractDirectory, "Contract.csproj");
            File.WriteAllText(contractProject, $$"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
                  <ItemGroup>
                    <ProjectReference Include="{{SecurityElement.Escape(frameworkProject)}}" />
                    <ProjectReference Include="{{SecurityElement.Escape(libraryProject)}}" />
                  </ItemGroup>
                </Project>
                """);
            File.WriteAllText(Path.Combine(contractDirectory, "Contract.cs"), """
                using Neo.SmartContract.Framework;
                using Library;

                public class DerivedData : BaseData
                {
                    public int Own = 5;
                }

                public class Contract : SmartContract
                {
                    public static int Number() => new DerivedData().Number;
                    public static int Property() => new DerivedData().Property;
                    public static int DefaultValue() => new DerivedData().DefaultValue;
                    public static int Own() => new DerivedData().Own;
                }
                """);

            var options = TestHelper.CreateDefaultOptions();
            options.Optimize = optimization;
            var context = new CompilationEngine(options).CompileProject(contractProject).Single();
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
            var (nef, manifest, _) = context.CreateResults();
            var contract = new TestEngine(true).Deploy<InheritedFieldsContract>(nef, manifest);
            Assert.AreEqual(new BigInteger(23), contract.Number());
            Assert.AreEqual(new BigInteger(17), contract.Property());
            Assert.AreEqual(BigInteger.Zero, contract.DefaultValue());
            Assert.AreEqual(new BigInteger(5), contract.Own());
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [DataTestMethod]
    [DataRow("Example.SmartContract.NFT")]
    [DataRow("Example.SmartContract.SampleRoyaltyNEP11Token")]
    public void CliCompilesExamplesWithInheritedFieldInitializers(string projectName)
    {
        string outputDirectory = Directory.CreateTempSubdirectory().FullName;
        try
        {
            string project = Path.Combine(Syntax.SyntaxProbeLoader.GetRepositoryRoot(),
                "examples", projectName, projectName + ".csproj");
            int exitCode = Program.Main([project, "--output", outputDirectory, "--optimize", "All"]);
            Assert.AreEqual(0, exitCode);
            Assert.AreEqual(1, Directory.GetFiles(outputDirectory, "*.nef").Length);
            Assert.AreEqual(1, Directory.GetFiles(outputDirectory, "*.manifest.json").Length);
        }
        finally
        {
            Directory.Delete(outputDirectory, recursive: true);
        }
    }

    public abstract class InheritedFieldsContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("number")]
        public abstract BigInteger? Number();
        [DisplayName("property")]
        public abstract BigInteger? Property();
        [DisplayName("defaultValue")]
        public abstract BigInteger? DefaultValue();
        [DisplayName("own")]
        public abstract BigInteger? Own();
    }
}
