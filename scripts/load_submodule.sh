#!/bin/bash

# Check if the current directory is a git repository
if [ ! -d ".git" ]; then
    echo "Error: This script must be run from the root of a Git repository."
    exit 1
fi

# Initialize and update git submodules
git submodule update --init --recursive

echo "Git submodules have been updated."
