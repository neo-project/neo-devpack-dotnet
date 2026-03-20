# Neo N3 HF_Faun Features Example

## Overview
This example demonstrates Neo v3.9 (HF_Faun) native additions, including hex encoding helpers, and treasury signature checks.

## Key Features
- `StdLib.HexEncode` and `StdLib.HexDecode` for hex conversions
- `Treasury.Verify` for committee signature verification

## Usage
The `SampleFaunFeatures` contract exposes simple read-only methods that can be called by clients to:
- Encode and decode hex strings
- Check if the current transaction is committee-signed

## Notes
This contract is intended as a lightweight reference for new native APIs introduced in Neo v3.9.
