using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJFXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnh5n0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeHmieHmhEr9AVwICeXg070rBRXBxRWloNLBAVwACeHmL2yhAbONrzw==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : CAT [2048 datoshi]
    /// 06 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteStringAdd")]
    public abstract byte[]? ByteStringAdd(byte[]? a, byte[]? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmieHmhEr9A
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : LDARG1 [2 datoshi]
    /// 08 : DIV [8 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : PACKSTRUCT [2048 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("div")]
    public abstract IList<object>? Div(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXg070rBRXBxRWloNLBA
    /// 00 : INITSLOT 0202 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : CALL EF [512 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : UNPACK [2048 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : STLOC1 [2 datoshi]
    /// 0C : DROP [2 datoshi]
    /// 0D : LDLOC1 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : CALL B0 [512 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("mix")]
    public abstract BigInteger? Mix(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : SUB [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("subtract")]
    public abstract BigInteger? Subtract(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    #endregion
}
