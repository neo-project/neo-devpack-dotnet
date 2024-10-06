using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":58,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":241,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":653,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":695,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":815,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":855,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":895,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":901,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":919,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1040,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1079,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9OgQMB0VYQU1QTEVAGEBY2CYXDAEAQfa0a+JBkl3oMUrYJgRFEEpgQFcAAXhgeAwBAEGb9mfOQeY/GIRAVwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIUBXAgJBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFxaXmecWkQtSYECUBpELMmEHhowUVTi1BBL1jF7SIPaXhowUVTi1BB5j8YhAhAVwEEeHBoC5cmBQgiDXhK2ShQygAUs6uqJiQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eXBoC5cmBQgiDXlK2ShQygAUs6uqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1+P7//6omBAlAenk17P7//0V7enl4NAQIQFcBBMJKeM9Kec9Kes8MCFRyYW5zZmVyQZUBb2F5cGgLl6okBQkiC3k3AABwaAuXqiYfe3p4E8AfDA5vbk5FUDE3UGF5bWVudHlBYn1bUkVAVwACeZkQtSYLDAZhbW91bnQ6eRCzJgNAeXg1dP7//0U13v3//3meSjXw/f//RQt5eAs1fv///0BXAAJ5mRC1JgsMBmFtb3VudDp5ELMmA0B5m3g1O/7//6omDgwJZXhjZXB0aW9uOjWX/f//eZ9KNan9//9FC3kLeDU3////QAwB/9swNA5K2CQJSsoAFCgDOkBXAAF4Qfa0a+JBkl3oMUA03kH4J+yMQFcBATT1CZcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eErZKFDKABSzqyQFCSIGeBCzqgwTb3duZXIgbXVzdCBiZSB2YWxpZOE0kHB4DAH/2zA0GcJKaM9KeM8MCFNldE93bmVyQZUBb2FAVwACeXhBm/ZnzkHmPxiEQFcAAjV9////CZcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg19f7//0BXAAI1Vf///wmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4NZX+//9ANTD///9ADAVIZWxsb0Gb9mfOQZJd6DFAVwECeSYDQHhwaAuXJgpBLVEIMBPOgHhwaErZKFDKABSzqyQFCSIGaBCzqgwRb3duZXIgbXVzdCBleGlzdHPhaAwB/9swNUP////CSgvPSmjPDAhTZXRPd25lckGVAW9hDAVXb3JsZAwFSGVsbG9Bm/ZnzkHmPxiEQFcAAzWc/v//CZcmFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwEAQFYBQK/iE+I="));

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
    /// <remarks>
    /// Script: VwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiUMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHNULL
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.JMPIFNOT 05
    /// 000A : OpCode.PUSHT
    /// 000B : OpCode.JMP 0D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.ISTYPE 28
    /// 0011 : OpCode.SWAP
    /// 0012 : OpCode.SIZE
    /// 0013 : OpCode.PUSHINT8 14
    /// 0015 : OpCode.NUMEQUAL
    /// 0016 : OpCode.BOOLAND
    /// 0017 : OpCode.NOT
    /// 0018 : OpCode.JMPIFNOT 25
    /// 001A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 003C : OpCode.THROW
    /// 003D : OpCode.SYSCALL 9BF667CE
    /// 0042 : OpCode.PUSH1
    /// 0043 : OpCode.PUSH1
    /// 0044 : OpCode.NEWBUFFER
    /// 0045 : OpCode.TUCK
    /// 0046 : OpCode.PUSH0
    /// 0047 : OpCode.ROT
    /// 0048 : OpCode.SETITEM
    /// 0049 : OpCode.SWAP
    /// 004A : OpCode.PUSH2
    /// 004B : OpCode.PACK
    /// 004C : OpCode.STLOC0
    /// 004D : OpCode.LDARG0
    /// 004E : OpCode.LDLOC0
    /// 004F : OpCode.UNPACK
    /// 0050 : OpCode.DROP
    /// 0051 : OpCode.REVERSE3
    /// 0052 : OpCode.CAT
    /// 0053 : OpCode.SWAP
    /// 0054 : OpCode.SYSCALL 925DE831
    /// 0059 : OpCode.DUP
    /// 005A : OpCode.ISNULL
    /// 005B : OpCode.JMPIFNOT 04
    /// 005D : OpCode.DROP
    /// 005E : OpCode.PUSH0
    /// 005F : OpCode.CONVERT 21
    /// 0061 : OpCode.RET
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNX3///8JlyYWDE5vIEF1dGhvcml6YXRpb24hOnl4NfX+//9A
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.CALL_L 7DFFFFFF
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.EQUAL
    /// 000A : OpCode.JMPIFNOT 16
    /// 000C : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 001F : OpCode.THROW
    /// 0020 : OpCode.LDARG1
    /// 0021 : OpCode.LDARG0
    /// 0022 : OpCode.CALL_L F5FEFFFF
    /// 0027 : OpCode.RET
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNVX///8JlyYWDE5vIEF1dGhvcml6YXRpb24hOnl4NZX+//9A
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.CALL_L 55FFFFFF
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.EQUAL
    /// 000A : OpCode.JMPIFNOT 16
    /// 000C : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 001F : OpCode.THROW
    /// 0020 : OpCode.LDARG1
    /// 0021 : OpCode.LDARG0
    /// 0022 : OpCode.CALL_L 95FEFFFF
    /// 0027 : OpCode.RET
    /// </remarks>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEhlbGxvQZv2Z85Bkl3oMUA=
    /// 0000 : OpCode.PUSHDATA1 48656C6C6F
    /// 0007 : OpCode.SYSCALL 9BF667CE
    /// 000C : OpCode.SYSCALL 925DE831
    /// 0011 : OpCode.RET
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEEeHBoC5cmBQgiDXhK2ShQygAUs6uqJiQMVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGgLlyYFCCINeUrZKFDKABSzq6omIgxUaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgxUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1+P7//6omBAlAenk17P7//0V7enl4NAQIQA==
    /// 0000 : OpCode.INITSLOT 0104
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHNULL
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.JMPIFNOT 05
    /// 000A : OpCode.PUSHT
    /// 000B : OpCode.JMP 0D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.ISTYPE 28
    /// 0011 : OpCode.SWAP
    /// 0012 : OpCode.SIZE
    /// 0013 : OpCode.PUSHINT8 14
    /// 0015 : OpCode.NUMEQUAL
    /// 0016 : OpCode.BOOLAND
    /// 0017 : OpCode.NOT
    /// 0018 : OpCode.JMPIFNOT 24
    /// 001A : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E
    /// 003B : OpCode.THROW
    /// 003C : OpCode.LDARG1
    /// 003D : OpCode.STLOC0
    /// 003E : OpCode.LDLOC0
    /// 003F : OpCode.PUSHNULL
    /// 0040 : OpCode.EQUAL
    /// 0041 : OpCode.JMPIFNOT 05
    /// 0043 : OpCode.PUSHT
    /// 0044 : OpCode.JMP 0D
    /// 0046 : OpCode.LDARG1
    /// 0047 : OpCode.DUP
    /// 0048 : OpCode.ISTYPE 28
    /// 004A : OpCode.SWAP
    /// 004B : OpCode.SIZE
    /// 004C : OpCode.PUSHINT8 14
    /// 004E : OpCode.NUMEQUAL
    /// 004F : OpCode.BOOLAND
    /// 0050 : OpCode.NOT
    /// 0051 : OpCode.JMPIFNOT 22
    /// 0053 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 0072 : OpCode.THROW
    /// 0073 : OpCode.LDARG2
    /// 0074 : OpCode.PUSH0
    /// 0075 : OpCode.LT
    /// 0076 : OpCode.JMPIFNOT 2A
    /// 0078 : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E
    /// 009F : OpCode.THROW
    /// 00A0 : OpCode.LDARG0
    /// 00A1 : OpCode.SYSCALL F827EC8C
    /// 00A6 : OpCode.NOT
    /// 00A7 : OpCode.JMPIFNOT 04
    /// 00A9 : OpCode.PUSHF
    /// 00AA : OpCode.RET
    /// 00AB : OpCode.LDARG2
    /// 00AC : OpCode.PUSH0
    /// 00AD : OpCode.NOTEQUAL
    /// 00AE : OpCode.JMPIFNOT 17
    /// 00B0 : OpCode.LDARG2
    /// 00B1 : OpCode.NEGATE
    /// 00B2 : OpCode.LDARG0
    /// 00B3 : OpCode.CALL_L F8FEFFFF
    /// 00B8 : OpCode.NOT
    /// 00B9 : OpCode.JMPIFNOT 04
    /// 00BB : OpCode.PUSHF
    /// 00BC : OpCode.RET
    /// 00BD : OpCode.LDARG2
    /// 00BE : OpCode.LDARG1
    /// 00BF : OpCode.CALL_L ECFEFFFF
    /// 00C4 : OpCode.DROP
    /// 00C5 : OpCode.LDARG3
    /// 00C6 : OpCode.LDARG2
    /// 00C7 : OpCode.LDARG1
    /// 00C8 : OpCode.LDARG0
    /// 00C9 : OpCode.CALL 04
    /// 00CB : OpCode.PUSHT
    /// 00CC : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNZz+//8JlyYWDE5vIGF1dGhvcml6YXRpb24uOnp5eDcBAEA=
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.CALL_L 9CFEFFFF
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.EQUAL
    /// 000A : OpCode.JMPIFNOT 16
    /// 000C : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E
    /// 001F : OpCode.THROW
    /// 0020 : OpCode.LDARG2
    /// 0021 : OpCode.LDARG1
    /// 0022 : OpCode.LDARG0
    /// 0023 : OpCode.CALLT 0100
    /// 0026 : OpCode.RET
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion

}
