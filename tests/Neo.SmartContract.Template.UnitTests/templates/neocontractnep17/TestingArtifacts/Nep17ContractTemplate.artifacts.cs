using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":58,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":239,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":633,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":675,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":792,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":829,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":869,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":875,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":893,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1010,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1049,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9HAQMB0VYQU1QTEVAGEBY2CYXDAEAQfa0a+JBkl3oMUrYJgRFEEpgQFcAAXhgeAwBAEGb9mfOQeY/GIRAVwEBeHBo2CYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQFcCAkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIXFpeZ5xaRC1JgQJQGmxJBB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQIQFcBBHhwaNgmBQgiDXhK2ShQygAUs6uqJiQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eXBo2CYFCCINeUrZKFDKABSzq6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMJAQJQHoQmCYWept4Nfz+//8kBAlAenk18f7//0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBo2CYFCSIKeTcAAHBo2KomH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFcAAnmZELUmCwwGYW1vdW50OnmxJANAeXg1gv7//0U17f3//3meSjX//f//RQt5eAs0h0BXAAJ5mRC1JgsMBmFtb3VudDp5sSQDQHmbeDVN/v//JA4MCWV4Y2VwdGlvbjo1q/3//3mfSjW9/f//RQt5C3g1Rf///0AMAf/bMDQOStgkCUrKABQoAzpAVwABeEH2tGviQZJd6DFANN5B+CfsjEBXAQE09QmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnhK2ShQygAUs6skBQkiBHixJBgME293bmVyIG11c3QgYmUgdmFsaWTgNJBweAwB/9swNBZ4aBLADAhTZXRPd25lckGVAW9hQFcAAnl4QZv2Z85B5j8YhEBXAAI0gAmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4Nf3+//9AVwACNVv///8JlyYWDBFObyBBdXRob3JpemF0aW9uITp5eDWh/v//QDU2////QAwFSGVsbG9Bm/ZnzkGSXegxQFcBAnkmA0B4cGjYJgpBLVEIMBPOgHhwaErZKFDKABSzqyQFCSIEaLEkFgwRb3duZXIgbXVzdCBleGlzdHPgaAwB/9swNUf///9oCxLADAhTZXRPd25lckGVAW9hDAVXb3JsZAwFSGVsbG9Bm/ZnzkHmPxiEQFcAAzWm/v//CZcmFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwEAQFYBQIjmRUU="));

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
    /// Script: VwEBeHBo2CYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.ISNULL [2 datoshi]
    /// 07 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 09 : OpCode.PUSHT [1 datoshi]
    /// 0A : OpCode.JMP 0D [2 datoshi]
    /// 0C : OpCode.LDARG0 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISTYPE 28 [2 datoshi]
    /// 10 : OpCode.SWAP [2 datoshi]
    /// 11 : OpCode.SIZE [4 datoshi]
    /// 12 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 14 : OpCode.NUMEQUAL [8 datoshi]
    /// 15 : OpCode.BOOLAND [8 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIFNOT 25 [2 datoshi]
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// 3B : OpCode.THROW [512 datoshi]
    /// 3C : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 41 : OpCode.PUSH1 [1 datoshi]
    /// 42 : OpCode.PUSH1 [1 datoshi]
    /// 43 : OpCode.NEWBUFFER [256 datoshi]
    /// 44 : OpCode.TUCK [2 datoshi]
    /// 45 : OpCode.PUSH0 [1 datoshi]
    /// 46 : OpCode.ROT [2 datoshi]
    /// 47 : OpCode.SETITEM [8192 datoshi]
    /// 48 : OpCode.SWAP [2 datoshi]
    /// 49 : OpCode.PUSH2 [1 datoshi]
    /// 4A : OpCode.PACK [2048 datoshi]
    /// 4B : OpCode.STLOC0 [2 datoshi]
    /// 4C : OpCode.LDARG0 [2 datoshi]
    /// 4D : OpCode.LDLOC0 [2 datoshi]
    /// 4E : OpCode.UNPACK [2048 datoshi]
    /// 4F : OpCode.DROP [2 datoshi]
    /// 50 : OpCode.REVERSE3 [2 datoshi]
    /// 51 : OpCode.CAT [2048 datoshi]
    /// 52 : OpCode.SWAP [2 datoshi]
    /// 53 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 58 : OpCode.DUP [2 datoshi]
    /// 59 : OpCode.ISNULL [2 datoshi]
    /// 5A : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 5C : OpCode.DROP [2 datoshi]
    /// 5D : OpCode.PUSH0 [1 datoshi]
    /// 5E : OpCode.CONVERT 21 'Integer' [8192 datoshi]
    /// 60 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeZkQtSYLDAZhbW91bnQ6ebEkA0B5m3g1Tf7//yQODAlleGNlcHRpb246Nav9//95n0o1vf3//0ULeQt4NUX///9A
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.SIGN [4 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.LT [8 datoshi]
    /// 07 : OpCode.JMPIFNOT 0B [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 616D6F756E74 'amount' [8 datoshi]
    /// 11 : OpCode.THROW [512 datoshi]
    /// 12 : OpCode.LDARG1 [2 datoshi]
    /// 13 : OpCode.NZ [4 datoshi]
    /// 14 : OpCode.JMPIF 03 [2 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.NEGATE [4 datoshi]
    /// 19 : OpCode.LDARG0 [2 datoshi]
    /// 1A : OpCode.CALL_L 4DFEFFFF [512 datoshi]
    /// 1F : OpCode.JMPIF 0E [2 datoshi]
    /// 21 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 2C : OpCode.THROW [512 datoshi]
    /// 2D : OpCode.CALL_L ABFDFFFF [512 datoshi]
    /// 32 : OpCode.LDARG1 [2 datoshi]
    /// 33 : OpCode.SUB [8 datoshi]
    /// 34 : OpCode.DUP [2 datoshi]
    /// 35 : OpCode.CALL_L BDFDFFFF [512 datoshi]
    /// 3A : OpCode.DROP [2 datoshi]
    /// 3B : OpCode.PUSHNULL [1 datoshi]
    /// 3C : OpCode.LDARG1 [2 datoshi]
    /// 3D : OpCode.PUSHNULL [1 datoshi]
    /// 3E : OpCode.LDARG0 [2 datoshi]
    /// 3F : OpCode.CALL_L 45FFFFFF [512 datoshi]
    /// 44 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeZkQtSYLDAZhbW91bnQ6ebEkA0B5eDWC/v//RTXt/f//eZ5KNf/9//9FC3l4CzSHQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.SIGN [4 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.LT [8 datoshi]
    /// 07 : OpCode.JMPIFNOT 0B [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 616D6F756E74 'amount' [8 datoshi]
    /// 11 : OpCode.THROW [512 datoshi]
    /// 12 : OpCode.LDARG1 [2 datoshi]
    /// 13 : OpCode.NZ [4 datoshi]
    /// 14 : OpCode.JMPIF 03 [2 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.LDARG0 [2 datoshi]
    /// 19 : OpCode.CALL_L 82FEFFFF [512 datoshi]
    /// 1E : OpCode.DROP [2 datoshi]
    /// 1F : OpCode.CALL_L EDFDFFFF [512 datoshi]
    /// 24 : OpCode.LDARG1 [2 datoshi]
    /// 25 : OpCode.ADD [8 datoshi]
    /// 26 : OpCode.DUP [2 datoshi]
    /// 27 : OpCode.CALL_L FFFDFFFF [512 datoshi]
    /// 2C : OpCode.DROP [2 datoshi]
    /// 2D : OpCode.PUSHNULL [1 datoshi]
    /// 2E : OpCode.LDARG1 [2 datoshi]
    /// 2F : OpCode.LDARG0 [2 datoshi]
    /// 30 : OpCode.PUSHNULL [1 datoshi]
    /// 31 : OpCode.CALL 87 [512 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAVIZWxsb0Gb9mfOQZJd6DFA
    /// 00 : OpCode.PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// 07 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0C : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 11 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEEeHBo2CYFCCINeErZKFDKABSzq6omJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGjYJgUIIg15StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1/P7//yQECUB6eTXx/v//RXt6eXg0BAhA
    /// 00 : OpCode.INITSLOT 0104 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.ISNULL [2 datoshi]
    /// 07 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 09 : OpCode.PUSHT [1 datoshi]
    /// 0A : OpCode.JMP 0D [2 datoshi]
    /// 0C : OpCode.LDARG0 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISTYPE 28 [2 datoshi]
    /// 10 : OpCode.SWAP [2 datoshi]
    /// 11 : OpCode.SIZE [4 datoshi]
    /// 12 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 14 : OpCode.NUMEQUAL [8 datoshi]
    /// 15 : OpCode.BOOLAND [8 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIFNOT 24 [2 datoshi]
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E [8 datoshi]
    /// 3A : OpCode.THROW [512 datoshi]
    /// 3B : OpCode.LDARG1 [2 datoshi]
    /// 3C : OpCode.STLOC0 [2 datoshi]
    /// 3D : OpCode.LDLOC0 [2 datoshi]
    /// 3E : OpCode.ISNULL [2 datoshi]
    /// 3F : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 41 : OpCode.PUSHT [1 datoshi]
    /// 42 : OpCode.JMP 0D [2 datoshi]
    /// 44 : OpCode.LDARG1 [2 datoshi]
    /// 45 : OpCode.DUP [2 datoshi]
    /// 46 : OpCode.ISTYPE 28 [2 datoshi]
    /// 48 : OpCode.SWAP [2 datoshi]
    /// 49 : OpCode.SIZE [4 datoshi]
    /// 4A : OpCode.PUSHINT8 14 [1 datoshi]
    /// 4C : OpCode.NUMEQUAL [8 datoshi]
    /// 4D : OpCode.BOOLAND [8 datoshi]
    /// 4E : OpCode.NOT [4 datoshi]
    /// 4F : OpCode.JMPIFNOT 22 [2 datoshi]
    /// 51 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// 70 : OpCode.THROW [512 datoshi]
    /// 71 : OpCode.LDARG2 [2 datoshi]
    /// 72 : OpCode.PUSH0 [1 datoshi]
    /// 73 : OpCode.LT [8 datoshi]
    /// 74 : OpCode.JMPIFNOT 2A [2 datoshi]
    /// 76 : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E [8 datoshi]
    /// 9D : OpCode.THROW [512 datoshi]
    /// 9E : OpCode.LDARG0 [2 datoshi]
    /// 9F : OpCode.SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// A4 : OpCode.JMPIF 04 [2 datoshi]
    /// A6 : OpCode.PUSHF [1 datoshi]
    /// A7 : OpCode.RET [0 datoshi]
    /// A8 : OpCode.LDARG2 [2 datoshi]
    /// A9 : OpCode.PUSH0 [1 datoshi]
    /// AA : OpCode.NOTEQUAL [32 datoshi]
    /// AB : OpCode.JMPIFNOT 16 [2 datoshi]
    /// AD : OpCode.LDARG2 [2 datoshi]
    /// AE : OpCode.NEGATE [4 datoshi]
    /// AF : OpCode.LDARG0 [2 datoshi]
    /// B0 : OpCode.CALL_L FCFEFFFF [512 datoshi]
    /// B5 : OpCode.JMPIF 04 [2 datoshi]
    /// B7 : OpCode.PUSHF [1 datoshi]
    /// B8 : OpCode.RET [0 datoshi]
    /// B9 : OpCode.LDARG2 [2 datoshi]
    /// BA : OpCode.LDARG1 [2 datoshi]
    /// BB : OpCode.CALL_L F1FEFFFF [512 datoshi]
    /// C0 : OpCode.DROP [2 datoshi]
    /// C1 : OpCode.LDARG3 [2 datoshi]
    /// C2 : OpCode.LDARG2 [2 datoshi]
    /// C3 : OpCode.LDARG1 [2 datoshi]
    /// C4 : OpCode.LDARG0 [2 datoshi]
    /// C5 : OpCode.CALL 04 [512 datoshi]
    /// C7 : OpCode.PUSHT [1 datoshi]
    /// C8 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNab+//8JlyYWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AQBA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.CALL_L A6FEFFFF [512 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.EQUAL [32 datoshi]
    /// 0A : OpCode.JMPIFNOT 16 [2 datoshi]
    /// 0C : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// 1F : OpCode.THROW [512 datoshi]
    /// 20 : OpCode.LDARG2 [2 datoshi]
    /// 21 : OpCode.LDARG1 [2 datoshi]
    /// 22 : OpCode.LDARG0 [2 datoshi]
    /// 23 : OpCode.CALLT 0100 [32768 datoshi]
    /// 26 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
