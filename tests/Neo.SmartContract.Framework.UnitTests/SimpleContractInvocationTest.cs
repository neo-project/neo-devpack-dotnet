// Copyright (C) 2015-2025 The Neo Project.
//
// SimpleContractInvocationTest.cs file belongs to the neo project and is free
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
using scfx::Neo.SmartContract.Framework.ContractInvocation.Attributes;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SimpleContractInvocationTest
    {
        #region Custom Method Attribute Tests

        [TestMethod]
        public void Test_CustomMethodAttribute_Creation()
        {
            var attribute = new CustomMethodAttribute();
            
            Assert.IsNotNull(attribute);
            Assert.IsNull(attribute.MethodName);
            Assert.IsTrue(attribute.ModifiesState);
            Assert.IsTrue(attribute.AllowCall);
            Assert.IsTrue(attribute.AllowNotify);
            Assert.IsFalse(attribute.ReadOnly);
        }

        [TestMethod]
        public void Test_CustomMethodAttribute_WithMethodName()
        {
            var attribute = new CustomMethodAttribute("customMethod");
            
            Assert.AreEqual("customMethod", attribute.MethodName);
        }

        [TestMethod]
        public void Test_CustomMethodAttribute_ReadOnlyFlag()
        {
            var attribute = new CustomMethodAttribute
            {
                ReadOnly = true
            };
            
            Assert.IsTrue(attribute.ReadOnly);
            var flags = attribute.GetEffectiveCallFlags();
            Assert.AreEqual(scfx::Neo.SmartContract.Framework.Services.CallFlags.ReadOnly, flags);
        }

        [TestMethod]
        public void Test_CustomMethodAttribute_ParameterValidation()
        {
            var attribute = new CustomMethodAttribute
            {
                MinParameters = 2,
                MaxParameters = 4,
                ValidateParameters = true
            };
            
            // Valid parameter count
            Assert.IsTrue(attribute.ValidateParameterConstraints(new object[] { "param1", "param2" }));
            Assert.IsTrue(attribute.ValidateParameterConstraints(new object[] { "param1", "param2", "param3" }));
            
            // Invalid parameter count
            Assert.IsFalse(attribute.ValidateParameterConstraints(new object[] { "param1" })); // Too few
            Assert.IsFalse(attribute.ValidateParameterConstraints(new object[] { "a", "b", "c", "d", "e" })); // Too many
        }

        [TestMethod]
        public void Test_CustomMethodAttribute_ArrayParameterValidation()
        {
            var attribute = new CustomMethodAttribute
            {
                SupportsArrayParameters = false,
                ValidateParameters = true
            };
            
            // Should fail with array parameter
            Assert.IsFalse(attribute.ValidateParameterConstraints(new object[] { new int[] { 1, 2, 3 } }));
            
            // Should pass with non-array parameters
            Assert.IsTrue(attribute.ValidateParameterConstraints(new object[] { "string", 123 }));
        }

        #endregion

        #region Parameter Transformation Tests

        [TestMethod]
        public void Test_ParameterTransformation_None()
        {
            var attribute = new CustomMethodAttribute
            {
                ParameterTransform = ParameterTransformStrategy.None
            };
            
            var originalParams = new object[] { "test", 123 };
            var transformedParams = attribute.TransformParameters(originalParams);
            
            Assert.AreSame(originalParams, transformedParams);
        }

        [TestMethod]
        public void Test_ParameterTransformation_WrapInArray()
        {
            var attribute = new CustomMethodAttribute
            {
                ParameterTransform = ParameterTransformStrategy.WrapInArray
            };
            
            var originalParams = new object[] { "test", 123 };
            var transformedParams = attribute.TransformParameters(originalParams);
            
            Assert.AreEqual(1, transformedParams.Length);
            Assert.AreSame(originalParams, transformedParams[0]);
        }

        [TestMethod]
        public void Test_ParameterTransformation_FlattenArrays()
        {
            var attribute = new CustomMethodAttribute
            {
                ParameterTransform = ParameterTransformStrategy.FlattenArrays
            };
            
            // Test with array parameters
            var originalParams = new object[] { "test", new int[] { 1, 2, 3 }, "end" };
            var transformedParams = attribute.TransformParameters(originalParams);
            
            // Should flatten the array parameter
            Assert.AreEqual(5, transformedParams.Length);
            Assert.AreEqual("test", transformedParams[0]);
            Assert.AreEqual(1, transformedParams[1]);
            Assert.AreEqual(2, transformedParams[2]);
            Assert.AreEqual(3, transformedParams[3]);
            Assert.AreEqual("end", transformedParams[4]);
        }

        [TestMethod]
        public void Test_ParameterTransformation_SerializeToByteArray()
        {
            var attribute = new CustomMethodAttribute
            {
                ParameterTransform = ParameterTransformStrategy.SerializeToByteArray
            };
            
            var originalParams = new object[] { "test", 123 };
            var transformedParams = attribute.TransformParameters(originalParams);
            
            Assert.AreEqual(1, transformedParams.Length);
            Assert.IsInstanceOfType(transformedParams[0], typeof(byte[]));
        }

        #endregion

        #region Contract Method Attribute Tests

        [TestMethod]
        public void Test_ContractMethodAttribute_Creation()
        {
            var attribute = new ContractMethodAttribute();
            
            Assert.IsNotNull(attribute);
            Assert.IsNull(attribute.MethodName);
            Assert.IsTrue(attribute.ModifiesState);
            Assert.IsTrue(attribute.AllowCall);
            Assert.IsTrue(attribute.AllowNotify);
            Assert.IsFalse(attribute.ReadOnly);
        }

        [TestMethod]
        public void Test_ContractMethodAttribute_WithMethodName()
        {
            var attribute = new ContractMethodAttribute("testMethod");
            
            Assert.AreEqual("testMethod", attribute.MethodName);
        }

        [TestMethod]
        public void Test_ContractMethodAttribute_ReadOnlyFlags()
        {
            var attribute = new ContractMethodAttribute
            {
                ReadOnly = true
            };
            
            var flags = attribute.GetEffectiveCallFlags();
            Assert.AreEqual(scfx::Neo.SmartContract.Framework.Services.CallFlags.ReadOnly, flags);
        }

        [TestMethod]
        public void Test_ContractMethodAttribute_CustomFlags()
        {
            var attribute = new ContractMethodAttribute
            {
                ModifiesState = true,
                AllowCall = false,
                AllowNotify = true
            };
            
            var flags = attribute.GetEffectiveCallFlags();
            Assert.IsTrue((flags & scfx::Neo.SmartContract.Framework.Services.CallFlags.States) != 0);
            Assert.IsTrue((flags & scfx::Neo.SmartContract.Framework.Services.CallFlags.AllowCall) == 0);
            Assert.IsTrue((flags & scfx::Neo.SmartContract.Framework.Services.CallFlags.AllowNotify) != 0);
        }

        #endregion

        #region Error Handling Tests

        [TestMethod]
        public void Test_CustomMethodAttribute_NullParameterValidation()
        {
            var attribute = new CustomMethodAttribute
            {
                ValidateParameters = true,
                MinParameters = 1
            };
            
            // Should handle null parameters gracefully
            Assert.IsFalse(attribute.ValidateParameterConstraints(null));
        }

        [TestMethod]
        public void Test_CustomMethodAttribute_EmptyParameterValidation()
        {
            var attribute = new CustomMethodAttribute
            {
                ValidateParameters = true,
                MinParameters = 0,
                MaxParameters = 0
            };
            
            // Should pass with empty parameters
            Assert.IsTrue(attribute.ValidateParameterConstraints(new object[0]));
        }

        [TestMethod]
        public void Test_ParameterTransformation_NullParameters()
        {
            var attribute = new CustomMethodAttribute
            {
                ParameterTransform = ParameterTransformStrategy.WrapInArray
            };
            
            // Should handle null parameters gracefully
            var result = attribute.TransformParameters(null);
            Assert.IsNull(result);
        }

        #endregion
    }
}