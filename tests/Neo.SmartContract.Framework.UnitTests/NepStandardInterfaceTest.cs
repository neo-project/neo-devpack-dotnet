// Copyright (C) 2015-2026 The Neo Project.
//
// NepStandardInterfaceTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using NepStandard = scfx::Neo.SmartContract.Framework.NepStandard;
using SupportedStandardsAttribute = scfx::Neo.SmartContract.Framework.Attributes.SupportedStandardsAttribute;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class NepStandardInterfaceTest
    {
        [TestMethod]
        public void Nep17Token_Implements_NEP17_Interface()
        {
            Assert.IsTrue(typeof(scfx::Neo.SmartContract.Framework.Interfaces.INEP17)
                .IsAssignableFrom(typeof(scfx::Neo.SmartContract.Framework.Nep17Token)));

            var transfer = typeof(scfx::Neo.SmartContract.Framework.Interfaces.INEP17)
                .GetMethod(nameof(scfx::Neo.SmartContract.Framework.Interfaces.INEP17.Transfer));
            Assert.IsNotNull(transfer);
            Assert.AreEqual(4, transfer!.GetParameters().Length);
        }

        [TestMethod]
        public void Nep11Token_Implements_NEP11_Interface()
        {
            Assert.IsTrue(typeof(scfx::Neo.SmartContract.Framework.Interfaces.INEP11)
                .IsAssignableFrom(typeof(scfx::Neo.SmartContract.Framework.Nep11Token<scfx::Neo.SmartContract.Framework.Nep11TokenState>)));

            var methods = typeof(scfx::Neo.SmartContract.Framework.Interfaces.INEP11).GetMethods().Select(m => m.Name).ToArray();
            CollectionAssert.Contains(methods, nameof(scfx::Neo.SmartContract.Framework.Interfaces.INEP11.OwnerOf));
            CollectionAssert.Contains(methods, nameof(scfx::Neo.SmartContract.Framework.Interfaces.INEP11.Properties));
            CollectionAssert.Contains(methods, nameof(scfx::Neo.SmartContract.Framework.Interfaces.INEP11.Transfer));
            CollectionAssert.Contains(methods, nameof(scfx::Neo.SmartContract.Framework.Interfaces.INEP11.Tokens));
            CollectionAssert.Contains(methods, nameof(scfx::Neo.SmartContract.Framework.Interfaces.INEP11.TokensOf));
        }

        [TestMethod]
        public void Nep17Interface_Defines_SupportedStandardAttribute()
        {
            var attribute = typeof(scfx::Neo.SmartContract.Framework.Interfaces.INEP17)
                .GetCustomAttributes(typeof(SupportedStandardsAttribute), false)
                .SingleOrDefault() as SupportedStandardsAttribute;

            Assert.IsNotNull(attribute);
        }

        [TestMethod]
        public void Nep11Interface_Defines_SupportedStandardAttribute()
        {
            var attribute = typeof(scfx::Neo.SmartContract.Framework.Interfaces.INEP11)
                .GetCustomAttributes(typeof(SupportedStandardsAttribute), false)
                .SingleOrDefault() as SupportedStandardsAttribute;

            Assert.IsNotNull(attribute);
        }
    }
}
