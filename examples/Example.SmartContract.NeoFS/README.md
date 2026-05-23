# NeoFS Oracle Smart Contract Example

This example demonstrates how a Neo N3 smart contract can read NeoFS data through the native Oracle service.

Reference:
- Neo Oracle Service: https://docs.neo.org/docs/n3/Advances/Oracles.html
- NeoFS concepts: https://docs.neo.org/docs/n3/Advances/neofs/introduction/Concepts.html

## What It Shows

- Building NeoFS Oracle URLs with the `neofs://<Container-ID>/<Object-ID>` scheme.
- Requesting a full NeoFS object payload with `Oracle.Request`.
- Requesting a byte range with the NeoFS `range` command.
- Discovering the `header` and `hash` URL forms.
- Validating that the Oracle contract is the callback caller before storing the response.

## NeoFS URL Forms

```text
neofs://<Container-ID>/<Object-ID>
neofs://<Container-ID>/<Object-ID>/range/<offset>|<length>
neofs://<Container-ID>/<Object-ID>/header
neofs://<Container-ID>/<Object-ID>/hash
```

The empty Oracle filter returns the original NeoFS response. Use a JSONPath filter only when the NeoFS object payload is JSON and the contract needs a selected value.

## Contract Flow

1. `RequestObject` calls `Oracle.Request` with the full NeoFS object URL.
2. `RequestRange` calls `Oracle.Request` with a `range` URL.
3. `OnOracleResponse` rejects direct calls unless `Runtime.CallingScriptHash` is the native Oracle contract.
4. Successful responses are stored under `NeoFSPayload`.
