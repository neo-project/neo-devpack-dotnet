// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_WriteInTry.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_WriteInTry : SmartContract.Framework.SmartContract
    {
        public static void BaseTry()
        {
            try { Write(); }
            catch
            {
                try { Delete(); }
                finally
                {
                    try { throw new Exception("throw in nested finally"); }
                    catch { throw; }
                }
            }
            finally
            {
                try { Storage.Delete("\x00"); }
                catch { ExecutionEngine.Abort(); }
            }
        }

        private static void Write() => Storage.Put("\x00", '\x00');
        private static void Delete() => Storage.Delete("\x00");
        public static void TryWrite()
        {
            try { Write(); throw new Exception("throw in TryWrite try"); }
            catch { try { Delete(); throw new Exception("throw in TryWrite catch"); } finally { } }
        }
        public static void TryWriteWithVulnerability()
        {
            try { Delete(); } catch { }  // unsafe
        }

        public static void RecursiveTry(int i)
        {
            try
            {
                Write();
                if (i > 0)
                    RecursiveTry(i - 1);
            }
            finally
            {
                MutualRecursiveTry(i - 1);
            }
        }

        public static void MutualRecursiveTry(int i)
        {
            try
            {
                if (i > 0)
                    RecursiveTry(i - 1);
                TryWriteWithVulnerability();
                Delete();
            }
            finally { }
        }

        public static void SafeTryWithCatchWithThrowInFinally()
        {
            try
            {
                try { Write(); }
                catch { }
                finally { throw new Exception(); }
            }
            catch
            {
                try { Write(); }
                catch { }
                finally { throw new Exception(); }
            }
            finally { ExecutionEngine.Abort(); }
        }

        public static void UnsafeNestedTryWrite(bool recursive)
        {
            try
            {
                try { Write(); }
                finally
                {
                    if (recursive)
                        UnsafeNestedTryWrite(false);
                }
                throw new Exception();
            }
            // no catch above and is safe
            // but will be catched below, so the try above is unsafe
            // for containing write in a nested try
            catch { }
        }
    }
}
