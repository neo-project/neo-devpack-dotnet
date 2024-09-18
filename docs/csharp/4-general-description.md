# 4 General description

**This text is informative.**

This specification is intended to be used by implementers, academics, and smart contract developers. As such, it contains a considerable amount of explanatory material that, strictly speaking, is not necessary in a formal language specification for Neo smart contracts.

This specification is divided into the following subdivisions: front matter; language syntax, constraints, and semantics specific to Neo smart contracts; and annexes.

Examples are provided to illustrate possible forms of the constructions described, with a focus on Neo smart contract patterns. References are used to refer to related clauses. Notes are provided to give advice or guidance to implementers or smart contract developers, including Neo-specific best practices. Annexes provide additional information and summarize the information contained in this specification.

**End of informative text.**

Informative text is indicated in the following ways:

1. Whole or partial clauses or annexes delimited by "**This clause/text is informative**" and "**End of informative text**".
1. *Example*: The following example … code fragment, possibly with some narrative … *end example*  The *Example:* and *end example* markers are in the same paragraph for single paragraph examples. If an example spans multiple paragraphs, the end example marker should be its own paragraph.
1. *Note*: narrative … *end note*  The *Note*: and *end note* markers are in the same paragraph for single paragraph notes. If a note spans multiple paragraphs, the *end note* marker should be its own paragraph.

All text not marked as being informative is normative.

**This text is informative.**

It's important to note that this specification describes a subset of C# specifically tailored for Neo smart contract development. As such, there are several key differences from standard C# programming:

1. Smart contracts in Neo are implemented as public methods in classes inheriting from SmartContract, rather than using a Main() method.
2. Users interact with smart contracts by building and submitting transactions to the Neo blockchain, not by running an executable.
3. Certain C# features are unsupported in Neo smart contracts due to the blockchain environment constraints. These include:
   - float, double, and decimal types
   - Multiple catch blocks in try-catch statements
   - Threading and parallel processing
   - unsafe keyword and unsafe contexts
   - dynamic keyword
   - File I/O operations

4. Neo-specific concepts such as storage, GAS costs, and inter-contract interactions are crucial for effective smart contract development.

Throughout this specification, we will highlight these differences and provide guidance on Neo-specific best practices and alternatives to unsupported features.

**End of informative text.**
