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
    [DisplayName("unitTest_Add_Assign_Checked")]
    public abstract IList<object>? UnitTest_Add_Assign_Checked();
    // 0000 : INITSLOT
    // 0003 : PUSHINT64
    // 000C : STLOC0
    // 000D : PUSHINT32
    // 0012 : STLOC1
    // 0013 : LDLOC0
    // 0014 : PUSH1
    // 0015 : ADD
    // 0016 : DUP
    // 0017 : PUSH0
    // 0018 : JMPGE
    // 001A : THROW
    // 001B : DUP
    // 001C : PUSHINT64
    // 0025 : JMPLE
    // 0027 : THROW
    // 0028 : STLOC0
    // 0029 : LDLOC1
    // 002A : PUSH1
    // 002B : ADD
    // 002C : DUP
    // 002D : PUSHINT32
    // 0032 : JMPGE
    // 0034 : THROW
    // 0035 : DUP
    // 0036 : PUSHINT32
    // 003B : JMPLE
    // 003D : THROW
    // 003E : STLOC1
    // 003F : NEWSTRUCT0
    // 0040 : DUP
    // 0041 : LDLOC0
    // 0042 : APPEND
    // 0043 : DUP
    // 0044 : LDLOC1
    // 0045 : APPEND
    // 0046 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Add_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Add_Assign_UnChecked();
    // 0000 : INITSLOT
    // 0003 : PUSHINT64
    // 000C : STLOC0
    // 000D : PUSHINT32
    // 0012 : STLOC1
    // 0013 : LDLOC0
    // 0014 : PUSH1
    // 0015 : ADD
    // 0016 : DUP
    // 0017 : PUSH0
    // 0018 : JMPGE
    // 001A : JMP
    // 001C : DUP
    // 001D : PUSHINT64
    // 0026 : JMPLE
    // 0028 : PUSHINT64
    // 0031 : AND
    // 0032 : STLOC0
    // 0033 : LDLOC1
    // 0034 : PUSH1
    // 0035 : ADD
    // 0036 : DUP
    // 0037 : PUSHINT32
    // 003C : JMPGE
    // 003E : JMP
    // 0040 : DUP
    // 0041 : PUSHINT32
    // 0046 : JMPLE
    // 0048 : PUSHINT64
    // 0051 : AND
    // 0052 : DUP
    // 0053 : PUSHINT32
    // 0058 : JMPLE
    // 005A : PUSHINT64
    // 0063 : SUB
    // 0064 : STLOC1
    // 0065 : NEWSTRUCT0
    // 0066 : DUP
    // 0067 : LDLOC0
    // 0068 : APPEND
    // 0069 : DUP
    // 006A : LDLOC1
    // 006B : APPEND
    // 006C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Left_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_Checked();
    // 0000 : INITSLOT
    // 0003 : PUSHINT64
    // 000C : STLOC0
    // 000D : PUSHINT32
    // 0012 : STLOC1
    // 0013 : LDLOC0
    // 0014 : PUSH1
    // 0015 : SHL
    // 0016 : DUP
    // 0017 : PUSH0
    // 0018 : JMPGE
    // 001A : THROW
    // 001B : DUP
    // 001C : PUSHINT64
    // 0025 : JMPLE
    // 0027 : THROW
    // 0028 : STLOC0
    // 0029 : LDLOC1
    // 002A : PUSH1
    // 002B : SHL
    // 002C : DUP
    // 002D : PUSHINT32
    // 0032 : JMPGE
    // 0034 : THROW
    // 0035 : DUP
    // 0036 : PUSHINT32
    // 003B : JMPLE
    // 003D : THROW
    // 003E : STLOC1
    // 003F : NEWSTRUCT0
    // 0040 : DUP
    // 0041 : LDLOC0
    // 0042 : APPEND
    // 0043 : DUP
    // 0044 : LDLOC1
    // 0045 : APPEND
    // 0046 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Left_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_UnChecked();
    // 0000 : INITSLOT
    // 0003 : PUSHINT64
    // 000C : STLOC0
    // 000D : PUSHINT32
    // 0012 : STLOC1
    // 0013 : LDLOC0
    // 0014 : PUSH1
    // 0015 : SHL
    // 0016 : DUP
    // 0017 : PUSH0
    // 0018 : JMPGE
    // 001A : JMP
    // 001C : DUP
    // 001D : PUSHINT64
    // 0026 : JMPLE
    // 0028 : PUSHINT64
    // 0031 : AND
    // 0032 : STLOC0
    // 0033 : LDLOC1
    // 0034 : PUSH1
    // 0035 : SHL
    // 0036 : DUP
    // 0037 : PUSHINT32
    // 003C : JMPGE
    // 003E : JMP
    // 0040 : DUP
    // 0041 : PUSHINT32
    // 0046 : JMPLE
    // 0048 : PUSHINT64
    // 0051 : AND
    // 0052 : DUP
    // 0053 : PUSHINT32
    // 0058 : JMPLE
    // 005A : PUSHINT64
    // 0063 : SUB
    // 0064 : STLOC1
    // 0065 : NEWSTRUCT0
    // 0066 : DUP
    // 0067 : LDLOC0
    // 0068 : APPEND
    // 0069 : DUP
    // 006A : LDLOC1
    // 006B : APPEND
    // 006C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Mul_Assign_Checked")]
    public abstract IList<object>? UnitTest_Mul_Assign_Checked();
    // 0000 : INITSLOT
    // 0003 : PUSHINT64
    // 000C : STLOC0
    // 000D : PUSHINT32
    // 0012 : STLOC1
    // 0013 : LDLOC0
    // 0014 : PUSH2
    // 0015 : MUL
    // 0016 : DUP
    // 0017 : PUSH0
    // 0018 : JMPGE
    // 001A : THROW
    // 001B : DUP
    // 001C : PUSHINT64
    // 0025 : JMPLE
    // 0027 : THROW
    // 0028 : STLOC0
    // 0029 : LDLOC1
    // 002A : PUSH2
    // 002B : MUL
    // 002C : DUP
    // 002D : PUSHINT32
    // 0032 : JMPGE
    // 0034 : THROW
    // 0035 : DUP
    // 0036 : PUSHINT32
    // 003B : JMPLE
    // 003D : THROW
    // 003E : STLOC1
    // 003F : NEWSTRUCT0
    // 0040 : DUP
    // 0041 : LDLOC0
    // 0042 : APPEND
    // 0043 : DUP
    // 0044 : LDLOC1
    // 0045 : APPEND
    // 0046 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Mul_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Mul_Assign_UnChecked();
    // 0000 : INITSLOT
    // 0003 : PUSHINT64
    // 000C : STLOC0
    // 000D : PUSHINT32
    // 0012 : STLOC1
    // 0013 : LDLOC0
    // 0014 : PUSH2
    // 0015 : MUL
    // 0016 : DUP
    // 0017 : PUSH0
    // 0018 : JMPGE
    // 001A : JMP
    // 001C : DUP
    // 001D : PUSHINT64
    // 0026 : JMPLE
    // 0028 : PUSHINT64
    // 0031 : AND
    // 0032 : STLOC0
    // 0033 : LDLOC1
    // 0034 : PUSH2
    // 0035 : MUL
    // 0036 : DUP
    // 0037 : PUSHINT32
    // 003C : JMPGE
    // 003E : JMP
    // 0040 : DUP
    // 0041 : PUSHINT32
    // 0046 : JMPLE
    // 0048 : PUSHINT64
    // 0051 : AND
    // 0052 : DUP
    // 0053 : PUSHINT32
    // 0058 : JMPLE
    // 005A : PUSHINT64
    // 0063 : SUB
    // 0064 : STLOC1
    // 0065 : NEWSTRUCT0
    // 0066 : DUP
    // 0067 : LDLOC0
    // 0068 : APPEND
    // 0069 : DUP
    // 006A : LDLOC1
    // 006B : APPEND
    // 006C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Right_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_Checked();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : PUSHINT32
    // 000A : STLOC1
    // 000B : LDLOC0
    // 000C : PUSH1
    // 000D : SHR
    // 000E : DUP
    // 000F : PUSH0
    // 0010 : JMPGE
    // 0012 : THROW
    // 0013 : DUP
    // 0014 : PUSHINT64
    // 001D : JMPLE
    // 001F : THROW
    // 0020 : STLOC0
    // 0021 : LDLOC1
    // 0022 : PUSH1
    // 0023 : SHR
    // 0024 : DUP
    // 0025 : PUSHINT32
    // 002A : JMPGE
    // 002C : THROW
    // 002D : DUP
    // 002E : PUSHINT32
    // 0033 : JMPLE
    // 0035 : THROW
    // 0036 : STLOC1
    // 0037 : NEWSTRUCT0
    // 0038 : DUP
    // 0039 : LDLOC0
    // 003A : APPEND
    // 003B : DUP
    // 003C : LDLOC1
    // 003D : APPEND
    // 003E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Right_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_UnChecked();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : PUSHINT32
    // 000A : STLOC1
    // 000B : LDLOC0
    // 000C : PUSH1
    // 000D : SHR
    // 000E : DUP
    // 000F : PUSH0
    // 0010 : JMPGE
    // 0012 : JMP
    // 0014 : DUP
    // 0015 : PUSHINT64
    // 001E : JMPLE
    // 0020 : PUSHINT64
    // 0029 : AND
    // 002A : STLOC0
    // 002B : LDLOC1
    // 002C : PUSH1
    // 002D : SHR
    // 002E : DUP
    // 002F : PUSHINT32
    // 0034 : JMPGE
    // 0036 : JMP
    // 0038 : DUP
    // 0039 : PUSHINT32
    // 003E : JMPLE
    // 0040 : PUSHINT64
    // 0049 : AND
    // 004A : DUP
    // 004B : PUSHINT32
    // 0050 : JMPLE
    // 0052 : PUSHINT64
    // 005B : SUB
    // 005C : STLOC1
    // 005D : NEWSTRUCT0
    // 005E : DUP
    // 005F : LDLOC0
    // 0060 : APPEND
    // 0061 : DUP
    // 0062 : LDLOC1
    // 0063 : APPEND
    // 0064 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Sub_Assign_Checked")]
    public abstract IList<object>? UnitTest_Sub_Assign_Checked();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : PUSHINT32
    // 000A : STLOC1
    // 000B : LDLOC0
    // 000C : PUSH1
    // 000D : SUB
    // 000E : DUP
    // 000F : PUSH0
    // 0010 : JMPGE
    // 0012 : THROW
    // 0013 : DUP
    // 0014 : PUSHINT64
    // 001D : JMPLE
    // 001F : THROW
    // 0020 : STLOC0
    // 0021 : LDLOC1
    // 0022 : PUSH1
    // 0023 : SUB
    // 0024 : DUP
    // 0025 : PUSHINT32
    // 002A : JMPGE
    // 002C : THROW
    // 002D : DUP
    // 002E : PUSHINT32
    // 0033 : JMPLE
    // 0035 : THROW
    // 0036 : STLOC1
    // 0037 : NEWSTRUCT0
    // 0038 : DUP
    // 0039 : LDLOC0
    // 003A : APPEND
    // 003B : DUP
    // 003C : LDLOC1
    // 003D : APPEND
    // 003E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Sub_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Sub_Assign_UnChecked();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : PUSHINT32
    // 000A : STLOC1
    // 000B : LDLOC0
    // 000C : PUSH1
    // 000D : SUB
    // 000E : DUP
    // 000F : PUSH0
    // 0010 : JMPGE
    // 0012 : JMP
    // 0014 : DUP
    // 0015 : PUSHINT64
    // 001E : JMPLE
    // 0020 : PUSHINT64
    // 0029 : AND
    // 002A : STLOC0
    // 002B : LDLOC1
    // 002C : PUSH1
    // 002D : SUB
    // 002E : DUP
    // 002F : PUSHINT32
    // 0034 : JMPGE
    // 0036 : JMP
    // 0038 : DUP
    // 0039 : PUSHINT32
    // 003E : JMPLE
    // 0040 : PUSHINT64
    // 0049 : AND
    // 004A : DUP
    // 004B : PUSHINT32
    // 0050 : JMPLE
    // 0052 : PUSHINT64
    // 005B : SUB
    // 005C : STLOC1
    // 005D : NEWSTRUCT0
    // 005E : DUP
    // 005F : LDLOC0
    // 0060 : APPEND
    // 0061 : DUP
    // 0062 : LDLOC1
    // 0063 : APPEND
    // 0064 : RET

    #endregion

}
