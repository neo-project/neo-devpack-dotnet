# 5 Conformance for Neo Smart Contracts

Conformance for Neo smart contracts written in C# is of interest to the following audiences:

- Those designing, implementing, or maintaining Neo smart contract development environments.
- Blockchain projects or organizations wishing to adopt Neo smart contracts.
- Testing organizations wishing to provide a Neo smart contract conformance test suite.
- Developers wishing to port code from one Neo smart contract implementation to another.
- Educators wishing to teach Neo smart contract development using C#.
- Authors wanting to write about Neo smart contract development in C#.

As such, conformance is crucial, and this specification aims to define the characteristics that make Neo smart contract implementations and programs conforming ones.

The text in this specification that specifies requirements is considered ***normative***. All other text is ***informative***. Unless stated otherwise, all text is normative. Normative text is further categorized into ***required*** and ***conditional***. ***Conditionally normative*** text specifies an optional feature and its requirements. If provided, its syntax and semantics shall be exactly as specified.

Undefined behavior is indicated in this specification only by the words 'undefined behavior.'

A ***strictly conforming Neo smart contract*** shall use only those features of C# specified in this specification as being required and supported by the Neo platform. It shall not use any unsupported C# features or produce output dependent on any unspecified, undefined, or implementation-defined behavior.

A ***conforming implementation*** of a Neo smart contract development environment shall accept any strictly conforming Neo smart contract.

A conforming implementation shall provide and support all the types, values, objects, properties, methods, and program syntax and semantics described in the normative parts of this specification, within the constraints of the Neo platform.

A conforming implementation shall interpret characters in conformance with the Unicode Standard. Conforming implementations shall accept smart contracts encoded with the UTF-8 encoding form.

A conforming implementation shall produce at least one diagnostic message if the source program violates any rule of syntax, or any requirement (defined as a "shall" or "shall not" or "error" or "warning" requirement), unless that requirement is marked with the words "no diagnostic is required".

A conforming implementation is permitted to provide additional types, values, objects, properties, and methods beyond those described in this specification, provided they do not alter the behavior of any strictly conforming Neo smart contract and are compatible with the Neo platform.

A conforming implementation shall be accompanied by documentation that defines all implementation-defined characteristics and any extensions specific to Neo smart contract development.

A conforming implementation shall support the subset of the .NET class library that is compatible with Neo smart contracts, as documented in the Neo platform specifications.

A ***conforming Neo smart contract*** is one that is acceptable to a conforming implementation and adheres to the limitations and best practices of the Neo platform.

> Note: Neo smart contracts written in C# have certain limitations compared to standard C# programs. Developers should be aware of unsupported features such as floating-point types, multiple catch blocks, threading, unsafe code, dynamic typing, and file I/O operations. Always refer to the latest Neo documentation for the most up-to-date information on supported C# features and best practices for smart contract development.
