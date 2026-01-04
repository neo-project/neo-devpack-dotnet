// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Abort.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using System;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Abort : SmartContract.Framework.SmartContract
    {
        public int TestAbort()
        {
            int v = 0;
            ExecutionEngine.Abort();
            v = 100;
            return v;
        }

        public int TestAbortMsg()
        {
            int v = 0;
            ExecutionEngine.Abort("ABORT MSG");
            v = 100;
            return v;
        }

        public int TestAbortInFunction(bool abortMsg)
        {
            int v = 0;
            if (abortMsg)
                v = TestAbortMsg();
            else
                v = TestAbort();
            v = 1;
            return v;
        }

        public int TestAbortInTry(bool abortMsg)
        {
            int v = 0;
            try
            {
                if (abortMsg)
                    v = TestAbortMsg();
                else
                    v = TestAbort();
            }
            catch { v = 1; }
            finally { v = 2; }
            return v;
        }

        public int TestAbortInCatch(bool abortMsg)
        {
            int v = 0;
            try { v = 1; throw new Exception(); }
            catch
            {
                if (abortMsg)
                    v = TestAbortMsg();
                else
                    v = TestAbort();
            }
            finally { v = 2; }
            return v;
        }

        public int TestAbortInFinally(bool abortMsg)
        {
            int v = 0;
            try { v = 1; }
            catch { v = 2; }
            finally
            {
                if (abortMsg)
                    v = TestAbortMsg();
                else
                    v = TestAbort();
            }
            return v;
        }
    }
}
