# Release Guide for R3E DevPack v0.0.1

## Overview

This guide explains how to trigger the release workflow for R3E DevPack v0.0.1.

## Current Release Workflow

The repository has an automated release workflow in `.github/workflows/main.yml` that:

1. **Test Job**: Runs tests and code coverage
2. **PublishPackage Job**: Publishes CI builds to GitHub Packages and MyGet (only on master branch)
3. **Release Job**: Creates GitHub releases and publishes to NuGet (only on master branch)

## How to Trigger a Release

### Method 1: Automatic Release (Recommended)

The release workflow automatically triggers when:
1. Code is pushed to the `master` branch
2. The version in `src/Directory.Build.props` doesn't have a corresponding GitHub release tag

Since we've updated the version to `0.0.1`, pushing to master will automatically:
- Create a GitHub release tagged `v0.0.1`
- Publish all R3E packages to NuGet

**Steps:**
```bash
# Ensure all changes are committed
git add .
git commit -m "Release v0.0.1 - R3E Edition"

# Push to master (this triggers the release)
git push origin master
```

### Method 2: Manual Tag Release

If you prefer to control the release timing:

```bash
# Create and push a tag
git tag -a v0.0.1 -m "Release v0.0.1 - R3E Edition"
git push origin v0.0.1
```

Then create a GitHub release manually:
1. Go to GitHub repository → Releases → Create a new release
2. Choose the `v0.0.1` tag
3. Title: "v0.0.1 - R3E Edition"
4. Copy content from `RELEASE_NOTES_v0.0.1.md`
5. Publish release

### Method 3: Manual Package Publishing

To manually build and publish packages:

```bash
# Build in Release mode
dotnet build -c Release

# Pack all projects
dotnet pack -c Release -o ./publish

# Publish to NuGet (requires API key)
dotnet nuget push "./publish/R3E.*.nupkg" \
  -s https://api.nuget.org/v3/index.json \
  -k YOUR_NUGET_API_KEY \
  --skip-duplicate
```

## Important Notes

### Update the Workflow (Optional)

The current workflow still references `nccs`. To update it for future releases:

1. Edit `.github/workflows/main.yml`
2. Replace lines 54-56 with:
```yaml
dotnet ./src/Neo.Compiler.CSharp/bin/Debug/net9.0/rncc.dll ./src/Neo.SmartContract.Template/bin/Debug/nep17/Nep17Contract.csproj -o ./tests/Neo.SmartContract.Template.UnitTests/templates/neocontractnep17/Artifacts/
dotnet ./src/Neo.Compiler.CSharp/bin/Debug/net9.0/rncc.dll ./src/Neo.SmartContract.Template/bin/Debug/ownable/Ownable.csproj -o ./tests/Neo.SmartContract.Template.UnitTests/templates/neocontractowner/Artifacts/
dotnet ./src/Neo.Compiler.CSharp/bin/Debug/net9.0/rncc.dll ./src/Neo.SmartContract.Template/bin/Debug/oracle/OracleRequest.csproj -o ./tests/Neo.SmartContract.Template.UnitTests/templates/neocontractoracle/Artifacts/
```

### Package Names

All packages will be published with the R3E prefix:
- `R3E.SmartContract.Framework`
- `R3E.Compiler.CSharp`
- `R3E.Compiler.CSharp.Tool`
- `R3E.SmartContract.Testing`
- `R3E.SmartContract.Analyzer`
- `R3E.SmartContract.Template`
- `R3E.Disassembler.CSharp`
- `R3E.SmartContract.Deploy`

### Pre-Release Checklist

Before triggering the release:

- [ ] All tests pass locally
- [ ] Version is set to `0.0.1` in all Directory.Build.props files
- [ ] Package names are updated to R3E.*
- [ ] CHANGELOG.md is updated
- [ ] RELEASE_NOTES_v0.0.1.md is complete
- [ ] README.md reflects R3E branding

## Post-Release Steps

After the release is published:

1. Verify packages on NuGet.org
2. Test package installation:
   ```bash
   dotnet tool install -g R3E.Compiler.CSharp.Tool --version 0.0.1
   ```
3. Update any documentation with package links
4. Announce the release on appropriate channels

## Troubleshooting

If the automatic release doesn't trigger:
1. Check GitHub Actions tab for workflow runs
2. Ensure you have push permissions to master
3. Verify the version doesn't already have a release tag
4. Check workflow logs for any errors

For manual publishing issues:
1. Ensure you have a valid NuGet API key
2. Check package names match R3E.* pattern
3. Verify version numbers are consistent