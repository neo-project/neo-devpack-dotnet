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

        public static object throwcall()
        {
            throw new System.Exception();
        }

        public static object try03()
        {
            int v = 0;
            try
            {
                v = 2;
                throwcall();
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

        public static object tryNest()
        {
            int v = 0;
            try
            {
                try
                {
                    v = 2;
                    throwcall();
                }
                catch
                {
                    v = 3;
                    throwcall();
                }
                finally
                {
                    throwcall();
                    v++;
                }
            }
            catch
            {
                v++;
            }
            return v;
        }

        public static object tryFinally()
        {
            int v = 0;
            try
            {
                throwcall();
                v = 2;
            }
            finally
            {
                v++;
            }
            return v;
        }
    }
}
