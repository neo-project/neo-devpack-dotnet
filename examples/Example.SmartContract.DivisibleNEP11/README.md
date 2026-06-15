# Divisible NEP-11 Example

This sample shows a divisible NEP-11 contract shape. It exposes the divisible-specific `balanceOf(owner, tokenId)`, `ownerOf(tokenId)` iterator, and `transfer(from, to, amount, tokenId, data)` members while keeping the standard `tokens` and `tokensOf` enumeration APIs.

The sample implements the divisible NEP-11 shape directly because the framework `Nep11Token<TState>` base class models indivisible NFTs: it has a single owner per token, emits transfers with amount `1`, and exposes `ownerOf(tokenId)` as a single `UInt160` owner.

The contract stores account balances by owner and token ID, then stores compact secondary indexes for:

- token IDs owned by an account
- owners of a token ID
- all minted token IDs

Build the sample with:

```bash
dotnet build examples/Example.SmartContract.DivisibleNEP11/Example.SmartContract.DivisibleNEP11.csproj
```

Generate NEF and manifest artifacts with the repository compiler:

```bash
dotnet run --project src/Neo.Compiler.CSharp -- examples/Example.SmartContract.DivisibleNEP11/Example.SmartContract.DivisibleNEP11.csproj --output artifacts --base-name SampleDivisibleNep11Token
```
