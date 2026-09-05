using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Nep11ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerifiable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep11Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":50,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":212,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":365,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":557,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":580,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":656,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":1307,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1337,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""name"",""type"":""String""},{""name"":""description"",""type"":""String""},{""name"":""image"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":1477,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1582,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":1588,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":1601,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1722,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""isContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep11/Nep11Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"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").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Events

    [DisplayName("SetOwner")]
    public event Neo.SmartContract.Testing.TestingStandards.IOwnable.delSetOwner? OnSetOwner;

    public delegate void delTransfer(UInt160? from, UInt160? to, BigInteger? amount, byte[]? tokenId);

    [DisplayName("Transfer")]
    public event delTransfer? OnTransfer;

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
    public abstract object? Tokens { [DisplayName("tokens")] get; }

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

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMoAK7cmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNDMgb3IgbGVzcyBieXRlcyBsb25nLjoTEYhOEFHQEcBweGjBRVCLQdWNXuhK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5A
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 2B [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 3C [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203433206F72206C657373206279746573206C6F6E672E [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH3 [1 datoshi]
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
    /// JMPIFNOT 34 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 54686520746F6B656E207769746820676976656E2022746F6B656E49642220646F6573206E6F742065786973742E [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("ownerOf")]
    public abstract UInt160? OwnerOf(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMoAQLgmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNjQgb3IgbGVzcyBieXRlcyBsb25nLjoTEYhOEFHQEcBweGjBRVCLQdWNXuhK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHLISgwEbmFtZWoRztBKDAtkZXNjcmlwdGlvbmoSztBKDAVpbWFnZWoTztBA
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// GE [8 datoshi]
    /// JMPIFNOT 3C [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203634206F72206C657373206279746573206C6F6E672E [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH3 [1 datoshi]
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
    /// JMPIFNOT 34 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 54686520746F6B656E207769746820676976656E2022746F6B656E49642220646F6573206E6F742065786973742E [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// NEWMAP [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 6E616D65 'name' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// SETITEM [8192 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 6465736372697074696F6E 'description' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// SETITEM [8192 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 696D616765 'image' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// SETITEM [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("properties")]
    public abstract IDictionary<object, object>? Properties(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQEcBwE3howUVQi0EHdlLzQA==
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
    /// JMPIF 24 [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964 [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 077652F3 'System.Storage.Local.Find' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tokensOf")]
    public abstract object? TokensOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIENWn///8kFgwRTm8gQXV0aG9yaXphdGlvbiE6eErZKCQGRQkiBsoAFLMkBQkiBHixcAwXcmVjaXBpZW50IG11c3QgYmUgdmFsaWRxaCQEaeA19v3//3BoeHl6e1QUwFA1Mv7//2hA
    /// INITSLOT 0204 [64 datoshi]
    /// CALL_L 69FFFFFF [512 datoshi]
    /// JMPIF 16 [2 datoshi]
    /// PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// THROW [512 datoshi]
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
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 04 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// NZ [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 726563697069656E74206D7573742062652076616C6964 [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// ABORTMSG [0 datoshi]
    /// CALL_L F6FDFFFF [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG3 [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// SWAP [2 datoshi]
    /// CALL_L 32FEFFFF [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mint")]
    public abstract byte[]? Mint(UInt160? to, string? name, string? description, string? image);

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
    /// Script: VwQDeErZKCQGRQkiBsoAFLMkIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ecoAK7cmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNDMgb3IgbGVzcyBieXRlcyBsb25nLjoTEYhOEFHQEcBweWjBRVCLQdWNXuhK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5za0H4J+yMJAQJQGt4mCYgahB40Gh5ajcBAFPBRVCLQTkM4woPeWs0DxF5eDQKenl4azQ9CEA=
    /// INITSLOT 0403 [64 datoshi]
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
    /// JMPIF 22 [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 2B [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 3C [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203433206F72206C657373206279746573206C6F6E672E [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 34 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 54686520746F6B656E207769746820676976656E2022746F6B656E49642220646F6573206E6F742065786973742E [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// NOTEQUAL [32 datoshi]
    /// JMPIFNOT 20 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// REVERSE3 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// PUSHM1 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// CALL 0F [512 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 0A [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// CALL 3D [512 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNXT+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwQAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// CALL_L 74FEFFFF [512 datoshi]
    /// JMPIF 16 [2 datoshi]
    /// PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0400 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
