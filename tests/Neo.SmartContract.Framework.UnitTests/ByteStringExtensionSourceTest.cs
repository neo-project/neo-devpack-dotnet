// Copyright (C) 2015-2026 The Neo Project.
//
// ByteStringExtensionSourceTest.cs file belongs to the neo project and is free
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

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class ByteStringExtensionSourceTest
{
    [TestMethod]
    public void CharacterClassHelpers_DoNotUseByteStringEnumeration()
    {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var sourcePath = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "ByteString.Extension.cs");
        var source = File.ReadAllText(sourcePath);

        Assert.IsFalse(source.Contains("foreach", StringComparison.Ordinal));
    }
}
