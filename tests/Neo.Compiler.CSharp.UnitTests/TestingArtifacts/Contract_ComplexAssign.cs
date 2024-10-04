using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ComplexAssign(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ComplexAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Add_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""unitTest_Add_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":71,""safe"":false},{""name"":""unitTest_Sub_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":180,""safe"":false},{""name"":""unitTest_Sub_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":243,""safe"":false},{""name"":""unitTest_Mul_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":344,""safe"":false},{""name"":""unitTest_Mul_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":415,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":524,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":595,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":704,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":767,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1kA1cCAAP/////AAAAAHAC////f3FoEZ5KEC4DOkoD/////wAAAAAyAzpwaRGeSgIAAACALgM6SgL///9/MgM6ccVKaM9Kac9AVwIAA/////8AAAAAcAL///9/cWgRnkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3HFSmjPSmnPQFcCABBwAgAAAIBxaBGfShAuAzpKA/////8AAAAAMgM6cGkRn0oCAAAAgC4DOkoC////fzIDOnHFSmjPSmnPQFcCABBwAgAAAIBxaBGfShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpEZ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfccVKaM9Kac9AVwIAA/////8AAAAAcAL///9/cWgSoEoQLgM6SgP/////AAAAADIDOnBpEqBKAgAAAIAuAzpKAv///38yAzpxxUpoz0ppz0BXAgAD/////wAAAABwAv///39xaBKgShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfccVKaM9Kac9AVwIAA/////8AAAAAcAL///9/cWgRqEoQLgM6SgP/////AAAAADIDOnBpEahKAgAAAIAuAzpKAv///38yAzpxxUpoz0ppz0BXAgAD/////wAAAABwAv///39xaBGoShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpEahKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfccVKaM9Kac9AVwIAEHACAAAAgHFoEalKEC4DOkoD/////wAAAAAyAzpwaRGpSgIAAACALgM6SgL///9/MgM6ccVKaM9Kac9AVwIAEHACAAAAgHFoEalKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRqUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xxUpoz0ppz0DJ88oN"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000C : OpCode.STLOC0
    /// 000D : OpCode.PUSHINT32 FFFFFF7F
    /// 0012 : OpCode.STLOC1
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.PUSH1
    /// 0015 : OpCode.ADD
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.PUSH0
    /// 0018 : OpCode.JMPGE 03
    /// 001A : OpCode.THROW
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0025 : OpCode.JMPLE 03
    /// 0027 : OpCode.THROW
    /// 0028 : OpCode.STLOC0
    /// 0029 : OpCode.LDLOC1
    /// 002A : OpCode.PUSH1
    /// 002B : OpCode.ADD
    /// 002C : OpCode.DUP
    /// 002D : OpCode.PUSHINT32 00000080
    /// 0032 : OpCode.JMPGE 03
    /// 0034 : OpCode.THROW
    /// 0035 : OpCode.DUP
    /// 0036 : OpCode.PUSHINT32 FFFFFF7F
    /// 003B : OpCode.JMPLE 03
    /// 003D : OpCode.THROW
    /// 003E : OpCode.STLOC1
    /// 003F : OpCode.NEWSTRUCT0
    /// 0040 : OpCode.DUP
    /// 0041 : OpCode.LDLOC0
    /// 0042 : OpCode.APPEND
    /// 0043 : OpCode.DUP
    /// 0044 : OpCode.LDLOC1
    /// 0045 : OpCode.APPEND
    /// 0046 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_Checked")]
    public abstract IList<object>? UnitTest_Add_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000C : OpCode.STLOC0
    /// 000D : OpCode.PUSHINT32 FFFFFF7F
    /// 0012 : OpCode.STLOC1
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.PUSH1
    /// 0015 : OpCode.ADD
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.PUSH0
    /// 0018 : OpCode.JMPGE 04
    /// 001A : OpCode.JMP 0E
    /// 001C : OpCode.DUP
    /// 001D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0026 : OpCode.JMPLE 0C
    /// 0028 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0031 : OpCode.AND
    /// 0032 : OpCode.STLOC0
    /// 0033 : OpCode.LDLOC1
    /// 0034 : OpCode.PUSH1
    /// 0035 : OpCode.ADD
    /// 0036 : OpCode.DUP
    /// 0037 : OpCode.PUSHINT32 00000080
    /// 003C : OpCode.JMPGE 04
    /// 003E : OpCode.JMP 0A
    /// 0040 : OpCode.DUP
    /// 0041 : OpCode.PUSHINT32 FFFFFF7F
    /// 0046 : OpCode.JMPLE 1E
    /// 0048 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0051 : OpCode.AND
    /// 0052 : OpCode.DUP
    /// 0053 : OpCode.PUSHINT32 FFFFFF7F
    /// 0058 : OpCode.JMPLE 0C
    /// 005A : OpCode.PUSHINT64 0000000001000000
    /// 0063 : OpCode.SUB
    /// 0064 : OpCode.STLOC1
    /// 0065 : OpCode.NEWSTRUCT0
    /// 0066 : OpCode.DUP
    /// 0067 : OpCode.LDLOC0
    /// 0068 : OpCode.APPEND
    /// 0069 : OpCode.DUP
    /// 006A : OpCode.LDLOC1
    /// 006B : OpCode.APPEND
    /// 006C : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Add_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000C : OpCode.STLOC0
    /// 000D : OpCode.PUSHINT32 FFFFFF7F
    /// 0012 : OpCode.STLOC1
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.PUSH1
    /// 0015 : OpCode.SHL
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.PUSH0
    /// 0018 : OpCode.JMPGE 03
    /// 001A : OpCode.THROW
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0025 : OpCode.JMPLE 03
    /// 0027 : OpCode.THROW
    /// 0028 : OpCode.STLOC0
    /// 0029 : OpCode.LDLOC1
    /// 002A : OpCode.PUSH1
    /// 002B : OpCode.SHL
    /// 002C : OpCode.DUP
    /// 002D : OpCode.PUSHINT32 00000080
    /// 0032 : OpCode.JMPGE 03
    /// 0034 : OpCode.THROW
    /// 0035 : OpCode.DUP
    /// 0036 : OpCode.PUSHINT32 FFFFFF7F
    /// 003B : OpCode.JMPLE 03
    /// 003D : OpCode.THROW
    /// 003E : OpCode.STLOC1
    /// 003F : OpCode.NEWSTRUCT0
    /// 0040 : OpCode.DUP
    /// 0041 : OpCode.LDLOC0
    /// 0042 : OpCode.APPEND
    /// 0043 : OpCode.DUP
    /// 0044 : OpCode.LDLOC1
    /// 0045 : OpCode.APPEND
    /// 0046 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000C : OpCode.STLOC0
    /// 000D : OpCode.PUSHINT32 FFFFFF7F
    /// 0012 : OpCode.STLOC1
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.PUSH1
    /// 0015 : OpCode.SHL
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.PUSH0
    /// 0018 : OpCode.JMPGE 04
    /// 001A : OpCode.JMP 0E
    /// 001C : OpCode.DUP
    /// 001D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0026 : OpCode.JMPLE 0C
    /// 0028 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0031 : OpCode.AND
    /// 0032 : OpCode.STLOC0
    /// 0033 : OpCode.LDLOC1
    /// 0034 : OpCode.PUSH1
    /// 0035 : OpCode.SHL
    /// 0036 : OpCode.DUP
    /// 0037 : OpCode.PUSHINT32 00000080
    /// 003C : OpCode.JMPGE 04
    /// 003E : OpCode.JMP 0A
    /// 0040 : OpCode.DUP
    /// 0041 : OpCode.PUSHINT32 FFFFFF7F
    /// 0046 : OpCode.JMPLE 1E
    /// 0048 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0051 : OpCode.AND
    /// 0052 : OpCode.DUP
    /// 0053 : OpCode.PUSHINT32 FFFFFF7F
    /// 0058 : OpCode.JMPLE 0C
    /// 005A : OpCode.PUSHINT64 0000000001000000
    /// 0063 : OpCode.SUB
    /// 0064 : OpCode.STLOC1
    /// 0065 : OpCode.NEWSTRUCT0
    /// 0066 : OpCode.DUP
    /// 0067 : OpCode.LDLOC0
    /// 0068 : OpCode.APPEND
    /// 0069 : OpCode.DUP
    /// 006A : OpCode.LDLOC1
    /// 006B : OpCode.APPEND
    /// 006C : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000C : OpCode.STLOC0
    /// 000D : OpCode.PUSHINT32 FFFFFF7F
    /// 0012 : OpCode.STLOC1
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.PUSH2
    /// 0015 : OpCode.MUL
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.PUSH0
    /// 0018 : OpCode.JMPGE 03
    /// 001A : OpCode.THROW
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0025 : OpCode.JMPLE 03
    /// 0027 : OpCode.THROW
    /// 0028 : OpCode.STLOC0
    /// 0029 : OpCode.LDLOC1
    /// 002A : OpCode.PUSH2
    /// 002B : OpCode.MUL
    /// 002C : OpCode.DUP
    /// 002D : OpCode.PUSHINT32 00000080
    /// 0032 : OpCode.JMPGE 03
    /// 0034 : OpCode.THROW
    /// 0035 : OpCode.DUP
    /// 0036 : OpCode.PUSHINT32 FFFFFF7F
    /// 003B : OpCode.JMPLE 03
    /// 003D : OpCode.THROW
    /// 003E : OpCode.STLOC1
    /// 003F : OpCode.NEWSTRUCT0
    /// 0040 : OpCode.DUP
    /// 0041 : OpCode.LDLOC0
    /// 0042 : OpCode.APPEND
    /// 0043 : OpCode.DUP
    /// 0044 : OpCode.LDLOC1
    /// 0045 : OpCode.APPEND
    /// 0046 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_Checked")]
    public abstract IList<object>? UnitTest_Mul_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000C : OpCode.STLOC0
    /// 000D : OpCode.PUSHINT32 FFFFFF7F
    /// 0012 : OpCode.STLOC1
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.PUSH2
    /// 0015 : OpCode.MUL
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.PUSH0
    /// 0018 : OpCode.JMPGE 04
    /// 001A : OpCode.JMP 0E
    /// 001C : OpCode.DUP
    /// 001D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0026 : OpCode.JMPLE 0C
    /// 0028 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0031 : OpCode.AND
    /// 0032 : OpCode.STLOC0
    /// 0033 : OpCode.LDLOC1
    /// 0034 : OpCode.PUSH2
    /// 0035 : OpCode.MUL
    /// 0036 : OpCode.DUP
    /// 0037 : OpCode.PUSHINT32 00000080
    /// 003C : OpCode.JMPGE 04
    /// 003E : OpCode.JMP 0A
    /// 0040 : OpCode.DUP
    /// 0041 : OpCode.PUSHINT32 FFFFFF7F
    /// 0046 : OpCode.JMPLE 1E
    /// 0048 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0051 : OpCode.AND
    /// 0052 : OpCode.DUP
    /// 0053 : OpCode.PUSHINT32 FFFFFF7F
    /// 0058 : OpCode.JMPLE 0C
    /// 005A : OpCode.PUSHINT64 0000000001000000
    /// 0063 : OpCode.SUB
    /// 0064 : OpCode.STLOC1
    /// 0065 : OpCode.NEWSTRUCT0
    /// 0066 : OpCode.DUP
    /// 0067 : OpCode.LDLOC0
    /// 0068 : OpCode.APPEND
    /// 0069 : OpCode.DUP
    /// 006A : OpCode.LDLOC1
    /// 006B : OpCode.APPEND
    /// 006C : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Mul_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHINT32 00000080
    /// 000A : OpCode.STLOC1
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.PUSH1
    /// 000D : OpCode.SHR
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSH0
    /// 0010 : OpCode.JMPGE 03
    /// 0012 : OpCode.THROW
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001D : OpCode.JMPLE 03
    /// 001F : OpCode.THROW
    /// 0020 : OpCode.STLOC0
    /// 0021 : OpCode.LDLOC1
    /// 0022 : OpCode.PUSH1
    /// 0023 : OpCode.SHR
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT32 00000080
    /// 002A : OpCode.JMPGE 03
    /// 002C : OpCode.THROW
    /// 002D : OpCode.DUP
    /// 002E : OpCode.PUSHINT32 FFFFFF7F
    /// 0033 : OpCode.JMPLE 03
    /// 0035 : OpCode.THROW
    /// 0036 : OpCode.STLOC1
    /// 0037 : OpCode.NEWSTRUCT0
    /// 0038 : OpCode.DUP
    /// 0039 : OpCode.LDLOC0
    /// 003A : OpCode.APPEND
    /// 003B : OpCode.DUP
    /// 003C : OpCode.LDLOC1
    /// 003D : OpCode.APPEND
    /// 003E : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHINT32 00000080
    /// 000A : OpCode.STLOC1
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.PUSH1
    /// 000D : OpCode.SHR
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSH0
    /// 0010 : OpCode.JMPGE 04
    /// 0012 : OpCode.JMP 0E
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001E : OpCode.JMPLE 0C
    /// 0020 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0029 : OpCode.AND
    /// 002A : OpCode.STLOC0
    /// 002B : OpCode.LDLOC1
    /// 002C : OpCode.PUSH1
    /// 002D : OpCode.SHR
    /// 002E : OpCode.DUP
    /// 002F : OpCode.PUSHINT32 00000080
    /// 0034 : OpCode.JMPGE 04
    /// 0036 : OpCode.JMP 0A
    /// 0038 : OpCode.DUP
    /// 0039 : OpCode.PUSHINT32 FFFFFF7F
    /// 003E : OpCode.JMPLE 1E
    /// 0040 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0049 : OpCode.AND
    /// 004A : OpCode.DUP
    /// 004B : OpCode.PUSHINT32 FFFFFF7F
    /// 0050 : OpCode.JMPLE 0C
    /// 0052 : OpCode.PUSHINT64 0000000001000000
    /// 005B : OpCode.SUB
    /// 005C : OpCode.STLOC1
    /// 005D : OpCode.NEWSTRUCT0
    /// 005E : OpCode.DUP
    /// 005F : OpCode.LDLOC0
    /// 0060 : OpCode.APPEND
    /// 0061 : OpCode.DUP
    /// 0062 : OpCode.LDLOC1
    /// 0063 : OpCode.APPEND
    /// 0064 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHINT32 00000080
    /// 000A : OpCode.STLOC1
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.PUSH1
    /// 000D : OpCode.SUB
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSH0
    /// 0010 : OpCode.JMPGE 03
    /// 0012 : OpCode.THROW
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001D : OpCode.JMPLE 03
    /// 001F : OpCode.THROW
    /// 0020 : OpCode.STLOC0
    /// 0021 : OpCode.LDLOC1
    /// 0022 : OpCode.PUSH1
    /// 0023 : OpCode.SUB
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT32 00000080
    /// 002A : OpCode.JMPGE 03
    /// 002C : OpCode.THROW
    /// 002D : OpCode.DUP
    /// 002E : OpCode.PUSHINT32 FFFFFF7F
    /// 0033 : OpCode.JMPLE 03
    /// 0035 : OpCode.THROW
    /// 0036 : OpCode.STLOC1
    /// 0037 : OpCode.NEWSTRUCT0
    /// 0038 : OpCode.DUP
    /// 0039 : OpCode.LDLOC0
    /// 003A : OpCode.APPEND
    /// 003B : OpCode.DUP
    /// 003C : OpCode.LDLOC1
    /// 003D : OpCode.APPEND
    /// 003E : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_Checked")]
    public abstract IList<object>? UnitTest_Sub_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHINT32 00000080
    /// 000A : OpCode.STLOC1
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.PUSH1
    /// 000D : OpCode.SUB
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSH0
    /// 0010 : OpCode.JMPGE 04
    /// 0012 : OpCode.JMP 0E
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001E : OpCode.JMPLE 0C
    /// 0020 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0029 : OpCode.AND
    /// 002A : OpCode.STLOC0
    /// 002B : OpCode.LDLOC1
    /// 002C : OpCode.PUSH1
    /// 002D : OpCode.SUB
    /// 002E : OpCode.DUP
    /// 002F : OpCode.PUSHINT32 00000080
    /// 0034 : OpCode.JMPGE 04
    /// 0036 : OpCode.JMP 0A
    /// 0038 : OpCode.DUP
    /// 0039 : OpCode.PUSHINT32 FFFFFF7F
    /// 003E : OpCode.JMPLE 1E
    /// 0040 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0049 : OpCode.AND
    /// 004A : OpCode.DUP
    /// 004B : OpCode.PUSHINT32 FFFFFF7F
    /// 0050 : OpCode.JMPLE 0C
    /// 0052 : OpCode.PUSHINT64 0000000001000000
    /// 005B : OpCode.SUB
    /// 005C : OpCode.STLOC1
    /// 005D : OpCode.NEWSTRUCT0
    /// 005E : OpCode.DUP
    /// 005F : OpCode.LDLOC0
    /// 0060 : OpCode.APPEND
    /// 0061 : OpCode.DUP
    /// 0062 : OpCode.LDLOC1
    /// 0063 : OpCode.APPEND
    /// 0064 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Sub_Assign_UnChecked();

    #endregion

}
