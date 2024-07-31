using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Inc_Dec : SmartContract.Framework.SmartContract
    {
        private static uint _property;
        private static int _intProperty;

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
            checked
            {
                --param;
                param--;
            }
            return param;
        }

        public static uint UnitTest_Param_Dec_UnChecked(uint param)
        {
            unchecked
            {
                --param;
                param--;
            }
            return param;
        }

        public static uint UnitTest_Param_Inc_Checked(uint param)
        {
            checked
            {
                ++param;
                param++;
            }
            return param;
        }

        public static uint UnitTest_Param_Inc_UnChecked(uint param)
        {
            unchecked
            {
                ++param;
                param++;
            }
            return param;
        }

        public static int UnitTest_Property_Inc_Checked_Int()
        {
            _intProperty = int.MaxValue;
            checked
            {
                ++_intProperty;
                _intProperty++;
            }
            return _intProperty;
        }

        public static int UnitTest_Property_Inc_UnChecked_Int()
        {
            _intProperty = int.MaxValue;
            unchecked
            {
                ++_intProperty;
                _intProperty++;
            }
            return _intProperty;
        }

        public static int UnitTest_Property_Dec_Checked_Int()
        {
            _intProperty = int.MinValue;
            checked
            {
                --_intProperty;
                _intProperty--;
            }
            return _intProperty;
        }

        public static int UnitTest_Property_Dec_UnChecked_Int()
        {
            _intProperty = int.MinValue;
            unchecked
            {
                --_intProperty;
                _intProperty--;
            }
            return _intProperty;
        }

        // Local Variable Tests
        public static int UnitTest_Local_Inc_Checked_Int()
        {
            int local = int.MaxValue;
            checked
            {
                ++local;
                local++;
            }
            return local;
        }

        public static int UnitTest_Local_Inc_UnChecked_Int()
        {
            int local = int.MaxValue;
            unchecked
            {
                ++local;
                local++;
            }
            return local;
        }

        public static int UnitTest_Local_Dec_Checked_Int()
        {
            int local = int.MinValue;
            checked
            {
                --local;
                local--;
            }
            return local;
        }

        public static int UnitTest_Local_Dec_UnChecked_Int()
        {
            int local = int.MinValue;
            unchecked
            {
                --local;
                local--;
            }
            return local;
        }

        // Parameter Tests
        public static int UnitTest_Param_Inc_Checked_Int(int param)
        {
            checked
            {
                ++param;
                param++;
            }
            return param;
        }

        public static int UnitTest_Param_Inc_UnChecked_Int(int param)
        {
            unchecked
            {
                ++param;
                param++;
            }
            return param;
        }

        public static int UnitTest_Param_Dec_Checked_Int(int param)
        {
            checked
            {
                --param;
                param--;
            }
            return param;
        }

        public static int UnitTest_Param_Dec_UnChecked_Int(int param)
        {
            unchecked
            {
                --param;
                param--;
            }
            return param;
        }

        public static void UnitTest_Not_DeadLoop()
        {
            for (uint i = 5; i < 7; i--) ;
        }
    }
}
