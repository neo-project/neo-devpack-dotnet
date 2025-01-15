using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Break(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Break"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""breakInTryCatch"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1DAVcGAAwBAAwC/wA1CgEAABAMAf81EQEAAHAiG2hB81S/HXE7AAU9FwwBAQwC/wA15gAAAD9oQZwI7Zwk4QwC/wA19AAAAAwBAZc5EAwB/zXWAAAAcCJWaEHzVL8dcTsPHgwJZXhjZXB0aW9uOnIMAQAMAv8ANaAAAAA9OQwC/wA1tQAAAAwBAJc5EhEQE8BKcspzEHQiEWpsznUMAQIMAv8ANHMiBmxrMO8/aEGcCO2cJKYMAv8ANH4MAQKXORAMAf80Y3AiPGhB81S/HXE7FR4QcmoTtUUMCWV4Y2VwdGlvbjpyEHNrE7VFPR8MAv8ANEUMAQKXOQwBAwwC/wA0Fz9oQZwI7ZwkwAwC/wA0KAwBA5c5QFcAAnl4QZv2Z85B5j8YhEBXAAJ5eEH2tGviQd8wuJpAVwABeEH2tGviQZJd6DFAEImfDw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAEADAL/ADUKAQAAEAwB/zURAQAAcCIbaEHzVL8dcTsABT0XDAEBDAL/ADXmAAAAP2hBnAjtnCThDAL/ADX0AAAADAEBlzkQDAH/NdYAAABwIlZoQfNUvx1xOw8eDAlleGNlcHRpb246cgwBAAwC/wA1oAAAAD05DAL/ADW1AAAADAEAlzkSERATwEpyynMQdCIRamzOdQwBAgwC/wA0cyIGbGsw7z9oQZwI7ZwkpgwC/wA0fgwBApc5EAwB/zRjcCI8aEHzVL8dcTsVHhByahO1RQwJZXhjZXB0aW9uOnIQc2sTtUU9HwwC/wA0RQwBApc5DAEDDAL/ADQXP2hBnAjtnCTADAL/ADQoDAEDlzlA
    /// INITSLOT 0600 [64 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L 0A010000 [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 FF '?' [8 datoshi]
    /// CALL_L 11010000 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// JMP 1B [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0005 [4 datoshi]
    /// ENDTRY 17 [4 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L E6000000 [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// JMPIF E1 [2 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L F4000000 [512 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 FF '?' [8 datoshi]
    /// CALL_L D6000000 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// JMP 56 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0F1E [4 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L A0000000 [512 datoshi]
    /// ENDTRY 39 [4 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L B5000000 [512 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// DUP [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 11 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC5 [2 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 73 [512 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// JMPLT EF [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// JMPIF A6 [2 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 7E [512 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 FF '?' [8 datoshi]
    /// CALL 63 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// JMP 3C [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 151E [4 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LT [8 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LT [8 datoshi]
    /// DROP [2 datoshi]
    /// ENDTRY 1F [4 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 45 [512 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 17 [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// JMPIF C0 [2 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 28 [512 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("breakInTryCatch")]
    public abstract void BreakInTryCatch();

    #endregion
}
