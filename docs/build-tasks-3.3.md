# Upgrading Contact Projects to Neo.BuildTasks v3.3 

This document describes how to update your C# contract project files - and any associated contract 
test project files - for Neo v3.3. Neo.BuildTools v3.3 adds new MSBuild tasks for running NCCS
(aka the Neo C# compiler) and Neo Express `batch` command during the build process. Prior to v3.3,
contract .csproj files needed to include custom targets to run these tasks.

For example, this is the latest (as of this writing) version of the 
[Registrar contract sample project file](https://github.com/ngdenterprise/neo-registrar-sample/blob/92e1a9943989827ead01101dd563490b1d8eb7f5/src/registrar.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <NeoContractName>$(AssemblyName)</NeoContractName>
    <NeoExpressBatchFile>..\express.batch</NeoExpressBatchFile>
    <Nullable>enable</Nullable>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Neo.SmartContract.Framework" Version="3.3.0" />
    <PackageReference Include="Neo.BuildTasks" Version="3.3.23" PrivateAssets="all" />
  </ItemGroup>

</Project>
```

This was the Neo 3.1 version the [Registrar contract sample project file](https://github.com/ngdenterprise/neo-registrar-sample/blob/668a049707565144334de5f6a622675bf3ac4d93/src/registrar.csproj). 

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <NeoContractName>registrar</NeoContractName>
    <NeoExpressBatchFile>..\express.batch</NeoExpressBatchFile>
    <Nullable>enable</Nullable>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Neo.SmartContract.Framework" Version="3.1.0" />
  </ItemGroup>

  <Target Name="ConfigureContractOutputProperties" BeforeTargets="ExecuteNccs">
    <PropertyGroup>
      <_NccsOutputDirectory>bin\sc</_NccsOutputDirectory>
      <_OutputNeoContractPath>$([MSBuild]::NormalizePath('$(MSBuildProjectDirectory)', '$(_NccsOutputDirectory)', '$(NeoContractName).nef'))</_OutputNeoContractPath>
      <_OutputNeoContractManifestPath>$([MSBuild]::NormalizePath('$(MSBuildProjectDirectory)', '$(_NccsOutputDirectory)', '$(NeoContractName).manifest.json'))</_OutputNeoContractManifestPath>
      <_OutputNeoContractDebugInfoPath>$([MSBuild]::NormalizePath('$(MSBuildProjectDirectory)', '$(_NccsOutputDirectory)', '$(NeoContractName).nefdbgnfo'))</_OutputNeoContractDebugInfoPath>
    </PropertyGroup>
  </Target>

  <Target Name="ConfigureNeoExpressBatchProperties" BeforeTargets="ExecuteNeoExpressBatch">
    <PropertyGroup>
      <_NeoExpressBatchPath>$([MSBuild]::NormalizePath('$(MSBuildProjectDirectory)', '$(NeoExpressBatchFile)'))</_NeoExpressBatchPath>
    </PropertyGroup>
  </Target>

  <Target Name="ExecuteNccs" AfterTargets="Compile" 
          Inputs="$(MSBuildProjectFullPath);@(Compile);" 
          Outputs="$(_OutputNeoContractPath);$(_OutputNeoContractManifestPath);$(_OutputNeoContractDebugInfoPath)">
    <PropertyGroup>
      <_NccsOptimizeArgument Condition="'$(Configuration)'=='Debug'">--no-optimize</_NccsOptimizeArgument>
      <_NccsOptimizeArgument Condition="'$(Configuration)'!='Debug'"></_NccsOptimizeArgument>
    </PropertyGroup>

    <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet nccs --debug $(_NccsOptimizeArgument) &quot;$(MSBuildProjectFullPath)&quot; --contract-name $(NeoContractName)" />
  </Target>

  <Target Name="ExecuteNeoExpressBatch" AfterTargets="Compile" 
          Inputs="$(_OutputNeoContractPath);$(_NeoExpressBatchPath)" 
          Outputs="$(IntermediateOutputPath)neoxp">
    <PropertyGroup>
      <_NeoExpressBatchDir>$([System.IO.Path]::GetDirectoryName('$(_NeoExpressBatchPath)'))</_NeoExpressBatchDir>
    </PropertyGroup>

    <Exec Command="dotnet neoxp batch ./express.batch --reset" WorkingDirectory="$(_NeoExpressBatchDir)" />
    <Touch Files="$(IntermediateOutputPath)neoxp" AlwaysCreate="true" />
  </Target>

</Project>
```

As you can see, we have simplified the project file significantly in the v3.3 release. 

## Contract Project File

As you can see in the sample project files above, the top level properties that control Neo contract compilation
have not changed. Upgrading your contract project file for v3.3 should simply require:

* Adding the Neo.BuildTasks `PackageReference` element. Note the `PrivateAssets="all"` attribute.
* Removing the extraneous `Target` elements. Equivalent targets are provided by Neo.BuildTasks

### NeoContractName

The `NeoContractName` property specifies the base name of the compiled .NEF and related files. Setting this property
triggers NCCS to run on the project. By setting this property value to `$(Assembly)`, the compiled contract file will have the same base name as the compiled C# assembly.

> Note: This property does not affect the contract name as it appears in the manifest. The manifest name
> can be specified by a `[DisplayName]` attribute on the contract class

`NeoContractName` is the only property that must be set to trigger NCCS compilation. The following properties
can also be set to provide finer control over the NCCS compilation process.

* `NeoContractOutput` property controls the output directory for the compiled contract. Defaults to `bin\sc`.
* `NeoContractAssembly` property controls the generation of a Neo.VM assembly file. Defaults to false.
* For Debug configuration builds, the NCCS `--debug` option is set and the `--no-optimize` / `--no-inline` options are
  omitted. For Release configuration builds, the options are reversed (`--no-optimize` / `--no-inline` set, `--debug` omitted)

### NeoExpressBatchFile

The `NeoExpressBatchFile` property specifies the path to a NeoExpress batch file (relative to the project file).
Setting this property triggers `neoxp batch` to run after compilation is complete.

> Note, MSBuild will not re-run `neoxp batch` if the NCCS output and NeoExpress batch files have not changed.
> If you need to trigger a build due to other file changes, you may need to run `dotnet clean` before running
> `dotnet build`.

Other properties that control the `neoxp batch` command execution:

* `NeoExpressBatchInputFile` property specifies the path to the `.neo-express` file (relative to the batch file,
  not relative to the project file). Unspecified by default (NeoExpress defaults to `default.neo-express`)
* `NeoExpressBatchReset` property specifies if the `--reset` option is specified. Defaults to true.
* `NeoExpressBatchCheckpoint` property specifies if a checkpoint value is added to `--reset` option. Must
  be specified in conjunction with `NeoExpressBatchReset`. Unspecified by default.
* `NeoExpressBatchTrace` property specifies if `--trace` option is specified. Defaults to false.

## Contract Test Project File

Project files for contract test projects have also been simplified. The v3.1 version of Neo.BuildTasks
included the `NeoContractInterface` task, so most test projects should already have a `PackageReference`
for Neo.BuildTasks. However, v3.3 of Neo.BuildTasks also includes Targets needed to run the 
`NeoContractInterface` task. So when upgrading your test project to v3.3, the extraneous `Target` elements
should be removed from the project file.

> Note, if the `RootNamespace` property is specified in a contract test project, that namespace will be
> used for the generated contract interface.

### NeoContractReference

To trigger the generation of a contract interface, v3.3 adds a new `NeoContractReference` item. Specifying
this item and including the path to the contract project will cause the contract interface file to be generated
and will ensure projects build in the correct order. 

```xml
<ItemGroup>
  <NeoContractReference Include="..\src\registrar.csproj"/>
</ItemGroup>
```

In Neo 3.1 test projects, it was common to have a `ProjectReference` to the contract project with
`ReferenceOutputAssembly` attribute set to false. This was only used to ensure correct project build
order. Specifying a `NeoContractReference` item subsumes the need to specify a `ProjectReference`
item, so any contract `ProjectReference` items should be replaced with `NeoContractReference` items.

### NeoContractGeneration

Neo.BuildTasks introduces the `NeoContractGeneration` item. Like `NeoContractReference`, `NeoContractGeneration`
items trigger the generation of contract interface files. A `NeoContractGeneration` item allows the developer to specify
the path to an existing Neo contract `.manifest.json` file. This is useful for scenarios where tests are invoking
a contract that is not built a part of the current project.

When specifying a `NeoContractGeneration` item, the `Include` value should be a simple name that will be used in the
name of the generation contract interface file. The path to the existing manifest is specified using a `ManifestPath`
sub element.

```xml
<ItemGroup>
  <NeoContractGeneration Include="tokenContract">
    <ManifestPath>../contracts/tokenContract.manifest.json</ManifestPath>
  </NeoContractGeneration>
</ItemGroup>
```

### ContractNameOverride

Sometimes, the contract name as specified in the `[DisplayName]` attribute is not a legal C# interface name.
When this is the case, Neo.BuildTasks v3.3 will raise an error at code generation time. v3.1 raised the error
at compilation time which with an error message that was harder to troubleshoot.

Both `NeoContractReference` and `NeoContractGeneration` support an optional `ContractNameOverride` attribute.
If specified, the `NeoContractInterface` task will use the specified value as the contract interface name,
regardless of what is listed in the manifest file.

> Note, `ContractNameOverride` must be a legal C# type name

```xml
<ItemGroup>
  <NeoContractReference Include="..\src\registrar.csproj" ContractNameOverride="Registrar"/>

  <NeoContractGeneration Include="tokenContract" ContractNameOverride="SampleToken">
    <ManifestPath>../contracts/tokenContract.manifest.json</ManifestPath>
  </NeoContractGeneration>
</ItemGroup>
```
