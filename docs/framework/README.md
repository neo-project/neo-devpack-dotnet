# Neo N3 Smart Contract Development with C#

Welcome to the comprehensive guide for developing Neo N3 smart contracts using C#. This documentation provides everything you need to get started, from basic concepts to advanced techniques, leveraging the `Neo.SmartContract.Framework` and `Neo.Compiler.CSharp`.

This guide is structured to help you build robust, efficient, and secure smart contracts on the Neo blockchain.

## Table of Contents

**I. Foundational Concepts**

*   [Introduction](./01-introduction/README.md)
    *   [What is Neo?](./01-introduction/01-what-is-neo.md)
    *   [What are Smart Contracts?](./01-introduction/02-smart-contracts.md)
    *   [Why C# for Neo?](./01-introduction/03-why-csharp.md)
*   [Getting Started](./02-getting-started/README.md)
    *   [Environment Setup](./02-getting-started/01-setup.md)
    *   [Framework & Compiler](./02-getting-started/02-installation.md)
    *   [Your First Contract](./02-getting-started/03-first-contract.md)
*   [Core Concepts](./03-core-concepts/README.md)
    *   [NeoVM & GAS](./03-core-concepts/01-neovm-gas.md)
    *   [Transactions](./03-core-concepts/02-transactions.md)
    *   [Contract Structure](./03-core-concepts/03-contract-structure.md)
    *   [Entry Points & Methods](./03-core-concepts/04-entry-points.md)
    *   [Data Types](./03-core-concepts/05-data-types.md)
    *   [Deployment Artifacts (NEF & Manifest)](./03-core-concepts/06-deployment-files.md)
    *   [Error Handling](./03-core-concepts/07-error-handling.md)

**II. The Smart Contract Framework (`Neo.SmartContract.Framework`)**

*   [Framework Features Overview](./04-framework-features/README.md)
    *   [Storage](./04-framework-features/01-storage.md)
    *   [Runtime Services](./04-framework-features/02-runtime.md)
    *   [Events](./04-framework-features/03-events.md)
    *   [Contract Interaction](./04-framework-features/04-contract-interaction.md)
    *   [Native Contracts](./04-framework-features/05-native-contracts/README.md)
        *   [LedgerContract](./04-framework-features/05-native-contracts/Ledger.md)
        *   [NeoToken (NEO)](./04-framework-features/05-native-contracts/NeoToken.md)
        *   [GasToken (GAS)](./04-framework-features/05-native-contracts/GasToken.md)
        *   [PolicyContract](./04-framework-features/05-native-contracts/Policy.md)
        *   [RoleManagement](./04-framework-features/05-native-contracts/RoleManagement.md)
        *   [OracleContract](./04-framework-features/05-native-contracts/Oracle.md)
        *   [NameService (NNS)](./04-framework-features/05-native-contracts/NameService.md)
        *   [StdLib](./04-framework-features/05-native-contracts/StdLib.md)
        *   [CryptoLib](./04-framework-features/05-native-contracts/CryptoLib.md)
        *   [ContractManagement](./04-framework-features/05-native-contracts/ContractManagement.md)
    *   [Attributes](./04-framework-features/06-attributes.md)
    *   [Helper Methods](./04-framework-features/07-helper-methods.md)

**III. The C# Compiler (`Neo.Compiler.CSharp`)**

*   [Compiler Documentation](./compiler/README.md)

**IV. Advanced Development**

*   [Advanced Topics](./06-advanced-topics/README.md)
    *   [Contract Upgrade & Migration](./06-advanced-topics/01-contract-upgrade.md)
    *   [Security Best Practices](./06-advanced-topics/02-security.md)
    *   [GAS Optimization](./06-advanced-topics/03-optimization.md)
    *   [Interop Layer](./06-advanced-topics/04-interop.md)

**V. Practical Guides**

*   [Tutorials](./07-tutorials/README.md)
    *   [Building a NEP-17 Token](./07-tutorials/01-nep17-token.md)
    *   [Creating a Voting Contract](./07-tutorials/02-voting-contract.md)
    *   [Using Oracles](./07-tutorials/03-oracle-usage.md)
*   [Testing & Deployment](./08-testing-deployment/README.md)
    *   [Unit Testing](./08-testing-deployment/01-unit-testing.md)
    *   [Testing on Neo Networks](./08-testing-deployment/02-blockchain-testing.md)
    *   [Deployment Process](./08-testing-deployment/03-deployment.md)
    *   [Interacting with Deployed Contracts](./08-testing-deployment/04-interaction.md)

**VI. Reference**

*   [Reference Materials](./09-reference/README.md)
    *   [NeoVM Opcodes](./09-reference/01-opcodes.md)
    *   [System Limits & Costs](./09-reference/02-limits.md)
    *   [NEP Standards](https://github.com/neo-project/proposals/tree/master/nep)