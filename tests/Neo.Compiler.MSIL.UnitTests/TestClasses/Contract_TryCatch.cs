namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_shift : SmartContract.Framework.SmartContract
    {
        public static object try01()
        {
            int v = 0;
            try
            {
                v = 2;
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }

        public static object try02()
        {
            int v = 0;
            try
            {
                v = 2;
                throw new System.Exception();
            }
            catch
            {
                v = 3;
            }
            finally
            {
                v++;
            }
            return v;
        }
    }
}
