using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NEP17(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NEP17"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":550,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":565,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":40,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":66,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":249,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":534,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP17Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP1FAlcAAQwEVEVTVEBXAAF4NANAVwABeDQDQFcAAXg0A0BXAAFAVwABGEBZ2CYXDAEAQfa0a+JBkl3oMUrYJgRFEEphQFcBAXhwaAuXJgUIIg14StkoUMoAFLOrqiYlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjpBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFAVwICQZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshcWl5nnFpELUmBAlAaRCzJhB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQIQFcBBHhwaAuXJgUIIg14StkoUMoAFLOrqiYkDB9UaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlwaAuXJgUIIg15StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IyqJgQJQHoQmCYXept4Nfj+//+qJgQJQHp5Nez+//9Fe3p5eDQECEBXAQTCSnjPSnnPSnrPDAhUcmFuc2ZlckGVAW9heXBoC5eqJAUJIgt5NwAAcGgLl6omH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFYCCgv+//8K4/3//xLAYEDCSljPSjXf/f//I9D9///CSljPSjXQ/f//I+T9//9Ad1sT8w=="));

    #endregion

    #region Events

    [DisplayName("Transfer")]
    public event Neo.SmartContract.Testing.TestingStandards.INep17Standard.delTransfer? OnTransfer;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHNULL
    // 0007 : EQUAL
    // 0008 : JMPIFNOT
    // 000A : PUSHT
    // 000B : JMP
    // 000D : LDARG0
    // 000E : DUP
    // 000F : ISTYPE
    // 0011 : SWAP
    // 0012 : SIZE
    // 0013 : PUSHINT8
    // 0015 : NUMEQUAL
    // 0016 : BOOLAND
    // 0017 : NOT
    // 0018 : JMPIFNOT
    // 001A : PUSHDATA1
    // 003C : THROW
    // 003D : SYSCALL
    // 0042 : PUSH1
    // 0043 : PUSH1
    // 0044 : NEWBUFFER
    // 0045 : TUCK
    // 0046 : PUSH0
    // 0047 : ROT
    // 0048 : SETITEM
    // 0049 : SWAP
    // 004A : PUSH2
    // 004B : PACK
    // 004C : STLOC0
    // 004D : LDARG0
    // 004E : LDLOC0
    // 004F : UNPACK
    // 0050 : DROP
    // 0051 : REVERSE3
    // 0052 : CAT
    // 0053 : SWAP
    // 0054 : SYSCALL
    // 0059 : DUP
    // 005A : ISNULL
    // 005B : JMPIFNOT
    // 005D : DROP
    // 005E : PUSH0
    // 005F : CONVERT
    // 0061 : RET

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHNULL
    // 0007 : EQUAL
    // 0008 : JMPIFNOT
    // 000A : PUSHT
    // 000B : JMP
    // 000D : LDARG0
    // 000E : DUP
    // 000F : ISTYPE
    // 0011 : SWAP
    // 0012 : SIZE
    // 0013 : PUSHINT8
    // 0015 : NUMEQUAL
    // 0016 : BOOLAND
    // 0017 : NOT
    // 0018 : JMPIFNOT
    // 001A : PUSHDATA1
    // 003B : THROW
    // 003C : LDARG1
    // 003D : STLOC0
    // 003E : LDLOC0
    // 003F : PUSHNULL
    // 0040 : EQUAL
    // 0041 : JMPIFNOT
    // 0043 : PUSHT
    // 0044 : JMP
    // 0046 : LDARG1
    // 0047 : DUP
    // 0048 : ISTYPE
    // 004A : SWAP
    // 004B : SIZE
    // 004C : PUSHINT8
    // 004E : NUMEQUAL
    // 004F : BOOLAND
    // 0050 : NOT
    // 0051 : JMPIFNOT
    // 0053 : PUSHDATA1
    // 0072 : THROW
    // 0073 : LDARG2
    // 0074 : PUSH0
    // 0075 : LT
    // 0076 : JMPIFNOT
    // 0078 : PUSHDATA1
    // 009F : THROW
    // 00A0 : LDARG0
    // 00A1 : SYSCALL
    // 00A6 : NOT
    // 00A7 : JMPIFNOT
    // 00A9 : PUSHF
    // 00AA : RET
    // 00AB : LDARG2
    // 00AC : PUSH0
    // 00AD : NOTEQUAL
    // 00AE : JMPIFNOT
    // 00B0 : LDARG2
    // 00B1 : NEGATE
    // 00B2 : LDARG0
    // 00B3 : CALL_L
    // 00B8 : NOT
    // 00B9 : JMPIFNOT
    // 00BB : PUSHF
    // 00BC : RET
    // 00BD : LDARG2
    // 00BE : LDARG1
    // 00BF : CALL_L
    // 00C4 : DROP
    // 00C5 : LDARG3
    // 00C6 : LDARG2
    // 00C7 : LDARG1
    // 00C8 : LDARG0
    // 00C9 : CALL
    // 00CB : PUSHT
    // 00CC : RET

    #endregion

}
