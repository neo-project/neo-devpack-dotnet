using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xdf18cb2476964c241558ed1e2e8881dcd2d50bde")]
    public class ManagementContract
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Name { get; }
        public static extern int Id { get; }
        public static extern uint ActiveBlockIndex { get; }
        public static extern Contract GetContract(UInt160 hash);
    }
}
