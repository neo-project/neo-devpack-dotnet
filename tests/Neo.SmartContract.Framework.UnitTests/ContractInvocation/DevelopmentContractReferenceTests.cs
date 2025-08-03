// Copyright (C) 2015-2025 The Neo Project.
//
// DevelopmentContractReferenceTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework.ContractInvocation;
using System;

namespace Neo.SmartContract.Framework.UnitTests.ContractInvocation
{
    [TestClass]
    public class DevelopmentContractReferenceTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var identifier = "TestContract";
            var projectPath = "./TestContract.csproj";
            var networkContext = new NetworkContext();
            
            var reference = new DevelopmentContractReference(identifier, projectPath, networkContext);
            
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.AreEqual(projectPath, reference.ProjectPath);
            Assert.AreEqual(networkContext, reference.NetworkContext);
            Assert.IsTrue(reference.CompileAsDependency);
            Assert.IsFalse(reference.IsResolved);
            Assert.IsNull(reference.ResolvedHash);
        }

        [TestMethod]
        public void Constructor_WithNullIdentifier_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new DevelopmentContractReference(null!, "./test.csproj"));
        }

        [TestMethod]
        public void Constructor_WithNullProjectPath_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new DevelopmentContractReference("TestContract", null!));
        }

        [TestMethod]
        public void Constructor_WithNullNetworkContext_ShouldCreateDefaultNetworkContext()
        {
            var reference = new DevelopmentContractReference("TestContract", "./test.csproj");
            
            Assert.IsNotNull(reference.NetworkContext);
            Assert.AreEqual("privnet", reference.NetworkContext.CurrentNetwork);
        }

        [TestMethod]
        public void ResolveHash_ShouldSetResolvedHashAndMarkAsResolved()
        {
            var reference = new DevelopmentContractReference("TestContract", "./test.csproj");
            var hash = UInt160.Zero;
            
            reference.ResolveHash(hash);
            
            Assert.AreEqual(hash, reference.ResolvedHash);
            Assert.IsTrue(reference.IsResolved);
        }

        [TestMethod]
        public void ResolveHash_WithNullHash_ShouldThrowArgumentNullException()
        {
            var reference = new DevelopmentContractReference("TestContract", "./test.csproj");
            
            Assert.ThrowsException<ArgumentNullException>(() => reference.ResolveHash(null!));
        }

        [TestMethod]
        public void FromProject_ShouldCreateReferenceWithCorrectProperties()
        {
            var projectPath = "./TestContract/TestContract.csproj";
            
            var reference = DevelopmentContractReference.FromProject(projectPath);
            
            Assert.AreEqual("TestContract", reference.Identifier);
            Assert.AreEqual(projectPath, reference.ProjectPath);
            Assert.IsTrue(reference.CompileAsDependency);
        }

        [TestMethod]
        public void FromProject_WithNullOrEmptyPath_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                DevelopmentContractReference.FromProject(null!));
            Assert.ThrowsException<ArgumentNullException>(() => 
                DevelopmentContractReference.FromProject(""));
        }

        [TestMethod]
        public void CompileAsDependency_ShouldBeConfigurable()
        {
            var reference = new DevelopmentContractReference("TestContract", "./test.csproj");
            
            Assert.IsTrue(reference.CompileAsDependency);
            
            reference.CompileAsDependency = false;
            Assert.IsFalse(reference.CompileAsDependency);
        }

        [TestMethod]
        public void IsResolved_ShouldReturnTrueOnlyAfterResolveHash()
        {
            var reference = new DevelopmentContractReference("TestContract", "./test.csproj");
            
            Assert.IsFalse(reference.IsResolved);
            
            reference.ResolveHash(UInt160.Zero);
            
            Assert.IsTrue(reference.IsResolved);
        }
    }
}