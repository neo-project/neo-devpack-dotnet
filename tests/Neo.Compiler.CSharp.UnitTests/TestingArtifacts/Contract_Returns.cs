using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Returns(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Returns"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""subtract"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":40,""safe"":false},{""name"":""div"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Array"",""offset"":80,""safe"":false},{""name"":""mix"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":155,""safe"":false},{""name"":""byteStringAdd"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":171,""safe"":false},{""name"":""tryReturn"",""parameters"":[],""returntype"":""Integer"",""offset"":447,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0CAlcAAnh5nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAJ4eZ9KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwECQ3B4eUoPKhxLAgAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqF4eUoPKhxLAgAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqJQEr9AVwICeXg0sMFFcHFpaDSAQFcAAnh5i9soQFcCARBwDAEADAEAQTkM4wo8AAAAAL0AAAA7OWZ4Jg4MCWV4Y2VwdGlvbjponErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwPSpxaJxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcD0CPpAAAAAMAQBB1Y1e6AwBAJc5DAEBDAEAQTkM4wpoSpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRXgmDgwJZXhjZXB0aW9uOj8MAQBB1Y1e6AwBAZc5DAECDAEAQTkM4wponErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3A/QFcBABBwCTXv/v//cGgRlzkMAQBB1Y1e6AwBApc5aJxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEBbZ06R").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteStringAdd")]
    public abstract byte[]? ByteStringAdd(byte[]? a, byte[]? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B4eUoPKhxLAgAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqF4eUoPKhxLAgAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqJQEr9A
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 1C [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 1C [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("div")]
    public abstract IList<object>? Div(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXg0sMFFcHFpaDSAQA==
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL B0 [512 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 80 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mix")]
    public abstract BigInteger? Mix(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("subtract")]
    public abstract BigInteger? Subtract(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAJNe/+//9waBGXOQwBAEHVjV7oDAEClzlonErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// CALL_L EFFEFFFF [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryReturn")]
    public abstract BigInteger? TryReturn();

    #endregion
}
