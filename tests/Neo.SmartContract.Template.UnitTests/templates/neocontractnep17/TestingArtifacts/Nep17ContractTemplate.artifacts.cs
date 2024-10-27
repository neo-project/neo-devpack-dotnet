using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":72,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":215,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":597,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":639,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":756,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":791,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":829,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":835,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":853,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":972,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD98QMMB0VYQU1QTEVAGEBXAQAMAQA0DXBoStgmBEUQ2yFAVwABeEH2tGviQZJd6DFAVwABeAwBADQDQFcAAnl4QZv2Z85B5j8YhEBXAQF4StkoJAZFCSIGygAUsyQlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjoMAQF4i9soNJpwaErYJgRFENshQFcEAkGb9mfOcAwBAXiL2yhxaWhBkl3oMXJqStgmBEUQ2yFza3mec2sQtSYECUBrsSQLaWhBL1jF7SIKa2loQeY/GIQIQFcABHhK2SgkBkUJIgbKABSzJCQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eUrZKCQGRQkiBsoAFLMkIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMJAQJQHoQmCYWept4NRf///8kBAlAenk1DP///0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBo2CYFCSIKeTcAAHBo2KomH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFcAAnmZELUmCwwGYW1vdW50OnmxJANAeXg1nf7//0U1Ef7//3meSjUr/v//RQt5eAs0h0BXAAJ5mRC1JgsMBmFtb3VudDp5sSQDQHmbeDVo/v//JA4MCWV4Y2VwdGlvbjo1z/3//3mfSjXp/f//RQt5C3g1Rf///0AMAf/bMDQOStgkCUrKABQoAzpAVwABeEH2tGviQZJd6DFANN5B+CfsjEBXAQE09SQWDBFObyBBdXRob3JpemF0aW9uITp4StkoJAZFCSIGygAUsyQFCSIEeLEME293bmVyIG11c3QgYmUgdmFsaWThNJBweAwB/9swNBZ4aBLADAhTZXRPd25lckGVAW9hQFcAAnl4QZv2Z85B5j8YhEBXAAI0gCQWDBFObyBBdXRob3JpemF0aW9uITp5eDX//v//QFcAAjVd////JBYMEU5vIEF1dGhvcml6YXRpb24hOnl4NaX+//9ANTr///9ADAVIZWxsb0Gb9mfOQZJd6DFAVwECeSYDQHhwaNgmCkEtUQgwE86AeHBoStkoJAZFCSIGygAUsyQFCSIEaLEMEW93bmVyIG11c3QgZXhpc3Rz4WgMAf/bMDVJ////aAsSwAwIU2V0T3duZXJBlQFvYQwFV29ybGQMBUhlbGxvQZv2Z85B5j8YhEBXAAM1qP7//yQWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AQBAX4v9jA=="));

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQxUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjoMAXiL2yg0mnBoStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISTYPE 28
    /// 07 : OpCode.JMPIF 06
    /// 09 : OpCode.DROP
    /// 0A : OpCode.PUSHF
    /// 0B : OpCode.JMP 06
    /// 0D : OpCode.SIZE
    /// 0E : OpCode.PUSHINT8 14
    /// 10 : OpCode.NUMEQUAL
    /// 11 : OpCode.JMPIF 25
    /// 13 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 35 : OpCode.THROW
    /// 36 : OpCode.PUSHDATA1 01
    /// 39 : OpCode.LDARG0
    /// 3A : OpCode.CAT
    /// 3B : OpCode.CONVERT 28
    /// 3D : OpCode.CALL 9A
    /// 3F : OpCode.STLOC0
    /// 40 : OpCode.LDLOC0
    /// 41 : OpCode.DUP
    /// 42 : OpCode.ISNULL
    /// 43 : OpCode.JMPIFNOT 04
    /// 45 : OpCode.DROP
    /// 46 : OpCode.PUSH0
    /// 47 : OpCode.CONVERT 21
    /// 49 : OpCode.RET
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNIAkFgxObyBBdXRob3JpemF0aW9uITp5eDX//v//QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.CALL 80
    /// 05 : OpCode.JMPIF 16
    /// 07 : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 1A : OpCode.THROW
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.CALL_L FFFEFFFF
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNV3///8kFgxObyBBdXRob3JpemF0aW9uITp5eDWl/v//QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.CALL_L 5DFFFFFF
    /// 08 : OpCode.JMPIF 16
    /// 0A : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 1D : OpCode.THROW
    /// 1E : OpCode.LDARG1
    /// 1F : OpCode.LDARG0
    /// 20 : OpCode.CALL_L A5FEFFFF
    /// 25 : OpCode.RET
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
    /// Script: VwAEeErZKCQGRQkiBsoAFLMkJAxUaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlK2SgkBkUJIgbKABSzJCIMVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1F////yQECUB6eTUM////RXt6eXg0BAhA
    /// 00 : OpCode.INITSLOT 0004
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISTYPE 28
    /// 07 : OpCode.JMPIF 06
    /// 09 : OpCode.DROP
    /// 0A : OpCode.PUSHF
    /// 0B : OpCode.JMP 06
    /// 0D : OpCode.SIZE
    /// 0E : OpCode.PUSHINT8 14
    /// 10 : OpCode.NUMEQUAL
    /// 11 : OpCode.JMPIF 24
    /// 13 : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E
    /// 34 : OpCode.THROW
    /// 35 : OpCode.LDARG1
    /// 36 : OpCode.DUP
    /// 37 : OpCode.ISTYPE 28
    /// 39 : OpCode.JMPIF 06
    /// 3B : OpCode.DROP
    /// 3C : OpCode.PUSHF
    /// 3D : OpCode.JMP 06
    /// 3F : OpCode.SIZE
    /// 40 : OpCode.PUSHINT8 14
    /// 42 : OpCode.NUMEQUAL
    /// 43 : OpCode.JMPIF 22
    /// 45 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 64 : OpCode.THROW
    /// 65 : OpCode.LDARG2
    /// 66 : OpCode.PUSH0
    /// 67 : OpCode.LT
    /// 68 : OpCode.JMPIFNOT 2A
    /// 6A : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E
    /// 91 : OpCode.THROW
    /// 92 : OpCode.LDARG0
    /// 93 : OpCode.SYSCALL F827EC8C
    /// 98 : OpCode.JMPIF 04
    /// 9A : OpCode.PUSHF
    /// 9B : OpCode.RET
    /// 9C : OpCode.LDARG2
    /// 9D : OpCode.PUSH0
    /// 9E : OpCode.NOTEQUAL
    /// 9F : OpCode.JMPIFNOT 16
    /// A1 : OpCode.LDARG2
    /// A2 : OpCode.NEGATE
    /// A3 : OpCode.LDARG0
    /// A4 : OpCode.CALL_L 17FFFFFF
    /// A9 : OpCode.JMPIF 04
    /// AB : OpCode.PUSHF
    /// AC : OpCode.RET
    /// AD : OpCode.LDARG2
    /// AE : OpCode.LDARG1
    /// AF : OpCode.CALL_L 0CFFFFFF
    /// B4 : OpCode.DROP
    /// B5 : OpCode.LDARG3
    /// B6 : OpCode.LDARG2
    /// B7 : OpCode.LDARG1
    /// B8 : OpCode.LDARG0
    /// B9 : OpCode.CALL 04
    /// BB : OpCode.PUSHT
    /// BC : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNaj+//8kFgxObyBhdXRob3JpemF0aW9uLjp6eXg3AQBA
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.CALL_L A8FEFFFF
    /// 08 : OpCode.JMPIF 16
    /// 0A : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E
    /// 1D : OpCode.THROW
    /// 1E : OpCode.LDARG2
    /// 1F : OpCode.LDARG1
    /// 20 : OpCode.LDARG0
    /// 21 : OpCode.CALLT 0100
    /// 24 : OpCode.RET
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
