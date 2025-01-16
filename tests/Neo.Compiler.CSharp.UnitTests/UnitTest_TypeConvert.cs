// Copyright (C) 2015-2024 The Neo Project.
//
// UnitTest_TypeConvert.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TypeConvert : DebugAndTestBase<Contract_TypeConvert>
    {
        [TestMethod]
        public void UnitTest_TestTypeConvert()
        {
            var arr = (Array)Contract.TestType()!;
            AssertGasConsumed(4207710);

            //test 0,1,2
            Assert.IsTrue(arr[0].Type == StackItemType.Integer);
            Assert.IsTrue(arr[1].Type == StackItemType.Buffer);
            Assert.IsTrue((arr[1].ConvertTo(StackItemType.ByteString) as PrimitiveType)?.GetInteger() == (arr[0] as PrimitiveType)?.GetInteger());

            Assert.IsTrue(arr[2].Type == StackItemType.Integer);
            Assert.IsTrue(arr[3].Type == StackItemType.Buffer);
            Assert.IsTrue((arr[3].ConvertTo(StackItemType.ByteString) as PrimitiveType)?.GetInteger() == (arr[2] as PrimitiveType)?.GetInteger());

            Assert.IsTrue(arr[4].Type == StackItemType.Buffer);
            Assert.IsTrue(arr[5].Type == StackItemType.Integer);
            Assert.IsTrue((arr[4].ConvertTo(StackItemType.ByteString) as PrimitiveType)?.GetInteger() == (arr[5] as PrimitiveType)?.GetInteger());

            Assert.IsTrue(arr[6].Type == StackItemType.Buffer);
            Assert.IsTrue(arr[7].Type == StackItemType.Integer);
            Assert.IsTrue((arr[6].ConvertTo(StackItemType.ByteString) as PrimitiveType)?.GetInteger() == (arr[7] as PrimitiveType)?.GetInteger());
        }
    }
}
