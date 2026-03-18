// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RuntimeAssemblyResolver.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class RuntimeAssemblyResolverTests
    {
        [TestMethod]
        public void ResolveDependencyAssembly_Finds_Loaded_TestingAssembly()
        {
            string path = RuntimeAssemblyResolver.ResolveDependencyAssembly("Neo.SmartContract.Testing.dll");

            Assert.IsTrue(File.Exists(path));
            StringAssert.EndsWith(path, "Neo.SmartContract.Testing.dll");
        }

        [TestMethod]
        public void ResolveDependencyAssembly_Throws_For_MissingAssembly()
        {
            Assert.ThrowsException<FileNotFoundException>(() =>
                RuntimeAssemblyResolver.ResolveDependencyAssembly("Definitely.Missing.Dependency.dll"));
        }
    }
}
