using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Returns(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Returns"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""subtract"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""div"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Array"",""offset"":106,""safe"":false},{""name"":""mix"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":118,""safe"":false},{""name"":""byteStringAdd"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":136,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJFXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnh5n0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeHmieHmhEr9AVwICeXg070rBRXBxRWloNLBAVwACeHmL2yhAbONrzw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.CAT [2048 datoshi]
    /// 06 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteStringAdd")]
    public abstract byte[]? ByteStringAdd(byte[]? a, byte[]? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmieHmhEr9A
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.MOD [8 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.LDARG1 [2 datoshi]
    /// 08 : OpCode.DIV [8 datoshi]
    /// 09 : OpCode.PUSH2 [1 datoshi]
    /// 0A : OpCode.PACKSTRUCT [2048 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("div")]
    public abstract IList<object>? Div(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXg070rBRXBxRWloNLBA
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.CALL EF [512 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.UNPACK [2048 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.STLOC1 [2 datoshi]
    /// 0C : OpCode.DROP [2 datoshi]
    /// 0D : OpCode.LDLOC1 [2 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.CALL B0 [512 datoshi]
    /// 11 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("mix")]
    public abstract BigInteger? Mix(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.SUB [8 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0C : OpCode.JMPGE 04 [2 datoshi]
    /// 0E : OpCode.JMP 0A [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : OpCode.JMPLE 1E [2 datoshi]
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : OpCode.AND [8 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : OpCode.JMPLE 0C [2 datoshi]
    /// 2A : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : OpCode.SUB [8 datoshi]
    /// 34 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("subtract")]
    public abstract BigInteger? Subtract(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0C : OpCode.JMPGE 04 [2 datoshi]
    /// 0E : OpCode.JMP 0A [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : OpCode.JMPLE 1E [2 datoshi]
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : OpCode.AND [8 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : OpCode.JMPLE 0C [2 datoshi]
    /// 2A : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : OpCode.SUB [8 datoshi]
    /// 34 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    #endregion
}
