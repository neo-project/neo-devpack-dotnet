# Compiler diagnostics for CI

Use `nccs Contract.cs --sarif diagnostics.sarif` to write compiler and Neo analyzer
diagnostics in [SARIF 2.1.0](https://docs.oasis-open.org/sarif/sarif/v2.1.0/os/sarif-v2.1.0-os.html)
format. Combine it with `--diagnostics` to retain the concise error-only console
mode. The report includes all compilation diagnostic severities, independently of
console filtering. Compilation exit codes remain unchanged; failure to write the
requested report returns a nonzero exit code.

Create the report's parent directory before compiling. Reports contain absolute
file URIs and one-based source ranges, honor `#line` mappings, and use UTF-16
columns. Duplicate diagnostics shared by multiple contracts are emitted once;
results and rule metadata are sorted deterministically. A successful compilation
with no diagnostics writes an empty results array. Command-line validation and
file-system errors that do not carry a source diagnostic still use console output.

The report is written once for the whole compiler invocation. The option does
not upload the report or change the repository's workflow permissions.
