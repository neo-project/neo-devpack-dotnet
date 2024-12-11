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
    /// INITSLOT 0102 [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSHDATA1 A0 '?' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CALL 34 [512 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSHDATA1 A1 '?' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CALL 2C [512 datoshi]
    /// TRY 2500 [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSHDATA1 A1 '?' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CALL 21 [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// SYSCALL DBFEA874 'System.Runtime.GetExecutingScriptHash' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 6D61696E42 'mainB' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 06 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mainA")]
    public abstract bool? MainA(UInt160? callee, bool? throwInB);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEQwBsNswNN40JjsYAMIfDAZ3cml0ZUF4QWJ9W1JFDAFCOnA9AnkmBgwBQjoIQA==
    /// INITSLOT 0102 [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSHDATA1 B0 '?' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CALL DE [512 datoshi]
    /// CALL 26 [512 datoshi]
    /// TRY 1800 [4 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// PUSH15 [1 datoshi]
    /// PUSHDATA1 777269746541 'writeA' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 42 'B' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// LDARG1 [2 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// PUSHDATA1 42 'B' [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mainB")]
    public abstract bool? MainB(UInt160? callerA, bool? throw_);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EgwBoNswNOpA
    /// PUSH2 [1 datoshi]
    /// PUSHDATA1 A0 '?' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CALL EA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("writeA")]
    public abstract void WriteA();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EgwBsdswNLBA
    /// PUSH2 [1 datoshi]
    /// PUSHDATA1 B1 '?' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CALL B0 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("writeB")]
    public abstract void WriteB();

    #endregion
}
