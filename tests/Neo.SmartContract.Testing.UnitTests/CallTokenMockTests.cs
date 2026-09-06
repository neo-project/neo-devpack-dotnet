// Copyright (C) 2015-2026 The Neo Project.
//
// CallTokenMockTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.Extensions;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;

namespace Neo.SmartContract.Testing.UnitTests;

[TestClass]
public class CallTokenMockTests
{
    public abstract class TargetContract(SmartContractInitialize initialize) : SmartContract(initialize)
    {
        [DisplayName("combine")]
        public abstract BigInteger? Combine(BigInteger? first, BigInteger? second);

        [DisplayName("notify")]
        public abstract void Notify(BigInteger? amount);
    }

    public abstract class CallerContract(SmartContractInitialize initialize) : SmartContract(initialize)
    {
        [DisplayName("value")]
        public abstract BigInteger? Value();
    }

    [TestMethod]
    public void TokenAndDynamicCallsUseTheSameMockAndArgumentOrder()
    {
        var engine = new TestEngine();
        var target = DeployTarget(engine);
        Mock<TargetContract>? mock = null;
        engine.FromHash<TargetContract>(target.Hash, m =>
        {
            mock = m;
            m.Setup(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>()))
                .Returns((BigInteger? first, BigInteger? second) => first * 10 + second);
        });
        var tokenCaller = DeployTokenCaller(engine, target.Hash);
        using var script = new ScriptBuilder();
        script.EmitDynamicCall(target.Hash, "combine", 1, 2);
        script.Emit(OpCode.RET);
        var dynamicCaller = DeployCaller(engine, "DynamicCaller", script.ToArray());

