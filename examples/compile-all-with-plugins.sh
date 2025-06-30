#!/bin/bash
# Script to compile all example contracts with plugin generation

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
COMPILER="$SCRIPT_DIR/../src/Neo.Compiler.CSharp/bin/Debug/net9.0/nccs.dll"
EXAMPLES_DIR="$SCRIPT_DIR"

echo "Compiling all example contracts with plugin generation..."

# Find all .csproj files in example directories (excluding unit test projects)
for proj in $(find "$EXAMPLES_DIR" -name "*.csproj" -type f | grep -v ".UnitTests" | grep -v "TestContract" | sort); do
    dir=$(dirname "$proj")
    name=$(basename "$dir")
    
    echo "----------------------------------------"
    echo "Compiling: $name"
    echo "Project: $proj"
    
    cd "$dir"
    dotnet "$COMPILER" "$(basename "$proj")" --generate-plugin
    
    if [ $? -eq 0 ]; then
        echo "✓ Successfully compiled $name with plugin generation"
        
        # Check if plugin was created
        plugin_dir="bin/sc/*Plugin"
        if ls $plugin_dir 1> /dev/null 2>&1; then
            echo "✓ Plugin created at: $plugin_dir"
        fi
    else
        echo "✗ Failed to compile $name"
    fi
    
    cd - > /dev/null
done

echo "----------------------------------------"
echo "Compilation complete!"

# List all generated plugins
echo ""
echo "Generated plugins:"
find "$EXAMPLES_DIR" -type d -name "*Plugin" -path "*/bin/sc/*" | sort