// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler
{
    static class DiagnosticId
    {
        // No SmartContract is found in the sources.
        public const string NoEntryPoint = "NC1001";

        // Use of external methods not supported or allowed in the smart contract.
        public const string ExternMethod = "NC1002";

        // Smart contract class lacks a parameterless constructor, which might be required.
        public const string NoParameterlessConstructor = "NC1003";

        // Improper or conflicting use of multiple contracts (interfaces, abstract classes) in the smart contract.
        public const string MultiplyContracts = "NC1004";

        // Syntax in the smart contract code not supported by the compiler or the tool.
        public const string SyntaxNotSupported = "NC2001";

        // Event handlers in the smart contract returning values, typically not allowed.
        public const string EventReturns = "NC2002";

        // Using non-static delegates in the smart contract where static ones are required.
        public const string NonStaticDelegate = "NC2003";

        // Using floating point numbers in the smart contract, unsupported or inappropriate in this context.
        public const string FloatingPointNumber = "NC2004";

        // Multiple 'throw' statements in the smart contract, indicating complex or unclean exception handling.
        public const string MultiplyThrows = "NC2005";

        // Multiple 'catch' blocks in the smart contract, suggesting overcomplicated error handling.
        public const string MultiplyCatches = "NC2006";

        // Use of catch filters in the smart contract, potentially unsupported or not recommended.
        public const string CatchFilter = "NC2007";

        // Using multidimensional arrays in the smart contract, potentially unsupported or not recommended.
        public const string MultidimensionalArray = "NC2008";

        // Issues related to interface method calls in the smart contract, indicating incorrect or unsupported usage.
        public const string InterfaceCall = "NC2009";

        // Problems with array ranges in the smart contract, such as out-of-bounds errors.
        public const string ArrayRange = "NC2010";

        // Invalid usage of 'ToString' method or incorrect type handling in smart contract 'ToString' implementations.
        public const string InvalidToStringType = "NC2011";

        // Alignment clauses in the smart contract indicating syntax errors or misuse.
        public const string AlignmentClause = "NC2012";

        // Issues with format clauses in the smart contract, syntactically incorrect or misused.
        public const string FormatClause = "NC2013";

        // Invalid initial value types in the smart contract, indicating type mismatches or inappropriate value assignments.
        public const string InvalidInitialValueType = "NC3001";

        // Using invalid method names in the smart contract, conflicting with reserved names or naming conventions.
        public const string InvalidMethodName = "NC3002";

        // Conflicts between method names in the smart contract, issues with overloading or naming.
        public const string MethodNameConflict = "NC3003";

        // Conflicts in event names in the smart contract
        public const string EventNameConflict = "NC3004";

    }
}
