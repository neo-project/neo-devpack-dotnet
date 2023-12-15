using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_ComplexAssign : SmartContract.Framework.SmartContract
    {
        public static (uint,int) UnitTest_Add_Assign_Checked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            checked
            {
                value1+=1;
                value2+=1;
            }
            return (value1, value2);
        }

        public static (uint,int) UnitTest_Add_Assign_UnChecked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            unchecked
            {
                value1+=1;
                value2+=1;
            }
            return (value1, value2);
        }

        public static (uint,int) UnitTest_Sub_Assign_Checked()
        {
            uint value1 = uint.MinValue;
            int value2 = int.MinValue;
            checked
            {
                value1-=1;
                value2-=1;
            }
            return (value1, value2);
        }

        public static (uint,int) UnitTest_Sub_Assign_UnChecked()
        {
            uint value1 = uint.MinValue;
            int value2 = int.MinValue;
            unchecked
            {
                value1-=1;
                value2-=1;
            }
            return (value1, value2);
        }

        public static (uint,int) UnitTest_Mul_Assign_Checked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            checked
            {
                value1*=2;
                value2*=2;
            }
            return (value1, value2);
        }

        public static (uint,int) UnitTest_Mul_Assign_UnChecked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            unchecked
            {
                value1*=2;
                value2*=2;
            }
            return (value1, value2);
        }

        public static (uint,int) UnitTest_Left_Shift_Assign_Checked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            checked
            {
                value1<<=1;
                value2<<=1;
            }
            return (value1, value2);
        }

        public static (uint,int) UnitTest_Left_Shift_Assign_UnChecked()
        {
            uint value1 = uint.MaxValue;
            int value2 = int.MaxValue;
            unchecked
            {
                value1<<=1;
                value2<<=1;
            }
            return (value1, value2);
        }

        public static (uint,int) UnitTest_Right_Shift_Assign_Checked()
        {
            uint value1 = uint.MinValue;
            int value2 = int.MinValue;
            checked
            {
                value1>>=1;
                value2>>=1;
            }
            return (value1, value2);
        }

        public static (uint,int) UnitTest_Right_Shift_Assign_UnChecked()
        {
            uint value1 = uint.MinValue;
            int value2 = int.MinValue;
            unchecked
            {
                value1>>=1;
                value2>>=1;
            }
            return (value1, value2);
        }
    }
}
