using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inc_Dec : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inc_Dec"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Property_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":62,""safe"":false},{""name"":""unitTest_Property_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":144,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":198,""safe"":false},{""name"":""unitTest_Local_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":272,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":327,""safe"":false},{""name"":""unitTest_Local_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":402,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":465,""safe"":false},{""name"":""unitTest_Param_Dec_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":548,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":605,""safe"":false},{""name"":""unitTest_Param_Inc_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":682,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":747,""safe"":false},{""name"":""unitTest_Property_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":832,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":890,""safe"":false},{""name"":""unitTest_Property_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1004,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1062,""safe"":false},{""name"":""unitTest_Local_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1176,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1235,""safe"":false},{""name"":""unitTest_Local_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1350,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1409,""safe"":false},{""name"":""unitTest_Param_Inc_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1524,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1585,""safe"":false},{""name"":""unitTest_Param_Dec_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1702,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1763,""safe"":false},{""name"":""unitTest_Not_DeadLoop"",""parameters"":[],""returntype"":""Void"",""offset"":1880,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1926,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9iQcD/////wAAAABKYEVYnEoQLgM6SgP/////AAAAADIDOkpgRVhKnEoQLgM6SgP/////AAAAADIDOmBFWCICQAP/////AAAAAEpgRVicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUpgRVhKnEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFgRVgiAkAQSmBFWJ1KEC4DOkoD/////wAAAAAyAzpKYEVYSp1KEC4DOkoD/////wAAAAAyAzpgRVgiAkAQSmBFWJ1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSmBFWEqdShAuBCIOSgP/////AAAAADIMA/////8AAAAAkWBFWCICQFcBABBwaJ1KEC4DOkoD/////wAAAAAyAzpKcEVoSp1KEC4DOkoD/////wAAAAAyAzpwRWgiAkBXAQAQcGidShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUpwRWhKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwRWgiAkBXAQAD/////wAAAABwaJxKEC4DOkoD/////wAAAAAyAzpKcEVoSpxKEC4DOkoD/////wAAAAAyAzpwRWgiAkBXAQAD/////wAAAABwaJxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRSnBFaEqcShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBFaCICQFcAARBKgEV4nUoQLgM6SgP/////AAAAADIDOkqARXhKnUoQLgM6SgP/////AAAAADIDOoBFeCICQFcAARBKgEV4nUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFKgEV4Sp1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRgEV4IgJAVwABA/////8AAAAASoBFeJxKEC4DOkoD/////wAAAAAyAzpKgEV4SpxKEC4DOkoD/////wAAAAAyAzqARXgiAkBXAAED/////wAAAABKgEV4nEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFKgEV4SpxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRgEV4IgJAAv///39KYUVZnEoCAAAAgC4DOkoC////fzIDOkphRVlKnEoCAAAAgC4DOkoC////fzIDOmFFWSICQAL///9/SmFFWZxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSmFFWUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2FFWSICQAIAAACASmFFWZ1KAgAAAIAuAzpKAv///38yAzpKYUVZSp1KAgAAAIAuAzpKAv///38yAzphRVkiAkACAAAAgEphRVmdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0phRVlKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hRVkiAkBXAQAC////f3BonEoCAAAAgC4DOkoC////fzIDOkpwRWhKnEoCAAAAgC4DOkoC////fzIDOnBFaCICQFcBAAL///9/cGicSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWgiAkBXAQACAAAAgHBonUoCAAAAgC4DOkoC////fzIDOkpwRWhKnUoCAAAAgC4DOkoC////fzIDOnBFaCICQFcBAAIAAACAcGidSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRWhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWgiAkBXAAEC////f0qARXicSgIAAACALgM6SgL///9/MgM6SoBFeEqcSgIAAACALgM6SgL///9/MgM6gEV4IgJAVwABAv///39KgEV4nEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KgEV4SpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgEV4IgJAVwABAgAAAIBKgEV4nUoCAAAAgC4DOkoC////fzIDOkqARXhKnUoCAAAAgC4DOkoC////fzIDOoBFeCICQFcAAQIAAACASoBFeJ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSoBFeEqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4BFeCICQFcBABVwIiNoSp1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcEVoF7Uk3EBWAkDrkhIU"));

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
