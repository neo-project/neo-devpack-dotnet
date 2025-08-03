// Copyright (C) 2015-2025 The Neo Project.
//
// ContractInvocationBasicTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class ContractInvocationBasicTest
    {
        [TestMethod]
        public void Test_ContractInvocation_AssemblyLoading()
        {
            // Test that we can load the contract invocation types
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            Assert.IsNotNull(assembly);

            // Test that contract invocation namespace exists
            var contractInvocationType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.NetworkContext");
            Assert.IsNotNull(contractInvocationType, "NetworkContext type should exist");

            // Test that attributes namespace exists
            var customMethodAttributeType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.Attributes.CustomMethodAttribute");
            Assert.IsNotNull(customMethodAttributeType, "CustomMethodAttribute type should exist");
        }

        [TestMethod]
        public void Test_ContractInvocation_NetworkContext_Creation()
        {
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            var networkContextType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.NetworkContext");
            Assert.IsNotNull(networkContextType);

            // Create instance using reflection
            var networkContext = Activator.CreateInstance(networkContextType);
            Assert.IsNotNull(networkContext);

            // Test CurrentNetwork property
            var currentNetworkProp = networkContextType.GetProperty("CurrentNetwork");
            Assert.IsNotNull(currentNetworkProp);
            var currentNetwork = currentNetworkProp.GetValue(networkContext);
            Assert.IsNotNull(currentNetwork);
        }

        [TestMethod]
        public void Test_ContractInvocation_CustomMethodAttribute_Creation()
        {
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            var attributeType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.Attributes.CustomMethodAttribute");
            Assert.IsNotNull(attributeType);

            // Create instance using reflection
            var attribute = Activator.CreateInstance(attributeType);
            Assert.IsNotNull(attribute);

            // Test properties exist
            var methodNameProp = attributeType.GetProperty("MethodName");
            Assert.IsNotNull(methodNameProp);

            var modifiesStateProp = attributeType.GetProperty("ModifiesState");
            Assert.IsNotNull(modifiesStateProp);
            
            // Test default value
            var modifiesState = modifiesStateProp.GetValue(attribute);
            Assert.AreEqual(true, modifiesState);
        }

        [TestMethod]
        public void Test_ContractInvocation_ParameterTransformStrategy_Enum()
        {
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            var strategyType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.Attributes.ParameterTransformStrategy");
            Assert.IsNotNull(strategyType);
            Assert.IsTrue(strategyType.IsEnum);

            // Test enum values exist
            var values = Enum.GetNames(strategyType);
            Assert.IsTrue(Array.IndexOf(values, "None") >= 0);
            Assert.IsTrue(Array.IndexOf(values, "WrapInArray") >= 0);
            Assert.IsTrue(Array.IndexOf(values, "FlattenArrays") >= 0);
            Assert.IsTrue(Array.IndexOf(values, "SerializeToByteArray") >= 0);
        }

        [TestMethod]
        public void Test_ContractInvocation_DevelopmentContractReference_Type()
        {
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            var devRefType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.DevelopmentContractReference");
            Assert.IsNotNull(devRefType);

            // Test it has required properties
            var identifierProp = devRefType.GetProperty("Identifier");
            Assert.IsNotNull(identifierProp);

            var projectPathProp = devRefType.GetProperty("ProjectPath");
            Assert.IsNotNull(projectPathProp);

            var isResolvedProp = devRefType.GetProperty("IsResolved");
            Assert.IsNotNull(isResolvedProp);
        }

        [TestMethod]
        public void Test_ContractInvocation_NetworkType_Enum()
        {
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            var networkType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.NetworkType");
            Assert.IsNotNull(networkType);
            Assert.IsTrue(networkType.IsEnum);

            // Test enum values exist
            var values = Enum.GetNames(networkType);
            Assert.IsTrue(Array.IndexOf(values, "Mainnet") >= 0);
            Assert.IsTrue(Array.IndexOf(values, "Testnet") >= 0);
            Assert.IsTrue(Array.IndexOf(values, "Privnet") >= 0);
        }

        [TestMethod]
        public void Test_ContractInvocation_IContractReference_Interface()
        {
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            var interfaceType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.IContractReference");
            Assert.IsNotNull(interfaceType);
            Assert.IsTrue(interfaceType.IsInterface);

            // Test interface has required properties
            var identifierProp = interfaceType.GetProperty("Identifier");
            Assert.IsNotNull(identifierProp);

            var resolvedHashProp = interfaceType.GetProperty("ResolvedHash");
            Assert.IsNotNull(resolvedHashProp);

            var isResolvedProp = interfaceType.GetProperty("IsResolved");
            Assert.IsNotNull(isResolvedProp);
        }

        [TestMethod]
        public void Test_ContractInvocation_MethodResolver_Type()
        {
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            var resolverType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.MethodResolver");
            Assert.IsNotNull(resolverType);

            // Test it has ResolveMethod method
            var resolveMethod = resolverType.GetMethod("ResolveMethod", BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(resolveMethod);
        }

        [TestMethod]
        public void Test_ContractInvocation_ContractMethodAttribute_Type()
        {
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            var attributeType = assembly.GetType("Neo.SmartContract.Framework.ContractInvocation.Attributes.ContractMethodAttribute");
            Assert.IsNotNull(attributeType);

            // Create instance
            var attribute = Activator.CreateInstance(attributeType);
            Assert.IsNotNull(attribute);

            // Test properties
            var readOnlyProp = attributeType.GetProperty("ReadOnly");
            Assert.IsNotNull(readOnlyProp);

            var methodNameProp = attributeType.GetProperty("MethodName");
            Assert.IsNotNull(methodNameProp);
        }

        [TestMethod]
        public void Test_ContractInvocation_AllTypesExist()
        {
            var assembly = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly;
            
            // List of all expected types
            string[] expectedTypes = new[]
            {
                "Neo.SmartContract.Framework.ContractInvocation.IContractReference",
                "Neo.SmartContract.Framework.ContractInvocation.NetworkContext",
                "Neo.SmartContract.Framework.ContractInvocation.NetworkType",
                "Neo.SmartContract.Framework.ContractInvocation.DevelopmentContractReference",
                "Neo.SmartContract.Framework.ContractInvocation.DeployedContractReference",
                "Neo.SmartContract.Framework.ContractInvocation.ContractProxyBase",
                "Neo.SmartContract.Framework.ContractInvocation.DevelopmentContractProxy",
                "Neo.SmartContract.Framework.ContractInvocation.MethodResolver",
                "Neo.SmartContract.Framework.ContractInvocation.ContractInvocationFactory",
                "Neo.SmartContract.Framework.ContractInvocation.ContractInterfaceGenerator",
                "Neo.SmartContract.Framework.ContractInvocation.BlockchainInterface",
                "Neo.SmartContract.Framework.ContractInvocation.Attributes.ContractMethodAttribute",
                "Neo.SmartContract.Framework.ContractInvocation.Attributes.CustomMethodAttribute",
                "Neo.SmartContract.Framework.ContractInvocation.Attributes.ContractReferenceAttribute",
                "Neo.SmartContract.Framework.ContractInvocation.Attributes.ParameterTransformStrategy"
            };

            foreach (var typeName in expectedTypes)
            {
                var type = assembly.GetType(typeName);
                Assert.IsNotNull(type, $"Type {typeName} should exist in the assembly");
            }
        }
    }
}