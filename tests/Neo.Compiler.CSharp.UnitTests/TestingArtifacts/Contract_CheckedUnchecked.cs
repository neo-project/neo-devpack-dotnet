using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_CheckedUnchecked(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_CheckedUnchecked"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""addChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""addUnchecked"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":25,""safe"":false},{""name"":""castChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""castUnchecked"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":101,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x4e58396ad6de3b04dff8f898b97f872e38b84ece"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALOTrg4Lod/uZj4+N8EO97WajlYTgl0ZXN0QXJnczEBAAEPzk64OC6Hf7mY+PjfBDve1mo5WE4IdGVzdFZvaWQAAAAPAACGVwACeHmeSgIAAACALgM6SgL///9/MgM6QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwABeEoQLgM6SgP/////AAAAADIDOkBXAAF4ShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUBDcxnT"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgM6SgL///9/MgM6QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0C : OpCode.JMPGE 03 [2 datoshi]
    /// 0E : OpCode.THROW [512 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 15 : OpCode.JMPLE 03 [2 datoshi]
    /// 17 : OpCode.THROW [512 datoshi]
    /// 18 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("addChecked")]
    public abstract BigInteger? AddChecked(BigInteger? a, BigInteger? b);

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
    [DisplayName("addUnchecked")]
    public abstract BigInteger? AddUnchecked(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6SgP/////AAAAADIDOkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.JMPGE 03 [2 datoshi]
    /// 08 : OpCode.THROW [512 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 13 : OpCode.JMPLE 03 [2 datoshi]
    /// 15 : OpCode.THROW [512 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("castChecked")]
    public abstract BigInteger? CastChecked(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.JMPGE 04 [2 datoshi]
    /// 08 : OpCode.JMP 0E [2 datoshi]
    /// 0A : OpCode.DUP [2 datoshi]
    /// 0B : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 14 : OpCode.JMPLE 0C [2 datoshi]
    /// 16 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1F : OpCode.AND [8 datoshi]
    /// 20 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("castUnchecked")]
    public abstract BigInteger? CastUnchecked(BigInteger? a);

    #endregion
}
