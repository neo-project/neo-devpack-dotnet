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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2BAVcGAAwBAAwC/wA1SAEAABAMAf81TwEAAHAiG2hB81S/HXE7AAU9FwwBAQwC/wA1JAEAAD9oQZwI7Zwk4QwC/wA1MgEAAAwBAZc5EAwB/zUUAQAAcCJsaEHzVL8dcTsaMRByIg4MCWV4Y2VwdGlvbjpqE7Uk8T1LcjsAByIRPUsMAQAMAv8ANcwAAAA/PfEMAv8ANeAAAAAMAQCXORIREBPASnLKcxB0IhRqbM51DAECDAL/ADWeAAAAIgZsazDsP2hBnAjtnCSQDAL/ADWmAAAADAEClzkQDAH/NYgAAABwIl5oQfNUvx1xOwtAEHJqE7VFPUxyEHNrE7VFaxCXOTsOEwwBAwwC/wA0SWo6dCIZPTYMAv8ANFwMAQOXOQwBAgwC/wA0Lj896QwC/wA0RQwBApc5DAEDDAL/ADQXP2hBnAjtnCSeDAL/ADQoDAEDlzlAVwACeXhBm/ZnzkHmPxiEQFcAAnl4Qfa0a+JB3zC4mkBXAAF4Qfa0a+JBkl3oMUB6sNlU"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAEADAL/ADVIAQAAEAwB/zVPAQAAcCIbaEHzVL8dcTsABT0XDAEBDAL/ADUkAQAAP2hBnAjtnCThDAL/ADUyAQAADAEBlzkQDAH/NRQBAABwImxoQfNUvx1xOxoxEHIiDgwJZXhjZXB0aW9uOmoTtSTxPUtyOwAHIhE9SwwBAAwC/wA1zAAAAD898QwC/wA14AAAAAwBAJc5EhEQE8BKcspzEHQiFGpsznUMAQIMAv8ANZ4AAAAiBmxrMOw/aEGcCO2cJJAMAv8ANaYAAAAMAQKXORAMAf81iAAAAHAiXmhB81S/HXE7C0AQcmoTtUU9THIQc2sTtUVrEJc5Ow4TDAEDDAL/ADRJajp0Ihk9NgwC/wA0XAwBA5c5DAECDAL/ADQuPz3pDAL/ADRFDAEClzkMAQMMAv8ANBc/aEGcCO2cJJ4MAv8ANCgMAQOXOUA=
    /// INITSLOT 0600 [64 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L 48010000 [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 FF '?' [8 datoshi]
    /// CALL_L 4F010000 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// JMP 1B [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0005 [4 datoshi]
    /// ENDTRY 17 [4 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L 24010000 [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// JMPIF E1 [2 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L 32010000 [512 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 FF '?' [8 datoshi]
    /// CALL_L 14010000 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// JMP 6C [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 1A31 [4 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIF F1 [2 datoshi]
    /// ENDTRY 4B [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// TRY 0007 [4 datoshi]
    /// JMP 11 [2 datoshi]
    /// ENDTRY 4B [4 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L CC000000 [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// ENDTRY F1 [4 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L E0000000 [512 datoshi]
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
    /// JMP 14 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC5 [2 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L 9E000000 [512 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// JMPLT EC [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// JMPIF 90 [2 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L A6000000 [512 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 FF '?' [8 datoshi]
    /// CALL_L 88000000 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// JMP 5E [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0B40 [4 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LT [8 datoshi]
    /// DROP [2 datoshi]
    /// ENDTRY 4C [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LT [8 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// TRY 0E13 [4 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 49 [512 datoshi]
    /// LDLOC2 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 19 [2 datoshi]
    /// ENDTRY 36 [4 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 5C [512 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 2E [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// ENDTRY E9 [4 datoshi]
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
    /// JMPIF 9E [2 datoshi]
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
