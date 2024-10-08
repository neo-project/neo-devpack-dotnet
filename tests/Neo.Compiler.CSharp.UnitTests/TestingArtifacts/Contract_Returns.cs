using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Returns(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Returns"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""subtract"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""div"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Array"",""offset"":106,""safe"":false},{""name"":""mix"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":121,""safe"":false},{""name"":""byteStringAdd"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":139,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJRXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnh5n0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACxUp4eaHPSnh5os9AVwICeXg07ErBRXBxRWloNK1AVwACeHmL2yhAGjay3A=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.CAT
    /// 06 : OpCode.CONVERT 28
    /// 08 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringAdd")]
    public abstract byte[]? ByteStringAdd(byte[]? a, byte[]? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACxUp4eaHPSnh5os9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.NEWSTRUCT0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.LDARG1
    /// 07 : OpCode.DIV
    /// 08 : OpCode.APPEND
    /// 09 : OpCode.DUP
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.MOD
    /// 0D : OpCode.APPEND
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("div")]
    public abstract IList<object>? Div(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXg07ErBRXBxRWloNK1A
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.CALL EC
    /// 07 : OpCode.DUP
    /// 08 : OpCode.UNPACK
    /// 09 : OpCode.DROP
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.DROP
    /// 0D : OpCode.LDLOC1
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.CALL AD
    /// 11 : OpCode.RET
    /// </remarks>
    [DisplayName("mix")]
    public abstract BigInteger? Mix(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.SUB
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.RET
    /// </remarks>
    [DisplayName("subtract")]
    public abstract BigInteger? Subtract(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.RET
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    #endregion
}