        Assert.AreEqual(new BigInteger(12), tokenCaller.Value());
        Assert.AreEqual(new BigInteger(12), dynamicCaller.Value());
        mock!.Verify(c => c.Combine(new BigInteger(1), new BigInteger(2)), Times.Exactly(2));
    }

    [TestMethod]
    public void TokenMockCanReturnNull()
    {
        var engine = new TestEngine();
        var target = DeployTarget(engine);
        engine.FromHash<TargetContract>(target.Hash, m =>
            m.Setup(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>())).Returns((BigInteger?)null));
        var caller = DeployTokenCaller(engine, target.Hash);

        Assert.IsNull(caller.Value());
    }

    [TestMethod]
    public void VoidTokenMockDoesNotLeaveAResultOnTheStack()
    {
        var engine = new TestEngine();
        var target = DeployTarget(engine);
        Mock<TargetContract>? mock = null;
        engine.FromHash<TargetContract>(target.Hash, m =>
        {
            mock = m;
            m.Setup(c => c.Notify(It.IsAny<BigInteger?>()));
        });
        var caller = DeployTokenCaller(engine, target.Hash, "notify", false, [42], OpCode.DEPTH);

        Assert.AreEqual(BigInteger.Zero, caller.Value());
        mock!.Verify(c => c.Notify(new BigInteger(42)), Times.Once);
    }

    [TestMethod]
    public void TokenMockExceptionsFaultTheExecutionAndRestoreEngineStorage()
    {
        var engine = new TestEngine();
        var target = DeployTarget(engine);
        var storage = engine.Storage;
        engine.FromHash<TargetContract>(target.Hash, m =>
            m.Setup(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>()))
                .Throws(new InvalidOperationException("token mock failure")));
        var caller = DeployTokenCaller(engine, target.Hash);

        var exception = Assert.ThrowsException<TestException>(() => caller.Value());
        StringAssert.Contains(exception.ToString(), "token mock failure");
        Assert.AreSame(storage, engine.Storage);
    }

    [TestMethod]
    public void ReleasingATokenMockRestoresTheOriginalContractCall()
    {
        var engine = new TestEngine();
        var target = DeployTarget(engine);
        var caller = DeployTokenCaller(engine, target.Hash);
        Assert.AreEqual(new BigInteger(7), caller.Value());
        var mocked = engine.FromHash<TargetContract>(target.Hash, m =>
            m.Setup(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>())).Returns(new BigInteger(123)));

        Assert.AreEqual(new BigInteger(123), caller.Value());
        mocked.Dispose();
        Assert.AreEqual(new BigInteger(7), caller.Value());
    }

    [TestMethod]
    public void TokenMocksCanTargetUndeployedContracts()
    {
        var engine = new TestEngine();
        engine.FromHash<TargetContract>(UInt160.Zero, m =>
            m.Setup(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>())).Returns(new BigInteger(123)), false);
        var caller = DeployTokenCaller(engine, UInt160.Zero);

        Assert.AreEqual(new BigInteger(123), caller.Value());
    }

    [TestMethod]
    public void TokenMocksDoNotChangeOtherEnginesOrTheDefaultJumpTable()
    {
        var defaultJumpTable = (JumpTable)typeof(ApplicationEngine)
            .GetField("DefaultJumpTable", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        var originalHandler = defaultJumpTable[OpCode.CALLT];
        var first = new TestEngine();
        var firstTarget = DeployTarget(first);
        first.FromHash<TargetContract>(firstTarget.Hash, m =>
            m.Setup(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>())).Returns(new BigInteger(123)));
        var firstCaller = DeployTokenCaller(first, firstTarget.Hash);
        var second = new TestEngine();
        var secondTarget = DeployTarget(second);
        var secondCaller = DeployTokenCaller(second, secondTarget.Hash);

        Assert.AreEqual(new BigInteger(123), firstCaller.Value());
        Assert.AreEqual(new BigInteger(7), secondCaller.Value());
        Assert.AreSame(originalHandler, defaultJumpTable[OpCode.CALLT]);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TokenMocksRejectMismatchedReturnShapes(bool tokenReturnsValue)
    {
        var engine = new TestEngine();
        var target = DeployTarget(engine);
        Mock<TargetContract>? mock = null;
        engine.FromHash<TargetContract>(target.Hash, m =>
        {
            mock = m;
            m.Setup(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>())).Returns(new BigInteger(123));
            m.Setup(c => c.Notify(It.IsAny<BigInteger?>()));
        });
        var caller = tokenReturnsValue
            ? DeployTokenCaller(engine, target.Hash, "notify", true, [42])
            : DeployTokenCaller(engine, target.Hash, "combine", false, [1, 2]);

        Assert.ThrowsException<TestException>(() => caller.Value());
        mock!.Verify(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>()), Times.Never);
        mock.Verify(c => c.Notify(It.IsAny<BigInteger?>()), Times.Never);
    }

    [TestMethod]
    public void TokenMocksStillRequireReadStatesAndAllowCall()
    {
        var engine = new TestEngine();
        var target = DeployTarget(engine);
        Mock<TargetContract>? mock = null;
        engine.FromHash<TargetContract>(target.Hash, m =>
        {
            mock = m;
            m.Setup(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>())).Returns(new BigInteger(123));
        });
        var caller = DeployTokenCaller(engine, target.Hash);
        engine.CallFlags = CallFlags.AllowCall;

        Assert.ThrowsException<TestException>(() => caller.Value());
        mock!.Verify(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>()), Times.Never);
    }

    [TestMethod]
    public void TokenMocksRejectInsufficientArgumentsBeforeInvokingTheMock()
    {
        var engine = new TestEngine();
        var target = DeployTarget(engine);
        Mock<TargetContract>? mock = null;
        engine.FromHash<TargetContract>(target.Hash, m =>
        {
            mock = m;
            m.Setup(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>())).Returns(new BigInteger(123));
        });
        var caller = DeployTokenCaller(engine, target.Hash, arguments: [1], parametersCount: 2);

        Assert.ThrowsException<TestException>(() => caller.Value());
        mock!.Verify(c => c.Combine(It.IsAny<BigInteger?>(), It.IsAny<BigInteger?>()), Times.Never);
    }

    private static TargetContract DeployTarget(TestEngine engine)
    {
        byte[] script = [(byte)OpCode.INITSLOT, 0, 2, (byte)OpCode.PUSH7, (byte)OpCode.RET,
            (byte)OpCode.INITSLOT, 0, 1, (byte)OpCode.RET];
        var manifest = CreateManifest("Target", [
            Method("combine", 0, ContractParameterType.Integer, 2),
            Method("notify", 5, ContractParameterType.Void, 1)]);
        return engine.Deploy<TargetContract>(CreateNef(script), manifest);
    }

    private static CallerContract DeployTokenCaller(TestEngine engine, UInt160 target, string method = "combine",
        bool hasReturnValue = true, int[]? arguments = null, OpCode? afterCall = null, ushort? parametersCount = null)
    {
        arguments ??= [1, 2];
        using var script = new ScriptBuilder();
        for (int i = arguments.Length - 1; i >= 0; i--) script.EmitPush(arguments[i]);
        script.Emit(OpCode.CALLT, new byte[] { 0, 0 });
        if (afterCall.HasValue) script.Emit(afterCall.Value);
        script.Emit(OpCode.RET);
        var token = new MethodToken
        {
            Hash = target,
            Method = method,
            ParametersCount = parametersCount ?? (ushort)arguments.Length,
            HasReturnValue = hasReturnValue,
            CallFlags = CallFlags.All
        };
        return DeployCaller(engine, "TokenCaller", script.ToArray(), token);
    }

    private static CallerContract DeployCaller(TestEngine engine, string name, byte[] script, params MethodToken[] tokens)
        => engine.Deploy<CallerContract>(CreateNef(script, tokens), CreateManifest(name,
            [Method("value", 0, ContractParameterType.Integer, 0)]));

    private static NefFile CreateNef(byte[] script, params MethodToken[] tokens)
    {
        var nef = new NefFile { Compiler = "test", Source = "", Script = script, Tokens = tokens };
        nef.CheckSum = NefFile.ComputeChecksum(nef);
        return nef;
    }

    private static ContractManifest CreateManifest(string name, ContractMethodDescriptor[] methods)
        => new()
        {
            Name = name,
            Groups = [],
            SupportedStandards = [],
            Abi = new ContractAbi { Methods = methods, Events = [] },
            Permissions = [ContractPermission.DefaultPermission],
            Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
            Extra = null
        };

    private static ContractMethodDescriptor Method(string name, int offset, ContractParameterType returnType, int parameters)
    {
        var definitions = new ContractParameterDefinition[parameters];
        for (int i = 0; i < parameters; i++)
            definitions[i] = new ContractParameterDefinition { Name = "arg" + i, Type = ContractParameterType.Integer };
        return new ContractMethodDescriptor { Name = name, Offset = offset, ReturnType = returnType, Parameters = definitions };
    }
}
