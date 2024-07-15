using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inc_Dec : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inc_Dec"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Property_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":62,""safe"":false},{""name"":""unitTest_Property_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":144,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":198,""safe"":false},{""name"":""unitTest_Local_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":272,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":327,""safe"":false},{""name"":""unitTest_Local_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":402,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":465,""safe"":false},{""name"":""unitTest_Param_Dec_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":548,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":605,""safe"":false},{""name"":""unitTest_Param_Inc_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":682,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":747,""safe"":false},{""name"":""unitTest_Property_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":832,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":890,""safe"":false},{""name"":""unitTest_Property_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1004,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1062,""safe"":false},{""name"":""unitTest_Local_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1176,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1235,""safe"":false},{""name"":""unitTest_Local_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1350,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1409,""safe"":false},{""name"":""unitTest_Param_Inc_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1524,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1585,""safe"":false},{""name"":""unitTest_Param_Dec_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1702,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1763,""safe"":false},{""name"":""unitTest_Not_DeadLoop"",""parameters"":[],""returntype"":""Void"",""offset"":1880,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1926,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2JBwP/////AAAAAEpgRVicShAuAzpKA/////8AAAAAMgM6SmBFWEqcShAuAzpKA/////8AAAAAMgM6YEVYIgJAA/////8AAAAASmBFWJxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSmBFWEqcShAuBCIOSgP/////AAAAADIMA/////8AAAAAkWBFWCICQBBKYEVYnUoQLgM6SgP/////AAAAADIDOkpgRVhKnUoQLgM6SgP/////AAAAADIDOmBFWCICQBBKYEVYnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFKYEVYSp1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRYEVYIgJAVwEAEHBonUoQLgM6SgP/////AAAAADIDOkpwRWhKnUoQLgM6SgP/////AAAAADIDOnBFaCICQFcBABBwaJ1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSnBFaEqdShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBFaCICQFcBAAP/////AAAAAHBonEoQLgM6SgP/////AAAAADIDOkpwRWhKnEoQLgM6SgP/////AAAAADIDOnBFaCICQFcBAAP/////AAAAAHBonEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFKcEVoSpxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcEVoIgJAVwABEEqARXidShAuAzpKA/////8AAAAAMgM6SoBFeEqdShAuAzpKA/////8AAAAAMgM6gEV4IgJAVwABEEqARXidShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUqARXhKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJGARXgiAkBXAAED/////wAAAABKgEV4nEoQLgM6SgP/////AAAAADIDOkqARXhKnEoQLgM6SgP/////AAAAADIDOoBFeCICQFcAAQP/////AAAAAEqARXicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUqARXhKnEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJGARXgiAkAC////f0phRVmcSgIAAACALgM6SgL///9/MgM6SmFFWUqcSgIAAACALgM6SgL///9/MgM6YUVZIgJAAv///39KYUVZnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KYUVZSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYUVZIgJAAgAAAIBKYUVZnUoCAAAAgC4DOkoC////fzIDOkphRVlKnUoCAAAAgC4DOkoC////fzIDOmFFWSICQAIAAACASmFFWZ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSmFFWUqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2FFWSICQFcBAAL///9/cGicSgIAAACALgM6SgL///9/MgM6SnBFaEqcSgIAAACALgM6SgL///9/MgM6cEVoIgJAVwEAAv///39waJxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaCICQFcBAAIAAACAcGidSgIAAACALgM6SgL///9/MgM6SnBFaEqdSgIAAACALgM6SgL///9/MgM6cEVoIgJAVwEAAgAAAIBwaJ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFaEqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaCICQFcAAQL///9/SoBFeJxKAgAAAIAuAzpKAv///38yAzpKgEV4SpxKAgAAAIAuAzpKAv///38yAzqARXgiAkBXAAEC////f0qARXicSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0qARXhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+ARXgiAkBXAAECAAAAgEqARXidSgIAAACALgM6SgL///9/MgM6SoBFeEqdSgIAAACALgM6SgL///9/MgM6gEV4IgJAVwABAgAAAIBKgEV4nUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KgEV4Sp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgEV4IgJAVwEAFXAiI2hKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwRWgXtSTcQFYCQAyHVkI="));

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
