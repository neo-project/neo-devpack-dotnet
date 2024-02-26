using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Ownable : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.IOwnable
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Ownable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":44,""safe"":false},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":171,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":191,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":314,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":355,""safe"":false}],""events"":[{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM25jY3MAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACCaHR0cHM6Ly9naXRodWIuY29tL25lby1wcm9qZWN0L25lby1kZXZwYWNrLWRvdG5ldC90cmVlL21hc3Rlci9zcmMvTmVvLlNtYXJ0Q29udHJhY3QuVGVtcGxhdGUvdGVtcGxhdGVzL25lb2NvbnRyYWN0b3duZXIvT3duYWJsZS5jcwAC/aP6Q0bqUyolj8SX3a3bZDfJ/f8GdXBkYXRlAwAAD/2j+kNG6lMqJY/El92t22Q3yf3/B2Rlc3Ryb3kAAAAPAAD9gwEMAf/bMDQQStgkCUrKABQoAzoiAkBXAAF4Qfa0a+JBkl3oMUA03EH4J+yMQFcBATT1ENsglyYWDBFObyBBdXRob3JpemF0aW9uITp4StkoUMoAFLOrJAcQ2yAiBngQs6oME293bmVyIG11c3QgYmUgdmFsaWThNYr///9weAwB/9swNBnCSmjPSnjPDAhTZXRPd25lckGVAW9hQFcAAnl4QZv2Z85B5j8YhEAMBUhlbGxvQZv2Z85Bkl3oMSICQFcBAnkmBCJ0eHBoC5cmDEEtUQgwE85KgEV4cGhK2ShQygAUs6skBxDbICIGaBCzqgwRb3duZXIgbXVzdCBleGlzdHPhaAwB/9swNJLCSgvPSmjPDAhTZXRPd25lckGVAW9hDAVXb3JsZAwFSGVsbG9Bm/ZnzkHmPxiEQFcAAzXn/v//ENsglyYWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AABANcH+//+qJhYMEU5vIGF1dGhvcml6YXRpb24uOjcBAEDchhyR"));

    public static readonly Neo.SmartContract.Testing.Coverage.NeoDebugInfo DebugInfo = Neo.SmartContract.Testing.Coverage.NeoDebugInfo.FromDebugInfoJson(@"{""hash"":""0x773cedfec72cfb31ef91c6bd346c233264433489"",""documents"":[""Ownable.cs"",""..\\..\\..\\..\\..\\Neo.SmartContract.Framework\\Services\\Storage.cs""],""document-root"":""C:\\Red4Sec\\Neo\\neo-devpack-dotnet\\src\\Neo.SmartContract.Template\\bin\\Debug\\net7.0\\ownable"",""static-variables"":[],""methods"":[{""id"":""Neo.SmartContract.Template.Ownable.GetOwner()"",""name"":""Neo.SmartContract.Template.Ownable,GetOwner"",""range"":""0-20"",""params"":[],""return"":""Hash160"",""variables"":[],""sequence-points"":[""0[0]29:41-29:63"",""3[0]29:41-29:63"",""5[0]29:29-29:64"",""7[0]29:20-29:64"",""8[0]29:20-29:64"",""9[0]29:20-29:64"",""11[0]29:20-29:64"",""12[0]29:20-29:64"",""13[0]29:20-29:64"",""15[0]29:20-29:64"",""17[0]29:20-29:64"",""18[0]29:13-29:65"",""20[0]30:9-30:10""]},{""id"":""Neo.SmartContract.Framework.Services.Storage.Get(byte[])"",""name"":""Neo.SmartContract.Framework.Services.Storage,Get"",""range"":""21-35"",""params"":[""key,ByteArray,0""],""return"":""ByteArray"",""variables"":[],""sequence-points"":[""24[1]104:81-104:84"",""25[1]104:57-104:79"",""30[1]104:53-104:85""]},{""id"":""Neo.SmartContract.Template.Ownable.IsOwner()"",""name"":""Neo.SmartContract.Template.Ownable,IsOwner"",""range"":""36-43"",""params"":[],""return"":""Boolean"",""variables"":[],""sequence-points"":[""36[0]33:34-33:44"",""38[0]33:13-33:45""]},{""id"":""Neo.SmartContract.Template.Ownable.SetOwner(Neo.UInt160)"",""name"":""Neo.SmartContract.Template.Ownable,SetOwner"",""range"":""44-154"",""params"":[""newOwner,Hash160,0""],""return"":""Void"",""variables"":[""previous,Hash160,0""],""sequence-points"":[""47[0]42:17-42:26"",""49[0]42:30-42:35"",""50[0]42:30-42:35"",""52[0]42:17-42:35"",""53[0]42:13-43:74"",""55[0]43:53-43:72"",""74[0]43:17-43:74"",""75[0]45:36-45:44"",""76[0]45:36-45:52"",""77[0]45:36-45:52"",""79[0]45:36-45:52"",""80[0]45:36-45:52"",""81[0]45:36-45:52"",""83[0]45:36-45:52"",""84[0]45:36-45:52"",""85[0]45:36-45:72"",""87[0]45:36-45:72"",""88[0]45:36-45:72"",""90[0]45:36-45:72"",""92[0]45:57-45:65"",""93[0]45:57-45:72"",""94[0]45:57-45:72"",""95[0]45:56-45:72"",""96[0]45:74-45:95"",""117[0]45:13-45:96"",""118[0]47:32-47:42"",""123[0]47:21-47:42"",""124[0]48:49-48:57"",""125[0]48:25-48:47"",""128[0]48:25-48:47"",""130[0]48:13-48:58"",""132[0]49:13-49:43"",""133[0]49:13-49:43"",""134[0]49:24-49:32"",""135[0]49:13-49:43"",""136[0]49:13-49:43"",""137[0]49:34-49:42"",""138[0]49:13-49:43"",""139[0]49:13-49:43"",""149[0]49:13-49:43"",""154[0]50:9-50:10""]},{""id"":""Neo.SmartContract.Framework.Services.Storage.Put(byte[], Neo.SmartContract.Framework.ByteString)"",""name"":""Neo.SmartContract.Framework.Services.Storage,Put"",""range"":""155-170"",""params"":[""key,ByteArray,0"",""value,ByteArray,1""],""return"":""Void"",""variables"":[],""sequence-points"":[""158[1]106:90-106:95"",""159[1]106:85-106:88"",""160[1]106:69-106:83"",""165[1]106:65-106:96""]},{""id"":""Neo.SmartContract.Template.Ownable.MyMethod()"",""name"":""Neo.SmartContract.Template.Ownable,MyMethod"",""range"":""171-190"",""params"":[],""return"":""String"",""variables"":[],""sequence-points"":[""171[0]57:56-57:63"",""178[0]57:32-57:54"",""183[0]57:20-57:64"",""188[0]57:13-57:65"",""190[0]58:9-58:10""]},{""id"":""Neo.SmartContract.Template.Ownable._deploy(object, bool)"",""name"":""Neo.SmartContract.Template.Ownable,_deploy"",""range"":""191-313"",""params"":[""data,Any,0"",""update,Boolean,1""],""return"":""Void"",""variables"":[""initialOwner,Hash160,0""],""sequence-points"":[""194[0]63:17-63:23"",""195[0]63:13-67:14"",""197[0]66:17-66:24"",""199[0]70:17-70:21"",""200[0]70:17-70:29"",""201[0]70:17-70:29"",""202[0]70:25-70:29"",""203[0]70:17-70:29"",""204[0]70:13-70:65"",""206[0]70:38-70:57"",""211[0]70:38-70:64"",""212[0]70:38-70:64"",""213[0]70:31-70:64"",""214[0]70:31-70:64"",""215[0]70:31-70:64"",""216[0]72:45-72:49"",""217[0]72:21-72:49"",""218[0]74:36-74:48"",""219[0]74:36-74:56"",""220[0]74:36-74:56"",""222[0]74:36-74:56"",""223[0]74:36-74:56"",""224[0]74:36-74:56"",""226[0]74:36-74:56"",""227[0]74:36-74:56"",""228[0]74:36-74:80"",""230[0]74:36-74:80"",""231[0]74:36-74:80"",""233[0]74:36-74:80"",""235[0]74:61-74:73"",""236[0]74:61-74:80"",""237[0]74:61-74:80"",""238[0]74:60-74:80"",""239[0]74:82-74:101"",""258[0]74:13-74:102"",""259[0]76:49-76:61"",""260[0]76:25-76:47"",""263[0]76:25-76:47"",""265[0]76:13-76:62"",""267[0]77:13-77:43"",""268[0]77:13-77:43"",""269[0]77:24-77:28"",""270[0]77:13-77:43"",""271[0]77:13-77:43"",""272[0]77:30-77:42"",""273[0]77:13-77:43"",""274[0]77:13-77:43"",""284[0]77:13-77:43"",""289[0]78:58-78:65"",""296[0]78:49-78:56"",""303[0]78:25-78:47"",""308[0]78:13-78:66"",""313[0]79:9-79:10""]},{""id"":""Neo.SmartContract.Template.Ownable.Update(Neo.SmartContract.Framework.ByteString, string, object?)"",""name"":""Neo.SmartContract.Template.Ownable,Update"",""range"":""314-354"",""params"":[""nefFile,ByteArray,0"",""manifest,String,1"",""data,Any,2""],""return"":""Void"",""variables"":[],""sequence-points"":[""317[0]83:17-83:26"",""322[0]83:30-83:35"",""323[0]83:30-83:35"",""325[0]83:17-83:35"",""326[0]83:13-84:74"",""328[0]84:53-84:72"",""347[0]84:17-84:74"",""348[0]85:58-85:62"",""349[0]85:48-85:56"",""350[0]85:39-85:46"",""351[0]85:13-85:63"",""354[0]86:9-86:10""]},{""id"":""Neo.SmartContract.Template.Ownable.Destroy()"",""name"":""Neo.SmartContract.Template.Ownable,Destroy"",""range"":""355-386"",""params"":[],""return"":""Void"",""variables"":[],""sequence-points"":[""355[0]90:18-90:27"",""360[0]90:17-90:27"",""361[0]90:13-91:74"",""363[0]91:53-91:72"",""382[0]91:17-91:74"",""383[0]92:13-92:41"",""386[0]93:9-93:10""]}],""events"":[{""id"":""SetOwner"",""name"":""Neo.SmartContract.Template.Ownable,OnSetOwner"",""params"":[""previousOwner,Hash160,0"",""newOwner,Hash160,1""]}]}");

    #endregion

    #region Events

    [DisplayName("SetOwner")]
    public event Neo.SmartContract.Testing.TestingStandards.IOwnable.delSetOwner? OnSetOwner;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected Ownable(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
