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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9HAQMB0VYQU1QTEVAGEBY2CYXDAEAQfa0a+JBkl3oMUrYJgRFEEpgQFcAAXhgeAwBAEGb9mfOQeY/GIRAVwEBeHBo2CYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQFcCAkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIXFpeZ5xaRC1JgQJQGmxJBB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQIQFcBBHhwaNgmBQgiDXhK2ShQygAUs6uqJiQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eXBo2CYFCCINeUrZKFDKABSzq6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMJAQJQHoQmCYWept4Nfz+//8kBAlAenk18f7//0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBo2CYFCSIKeTcAAHBo2KomH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFcAAnmZELUmCwwGYW1vdW50OnmxJANAeXg1gv7//0U17f3//3meSjX//f//RQt5eAs0h0BXAAJ5mRC1JgsMBmFtb3VudDp5sSQDQHmbeDVN/v//JA4MCWV4Y2VwdGlvbjo1q/3//3mfSjW9/f//RQt5C3g1Rf///0AMAf/bMDQOStgkCUrKABQoAzpAVwABeEH2tGviQZJd6DFANN5B+CfsjEBXAQE09QmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnhK2ShQygAUs6skBQkiBnixqqoME293bmVyIG11c3QgYmUgdmFsaWThNJBweAwB/9swNBZ4aBLADAhTZXRPd25lckGVAW9hQFcAAnl4QZv2Z85B5j8YhEBXAAI0gAmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4Nf3+//9AVwACNVv///8JlyYWDBFObyBBdXRob3JpemF0aW9uITp5eDWh/v//QDU2////QAwFSGVsbG9Bm/ZnzkGSXegxQFcBAnkmA0B4cGjYJgpBLVEIMBPOgHhwaErZKFDKABSzqyQFCSIGaLGqqgwRb3duZXIgbXVzdCBleGlzdHPhaAwB/9swNUf///9oCxLADAhTZXRPd25lckGVAW9hDAVXb3JsZAwFSGVsbG9Bm/ZnzkHmPxiEQFcAAzWm/v//CZcmFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwEAQFYBQNPNKS8="));

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
    /// Script: VwEBeHBo2CYFCCINeErZKFDKABSzq6omJQxUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjpBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.JMPIFNOT 05
    /// 09 : OpCode.PUSHT
    /// 0A : OpCode.JMP 0D
    /// 0C : OpCode.LDARG0
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISTYPE 28
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.SIZE
    /// 12 : OpCode.PUSHINT8 14
    /// 14 : OpCode.NUMEQUAL
    /// 15 : OpCode.BOOLAND
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIFNOT 25
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 3B : OpCode.THROW
    /// 3C : OpCode.SYSCALL 9BF667CE
    /// 41 : OpCode.PUSH1
    /// 42 : OpCode.PUSH1
    /// 43 : OpCode.NEWBUFFER
    /// 44 : OpCode.TUCK
    /// 45 : OpCode.PUSH0
    /// 46 : OpCode.ROT
    /// 47 : OpCode.SETITEM
    /// 48 : OpCode.SWAP
    /// 49 : OpCode.PUSH2
    /// 4A : OpCode.PACK
    /// 4B : OpCode.STLOC0
    /// 4C : OpCode.LDARG0
    /// 4D : OpCode.LDLOC0
    /// 4E : OpCode.UNPACK
    /// 4F : OpCode.DROP
    /// 50 : OpCode.REVERSE3
    /// 51 : OpCode.CAT
    /// 52 : OpCode.SWAP
    /// 53 : OpCode.SYSCALL 925DE831
    /// 58 : OpCode.DUP
    /// 59 : OpCode.ISNULL
    /// 5A : OpCode.JMPIFNOT 04
    /// 5C : OpCode.DROP
    /// 5D : OpCode.PUSH0
    /// 5E : OpCode.CONVERT 21
    /// 60 : OpCode.RET
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNIAJlyYWDE5vIEF1dGhvcml6YXRpb24hOnl4Nf3+//9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.CALL 80
    /// 05 : OpCode.PUSHF
    /// 06 : OpCode.EQUAL
    /// 07 : OpCode.JMPIFNOT 16
    /// 09 : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 1C : OpCode.THROW
    /// 1D : OpCode.LDARG1
    /// 1E : OpCode.LDARG0
    /// 1F : OpCode.CALL_L FDFEFFFF
    /// 24 : OpCode.RET
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNVv///8JlyYWDE5vIEF1dGhvcml6YXRpb24hOnl4NaH+//9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.CALL_L 5BFFFFFF
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.EQUAL
    /// 0A : OpCode.JMPIFNOT 16
    /// 0C : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 1F : OpCode.THROW
    /// 20 : OpCode.LDARG1
    /// 21 : OpCode.LDARG0
    /// 22 : OpCode.CALL_L A1FEFFFF
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
    /// Script: VwEEeHBo2CYFCCINeErZKFDKABSzq6omJAxUaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlwaNgmBQgiDXlK2ShQygAUs6uqJiIMVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1/P7//yQECUB6eTXx/v//RXt6eXg0BAhA
    /// 00 : OpCode.INITSLOT 0104
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.JMPIFNOT 05
    /// 09 : OpCode.PUSHT
    /// 0A : OpCode.JMP 0D
    /// 0C : OpCode.LDARG0
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISTYPE 28
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.SIZE
    /// 12 : OpCode.PUSHINT8 14
    /// 14 : OpCode.NUMEQUAL
    /// 15 : OpCode.BOOLAND
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIFNOT 24
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E
    /// 3A : OpCode.THROW
    /// 3B : OpCode.LDARG1
    /// 3C : OpCode.STLOC0
    /// 3D : OpCode.LDLOC0
    /// 3E : OpCode.ISNULL
    /// 3F : OpCode.JMPIFNOT 05
    /// 41 : OpCode.PUSHT
    /// 42 : OpCode.JMP 0D
    /// 44 : OpCode.LDARG1
    /// 45 : OpCode.DUP
    /// 46 : OpCode.ISTYPE 28
    /// 48 : OpCode.SWAP
    /// 49 : OpCode.SIZE
    /// 4A : OpCode.PUSHINT8 14
    /// 4C : OpCode.NUMEQUAL
    /// 4D : OpCode.BOOLAND
    /// 4E : OpCode.NOT
    /// 4F : OpCode.JMPIFNOT 22
    /// 51 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 70 : OpCode.THROW
    /// 71 : OpCode.LDARG2
    /// 72 : OpCode.PUSH0
    /// 73 : OpCode.LT
    /// 74 : OpCode.JMPIFNOT 2A
    /// 76 : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E
    /// 9D : OpCode.THROW
    /// 9E : OpCode.LDARG0
    /// 9F : OpCode.SYSCALL F827EC8C
    /// A4 : OpCode.JMPIF 04
    /// A6 : OpCode.PUSHF
    /// A7 : OpCode.RET
    /// A8 : OpCode.LDARG2
    /// A9 : OpCode.PUSH0
    /// AA : OpCode.NOTEQUAL
    /// AB : OpCode.JMPIFNOT 16
    /// AD : OpCode.LDARG2
    /// AE : OpCode.NEGATE
    /// AF : OpCode.LDARG0
    /// B0 : OpCode.CALL_L FCFEFFFF
    /// B5 : OpCode.JMPIF 04
    /// B7 : OpCode.PUSHF
    /// B8 : OpCode.RET
    /// B9 : OpCode.LDARG2
    /// BA : OpCode.LDARG1
    /// BB : OpCode.CALL_L F1FEFFFF
    /// C0 : OpCode.DROP
    /// C1 : OpCode.LDARG3
    /// C2 : OpCode.LDARG2
    /// C3 : OpCode.LDARG1
    /// C4 : OpCode.LDARG0
    /// C5 : OpCode.CALL 04
    /// C7 : OpCode.PUSHT
    /// C8 : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNab+//8JlyYWDE5vIGF1dGhvcml6YXRpb24uOnp5eDcBAEA=
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.CALL_L A6FEFFFF
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
