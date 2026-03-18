// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NeoDebugInfo.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Compiler.SecurityAnalyzer;
using System;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class NeoDebugInfoTests
    {
        [TestMethod]
        public void FromDebugInfoJson_Parses_Minimal_Valid_DebugInfo()
        {
            var json = (JObject)JToken.Parse("""
            {
              "documents": ["a.cs"],
              "methods": [
                {
                  "range": "10-20",
                  "sequence-points": ["12[0]3:4-3:9"]
                }
              ]
            }
            """)!;

            var debugInfo = NeoDebugInfo.FromDebugInfoJson(json);

            Assert.AreEqual(1, debugInfo.Documents.Count);
            Assert.AreEqual("a.cs", debugInfo.Documents[0]);
            Assert.AreEqual(1, debugInfo.Methods.Count);
            Assert.AreEqual(10, debugInfo.Methods[0].Range.Start);
            Assert.AreEqual(20, debugInfo.Methods[0].Range.End);
            Assert.AreEqual(12, debugInfo.Methods[0].SequencePoints[0].Address);
            Assert.AreEqual(0, debugInfo.Methods[0].SequencePoints[0].Document);
            Assert.AreEqual(3, debugInfo.Methods[0].SequencePoints[0].Start.Line);
            Assert.AreEqual(4, debugInfo.Methods[0].SequencePoints[0].Start.Column);
        }

        [TestMethod]
        public void FromDebugInfoJson_Throws_When_Methods_Is_Not_Array()
        {
            var json = new JObject
            {
                ["documents"] = new JArray("a.cs"),
                ["methods"] = new JString("invalid")
            };

            Assert.ThrowsException<ArgumentNullException>(() => NeoDebugInfo.FromDebugInfoJson(json));
        }

        [TestMethod]
        public void FromDebugInfoJson_Throws_When_Range_Is_Invalid()
        {
            var json = (JObject)JToken.Parse("""
            {
              "documents": ["a.cs"],
              "methods": [
                {
                  "range": "invalid",
                  "sequence-points": ["12[0]3:4-3:9"]
                }
              ]
            }
            """)!;

            Assert.ThrowsException<FormatException>(() => NeoDebugInfo.FromDebugInfoJson(json));
        }

        [TestMethod]
        public void FromDebugInfoJson_Throws_When_SequencePoint_Is_Invalid()
        {
            var json = (JObject)JToken.Parse("""
            {
              "documents": ["a.cs"],
              "methods": [
                {
                  "range": "10-20",
                  "sequence-points": ["bad-sequence-point"]
                }
              ]
            }
            """)!;

            Assert.ThrowsException<FormatException>(() => NeoDebugInfo.FromDebugInfoJson(json));
        }
    }
}
