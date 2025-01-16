// Copyright (C) 2015-2024 The Neo Project.
//
// UnitTest_OnDeployment2.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_OnDeployment2 : DebugAndTestBase<Contract_OnDeployment2>
    {
        private readonly List<string> _logs = new();

        #region Ensure Deployed log

        protected override TestEngine CreateTestEngine()
        {
            var engine = base.CreateTestEngine();

            engine.OnRuntimeLog += (sender, log) =>
            {
                _logs.Add(log);
            };

            return engine;
        }

        public override void TestBaseSetup(NefFile nefFile, ContractManifest manifestFile, NeoDebugInfo? debugInfo = null)
        {
            base.TestBaseSetup(nefFile, manifestFile, debugInfo);

            Assert.AreEqual(1, _logs.Count);
            Assert.AreEqual("Deployed", _logs[0]);
        }

        #endregion

        [TestMethod]
        public void Test_OnDeployment2()
        {
            Assert.AreEqual(1, Contract_OnDeployment2.Manifest.Abi.Methods.Length);
            Assert.AreEqual(Contract_OnDeployment2.Manifest.Abi.Methods[0].Name, "_deploy");
            Assert.AreEqual(Contract_OnDeployment2.Manifest.Abi.Methods[0].Offset, 0);
            Assert.AreEqual(Contract_OnDeployment2.Manifest.Abi.Methods[0].ReturnType, ContractParameterType.Void);

            var args = Contract_OnDeployment2.Manifest.Abi.Methods[0].Parameters;

            Assert.AreEqual(2, args.Length);
            Assert.AreEqual(args[0].Name, "data");
            Assert.AreEqual(args[0].Type, ContractParameterType.Any);
            Assert.AreEqual(args[1].Name, "update");
            Assert.AreEqual(args[1].Type, ContractParameterType.Boolean);
        }
    }
}
