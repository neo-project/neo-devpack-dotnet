using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Storage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Storage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""mainA"",""parameters"":[{""name"":""callee"",""type"":""Hash160""},{""name"":""throwInB"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""writeA"",""parameters"":[],""returntype"":""Void"",""offset"":77,""safe"":false},{""name"":""mainB"",""parameters"":[{""name"":""callerA"",""type"":""Hash160""},{""name"":""throw_"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":86,""safe"":false},{""name"":""writeB"",""parameters"":[],""returntype"":""Void"",""offset"":135,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJBXAQIRDAGg2zA0NBEMAaHbMDQsOyUAEgwBodswNCF5Qdv+qHQSwB8MBW1haW5CeEFifVtSRQg9BnAJPQJAVwACeXhBm/ZnzkHmPxiEQBIMAaDbMDTqQFcBAhEMAbDbMDTeNCY7GADCHwwGd3JpdGVBeEFifVtSRQwBQjpwPQJ5JgYMAUI6CEASDAGx2zA0sEA+kN7c"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEQwBoNswNDQRDAGh2zA0LDslABIMAaHbMDQheUHb/qh0EsAfDAVtYWluQnhBYn1bUkUIPQZwCT0CQA==
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSH1 [1 datoshi]
    /// 04 : OpCode.PUSHDATA1 A0 [8 datoshi]
    /// 07 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : OpCode.CALL 34 [512 datoshi]
    /// 0B : OpCode.PUSH1 [1 datoshi]
    /// 0C : OpCode.PUSHDATA1 A1 [8 datoshi]
    /// 0F : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 11 : OpCode.CALL 2C [512 datoshi]
    /// 13 : OpCode.TRY 2500 [4 datoshi]
    /// 16 : OpCode.PUSH2 [1 datoshi]
    /// 17 : OpCode.PUSHDATA1 A1 [8 datoshi]
    /// 1A : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 1C : OpCode.CALL 21 [512 datoshi]
    /// 1E : OpCode.LDARG1 [2 datoshi]
    /// 1F : OpCode.SYSCALL DBFEA874 'System.Runtime.GetExecutingScriptHash' [16 datoshi]
    /// 24 : OpCode.PUSH2 [1 datoshi]
    /// 25 : OpCode.PACK [2048 datoshi]
    /// 26 : OpCode.PUSH15 [1 datoshi]
    /// 27 : OpCode.PUSHDATA1 6D61696E42 [8 datoshi]
    /// 2E : OpCode.LDARG0 [2 datoshi]
    /// 2F : OpCode.SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 34 : OpCode.DROP [2 datoshi]
    /// 35 : OpCode.PUSHT [1 datoshi]
    /// 36 : OpCode.ENDTRY 06 [4 datoshi]
    /// 38 : OpCode.STLOC0 [2 datoshi]
    /// 39 : OpCode.PUSHF [1 datoshi]
    /// 3A : OpCode.ENDTRY 02 [4 datoshi]
    /// 3C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("mainA")]
    public abstract bool? MainA(UInt160? callee, bool? throwInB);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEQwBsNswNN40JjsYAMIfDAZ3cml0ZUF4QWJ9W1JFDAFCOnA9AnkmBgwBQjoIQA==
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSH1 [1 datoshi]
    /// 04 : OpCode.PUSHDATA1 B0 [8 datoshi]
    /// 07 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : OpCode.CALL DE [512 datoshi]
    /// 0B : OpCode.CALL 26 [512 datoshi]
    /// 0D : OpCode.TRY 1800 [4 datoshi]
    /// 10 : OpCode.NEWARRAY0 [16 datoshi]
    /// 11 : OpCode.PUSH15 [1 datoshi]
    /// 12 : OpCode.PUSHDATA1 777269746541 [8 datoshi]
    /// 1A : OpCode.LDARG0 [2 datoshi]
    /// 1B : OpCode.SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 20 : OpCode.DROP [2 datoshi]
    /// 21 : OpCode.PUSHDATA1 42 [8 datoshi]
    /// 24 : OpCode.THROW [512 datoshi]
    /// 25 : OpCode.STLOC0 [2 datoshi]
    /// 26 : OpCode.ENDTRY 02 [4 datoshi]
    /// 28 : OpCode.LDARG1 [2 datoshi]
    /// 29 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 2B : OpCode.PUSHDATA1 42 [8 datoshi]
    /// 2E : OpCode.THROW [512 datoshi]
    /// 2F : OpCode.PUSHT [1 datoshi]
    /// 30 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("mainB")]
    public abstract bool? MainB(UInt160? callerA, bool? throw_);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EgwBoNswNOpA
    /// 00 : OpCode.PUSH2 [1 datoshi]
    /// 01 : OpCode.PUSHDATA1 A0 [8 datoshi]
    /// 04 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 06 : OpCode.CALL EA [512 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("writeA")]
    public abstract void WriteA();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EgwBsdswNLBA
    /// 00 : OpCode.PUSH2 [1 datoshi]
    /// 01 : OpCode.PUSHDATA1 B1 [8 datoshi]
    /// 04 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 06 : OpCode.CALL B0 [512 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("writeB")]
    public abstract void WriteB();

    #endregion
}
