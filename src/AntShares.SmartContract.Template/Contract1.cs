using AntShares.SmartContract.Framework;
using AntShares.SmartContract.Framework.Services.AntShares;
using System;
using System.Numerics;

namespace $safeprojectname$
{
	public class Contract1 : FunctionCode
{
    public static void Main()
    {
        Storage.Put(StorageContext.Current, "Hello", "World");
    }
}
}
