// Copyright (C) 2015-2026 The Neo Project.
//
// Oracle.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xfe924b7cfe89ddd271abaf7210a80a7e11178758")]
    public class Oracle
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public const uint MinimumResponseFee = 0_10000000;

        /// <summary>
        /// Gets the price in the unit of datoshi, 1 datoshi = 1e-8 GAS for the Oracle request.
        /// </summary>
        public static extern long GetPrice();

        /// <summary>
        /// Requests an Oracle response from the specified URL.
        /// The `gasForResponse` is in the unit of datoshi(1 datoshi = 1e-8 GAS),
        /// and must greater than the price of the Oracle request(see `GetPrice`).
        /// <para>
        /// The execution will fail if:
        ///  1. The 'url' is null or length exceeds MaxUrlLength(the default value is 256 bytes).
        ///  2. The 'filter' is null or length exceeds MaxFilterLength(the default value is 128 bytes).
        ///  3. The 'callback' is null or length exceeds MaxCallbackLength(the default value is 32 bytes) or starts with '_'.
        ///  4. The 'userData' is null or length exceeds MaxUserDataLength(the default value is 512 bytes).
        ///  5. The 'gasForResponse' is less than 0.1 GAS.
        ///  6. Too many pending responses for the specified URL.
        ///  7. The calling source(CallingScriptHash) is not a contract.
        /// </para>
        /// </summary>
        /// <param name="url">The URL to request an Oracle response from.</param>
        /// <param name="filter">The filter(json path expression) to apply to the response.</param>
        /// <param name="callback">The method name of calling contract to call when the response is received.</param>
        /// <param name="userData">The user data to pass to the callback.</param>
        /// <param name="gasForResponse">The amount of GAS(in the unit of datoshi) for the response.</param>
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
