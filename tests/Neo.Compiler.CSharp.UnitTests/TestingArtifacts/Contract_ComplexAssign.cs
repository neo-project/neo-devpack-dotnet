using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ComplexAssign : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ComplexAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Add_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""unitTest_Add_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":77,""safe"":false},{""name"":""unitTest_Sub_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":192,""safe"":false},{""name"":""unitTest_Sub_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":261,""safe"":false},{""name"":""unitTest_Mul_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":368,""safe"":false},{""name"":""unitTest_Mul_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":445,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":560,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":637,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":752,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":821,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2gA1cCAAP/////AAAAAHAC////f3FoEZ5KEC4DOkoD/////wAAAAAyAzpKcEVpEZ5KAgAAAIAuAzpKAv///38yAzpKcUXFSmjPSmnPIgJAVwIAA/////8AAAAAcAL///9/cWgRnkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFKcEVpEZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnFFxUpoz0ppzyICQFcCABBwAgAAAIBxaBGfShAuAzpKA/////8AAAAAMgM6SnBFaRGfSgIAAACALgM6SgL///9/MgM6SnFFxUpoz0ppzyICQFcCABBwAgAAAIBxaBGfShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUpwRWkRn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcUXFSmjPSmnPIgJAVwIAA/////8AAAAAcAL///9/cWgSoEoQLgM6SgP/////AAAAADIDOkpwRWkSoEoCAAAAgC4DOkoC////fzIDOkpxRcVKaM9Kac8iAkBXAgAD/////wAAAABwAv///39xaBKgShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUpwRWkSoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcUXFSmjPSmnPIgJAVwIAA/////8AAAAAcAL///9/cWgRqEoQLgM6SgP/////AAAAADIDOkpwRWkRqEoCAAAAgC4DOkoC////fzIDOkpxRcVKaM9Kac8iAkBXAgAD/////wAAAABwAv///39xaBGoShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUpwRWkRqEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcUXFSmjPSmnPIgJAVwIAEHACAAAAgHFoEalKEC4DOkoD/////wAAAAAyAzpKcEVpEalKAgAAAIAuAzpKAv///38yAzpKcUXFSmjPSmnPIgJAVwIAEHACAAAAgHFoEalKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSnBFaRGpSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pxRcVKaM9Kac8iAkBITezS"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Add_Assign_Checked")]
    public abstract IList<object>? UnitTest_Add_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Add_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Add_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Left_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Left_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Mul_Assign_Checked")]
    public abstract IList<object>? UnitTest_Mul_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Mul_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Mul_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Right_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Right_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Sub_Assign_Checked")]
    public abstract IList<object>? UnitTest_Sub_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Sub_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Sub_Assign_UnChecked();

    #endregion

    #region Constructor for internal use only

    protected Contract_ComplexAssign(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
