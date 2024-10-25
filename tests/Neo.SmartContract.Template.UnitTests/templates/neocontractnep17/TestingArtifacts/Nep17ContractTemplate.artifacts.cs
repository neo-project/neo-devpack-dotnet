using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":58,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":241,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":645,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":687,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":804,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":841,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":881,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":887,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":905,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1023,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1062,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9KQQMB0VYQU1QTEVAGEBY2CYXDAEAQfa0a+JBkl3oMUrYJgRFEEpgQFcAAXhgeAwBAEGb9mfOQeY/GIRAVwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIUBXAgJBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFxaXmecWkQtSYECUBpELMmEHhowUVTi1BBL1jF7SIPaXhowUVTi1BB5j8YhAhAVwEEeHBoC5cmBQgiDXhK2ShQygAUs6uqJiQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eXBoC5cmBQgiDXlK2ShQygAUs6uqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1+P7//6omBAlAenk17P7//0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBoC5eqJAUJIgt5NwAAcGgLl6omH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFcAAnmZELUmCwwGYW1vdW50OnkQsyYDQHl4NXn+//9FNeP9//95nko19f3//0ULeXgLNINAVwACeZkQtSYLDAZhbW91bnQ6eRCzJgNAeZt4NUP+//+qJg4MCWV4Y2VwdGlvbjo1n/3//3mfSjWx/f//RQt5C3g1P////0AMAf/bMDQOStgkCUrKABQoAzpAVwABeEH2tGviQZJd6DFANN5B+CfsjEBXAQE09QmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnhK2ShQygAUs6skBQkiBngQs6oME293bmVyIG11c3QgYmUgdmFsaWThNJBweAwB/9swNBZ4aBLADAhTZXRPd25lckGVAW9hQFcAAnl4QZv2Z85B5j8YhEBXAAI0gAmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4Nfv+//9AVwACNVv///8JlyYWDBFObyBBdXRob3JpemF0aW9uITp5eDWe/v//QDU2////QAwFSGVsbG9Bm/ZnzkGSXegxQFcBAnkmA0B4cGgLlyYKQS1RCDATzoB4cGhK2ShQygAUs6skBQkiBmgQs6oMEW93bmVyIG11c3QgZXhpc3Rz4WgMAf/bMDVG////aAsSwAwIU2V0T3duZXJBlQFvYQwFV29ybGQMBUhlbGxvQZv2Z85B5j8YhEBXAAM1pf7//wmXJhYMEU5vIGF1dGhvcml6YXRpb24uOnp5eDcBAEBWAUCUhRP2"));

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
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 0D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.SWAP
    /// 12 : OpCode.SIZE
    /// 13 : OpCode.PUSHINT8 14
    /// 15 : OpCode.NUMEQUAL
    /// 16 : OpCode.BOOLAND
    /// 17 : OpCode.NOT
    /// 18 : OpCode.JMPIFNOT 25
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 3C : OpCode.THROW
    /// 3D : OpCode.SYSCALL 9BF667CE
    /// 42 : OpCode.PUSH1
    /// 43 : OpCode.PUSH1
    /// 44 : OpCode.NEWBUFFER
    /// 45 : OpCode.TUCK
    /// 46 : OpCode.PUSH0
    /// 47 : OpCode.ROT
    /// 48 : OpCode.SETITEM
    /// 49 : OpCode.SWAP
    /// 4A : OpCode.PUSH2
    /// 4B : OpCode.PACK
    /// 4C : OpCode.STLOC0
    /// 4D : OpCode.LDARG0
    /// 4E : OpCode.LDLOC0
    /// 4F : OpCode.UNPACK
    /// 50 : OpCode.DROP
    /// 51 : OpCode.REVERSE3
    /// 52 : OpCode.CAT
    /// 53 : OpCode.SWAP
    /// 54 : OpCode.SYSCALL 925DE831
    /// 59 : OpCode.DUP
    /// 5A : OpCode.ISNULL
    /// 5B : OpCode.JMPIFNOT 04
    /// 5D : OpCode.DROP
    /// 5E : OpCode.PUSH0
    /// 5F : OpCode.CONVERT 21
    /// 61 : OpCode.RET
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNIAJlyYWDE5vIEF1dGhvcml6YXRpb24hOnl4Nfv+//9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.CALL 80
    /// 05 : OpCode.PUSHF
    /// 06 : OpCode.EQUAL
    /// 07 : OpCode.JMPIFNOT 16
    /// 09 : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 1C : OpCode.THROW
    /// 1D : OpCode.LDARG1
    /// 1E : OpCode.LDARG0
    /// 1F : OpCode.CALL_L FBFEFFFF
    /// 24 : OpCode.RET
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNVv///8JlyYWDE5vIEF1dGhvcml6YXRpb24hOnl4NZ7+//9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.CALL_L 5BFFFFFF
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.EQUAL
    /// 0A : OpCode.JMPIFNOT 16
    /// 0C : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 1F : OpCode.THROW
    /// 20 : OpCode.LDARG1
    /// 21 : OpCode.LDARG0
    /// 22 : OpCode.CALL_L 9EFEFFFF
    /// 27 : OpCode.RET
    /// </remarks>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEhlbGxvQZv2Z85Bkl3oMUA=
    /// 00 : OpCode.PUSHDATA1 48656C6C6F
    /// 07 : OpCode.SYSCALL 9BF667CE
    /// 0C : OpCode.SYSCALL 925DE831
    /// 11 : OpCode.RET
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEEeHBoC5cmBQgiDXhK2ShQygAUs6uqJiQMVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGgLlyYFCCINeUrZKFDKABSzq6omIgxUaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgxUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1+P7//6omBAlAenk17P7//0V7enl4NAQIQA==
    /// 00 : OpCode.INITSLOT 0104
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 0D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.SWAP
    /// 12 : OpCode.SIZE
    /// 13 : OpCode.PUSHINT8 14
    /// 15 : OpCode.NUMEQUAL
    /// 16 : OpCode.BOOLAND
    /// 17 : OpCode.NOT
    /// 18 : OpCode.JMPIFNOT 24
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E
    /// 3B : OpCode.THROW
    /// 3C : OpCode.LDARG1
    /// 3D : OpCode.STLOC0
    /// 3E : OpCode.LDLOC0
    /// 3F : OpCode.PUSHNULL
    /// 40 : OpCode.EQUAL
    /// 41 : OpCode.JMPIFNOT 05
    /// 43 : OpCode.PUSHT
    /// 44 : OpCode.JMP 0D
    /// 46 : OpCode.LDARG1
    /// 47 : OpCode.DUP
    /// 48 : OpCode.ISTYPE 28
    /// 4A : OpCode.SWAP
    /// 4B : OpCode.SIZE
    /// 4C : OpCode.PUSHINT8 14
    /// 4E : OpCode.NUMEQUAL
    /// 4F : OpCode.BOOLAND
    /// 50 : OpCode.NOT
    /// 51 : OpCode.JMPIFNOT 22
    /// 53 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 72 : OpCode.THROW
    /// 73 : OpCode.LDARG2
    /// 74 : OpCode.PUSH0
    /// 75 : OpCode.LT
    /// 76 : OpCode.JMPIFNOT 2A
    /// 78 : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E
    /// 9F : OpCode.THROW
    /// A0 : OpCode.LDARG0
    /// A1 : OpCode.SYSCALL F827EC8C
    /// A6 : OpCode.NOT
    /// A7 : OpCode.JMPIFNOT 04
    /// A9 : OpCode.PUSHF
    /// AA : OpCode.RET
    /// AB : OpCode.LDARG2
    /// AC : OpCode.PUSH0
    /// AD : OpCode.NOTEQUAL
    /// AE : OpCode.JMPIFNOT 17
    /// B0 : OpCode.LDARG2
    /// B1 : OpCode.NEGATE
    /// B2 : OpCode.LDARG0
    /// B3 : OpCode.CALL_L F8FEFFFF
    /// B8 : OpCode.NOT
    /// B9 : OpCode.JMPIFNOT 04
    /// BB : OpCode.PUSHF
    /// BC : OpCode.RET
    /// BD : OpCode.LDARG2
    /// BE : OpCode.LDARG1
    /// BF : OpCode.CALL_L ECFEFFFF
    /// C4 : OpCode.DROP
    /// C5 : OpCode.LDARG3
    /// C6 : OpCode.LDARG2
    /// C7 : OpCode.LDARG1
    /// C8 : OpCode.LDARG0
    /// C9 : OpCode.CALL 04
    /// CB : OpCode.PUSHT
    /// CC : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNaX+//8JlyYWDE5vIGF1dGhvcml6YXRpb24uOnp5eDcBAEA=
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.CALL_L A5FEFFFF
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.EQUAL
    /// 0A : OpCode.JMPIFNOT 16
    /// 0C : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E
    /// 1F : OpCode.THROW
    /// 20 : OpCode.LDARG2
    /// 21 : OpCode.LDARG1
    /// 22 : OpCode.LDARG0
    /// 23 : OpCode.CALLT 0100
    /// 26 : OpCode.RET
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
