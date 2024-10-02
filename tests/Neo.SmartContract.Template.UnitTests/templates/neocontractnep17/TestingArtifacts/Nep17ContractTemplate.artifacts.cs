using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":1126,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1141,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":43,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":89,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":272,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":684,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":726,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":846,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":886,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":926,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":932,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":950,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1071,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1110,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9hQRXAAEMB0VYQU1QTEVAVwABeDQDQFcAAXg0A0BXAAF4NANAVwABQFcAARhAWdgmFwwBAEH2tGviQZJd6DFK2CYERRBKYUBXAAF4YXgMAQBBm/ZnzkHmPxiEQFcBAXhwaAuXJgUIIg14StkoUMoAFLOrqiYlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjpBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFAVwICQZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshcWl5nnFpELUmBAlAaRCzJhB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQIQFcBBHhwaAuXJgUIIg14StkoUMoAFLOrqiYkDB9UaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlwaAuXJgUIIg15StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IyqJgQJQHoQmCYXept4Nfj+//+qJgQJQHp5Nez+//9Fe3p5eDQECEBXAQTCSnjPSnnPSnrPDAhUcmFuc2ZlckGVAW9heXBoC5eqJAUJIgt5NwAAcGgLl6omH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFcAAnmZELUmCwwGYW1vdW50OnkQsyYDQHl4NXT+//9FNd79//95nko18P3//0ULeXgLNX7///9AVwACeZkQtSYLDAZhbW91bnQ6eRCzJgNAeZt4NTv+//+qJg4MCWV4Y2VwdGlvbjo1l/3//3mfSjWp/f//RQt5C3g1N////0AMAf/bMDQOStgkCUrKABQoAzpAVwABeEH2tGviQZJd6DFANN5B+CfsjEBXAQE09QmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnhK2ShQygAUs6skBQkiBngQs6oME293bmVyIG11c3QgYmUgdmFsaWThNJBweAwB/9swNBnCSmjPSnjPDAhTZXRPd25lckGVAW9hQFcAAnl4QZv2Z85B5j8YhEBXAAI1ff///wmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4NfX+//9AVwACNVX///8JlyYWDBFObyBBdXRob3JpemF0aW9uITp5eDWV/v//QDUw////QAwFSGVsbG9Bm/ZnzkGSXegxQFcBAnkmA0B4cGgLlyYKQS1RCDATzoB4cGhK2ShQygAUs6skBQkiBmgQs6oMEW93bmVyIG11c3QgZXhpc3Rz4WgMAf/bMDVD////wkoLz0pozwwIU2V0T3duZXJBlQFvYQwFV29ybGQMBUhlbGxvQZv2Z85B5j8YhEBXAAM1nP7//wmXJhYMEU5vIGF1dGhvcml6YXRpb24uOnp5eDcBAEBWAgrO+///CqP7//8SwGBAwkpYz0o1ovv//yOQ+///wkpYz0o1k/v//yOn+///QNnyp4Q="));

    #endregion

    #region Events

    [DisplayName("SetOwner")]
    public event Neo.SmartContract.Testing.TestingStandards.IOwnable.delSetOwner? OnSetOwner;

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
    public abstract UInt160? Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract bool? Verify { [DisplayName("verify")] get; }

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
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);
    // 0000 : INITSLOT
    // 0003 : CALL_L
    // 0008 : PUSHF
    // 0009 : EQUAL
    // 000A : JMPIFNOT
    // 000C : PUSHDATA1
    // 001F : THROW
    // 0020 : LDARG1
    // 0021 : LDARG0
    // 0022 : CALL_L
    // 0027 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);
    // 0000 : INITSLOT
    // 0003 : CALL_L
    // 0008 : PUSHF
    // 0009 : EQUAL
    // 000A : JMPIFNOT
    // 000C : PUSHDATA1
    // 001F : THROW
    // 0020 : LDARG1
    // 0021 : LDARG0
    // 0022 : CALL_L
    // 0027 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();
    // 0000 : PUSHDATA1
    // 0007 : SYSCALL
    // 000C : SYSCALL
    // 0011 : RET

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

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);
    // 0000 : INITSLOT
    // 0003 : CALL_L
    // 0008 : PUSHF
    // 0009 : EQUAL
    // 000A : JMPIFNOT
    // 000C : PUSHDATA1
    // 001F : THROW
    // 0020 : LDARG2
    // 0021 : LDARG1
    // 0022 : LDARG0
    // 0023 : CALLT
    // 0026 : RET

    #endregion

}
