using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJBXAQIRDAGg2zA0NBEMAaHbMDQsOyUAEgwBodswNCF5Qdv+qHQSwB8MBW1haW5CeEFifVtSRQg9BnAJPQJAVwACeXhBm/ZnzkHmPxiEQBIMAaDbMDTqQFcBAhEMAbDbMDTeNCY7GADCHwwGd3JpdGVBeEFifVtSRQwBQjpwPQJ5JgYMAUI6CEASDAGx2zA0sEA+kN7c").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEQwBoNswNDQRDAGh2zA0LDslABIMAaHbMDQheUHb/qh0EsAfDAVtYWluQnhBYn1bUkUIPQZwCT0CQA==
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSH1 [1 datoshi]
    /// 04 : PUSHDATA1 A0 '?' [8 datoshi]
    /// 07 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : CALL 34 [512 datoshi]
    /// 0B : PUSH1 [1 datoshi]
    /// 0C : PUSHDATA1 A1 '?' [8 datoshi]
    /// 0F : CONVERT 30 'Buffer' [8192 datoshi]
    /// 11 : CALL 2C [512 datoshi]
    /// 13 : TRY 2500 [4 datoshi]
    /// 16 : PUSH2 [1 datoshi]
    /// 17 : PUSHDATA1 A1 '?' [8 datoshi]
    /// 1A : CONVERT 30 'Buffer' [8192 datoshi]
    /// 1C : CALL 21 [512 datoshi]
    /// 1E : LDARG1 [2 datoshi]
    /// 1F : SYSCALL DBFEA874 'System.Runtime.GetExecutingScriptHash' [16 datoshi]
    /// 24 : PUSH2 [1 datoshi]
    /// 25 : PACK [2048 datoshi]
    /// 26 : PUSH15 [1 datoshi]
    /// 27 : PUSHDATA1 6D61696E42 'mainB' [8 datoshi]
    /// 2E : LDARG0 [2 datoshi]
    /// 2F : SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 34 : DROP [2 datoshi]
    /// 35 : PUSHT [1 datoshi]
    /// 36 : ENDTRY 06 [4 datoshi]
    /// 38 : STLOC0 [2 datoshi]
    /// 39 : PUSHF [1 datoshi]
    /// 3A : ENDTRY 02 [4 datoshi]
    /// 3C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("mainA")]
    public abstract bool? MainA(UInt160? callee, bool? throwInB);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEQwBsNswNN40JjsYAMIfDAZ3cml0ZUF4QWJ9W1JFDAFCOnA9AnkmBgwBQjoIQA==
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSH1 [1 datoshi]
    /// 04 : PUSHDATA1 B0 '?' [8 datoshi]
    /// 07 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : CALL DE [512 datoshi]
    /// 0B : CALL 26 [512 datoshi]
    /// 0D : TRY 1800 [4 datoshi]
    /// 10 : NEWARRAY0 [16 datoshi]
    /// 11 : PUSH15 [1 datoshi]
    /// 12 : PUSHDATA1 777269746541 'writeA' [8 datoshi]
    /// 1A : LDARG0 [2 datoshi]
    /// 1B : SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 20 : DROP [2 datoshi]
    /// 21 : PUSHDATA1 42 'B' [8 datoshi]
    /// 24 : THROW [512 datoshi]
    /// 25 : STLOC0 [2 datoshi]
    /// 26 : ENDTRY 02 [4 datoshi]
    /// 28 : LDARG1 [2 datoshi]
    /// 29 : JMPIFNOT 06 [2 datoshi]
    /// 2B : PUSHDATA1 42 'B' [8 datoshi]
    /// 2E : THROW [512 datoshi]
    /// 2F : PUSHT [1 datoshi]
    /// 30 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("mainB")]
    public abstract bool? MainB(UInt160? callerA, bool? throw_);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EgwBoNswNOpA
    /// 00 : PUSH2 [1 datoshi]
    /// 01 : PUSHDATA1 A0 '?' [8 datoshi]
    /// 04 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 06 : CALL EA [512 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("writeA")]
    public abstract void WriteA();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EgwBsdswNLBA
    /// 00 : PUSH2 [1 datoshi]
    /// 01 : PUSHDATA1 B1 '?' [8 datoshi]
    /// 04 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 06 : CALL B0 [512 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("writeB")]
    public abstract void WriteB();

    #endregion
}
