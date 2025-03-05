// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_MemberAccess.cs file belongs to the neo project and is free
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
    public class Contract_MemberAccess : SmartContract.Framework.SmartContract
    {
        public static void TestMain()
        {
            var my = new MyClass();
            Runtime.Log(my.Data1.ToString());
            Runtime.Log(MyClass.Data2);
            //Runtime.Log(MyClass.Data3.ToString());
            Runtime.Log(my.Data4);
            Runtime.Log(my.Method());
        }

        public static void TestComplexAssignment()
        {
            var my = new MyClass();
            ExecutionEngine.Assert(my.PropertyComplexAssignment() == -1);
            ExecutionEngine.Assert((my.Data1 /= -1) == 1);
            ExecutionEngine.Assert(my.FieldComplexAssignment() == 0);
            ExecutionEngine.Assert(my.FieldComplexAssignmentString() == "hello2");
            ExecutionEngine.Assert((my.Data4 += "33") == "hello233");
        }

        public static void TestStaticComplexAssignment()
        {
            MyClass.Data3 = 0;
            ExecutionEngine.Assert(MyClass.Data3 == 0);
            MyClass.Data3 += 1;
            ExecutionEngine.Assert(MyClass.Data3 == 1);

            ExecutionEngine.Assert(MyClass.Data6 == false);
            MyClass.Data6 |= true;
            ExecutionEngine.Assert(MyClass.Data6 == true);
            MyClass.Data6 ^= true;
            ExecutionEngine.Assert(MyClass.Data6 == false);
        }

        public class MyClass
        {
            public int Data1 { get; set; }  // non-static IPropertySymbol

            public const string Data2 = "msg";

            public static int Data3 = 3;  // static IFieldSymbol

            public string Data4 = "hello";  // non-static IFieldSymbol

            public int Data5 = 5;  // non-static IFieldSymbol

            public static bool Data6 { get; set; } = false;  // static IPropertySymbol

            public string Method() => "";

            public int PropertyComplexAssignment() => Data1 -= 1;
            public int FieldComplexAssignment() => Data5 ^= Data5;
            public string FieldComplexAssignmentString() => Data4 += "2";
        }
    }
}
