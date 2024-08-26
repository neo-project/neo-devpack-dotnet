using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ComplexAssign(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ComplexAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Add_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""unitTest_Add_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":75,""safe"":false},{""name"":""unitTest_Sub_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":188,""safe"":false},{""name"":""unitTest_Sub_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":255,""safe"":false},{""name"":""unitTest_Mul_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":360,""safe"":false},{""name"":""unitTest_Mul_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":435,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":548,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":623,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":736,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":803,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2MA1cCAAP/////AAAAAHAC////f3FoEZ5KEC4DOkoD/////wAAAAAyAzpKcEVpEZ5KAgAAAIAuAzpKAv///38yAzpKcUXFSmjPSmnPQFcCAAP/////AAAAAHAC////f3FoEZ5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSnBFaRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pxRcVKaM9Kac9AVwIAEHACAAAAgHFoEZ9KEC4DOkoD/////wAAAAAyAzpKcEVpEZ9KAgAAAIAuAzpKAv///38yAzpKcUXFSmjPSmnPQFcCABBwAgAAAIBxaBGfShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUpwRWkRn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcUXFSmjPSmnPQFcCAAP/////AAAAAHAC////f3FoEqBKEC4DOkoD/////wAAAAAyAzpKcEVpEqBKAgAAAIAuAzpKAv///38yAzpKcUXFSmjPSmnPQFcCAAP/////AAAAAHAC////f3FoEqBKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSnBFaRKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pxRcVKaM9Kac9AVwIAA/////8AAAAAcAL///9/cWgRqEoQLgM6SgP/////AAAAADIDOkpwRWkRqEoCAAAAgC4DOkoC////fzIDOkpxRcVKaM9Kac9AVwIAA/////8AAAAAcAL///9/cWgRqEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFKcEVpEahKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnFFxUpoz0ppz0BXAgAQcAIAAACAcWgRqUoQLgM6SgP/////AAAAADIDOkpwRWkRqUoCAAAAgC4DOkoC////fzIDOkpxRcVKaM9Kac9AVwIAEHACAAAAgHFoEalKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSnBFaRGpSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pxRcVKaM9Kac9AEjacdw=="));

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

}
