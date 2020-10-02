namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_Struct : SmartContract.Framework.SmartContract
    {
        public static object Test1(int a, int b, int c)
        {
            State[] states = new State[] { new State() };

            State state = states[0];
            state.a = a;
            state.b = b;
            state.c = c;

            return states[0];
        }

        private struct State
        {
            public int a;
            public int b;
            public int c;
        }
    }
}
