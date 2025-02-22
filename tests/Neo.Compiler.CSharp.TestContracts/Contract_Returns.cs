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
using Neo.SmartContract.Framework.Services;

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

        private static int TryReturnInternal(bool exception)
        {
            int a = 0;
            Storage.Put("\x00", "\x00");
            try
            {
                try
                {
                    if (exception)
                        throw new System.Exception();
                    else
                        return ++a;
                }
                catch { return ++a; }
                finally
                {
                    ExecutionEngine.Assert(Storage.Get("\x00")! == "\x00");
                    Storage.Put("\x00", "\x01");
                    a++;
                    if (exception)
                        throw new System.Exception();
                }
            }
            finally
            {
                ExecutionEngine.Assert(Storage.Get("\x00")! == "\x01");
                Storage.Put("\x00", "\x02");
                ++a;
            }
#pragma warning disable CS0162 // Unreachable code detected
            Storage.Put("\x00", "\x03");
#pragma warning restore CS0162 // Unreachable code detected
        }

        public static int TryReturn()
        {
            int a = 0;
            // The following is an extremely dangerous case.
            // catch { return ++a; } pushes an `a` into the evaluation stack
            // but there is exception in finally, and the pushed `a` is never popped
            // and its value is kept into current evaluation stack in TryReturn.
            // No idea about the fix.
            //try { a = TryReturnInternal(true); }
            //catch
            //{
            //    ExecutionEngine.Assert(a == 0);
            //    a += 1;
            //}
            //finally { ExecutionEngine.Assert(Storage.Get("\x00")! == "\x02"); }
            //ExecutionEngine.Assert(a == 1);
            a = TryReturnInternal(false);
            ExecutionEngine.Assert(a == 1);
            ExecutionEngine.Assert(Storage.Get("\x00")! == "\x02");
            return ++a;
        }
    }
}
