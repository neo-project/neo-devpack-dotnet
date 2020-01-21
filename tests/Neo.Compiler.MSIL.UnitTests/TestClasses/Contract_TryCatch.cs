namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_shift : SmartContract.Framework.SmartContract
    {
        public static object Main(string method, object[] args)
        {
            int v = 0;
            try
            {
                v = 2;
                return v;
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
