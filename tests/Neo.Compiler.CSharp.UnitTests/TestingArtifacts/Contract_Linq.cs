using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":63,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":227,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":327,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":396,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":498,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":521,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":721,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":997,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1115,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1267,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1358,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1368,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1466,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1529,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1629,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1732,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1882,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1983,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2116,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2212,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2407,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2540,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2731,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2839,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPwO85zuDk6SXGwqBqeeFEDdhvzqwEaXRvYQEAAQ8AAP3NCzcAAEA3AQBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnyICQFcAAQpsAAAAEHg0BSICQFcEA3g0KgwEZnVuY3o0O3hKcMpxEHIiEGhqznNreXo2SoFFapxyamkw8HkiAkBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcAAngLlyYReQwIIGlzIG51bGyL2yg6QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwABCloAAAB4NAUiAkBXBAJ4NDEMCXByZWRpY2F0ZXk0k3hKcMpxEHIiEmhqznNreTaqJgUJIgxqnHJqaTDuCCICQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeBC3QFcAAXg0BqoiAkBXBAF4NB14SnDKcRByIgxoas5zCCIMapxyamkw9AkiAkBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcAAQpcAAAAeDQFIgJAVwQCeDQzDAlwcmVkaWNhdGV5Ner+//94SnDKcRByIhFoas5za3k2JgUIIgxqnHJqaTDvCSICQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeBC3QFcAAnlgCgsAAAB4NJ0iAkBXAAF4WLdAVwABeDQFIgJAVwYBeDWhAAAAEHAQcXhKcspzEHQicGpsznVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpbZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnFFbJx0bGswkGgQlyYUDA9zb3VyY2UgaXMgZW1wdHk6aWihIgJAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAEK3AAAAHg0BSICQFcGAng1swAAAAwIc2VsZWN0b3J5NaP9//8QcBBxeEpyynMQdCJyamzOdWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlteTaeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pxRWycdGxrMI5oEJcmFAwPc291cmNlIGlzIGVtcHR5OmlooSICQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABEnigSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAF4NAUiAkBXBQF4NE8QcHhKccpyEHMiPGlrznRoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVrnHNrajDEaCICQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABCo4AAAB4NAUiAkBXBQJ4NGUMCXByZWRpY2F0ZXk1G/z//xBweEpxynIQcyJBaWvOdGx5NiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFa5xza2owv2giAkBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcAAXgQt0BXAAJ5eDQFIgJAVwACeWEKRQAAAHg0BSICQFcEAng1wPz//wwJcHJlZGljYXRleTV0+///eEpwynEQciIRaGrOc2t5NiYFCCIMapxyamkw7wkiAkBXAAF4WZdAVwACeXg0qiICQFcFAsJweEpxynIQcyIfaWvOdGgQCxLASjQ6bDcCAEsQUdBsSxFR0M9rnHNrajDhEAsSwEo0Hnk3AgBLEFHQeUsRUdBxaWg1X////yICQMJAz0BXAAF4EAvQQBBR0EARUdBAVwUCwnB4SnHKchBzIh9pa850aBALEsBKNNhsNwIASxBR0GxLEVHQz2ucc2tqMOFoec5xaWg1Dv///yICQM5AVwUCwnB4SnHKchBzIiJpa850aMVKC89KEM9KND1sNwIASxBR0GxLEVHQz2ucc2tqMN7FSgvPShDPSjQeeTcCAEsQUdB5SxFR0HFpaDW4/v//IgJAwkDPQFcAAUAQUdBAEVHQQFcAAQ8KXAAAAHg0BSICQFcEA3g0MwwJcHJlZGljYXRleTUY+v//eEpwynEQciIRaGrOc2t5NiYFayIMapxyamkw73oiAkBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcAAXgQt0BXAAEKXgAAAHg0BSICQFcFAng0MQwIc2VsZWN0b3J5NbP5///CcHhKccpyEHMiDmlrznRobHk2z2ucc2tqMPJoIgJAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkDCQM9AVwABeBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXBQHCcHhKccpyEHMiImlrznRoEAsSwEo1OP7//2w3AgBLEFHQbEsRUdDPa5xza2ow3goOAAAAaDVA////IgJAVwABxUoLz0oQz0o1rP7//3gQzksQUdB4Ec5LEVHQQBDOQBHOQFcAAnl4NAUiAkBXBQJ4NFnCcHhKccpyEHMiRmlrznR5ELcmN3lKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+BRSIFaGzPa5xza2owumgiAkBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QMJAz0BXAAF4NAUiAkBXBQF4NWb6//8QcHhKccpyEHMiPWlrznRobJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFa5xza2oww2giAkBXAAEKiwAAAHg0BSICQFcFAng0YgwIc2VsZWN0b3J5NdP3//8QcHhKccpyEHMiP2lrznRobHk2nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEVrnHNrajDBaCICQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABEnigSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAJ5eDQFIgJAVwUCeDRZwnB4SnHKchBzIkZpa850eRC2JgQiP2hsz3lKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+BRWucc2tqMLpoIgJAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkDCQM9AVwEBChwAAAB4Nd38//9wCqEAAAAKoQAAAGg0IyICQFcAAcVKC89KEM9KNTv8//94NwIASxBR0HhLEVHQQFcFA3g0UQwLa2V5U2VsZWN0b3J5NVn2//8MD2VsZW1lbnRTZWxlY3Rvcno1Qvb//8hweEpxynIQcyIUaWvOdGx6NkpseTZoU9BFa5xza2ow7GgiAkBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QMhA0EBXAAF4QFcAAXgQzkAQzkBXAAEKYgAAAHg0BSICQFcFAng0NQwJcHJlZGljYXRleTXL9f//wnB4SnHKchBzIhFpa850bHk2JgVobM9rnHNrajDvaCICQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAwkDPQFcAAXgQt0BWAkBXAAN5ep5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfIgJAVwABDARiYXNlIgJAVwABDARiYXNlIgJAVwADeXqgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnyICQFcAAXgQzkBXAAJ5StgmGkUMFHZhbHVlIGNhbm5vdCBiZSBudWxsOkp4EFHQQA3Nw2c="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion

    #region Constructor for internal use only

    protected Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
