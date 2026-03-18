// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ProjectBoundaries.cs file belongs to the neo project and is free
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
using System.Xml.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class ProjectBoundaryTests
    {
        [TestMethod]
        public void CompilerProject_DoesNotReference_TestingProject()
        {
            string csprojPath = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "..",
                "src", "Neo.Compiler.CSharp", "Neo.Compiler.CSharp.csproj"));

            var project = XDocument.Load(csprojPath);
            var references = project.Descendants("ProjectReference");

            Assert.IsFalse(
                references.Any(reference => (string?)reference.Attribute("Include") is string include
                    && include.Contains("Neo.SmartContract.Testing.csproj")),
                "Neo.Compiler.CSharp.csproj should not reference Neo.SmartContract.Testing.csproj");
        }
    }
}
