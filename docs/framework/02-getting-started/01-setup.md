# Environment Setup

Before you start writing Neo smart contracts in C#, you need to set up your development environment. The primary requirement is the .NET SDK.

## 1. Install .NET SDK

Neo N3 smart contract development with C# requires the .NET SDK. The specific version compatibility might evolve, but generally, recent LTS (Long-Term Support) versions are recommended.

*   **Check for Existing Installation:** Open your terminal or command prompt and run:
    ```bash
    dotnet --version
    ```
    If this command outputs a version number (e.g., 8.x.x, 9.x.x, or later), you likely have it installed.
*   **Download and Install:** If you don't have it, download the appropriate .NET SDK installer for your operating system from the official Microsoft website:
    [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
    Follow the installation instructions for your OS.
*   **Verify Installation:** After installation, close and reopen your terminal and run `dotnet --version` again to confirm.

## 2. Choose a Code Editor/IDE

You can write C# code in any text editor, but using an IDE with C# support significantly improves the development experience.

*   **Visual Studio Code (Recommended):** A free, lightweight, and powerful editor with excellent C# support via extensions.
    *   Install the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension from Microsoft for the best experience.
*   **Visual Studio (Windows/Mac):** A full-featured IDE offering comprehensive debugging, testing, and development tools for .NET.
*   **JetBrains Rider (Cross-Platform):** Another powerful, paid IDE for .NET development.

## 3. (Optional) Install Neo Blockchain Toolkit

While not strictly required for *writing* C# code, the [Neo Blockchain Toolkit for .NET](https://github.com/neo-project/neo-blockchain-toolkit) provides essential tools for debugging, testing, and deploying contracts. It includes:

*   **Neo Express:** A private Neo blockchain instance for local development and testing.
*   **Neo Contract Debugger:** Enables stepping through your C# code as it executes on Neo Express.

Installation is typically done via the .NET tool command:

```bash
dotnet tool install Neo.Express -g
```

Refer to the toolkit's documentation for detailed setup instructions.

With the .NET SDK installed and an editor chosen, you're ready to proceed with setting up your first Neo C# smart contract project.

[Previous: Getting Started](./README.md) | [Next: Framework & Compiler](./02-installation.md)