# Your First Neo C# Smart Contract

Let's create a very simple "Hello World" smart contract to illustrate the basic structure and compilation process.

## 1. Create the Project

Follow the steps from the previous section ([Framework & Compiler Setup](./02-installation.md)) to create a new .NET class library project and add the `Neo.SmartContract.Framework` and `Neo.Compiler.CSharp` NuGet packages.

```bash
dotnet new classlib -n HelloWorldContract
cd HelloWorldContract
dotnet add package Neo.SmartContract.Framework
dotnet add package Neo.Compiler.CSharp
```

## 2. Configure the `.csproj` File

Edit the `HelloWorldContract.csproj` file to configure it for Neo compilation:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework> 
    <ImplicitUsings>disable</ImplicitUsings> <!-- Disable implicit usings for clarity -->
    <Nullable>disable</Nullable> <!-- Disable nullable for simplicity here -->

    <!-- Neo specific properties -->
    <NeoContractName>HelloWorld</NeoContractName> <!-- Base name for output files -->
    <RootNamespace>Neo.SmartContract.Examples</RootNamespace> <!-- Set a namespace -->

    <!-- Recommended properties -->
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <ProduceReferenceAssembly>false</ProduceReferenceAssembly>
    <Deterministic>false</Deterministic>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Neo.SmartContract.Framework" Version="3.*" />
    <PackageReference Include="Neo.Compiler.CSharp" Version="3.*" />
  </ItemGroup>

  <!-- Optional: Include Debug Info for Debug builds -->
  <PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
    <NeoContractDebugInfo>true</NeoContractDebugInfo>
  </PropertyGroup>

</Project>
```

*Note: We disabled `ImplicitUsings` and `Nullable` for this basic example to keep the code explicit.* Replace `3.*` with the specific latest versions if desired.

## 3. Write the Smart Contract Code

Delete the default `Class1.cs` file.

Create a new file named `HelloWorldContract.cs` with the following code:

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace Neo.SmartContract.Examples
{
    [DisplayName("HelloWorldContract")]
    [ManifestExtra("Author", "Your Name Here")]
    [ManifestExtra("Description", "A simple Hello World contract")]
    [SupportedStandards("NEP-17")] // Example: Declare standards compliance if any
    [ContractPermission("*", "*")] // Allow calling any contract/method (use cautiously!)
    public class HelloWorldContract : SmartContract
    {
        // Define a constant prefix for storage
        private static readonly StorageMap MessageMap = new StorageMap(Storage.CurrentContext, "MSG");

        // Event declaration
        public delegate void MessageChangedDelegate(string oldValue, string newValue);
        [DisplayName("MessageChanged")]
        public static event MessageChangedDelegate OnMessageChanged;

        // Method to say hello
        public static string SayHello()
        {
            return "Hello, Neo World!";
        }

        // Method to get the stored message
        public static string GetMessage()
        {
            return MessageMap.GetString("CurrentMessage") ?? "No message set."; // Use GetString for string type
        }

        // Method to set the message (requires witness verification)
        public static bool SetMessage(string newMessage)
        { 
            // Example: Basic Access Control - only contract deployer can set message
            // Replace with your actual owner address check logic
            // if (!Runtime.CheckWitness(GetOwner())) return false; 

            string oldValue = GetMessage();
            MessageMap.Put("CurrentMessage", newMessage);
            
            // Fire the event
            OnMessageChanged(oldValue, newMessage);
            Runtime.Log("Message updated"); // Log the action
            return true;
        }

        // Example: A private helper method (won't be in the manifest)
        private static ByteString GetOwner() 
        { 
             // In a real contract, this would be the deployer's address, often stored during deploy
             // return (ByteString)"NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(); // Example address
             return default; // Placeholder
        }

        // _deploy method (optional, executed only once on deployment/update)
        public static void _deploy(object data, bool update)
        {
            if (update) return; // Don't re-run on update

            // Initial setup on first deployment
            MessageMap.Put("CurrentMessage", "Initial Message");
            Runtime.Log("Contract deployed and initialized.");
        }

        // onNEP17Payment method (if supporting NEP-17 deposits)
        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
        {
            Runtime.Log($"Received {amount} NEP-17 tokens from {from}");
            // Handle received payment
        }

        // update method (optional, for contract updates)
        // public static void update(ByteString nefFile, string manifest) { ... }

        // destroy method (optional, to allow contract self-destruction)
        // public static void destroy() { ... }
    }
}
```

**Explanation:**

*   **Namespaces:** We import necessary framework namespaces.
*   **Attributes:** Decorate the class with attributes like `DisplayName`, `ManifestExtra`, `SupportedStandards`, `ContractPermission` which populate the contract's manifest file.
*   **Inheritance:** The contract class inherits from `SmartContract` (optional but common).
*   **`StorageMap`:** Used for easy access to the contract's persistent storage.
*   **Events:** Declared using delegates and the `event` keyword. Marked with `DisplayName` for the manifest.
*   **Methods:** Public static methods (`SayHello`, `GetMessage`, `SetMessage`) become callable contract methods.
*   **`Runtime.Log`:** Used to emit log messages during execution (visible in some explorers/tools).
*   **`_deploy`:** A special method executed automatically only when the contract is first deployed or updated.
*   **`OnNEP17Payment`:** A special method automatically called if the contract receives NEP-17 tokens.

## 4. Compile the Contract

Open your terminal in the `HelloWorldContract` directory and run the build command:

```bash
dotnet build
```

If successful, this will:

1.  Compile the C# code into a standard .NET DLL.
2.  Invoke the `Neo.Compiler.CSharp` (`nccs`).
3.  Generate the following files in the `bin/Debug/net6.0/` (or similar) directory:
    *   `HelloWorld.nef`: The NeoVM bytecode.
    *   `HelloWorld.manifest.json`: The contract manifest describing methods, events, etc.
    *   `HelloWorld.nefdbgnfo`: Debug information (if compiled in Debug configuration with `<NeoContractDebugInfo>true</NeoContractDebugInfo>`).

Congratulations! You have successfully created and compiled your first Neo N3 smart contract using C#.

[Previous: Framework & Compiler](./02-installation.md) | [Next Section: Core Concepts](../03-core-concepts/README.md)