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
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.PUSHNULL [1 datoshi]
    /// 07 : OpCode.EQUAL [32 datoshi]
    /// 08 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0A : OpCode.PUSHT [1 datoshi]
    /// 0B : OpCode.JMP 0D [2 datoshi]
    /// 0D : OpCode.LDARG0 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.ISTYPE 28 [2 datoshi]
    /// 11 : OpCode.SWAP [2 datoshi]
    /// 12 : OpCode.SIZE [4 datoshi]
    /// 13 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 15 : OpCode.NUMEQUAL [8 datoshi]
    /// 16 : OpCode.BOOLAND [8 datoshi]
    /// 17 : OpCode.NOT [4 datoshi]
    /// 18 : OpCode.JMPIFNOT 25 [2 datoshi]
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// 3C : OpCode.THROW [512 datoshi]
    /// 3D : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 42 : OpCode.PUSH1 [1 datoshi]
    /// 43 : OpCode.PUSH1 [1 datoshi]
    /// 44 : OpCode.NEWBUFFER [256 datoshi]
    /// 45 : OpCode.TUCK [2 datoshi]
    /// 46 : OpCode.PUSH0 [1 datoshi]
    /// 47 : OpCode.ROT [2 datoshi]
    /// 48 : OpCode.SETITEM [8192 datoshi]
    /// 49 : OpCode.SWAP [2 datoshi]
    /// 4A : OpCode.PUSH2 [1 datoshi]
    /// 4B : OpCode.PACK [2048 datoshi]
    /// 4C : OpCode.STLOC0 [2 datoshi]
    /// 4D : OpCode.LDARG0 [2 datoshi]
    /// 4E : OpCode.LDLOC0 [2 datoshi]
    /// 4F : OpCode.UNPACK [2048 datoshi]
    /// 50 : OpCode.DROP [2 datoshi]
    /// 51 : OpCode.REVERSE3 [2 datoshi]
    /// 52 : OpCode.CAT [2048 datoshi]
    /// 53 : OpCode.SWAP [2 datoshi]
    /// 54 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 59 : OpCode.DUP [2 datoshi]
    /// 5A : OpCode.ISNULL [2 datoshi]
    /// 5B : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 5D : OpCode.DROP [2 datoshi]
    /// 5E : OpCode.PUSH0 [1 datoshi]
    /// 5F : OpCode.CONVERT (Integer) [8192 datoshi]
    /// 61 : OpCode.RET [0 datoshi]
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
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.CALL 80 [512 datoshi]
    /// 05 : OpCode.PUSHF [1 datoshi]
    /// 06 : OpCode.EQUAL [32 datoshi]
    /// 07 : OpCode.JMPIFNOT 16 [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.LDARG1 [2 datoshi]
    /// 1E : OpCode.LDARG0 [2 datoshi]
    /// 1F : OpCode.CALL_L FBFEFFFF [512 datoshi]
    /// 24 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNVv///8JlyYWDE5vIEF1dGhvcml6YXRpb24hOnl4NZ7+//9A
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.CALL_L 5BFFFFFF [512 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.EQUAL [32 datoshi]
    /// 0A : OpCode.JMPIFNOT 16 [2 datoshi]
    /// 0C : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// 1F : OpCode.THROW [512 datoshi]
    /// 20 : OpCode.LDARG1 [2 datoshi]
    /// 21 : OpCode.LDARG0 [2 datoshi]
    /// 22 : OpCode.CALL_L 9EFEFFFF [512 datoshi]
    /// 27 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEhlbGxvQZv2Z85Bkl3oMUA=
    /// 00 : OpCode.PUSHDATA1 48656C6C6F [8 datoshi]
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
    /// Script: VwEEeHBoC5cmBQgiDXhK2ShQygAUs6uqJiQMVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGgLlyYFCCINeUrZKFDKABSzq6omIgxUaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgxUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1+P7//6omBAlAenk17P7//0V7enl4NAQIQA==
    /// 00 : OpCode.INITSLOT 0104 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.PUSHNULL [1 datoshi]
    /// 07 : OpCode.EQUAL [32 datoshi]
    /// 08 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0A : OpCode.PUSHT [1 datoshi]
    /// 0B : OpCode.JMP 0D [2 datoshi]
    /// 0D : OpCode.LDARG0 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.ISTYPE 28 [2 datoshi]
    /// 11 : OpCode.SWAP [2 datoshi]
    /// 12 : OpCode.SIZE [4 datoshi]
    /// 13 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 15 : OpCode.NUMEQUAL [8 datoshi]
    /// 16 : OpCode.BOOLAND [8 datoshi]
    /// 17 : OpCode.NOT [4 datoshi]
    /// 18 : OpCode.JMPIFNOT 24 [2 datoshi]
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E [8 datoshi]
    /// 3B : OpCode.THROW [512 datoshi]
    /// 3C : OpCode.LDARG1 [2 datoshi]
    /// 3D : OpCode.STLOC0 [2 datoshi]
    /// 3E : OpCode.LDLOC0 [2 datoshi]
    /// 3F : OpCode.PUSHNULL [1 datoshi]
    /// 40 : OpCode.EQUAL [32 datoshi]
    /// 41 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 43 : OpCode.PUSHT [1 datoshi]
    /// 44 : OpCode.JMP 0D [2 datoshi]
    /// 46 : OpCode.LDARG1 [2 datoshi]
    /// 47 : OpCode.DUP [2 datoshi]
    /// 48 : OpCode.ISTYPE 28 [2 datoshi]
    /// 4A : OpCode.SWAP [2 datoshi]
    /// 4B : OpCode.SIZE [4 datoshi]
    /// 4C : OpCode.PUSHINT8 14 [1 datoshi]
    /// 4E : OpCode.NUMEQUAL [8 datoshi]
    /// 4F : OpCode.BOOLAND [8 datoshi]
    /// 50 : OpCode.NOT [4 datoshi]
    /// 51 : OpCode.JMPIFNOT 22 [2 datoshi]
    /// 53 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// 72 : OpCode.THROW [512 datoshi]
    /// 73 : OpCode.LDARG2 [2 datoshi]
    /// 74 : OpCode.PUSH0 [1 datoshi]
    /// 75 : OpCode.LT [8 datoshi]
    /// 76 : OpCode.JMPIFNOT 2A [2 datoshi]
    /// 78 : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E [8 datoshi]
    /// 9F : OpCode.THROW [512 datoshi]
    /// A0 : OpCode.LDARG0 [2 datoshi]
    /// A1 : OpCode.SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// A6 : OpCode.NOT [4 datoshi]
    /// A7 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// A9 : OpCode.PUSHF [1 datoshi]
    /// AA : OpCode.RET [0 datoshi]
    /// AB : OpCode.LDARG2 [2 datoshi]
    /// AC : OpCode.PUSH0 [1 datoshi]
    /// AD : OpCode.NOTEQUAL [32 datoshi]
    /// AE : OpCode.JMPIFNOT 17 [2 datoshi]
    /// B0 : OpCode.LDARG2 [2 datoshi]
    /// B1 : OpCode.NEGATE [4 datoshi]
    /// B2 : OpCode.LDARG0 [2 datoshi]
    /// B3 : OpCode.CALL_L F8FEFFFF [512 datoshi]
    /// B8 : OpCode.NOT [4 datoshi]
    /// B9 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// BB : OpCode.PUSHF [1 datoshi]
    /// BC : OpCode.RET [0 datoshi]
    /// BD : OpCode.LDARG2 [2 datoshi]
    /// BE : OpCode.LDARG1 [2 datoshi]
    /// BF : OpCode.CALL_L ECFEFFFF [512 datoshi]
    /// C4 : OpCode.DROP [2 datoshi]
    /// C5 : OpCode.LDARG3 [2 datoshi]
    /// C6 : OpCode.LDARG2 [2 datoshi]
    /// C7 : OpCode.LDARG1 [2 datoshi]
    /// C8 : OpCode.LDARG0 [2 datoshi]
    /// C9 : OpCode.CALL 04 [512 datoshi]
    /// CB : OpCode.PUSHT [1 datoshi]
    /// CC : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNaX+//8JlyYWDE5vIGF1dGhvcml6YXRpb24uOnp5eDcBAEA=
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.CALL_L A5FEFFFF [512 datoshi]
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
