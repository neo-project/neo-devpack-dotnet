using Neo.SmartContract.Framework;
using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_ComplexAssign : SmartContract.Framework.SmartContract
    {
        public static (uint, int) UnitTest_Add_Assign_Checked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            checked
            {
                value1 += 1;
                value2 += 1;
            }
            return (value1, value2);
        }

        public static (uint, int) UnitTest_Add_Assign_UnChecked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            unchecked
            {
                value1 += 1;
                value2 += 1;
            }
            return (value1, value2);
        }

        public static (uint, int) UnitTest_Sub_Assign_Checked()
        {
            uint value1 = uint.MinValue;
            int value2 = int.MinValue;
            checked
            {
                value1 -= 1;
                value2 -= 1;
            }
            return (value1, value2);
        }

        public static (uint, int) UnitTest_Sub_Assign_UnChecked()
        {
            uint value1 = uint.MinValue;
            int value2 = int.MinValue;
            unchecked
            {
                value1 -= 1;
                value2 -= 1;
            }
            return (value1, value2);
        }

        public static (uint, int) UnitTest_Mul_Assign_Checked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            checked
            {
                value1 *= 2;
                value2 *= 2;
            }
            return (value1, value2);
        }

        public static (uint, int) UnitTest_Mul_Assign_UnChecked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            unchecked
            {
                value1 *= 2;
                value2 *= 2;
            }
            return (value1, value2);
        }

        public static (uint, int) UnitTest_Left_Shift_Assign_Checked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            checked
            {
                value1 <<= 1;
                value2 <<= 1;
            }
            return (value1, value2);
        }

        public static (uint, int) UnitTest_Left_Shift_Assign_UnChecked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            unchecked
            {
                value1 <<= 1;
                value2 <<= 1;
            }
            return (value1, value2);
        }

        public static (uint, int) UnitTest_Right_Shift_Assign_Checked()
        {
            uint value1 = uint.MinValue;
            int value2 = int.MinValue;
            checked
            {
                value1 >>= 1;
                value2 >>= 1;
            }
            return (value1, value2);
        }

        public static (uint, int) UnitTest_Right_Shift_Assign_UnChecked()
        {
            uint value1 = uint.MinValue;
            int value2 = int.MinValue;
            unchecked
            {
                value1 >>= 1;
                value2 >>= 1;
            }
            return (value1, value2);
        }

        public class TestClass
        {
            public byte Property = 7;
            public byte[] ArrayProperty = [0, 1, 2, 3];
        }

        public static void UnitTest_Member_Element_Complex_Assign()
        {
            TestClass t = new();
            t.Property %= 3;
            ExecutionEngine.Assert(t.Property == 1);
            t.ArrayProperty[3] &= 4;
            ExecutionEngine.Assert(t.ArrayProperty[0] == 0);
        }
    }
}
