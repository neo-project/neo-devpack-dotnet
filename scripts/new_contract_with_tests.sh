#!/usr/bin/env bash

# Helper script for scaffolding a smart contract together with its MSTest project.
# Usage:
#   ./scripts/new_contract_with_tests.sh MyContract [Template]
#   ./scripts/new_contract_with_tests.sh MyContract [Template] [Feature...]
#   Template defaults to "Basic". Available templates: Basic, NEP17, NEP11, Ownable, Oracle.
#   Features can be provided as additional arguments (e.g. "NEP17 Ownable Oracle") or comma separated values.

set -euo pipefail

if [[ $# -lt 1 ]]; then
  echo "Usage: $0 <ContractName> [Template] [Feature ...]" >&2
  exit 1
fi

contract_name="$1"
shift

template="Basic"
features=()

if [[ $# -gt 0 ]]; then
  template="$1"
  shift

  # If the template argument looks like inline features, treat it as such
  if [[ "$template" == "--features" ]]; then
    template="Basic"
  elif [[ "$template" == -* ]]; then
    echo "Unexpected option '$template'. Pass features without flags." >&2
    exit 1
  elif [[ "$template" == *","* || "$template" == *"+"* ]]; then
    features+=("$template")
    template="Basic"
  fi
fi

if [[ $# -gt 0 ]]; then
  features+=("$@")
fi

feature_args=()
if [[ ${#features[@]} -gt 0 ]]; then
  for feature in "${features[@]}"; do
    if [[ -n "$feature" ]]; then
      feature_args+=(--features "$feature")
    fi
  done
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/.." && pwd)"

if [[ ${#feature_args[@]} -gt 0 ]]; then
  echo "Scaffolding contract '${contract_name}' with template '${template}' and features '${features[*]}' (tests included)..."
else
  echo "Scaffolding contract '${contract_name}' with template '${template}' (tests included)..."
fi
dotnet run --project "${repo_root}/src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj" -- \
  new "${contract_name}" --template "${template}" "${feature_args[@]}" --with-tests

cat <<EOF

Next steps (from the scaffold output):
  cd ${contract_name}
  dotnet tool restore
  dotnet build
  dotnet tool run nccs ${contract_name}.csproj

  cd ../${contract_name}.UnitTests
  dotnet test

EOF
