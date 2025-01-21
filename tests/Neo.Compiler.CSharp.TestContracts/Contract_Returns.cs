// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_Returns.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Returns : SmartContract.Framework.SmartContract
    {
        /// <summary>
        /// No return
        /// </summary>
        public static int Sum(int a, int b)
        {
            return a + b;
        }

        /// <summary>
        /// One return
        /// </summary>
        public static int Subtract(int a, int b)
        {
            return a - b;
        }

        /// <summary>
        /// Multiple returns
        /// </summary>
        public static (int, int) Div(int a, int b)
        {
            return (a / b, a % b);
        }

        /// <summary>
        /// Use the double return
        /// </summary>
        public static int Mix(int a, int b)
        {
            (int c, int d) = Div(a, b);

            return Subtract(c, d);
        }

        /// <summary>
        /// ByteString add
        /// </summary>
        public static ByteString ByteStringAdd(ByteString a, ByteString b)
        {
            return a + b;
        }
    }
}
