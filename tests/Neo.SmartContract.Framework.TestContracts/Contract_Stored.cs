// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Stored.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Stored : SmartContract
    {
        // Test non-static

        [Stored]
        public BigInteger WithoutConstructor { [Safe] get; protected set; }

        public void putWithoutConstructor(BigInteger value)
        {
            WithoutConstructor = value;
        }

        [Safe]
        public BigInteger getWithoutConstructor() => WithoutConstructor;

        // ---

        [Stored(0x01)]
        public static BigInteger WithKey { [Safe] get; protected set; }

        public static void putWithKey(BigInteger value)
        {
            WithKey = value;
        }

        [Safe]
        public static BigInteger getWithKey() => WithKey;

        // ---

        [Stored("testMe")]
        public static BigInteger WithString { [Safe] get; protected set; }

        public static void putWithString(BigInteger value)
        {
            WithString = value;
        }

        [Safe]
        public static BigInteger getWithString() => WithString;


        [Stored]
        public static BigInteger PrivateGetterPublicSetter { [Safe] private get; set; }

        [Safe]
        public static BigInteger getPrivateGetterPublicSetter() => PrivateGetterPublicSetter;


        [Stored]
        public BigInteger NonStaticPrivateGetterPublicSetter { [Safe] private get; set; }

        [Safe]
        public BigInteger getNonStaticPrivateGetterPublicSetter() => NonStaticPrivateGetterPublicSetter;

        [Stored("testStaticMulti")]
        public static BigInteger MultiWithStringStstic { [Safe] get; protected set; }

        [Stored("testMulti")]
        public BigInteger MultiWithString { [Safe] get; protected set; }

        public static BigInteger TestStaticMultiSet()
        {
            MultiWithStringStstic++;
            MultiWithStringStstic++;
            MultiWithStringStstic++;
            return MultiWithStringStstic;
        }

        public static BigInteger TestStaticMultiGet()
        {
            return MultiWithStringStstic;
        }

        public BigInteger TestMultiSet()
        {
            MultiWithString++;
            MultiWithString++;
            MultiWithString++;
            return MultiWithString;
        }

        public BigInteger TestMultiGet()
        {
            return MultiWithString;
        }
    }
}
