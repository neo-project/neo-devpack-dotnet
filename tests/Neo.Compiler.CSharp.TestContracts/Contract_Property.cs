using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Property : SmartContract.Framework.SmartContract
    {
        // Read-only property
        public static string Symbol => "TokenSymbol";

        // Static properties with different access modifiers
        private static BigInteger TestStaticProperty { get; set; } = 0;
        public static string PublicStaticProperty { get; set; } = "Initial";
        protected static int ProtectedStaticProperty { get; private set; } = 100;

        // Instance properties with different access modifiers
        private BigInteger TestProperty { get; set; } = 0;
        public string PublicProperty { get; set; } = "Test";
        protected bool ProtectedProperty { get; set; } = false;
        private int PrivateGetPublicSet { get; set; } = 42;

        // Uninitialized property
        public int UninitializedProperty { get; set; }
        public static int UninitializedStaticProperty { get; set; }

        // Computed property
        public BigInteger ComputedProperty => TestProperty * 2;

        // Test methods for static properties
        public static BigInteger TestStaticPropertyInc()
        {
            TestStaticProperty++;
            TestStaticProperty++;
            TestStaticProperty++;

            ++TestStaticProperty;
            ++TestStaticProperty;
            ++TestStaticProperty;
            return TestStaticProperty;
        }

        public static BigInteger TestStaticPropertyDec()
        {
            TestStaticProperty--;
            TestStaticProperty--;
            TestStaticProperty--;

            --TestStaticProperty;
            --TestStaticProperty;
            --TestStaticProperty;
            return TestStaticProperty;
        }

        public static BigInteger TestStaticPropertyMul()
        {
            TestStaticProperty *= 2;
            TestStaticProperty *= 2;
            TestStaticProperty *= 2;
            return TestStaticProperty;
        }

        // Test methods for instance properties
        public BigInteger TestPropertyInc()
        {
            TestProperty++;
            TestProperty++;
            TestProperty++;

            ++TestProperty;
            ++TestProperty;
            ++TestProperty;
            return TestProperty;
        }

        public BigInteger TestPropertyDec()
        {
            TestProperty--;
            TestProperty--;
            TestProperty--;

            --TestProperty;
            --TestProperty;
            --TestProperty;
            return TestProperty;
        }

        public BigInteger TestPropertyMul()
        {
            TestProperty *= 2;
            TestProperty *= 2;
            TestProperty *= 2;
            return TestProperty;
        }

        // Test methods for uninitialized property
        public BigInteger UninitializedPropertyInc()
        {
            UninitializedProperty++;
            UninitializedProperty++;
            UninitializedProperty++;

            ++UninitializedProperty;
            ++UninitializedProperty;
            ++UninitializedProperty;
            return UninitializedProperty;
        }

        public BigInteger UninitializedPropertyDec()
        {
            UninitializedProperty--;
            UninitializedProperty--;
            UninitializedProperty--;

            --UninitializedProperty;
            --UninitializedProperty;
            --UninitializedProperty;
            return UninitializedProperty;
        }

        public BigInteger UninitializedPropertyMul()
        {
            UninitializedProperty *= 2;
            UninitializedProperty *= 2;
            UninitializedProperty *= 2;
            return UninitializedProperty;
        }

        public BigInteger UninitializedStaticPropertyInc()
        {
            UninitializedStaticProperty++;
            UninitializedStaticProperty++;
            UninitializedStaticProperty++;

            ++UninitializedProperty;
            ++UninitializedProperty;
            ++UninitializedProperty;
            return UninitializedProperty;
        }

        public BigInteger UninitializedStaticPropertyDec()
        {
            UninitializedStaticProperty--;
            UninitializedStaticProperty--;
            UninitializedStaticProperty--;

            --UninitializedStaticProperty;
            --UninitializedStaticProperty;
            --UninitializedStaticProperty;
            return UninitializedStaticProperty;
        }

        public BigInteger UninitializedStaticPropertyMul()
        {
            UninitializedStaticProperty *= 2;
            UninitializedStaticProperty *= 2;
            UninitializedStaticProperty *= 2;
            return UninitializedStaticProperty;
        }

        // Test methods for string properties
        public string ConcatPublicProperties()
        {
            return PublicProperty + PublicStaticProperty;
        }

        // Test methods for boolean properties
        public bool ToggleProtectedProperty()
        {
            ProtectedProperty = !ProtectedProperty;
            return ProtectedProperty;
        }

        // Test method for computed property
        public BigInteger GetComputedValue()
        {
            return ComputedProperty;
        }

        // Reset all properties
        public void Reset()
        {
            TestStaticProperty = 0;
            PublicStaticProperty = "Initial";
            ProtectedStaticProperty = 100;
            TestProperty = 0;
            PublicProperty = "Test";
            ProtectedProperty = false;
            PrivateGetPublicSet = 42;
            UninitializedProperty = 0;
        }
    }
}
