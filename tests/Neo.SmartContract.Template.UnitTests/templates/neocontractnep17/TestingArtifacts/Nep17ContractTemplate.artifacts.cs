using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerifiable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":50,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":212,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":584,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":614,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":725,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":760,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":798,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":804,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":817,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":938,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""isContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP17Payment""]}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wppc0NvbnRyYWN0AQABBf2j+kNG6lMqJY/El92t22Q3yf3/BnVwZGF0ZQMAAA8AAP3PAwwHRVhBTVBMRUAYQAwBAEH2tGviQZJd6DFK2CYERRBAVwABeAwBAEGb9mfOQeY/GIRAVwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46ERGIThBR0BHAcHhowUVQi0HVjV7oStgmBUUQQNshQFcCAhERiE4QUdARwHB4aMFFUItB1Y1e6ErYJgZFECIE2yFxaXmecWkQtSYECUBpsSQPeGjBRVCLQXVU9ZQiDml4aMFFUItBOQzjCghAVwAEeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5StkoJAZFCSIGygAUsyQiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmIwweVGhlIGFtb3VudCBjYW5ub3QgYmUgbmVnYXRpdmUuOnhB+CfsjCQECUB6EJgmFnqbeDUW////JAQJQHp5NQv///9Fe3p5eDQECEBXAQR4eXpTE8AMCFRyYW5zZmVyQZUBb2F5cGjYJgUJIgZ5NwAAJh97engTwB8MDm9uTkVQMTdQYXltZW50eUFifVtSRUBXAAJ5mRC1JgsMBmFtb3VudDp5sSQDQHl4NZ/+//9FNR7+//95nko1Kv7//0ULeXgLNIpAVwACeZkQtSYLDAZhbW91bnQ6ebEkA0B5m3g1av7//yQODAlleGNlcHRpb246Ndz9//95n0o16P3//0ULeQt4NUj///9ADAH/EY1B1Y1e6ErYJAlKygAUKAM6QDTqQfgn7IxAVwIBNPUkFgwRTm8gQXV0aG9yaXphdGlvbiE6eErZKCQGRQkiBsoAFLMkBQkiBHixcAwTb3duZXIgbXVzdCBiZSB2YWxpZHFoJARp4DSWcHgMAf8RjUE5DOMKaHhQEsAMCFNldE93bmVyQZUBb2FAVwACNIYkFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1Ef///0BXAAI1Y////yQWDBFObyBBdXRob3JpemF0aW9uITp5eDW3/v//QDVA////QAwFSGVsbG9B1Y1e6EBXAwJ5JgNAeHBo2CYKQS1RCDATzoB4cGhK2SgkBkUJIgbKABSzJAUJIgRosXEMEW93bmVyIG11c3QgZXhpc3RzcmkkBGrgaAwB/xGNQTkM4woLaFASwAwIU2V0T3duZXJBlQFvYQwFV29ybGQMBUhlbGxvQTkM4wpAVwADNbH+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwEAQDNYxMk=").AsSerializable<Neo.SmartContract.NefFile>();

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46ERGIThBR0BHAcHhowUVQi0HVjV7oStgmBUUQQNshQA==
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIF 25 [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNIYkFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1Ef///0A=
    /// INITSLOT 0002 [64 datoshi]
    /// CALL 86 [512 datoshi]
    /// JMPIF 16 [2 datoshi]
    /// PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 11FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNWP///8kFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1t/7//0A=
    /// INITSLOT 0002 [64 datoshi]
    /// CALL_L 63FFFFFF [512 datoshi]
    /// JMPIF 16 [2 datoshi]
    /// PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L B7FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAVIZWxsb0HVjV7oQA==
    /// PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwAEeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5StkoJAZFCSIGygAUsyQiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmIwweVGhlIGFtb3VudCBjYW5ub3QgYmUgbmVnYXRpdmUuOnhB+CfsjCQECUB6EJgmFnqbeDUW////JAQJQHp5NQv///9Fe3p5eDQECEA=
    /// INITSLOT 0004 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIF 24 [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIF 22 [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 23 [2 datoshi]
    /// PUSHDATA1 54686520616D6F756E742063616E6E6F74206265206E656761746976652E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// NOTEQUAL [32 datoshi]
    /// JMPIFNOT 16 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 16FFFFFF [512 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CALL_L 0BFFFFFF [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG3 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 04 [512 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNbH+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwEAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// CALL_L B1FEFFFF [512 datoshi]
    /// JMPIF 16 [2 datoshi]
    /// PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
