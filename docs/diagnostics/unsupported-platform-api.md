# Unsupported platform API diagnostics

Neo smart contracts run in a deterministic virtual machine and cannot use
platform APIs that depend on the host process, operating system, or external
runtime state. These diagnostics identify platform APIs before contract code
is compiled.

| Diagnostic | Unsupported API | Recommended alternative |
| --- | --- | --- |
| <a id="nc4028"></a>NC4028 | `System.Diagnostics` namespaces and APIs | Use contract-visible state and explicit events instead of process diagnostics. |
| <a id="nc4058"></a>NC4058 | Host-dependent APIs such as `System.DateTime`, `System.Console`, and `System.IO` | Use deterministic Neo framework APIs and pass external values into the contract explicitly. |
