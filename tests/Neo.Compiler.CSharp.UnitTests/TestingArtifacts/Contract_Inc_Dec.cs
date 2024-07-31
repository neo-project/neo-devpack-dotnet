using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inc_Dec : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inc_Dec"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Property_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":60,""safe"":false},{""name"":""unitTest_Property_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":140,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":192,""safe"":false},{""name"":""unitTest_Local_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":264,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":317,""safe"":false},{""name"":""unitTest_Local_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":390,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":451,""safe"":false},{""name"":""unitTest_Param_Dec_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":532,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":587,""safe"":false},{""name"":""unitTest_Param_Inc_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":662,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":725,""safe"":false},{""name"":""unitTest_Property_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":808,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":864,""safe"":false},{""name"":""unitTest_Property_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":976,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1032,""safe"":false},{""name"":""unitTest_Local_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1144,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1201,""safe"":false},{""name"":""unitTest_Local_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1314,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1371,""safe"":false},{""name"":""unitTest_Param_Inc_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1484,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1543,""safe"":false},{""name"":""unitTest_Param_Dec_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1658,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1717,""safe"":false},{""name"":""unitTest_Not_DeadLoop"",""parameters"":[],""returntype"":""Void"",""offset"":1832,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1878,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1ZBwP/////AAAAAEpgRVicShAuAzpKA/////8AAAAAMgM6SmBFWEqcShAuAzpKA/////8AAAAAMgM6YEVYQAP/////AAAAAEpgRVicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUpgRVhKnEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFgRVhAEEpgRVidShAuAzpKA/////8AAAAAMgM6SmBFWEqdShAuAzpKA/////8AAAAAMgM6YEVYQBBKYEVYnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFKYEVYSp1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRYEVYQFcBABBwaJ1KEC4DOkoD/////wAAAAAyAzpKcEVoSp1KEC4DOkoD/////wAAAAAyAzpwRWhAVwEAEHBonUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFKcEVoSp1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcEVoQFcBAAP/////AAAAAHBonEoQLgM6SgP/////AAAAADIDOkpwRWhKnEoQLgM6SgP/////AAAAADIDOnBFaEBXAQAD/////wAAAABwaJxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSnBFaEqcShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBFaEBXAAEQSoBFeJ1KEC4DOkoD/////wAAAAAyAzpKgEV4Sp1KEC4DOkoD/////wAAAAAyAzqARXhAVwABEEqARXidShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUqARXhKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJGARXhAVwABA/////8AAAAASoBFeJxKEC4DOkoD/////wAAAAAyAzpKgEV4SpxKEC4DOkoD/////wAAAAAyAzqARXhAVwABA/////8AAAAASoBFeJxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSoBFeEqcShAuBCIOSgP/////AAAAADIMA/////8AAAAAkYBFeEAC////f0phRVmcSgIAAACALgM6SgL///9/MgM6SmFFWUqcSgIAAACALgM6SgL///9/MgM6YUVZQAL///9/SmFFWZxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSmFFWUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2FFWUACAAAAgEphRVmdSgIAAACALgM6SgL///9/MgM6SmFFWUqdSgIAAACALgM6SgL///9/MgM6YUVZQAIAAACASmFFWZ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSmFFWUqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2FFWUBXAQAC////f3BonEoCAAAAgC4DOkoC////fzIDOkpwRWhKnEoCAAAAgC4DOkoC////fzIDOnBFaEBXAQAC////f3BonEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoQFcBAAIAAACAcGidSgIAAACALgM6SgL///9/MgM6SnBFaEqdSgIAAACALgM6SgL///9/MgM6cEVoQFcBAAIAAACAcGidSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRWhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWhAVwABAv///39KgEV4nEoCAAAAgC4DOkoC////fzIDOkqARXhKnEoCAAAAgC4DOkoC////fzIDOoBFeEBXAAEC////f0qARXicSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0qARXhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+ARXhAVwABAgAAAIBKgEV4nUoCAAAAgC4DOkoC////fzIDOkqARXhKnUoCAAAAgC4DOkoC////fzIDOoBFeEBXAAECAAAAgEqARXidSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0qARXhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+ARXhAVwEAFXAiI2hKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwRWgXtSTcQFYCQFCcoOc="));

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
