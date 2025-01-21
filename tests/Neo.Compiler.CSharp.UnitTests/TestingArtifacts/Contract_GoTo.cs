using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_GoTo(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_GoTo"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testTry"",""parameters"":[],""returntype"":""Integer"",""offset"":63,""safe"":false},{""name"":""testTryComplex"",""parameters"":[{""name"":""exception"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":138,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1vAlcBABFwaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaBOXJspoQFcCABFwO0AAaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaBOXJgVoPQk9BXE9AiK9QFcJAQwBAAwC/wA1rAEAAAhwEAwB/zWxAQAAcWlyIl1qQfNUvx1zOwAtaHRsCJckC2zYJAdsCZckGDsAByIVPdMMAv8ANZIBAAAMAQCXOT89Mz3taHRsCJckDWzYJA1sCZckDCIKC3AiEglwIg4MAQEMAv8ANUIBAAA/akGcCO2cJJ9oCZc5DAL/ADVMAQAADAEBlzkQDAH/NS4BAAByInxqQfNUvx1zOw8mDAlleGNlcHRpb246dDsAByIRPWYMAQAMAv8ANfEAAAA/PfEMAv8ANQUBAAAMAQCXORIREA8UwEp0ynUQdiIubG7OdwdvB3cIbwgQlyQcbwgRlyQWbwgSlyQEIg4MAQIMAv8ANakAAABunHZubTDSP2pBnAjtnCSADAL/ADWwAAAADAEClzkQDAH/NZIAAAByImhqQfNUvx1zOx9KeHRsCZckFWwIlyQEIg4MCWV4Y2VwdGlvbjo9QnQ7DhMMAQMMAv8ANElsOnUiGT02DAL/ADRcDAEDlzkMAQIMAv8ANC4/PekMAv8ANEUMAQKXOQwBAwwC/wA0Fz9qQZwI7ZwklAwC/wA0KAwBA5c5QFcAAnl4QZv2Z85B5j8YhEBXAAJ5eEH2tGviQd8wuJpAVwABeEH2tGviQZJd6DFAR7EYNA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoE5cmymhA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT CA [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract BigInteger? Test();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEXA7QABoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoE5cmBWg9CT0FcT0CIr1A
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 4000 [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 09 [4 datoshi]
    /// ENDTRY 05 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// JMP BD [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTry")]
    public abstract BigInteger? TestTry();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwkBDAEADAL/ADWsAQAACHAQDAH/NbEBAABxaXIiXWpB81S/HXM7AC1odGwIlyQLbNgkB2wJlyQYOwAHIhU90wwC/wA1kgEAAAwBAJc5Pz0zPe1odGwIlyQNbNgkDWwJlyQMIgoLcCISCXAiDgwBAQwC/wA1QgEAAD9qQZwI7Zwkn2gJlzkMAv8ANUwBAAAMAQGXORAMAf81LgEAAHIifGpB81S/HXM7DyYMCWV4Y2VwdGlvbjp0OwAHIhE9ZgwBAAwC/wA18QAAAD898QwC/wA1BQEAAAwBAJc5EhEQDxTASnTKdRB2Ii5sbs53B28HdwhvCBCXJBxvCBGXJBZvCBKXJAQiDgwBAgwC/wA1qQAAAG6cdm5tMNI/akGcCO2cJIAMAv8ANbAAAAAMAQKXORAMAf81kgAAAHIiaGpB81S/HXM7H0p4dGwJlyQVbAiXJAQiDgwJZXhjZXB0aW9uOj1CdDsOEwwBAwwC/wA0SWw6dSIZPTYMAv8ANFwMAQOXOQwBAgwC/wA0Lj896QwC/wA0RQwBApc5DAEDDAL/ADQXP2pBnAjtnCSUDAL/ADQoDAEDlzlA
    /// INITSLOT 0901 [64 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L AC010000 [512 datoshi]
    /// PUSHT [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 FF '?' [8 datoshi]
    /// CALL_L B1010000 [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// JMP 5D [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// STLOC3 [2 datoshi]
    /// TRY 002D [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 0B [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 07 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 18 [2 datoshi]
    /// TRY 0007 [4 datoshi]
    /// JMP 15 [2 datoshi]
    /// ENDTRY D3 [4 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L 92010000 [512 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// ENDTRY 33 [4 datoshi]
    /// ENDTRY ED [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 0D [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0D [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 0C [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// JMP 12 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L 42010000 [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// JMPIF 9F [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L 4C010000 [512 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 FF '?' [8 datoshi]
    /// CALL_L 2E010000 [512 datoshi]
    /// STLOC2 [2 datoshi]
    /// JMP 7C [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// STLOC3 [2 datoshi]
    /// TRY 0F26 [4 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC4 [2 datoshi]
    /// TRY 0007 [4 datoshi]
    /// JMP 11 [2 datoshi]
    /// ENDTRY 66 [4 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L F1000000 [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// ENDTRY F1 [4 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L 05010000 [512 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHM1 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// DUP [2 datoshi]
    /// STLOC4 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC5 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC6 [2 datoshi]
    /// JMP 2E [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC 07 [2 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// STLOC 08 [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 1C [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 16 [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L A9000000 [512 datoshi]
    /// LDLOC6 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC6 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// JMPLT D2 [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// JMPIF 80 [2 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL_L B0000000 [512 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHDATA1 FF '?' [8 datoshi]
    /// CALL_L 92000000 [512 datoshi]
    /// STLOC2 [2 datoshi]
    /// JMP 68 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// STLOC3 [2 datoshi]
    /// TRY 1F4A [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 15 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// ENDTRY 42 [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// TRY 0E13 [4 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 49 [512 datoshi]
    /// LDLOC4 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC5 [2 datoshi]
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
    /// LDLOC2 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// JMPIF 94 [2 datoshi]
    /// PUSHDATA1 FF00 [8 datoshi]
    /// CALL 28 [512 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTryComplex")]
    public abstract void TestTryComplex(bool? exception);

    #endregion
}
