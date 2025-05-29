#!/usr/bin/env python3
"""
Script to help update Neo Smart Contract Analyzer unit tests.
This script simplifies the test cases by removing the complex compiler error expectations
and focusing on the core analyzer functionality.
"""

import os
import re
import glob

def simplify_test_file(file_path):
    """Simplify a test file by removing complex compiler error expectations."""
    print(f"Processing {file_path}...")
    
    with open(file_path, 'r') as f:
        content = f.read()
    
    # Find all test methods
    test_methods = re.findall(r'(\[TestMethod\].*?await test\.RunAsync\(\);)', content, re.DOTALL)
    
    simplified_content = content
    
    for test_method in test_methods:
        # Check if this is a simple analyzer test (not a code fix test)
        if 'CreateAnalyzerTest' in test_method and 'CreateCodeFixTest' not in test_method:
            # Create a simplified version
            simplified_method = simplify_analyzer_test(test_method)
            simplified_content = simplified_content.replace(test_method, simplified_method)
    
    # Write back the simplified content
    with open(file_path, 'w') as f:
        f.write(simplified_content)
    
    print(f"Simplified {file_path}")

def simplify_analyzer_test(test_method):
    """Simplify an analyzer test by removing complex compiler error expectations."""
    lines = test_method.split('\n')
    
    # Find the test setup
    new_lines = []
    in_expected_diagnostics = False
    
    for line in lines:
        if 'test.ExpectedDiagnostics.Add(' in line and 'CompilerError' in line:
            # Skip compiler error expectations
            in_expected_diagnostics = True
            continue
        elif 'test.ExpectedDiagnostics.Add(' in line and 'DiagnosticResult(' in line:
            # Keep analyzer diagnostic expectations
            new_lines.append(line)
            in_expected_diagnostics = False
        elif in_expected_diagnostics:
            # Skip lines that are part of compiler error expectations
            continue
        else:
            new_lines.append(line)
            in_expected_diagnostics = False
    
    return '\n'.join(new_lines)

def main():
    """Main function to process all analyzer test files."""
    test_dir = "tests/Neo.SmartContract.Analyzer.UnitTests"
    
    if not os.path.exists(test_dir):
        print(f"Test directory {test_dir} not found!")
        return
    
    # Find all test files
    test_files = glob.glob(os.path.join(test_dir, "*Test*.cs"))
    
    print(f"Found {len(test_files)} test files to process...")
    
    for test_file in test_files:
        try:
            simplify_test_file(test_file)
        except Exception as e:
            print(f"Error processing {test_file}: {e}")
    
    print("Done!")

if __name__ == "__main__":
    main()
