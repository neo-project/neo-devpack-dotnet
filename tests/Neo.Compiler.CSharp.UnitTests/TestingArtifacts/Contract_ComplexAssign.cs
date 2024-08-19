using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ComplexAssign : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ComplexAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Add_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""unitTest_Add_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":123,""safe"":false},{""name"":""unitTest_Sub_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":356,""safe"":false},{""name"":""unitTest_Sub_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":447,""safe"":false},{""name"":""unitTest_Mul_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":648,""safe"":false},{""name"":""unitTest_Mul_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":771,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":1004,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":1127,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":1360,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":1451,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

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
