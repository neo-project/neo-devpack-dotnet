using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inc_Dec : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inc_Dec"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Property_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":132,""safe"":false},{""name"":""unitTest_Property_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":332,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":432,""safe"":false},{""name"":""unitTest_Local_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":600,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":701,""safe"":false},{""name"":""unitTest_Local_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":870,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":1003,""safe"":false},{""name"":""unitTest_Param_Dec_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1204,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1303,""safe"":false},{""name"":""unitTest_Param_Inc_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1470,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1569,""safe"":false},{""name"":""unitTest_Property_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1736,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1792,""safe"":false},{""name"":""unitTest_Property_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":2000,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":2056,""safe"":false},{""name"":""unitTest_Local_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":2264,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":2321,""safe"":false},{""name"":""unitTest_Local_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":2530,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":2587,""safe"":false},{""name"":""unitTest_Param_Inc_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2796,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2847,""safe"":false},{""name"":""unitTest_Param_Dec_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3050,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3101,""safe"":false},{""name"":""unitTest_Not_DeadLoop"",""parameters"":[],""returntype"":""Void"",""offset"":3304,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":3398,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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AAAAAAAAAJGARXhAAv///39KYUVZnEoCAAAAgC4DOkoC////fzIDOkphRVlKnEoCAAAAgC4DOkoC////fzIDOmFFWUAC////f0phRVmcSgIAAACALgQiCkoC////fzJOBf////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAkUoC////fzIkBQAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAn0phRVlKnEoCAAAAgC4EIgpKAv///38yTgX/////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJFKAv///38yJAUAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJ9hRVlAAgAAAIBKYUVZnUoCAAAAgC4DOkoC////fzIDOkphRVlKnUoCAAAAgC4DOkoC////fzIDOmFFWUACAAAAgEphRVmdSgIAAACALgQiCkoC////fzJOBf////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAkUoC////fzIkBQAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAn0phRVlKnUoCAAAAgC4EIgpKAv///38yTgX/////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJFKAv///38yJAUAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJ9hRVlAVwEAAv///39waJxKAgAAAIAuAzpKAv///38yAzpKcEVoSpxKAgAAAIAuAzpKAv///38yAzpwRWhAVwEAAv///39waJxKAgAAAIAuBCIKSgL///9/Mk4F/////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACRSgL///9/MiQFAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACfSnBFaEqcSgIAAACALgQiCkoC////fzJOBf////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAkUoC////fzIkBQAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAn3BFaEBXAQACAAAAgHBonUoCAAAAgC4DOkoC////fzIDOkpwRWhKnUoCAAAAgC4DOkoC////fzIDOnBFaEBXAQACAAAAgHBonUoCAAAAgC4EIgpKAv///38yTgX/////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJFKAv///38yJAUAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJ9KcEVoSp1KAgAAAIAuBCIKSgL///9/Mk4F/////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACRSgL///9/MiQFAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACfcEVoQFcAAXicSgIAAACALgM6SgL///9/MgM6SoBFeEqcSgIAAACALgM6SgL///9/MgM6gEV4QFcAAXicSgIAAACALgQiCkoC////fzJOBf////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAkUoC////fzIkBQAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAn0qARXhKnEoCAAAAgC4EIgpKAv///38yTgX/////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJFKAv///38yJAUAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJ+ARXhAVwABeJ1KAgAAAIAuAzpKAv///38yAzpKgEV4Sp1KAgAAAIAuAzpKAv///38yAzqARXhAVwABeJ1KAgAAAIAuBCIKSgL///9/Mk4F/////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACRSgL///9/MiQFAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACfSoBFeEqdSgIAAACALgQiCkoC////fzJOBf////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAkUoC////fzIkBQAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAn4BFeEBXAQAVcCJTaEqdShAuBCImSgX/////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADIkBf////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAkXBFaBe1JKxAVgJAZoMHlA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Dec_Checked")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Inc_Checked")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Not_DeadLoop")]
    public abstract void UnitTest_Not_DeadLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Dec_Checked")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Inc_Checked")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Dec_Checked")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Inc_Checked")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked_Int();

    #endregion

    #region Constructor for internal use only

    protected Contract_Inc_Dec(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
