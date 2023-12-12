using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract2_Inc_Dec : SmartContract.Framework.SmartContract
    {
        private static uint _property;

        public static uint UnitTest_Property_Inc_Checked()
        {
            _property = uint.MaxValue;
            checked
            {
                ++_property;
                _property++;
            }
            return _property;
        }

        public static uint UnitTest_Property_Inc_UnChecked()
        {
            _property = uint.MaxValue;
            unchecked
            {
                ++_property;
                _property++;
            }
            return _property;
        }

        public static uint UnitTest_Property_Dec_Checked()
        {
            _property = uint.MinValue;
            checked
            {
                --_property;
                _property--;
            }
            return _property;
        }

        public static uint UnitTest_Property_Dec_UnChecked()
        {
            _property = uint.MinValue;
            unchecked
            {
                --_property;
                _property--;
            }
            return _property;
        }

        public static uint UnitTest_Local_Dec_Checked()
        {
            uint local = uint.MinValue;
            checked
            {
                --local;
                local--;
            }
            return local;
        }

        public static uint UnitTest_Local_Dec_UnChecked()
        {
            uint local = uint.MinValue;
            unchecked
            {
                --local;
                local--;
            }
            return local;
        }

        public static uint UnitTest_Local_Inc_Checked()
        {
            uint local = uint.MaxValue;
            checked
            {
                ++local;
                local++;
            }
            return local;
        }

        public static uint UnitTest_Local_Inc_UnChecked()
        {
            uint local = uint.MaxValue;
            unchecked
            {
                ++local;
                local++;
            }
            return local;
        }

        public static uint UnitTest_Param_Dec_Checked(uint param)
        {
            param = uint.MinValue;
            checked
            {
                --param;
                param--;
            }
            return param;
        }

        public static uint UnitTest_Param_Dec_UnChecked(uint param)
        {
            param = uint.MinValue;
            unchecked
            {
                --param;
                param--;
            }
            return param;
        }

        public static uint UnitTest_Param_Inc_Checked(uint param)
        {
            param = uint.MaxValue;
            checked
            {
                ++param;
                param++;
            }
            return param;
        }

        public static uint UnitTest_Param_Inc_UnChecked(uint param)
        {
            param = uint.MaxValue;
            unchecked
            {
                ++param;
                param++;
            }
            return param;
        }

        public static void UnitTest_Not_DeadLoop()
        {
            for (uint i = 5; i < 7; i--) ;
        }

    }
}
