# Introduction

This specification is based on a subset of the standard C# language, specifically tailored for writing smart contracts on the NEO blockchain platform. The original C# language was developed within Microsoft, with Anders Hejlsberg, Scott Wiltamuth, and Peter Golde as its principal inventors. This neo C# variant, while maintaining core C# principles, has been adapted to meet the unique requirements and constraints of blockchain environments.

Neo C# is designed to provide developers with a familiar and powerful language for creating smart contracts, while ensuring compatibility with the NEO blockchain's execution environment. As such, it includes several important differences from standard C#:

- NEO C# does not support floating-point types, to ensure deterministic execution across all nodes in the blockchain network.
- File operations are not supported, as smart contracts operate in a sandboxed environment without direct access to the file system.
- Threading is not available, as smart contracts are designed to execute as single-threaded, atomic operations.
- Unsafe code and pointer operations are not supported, to maintain security and prevent direct memory manipulation.
- The `dynamic` keyword is not supported, all types must be clearly defined with a type known at compile time, to ensure security and predictability in smart contract execution.
- Certain other features of standard C# may be limited or unavailable to maintain security and predictability in the blockchain context.

The goals of neo C# include:

- Providing a simple, modern, and object-oriented programming language for smart contract development on the NEO platform.
- Maintaining strong type checking and other software engineering principles to ensure contract reliability and security.
- Offering a familiar syntax for developers with C# experience, to ease the transition into blockchain development.
- Supporting the creation of efficient and cost-effective smart contracts, optimized for execution in the NEO virtual machine.
- Enabling the development of complex decentralized applications (dApps) and blockchain-based solutions.

While NEO C# is based on the C# language, it is important for developers to be aware of its specific limitations and optimizations for blockchain use. This specification aims to provide a clear and comprehensive guide to writing smart contracts using NEO C#, highlighting both its capabilities and constraints within the NEO ecosystem.

The name C# is pronounced "C Sharp". And in this specification, when we refer to C#, we mean neo C#.

The name C# is written as the LATIN CAPITAL LETTER C (U+0043) followed by the NUMBER SIGN # (U+0023).

<!--

> Uncomment and update the date before submission to ECMA:

## COPYRIGHT NOTICE

*© 2017 Ecma International*

*This document may be copied, published and distributed to others, and certain derivative works of it may be prepared, copied, published, and distributed, in whole or in part, provided that the above copyright notice and this Copyright License and Disclaimer are included on all such copies and derivative works. The only derivative works that are permissible under this Copyright License and Disclaimer are:*

1. *works which incorporate all or portion of this document for the purpose of providing commentary or explanation (such as an annotated version of the document),*
1. *works which incorporate all or portion of this document for the purpose of incorporating features that provide accessibility,*
1. *translations of this document into languages other than English and into different formats and*
1. *works by making use of this specification in standard conformant products by implementing (e.g. by copy and paste wholly or partly) the functionality therein.*

*However, the content of this document itself may not be modified in any way, including by removing the copyright notice or references to Ecma International, except as required to translate it into languages other than English or into a different format.*

*The official version of an Ecma International document is the English language version on the Ecma International website. In the event of discrepancies between a translated version and the official version, the official version shall govern.*

*The limited permissions granted above are perpetual and will not be revoked by Ecma International or its successors or assigns.*

*This document and the information contained herein is provided on an "AS IS" basis and ECMA INTERNATIONAL DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO ANY WARRANTY THAT THE USE OF THE INFORMATION HEREIN WILL NOT INFRINGE ANY OWNERSHIP RIGHTS OR ANY IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE."*
-->
