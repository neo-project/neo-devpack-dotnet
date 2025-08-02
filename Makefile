# Neo Smart Contract Compiler (nccs) Makefile
# This Makefile provides convenient commands for building, testing, and using the Neo C# compiler

# Variables
DOTNET = dotnet
NCCS_PROJECT = src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj
NCCS_BINARY = src/Neo.Compiler.CSharp/bin/Release/net9.0/nccs
DOCKER_IMAGE = neo-devpack-dotnet
DOCKER_TAG = latest
INSTALL_DIR = /usr/local/bin

# Platform detection
UNAME_S := $(shell uname -s)
UNAME_M := $(shell uname -m)

ifeq ($(UNAME_S),Linux)
    PLATFORM = linux
    ifeq ($(UNAME_M),x86_64)
        RID = linux-x64
    else ifeq ($(UNAME_M),aarch64)
        RID = linux-arm64
    else
        RID = linux-x64
    endif
else ifeq ($(UNAME_S),Darwin)
    PLATFORM = osx
    ifeq ($(UNAME_M),arm64)
        RID = osx-arm64
    else
        RID = osx-x64
    endif
else ifeq ($(OS),Windows_NT)
    PLATFORM = win
    RID = win-x64
    NCCS_BINARY = src/Neo.Compiler.CSharp/bin/Release/net9.0/nccs.exe
endif

# Default target
.PHONY: all
all: build

# Help command
.PHONY: help
help:
	@echo "Neo Smart Contract Compiler (nccs) Makefile"
	@echo ""
	@echo "Available commands:"
	@echo "  make build          - Build the nccs compiler"
	@echo "  make test           - Run all tests"
	@echo "  make install        - Install nccs to system (requires sudo)"
	@echo "  make uninstall      - Uninstall nccs from system (requires sudo)"
	@echo "  make clean          - Clean build artifacts"
	@echo "  make docker-build   - Build Docker image"
	@echo "  make docker-run     - Run nccs in Docker container"
	@echo "  make compile FILE=<path>  - Compile a contract file/project/solution"
	@echo "  make compile-all    - Compile all contracts in examples directory"
	@echo "  make format         - Format code using dotnet format"
	@echo "  make lint           - Run code analysis"
	@echo "  make package        - Create NuGet packages"
	@echo "  make publish        - Publish self-contained executable"
	@echo "  make version        - Show nccs version"

# Build the compiler
.PHONY: build
build:
	@echo "Building Neo C# Compiler..."
	@$(DOTNET) build $(NCCS_PROJECT) -c Release
	@echo "Build completed successfully!"

# Run tests
.PHONY: test
test:
	@echo "Running tests..."
	@$(DOTNET) test ./neo-devpack-dotnet.sln --no-build -c Release
	@echo "All tests passed!"

# Install nccs to system
.PHONY: install
install: build
	@echo "Installing nccs to $(INSTALL_DIR)..."
	@if [ "$(PLATFORM)" = "win" ]; then \
		echo "Please run as Administrator:"; \
		echo "copy $(NCCS_BINARY) $(INSTALL_DIR)"; \
	else \
		sudo cp $(NCCS_BINARY) $(INSTALL_DIR)/nccs; \
		sudo chmod +x $(INSTALL_DIR)/nccs; \
		echo "nccs installed successfully!"; \
		echo "You can now use 'nccs' command from anywhere."; \
	fi

# Uninstall nccs from system
.PHONY: uninstall
uninstall:
	@echo "Uninstalling nccs from $(INSTALL_DIR)..."
	@if [ "$(PLATFORM)" = "win" ]; then \
		echo "Please run as Administrator:"; \
		echo "del $(INSTALL_DIR)\\nccs.exe"; \
	else \
		sudo rm -f $(INSTALL_DIR)/nccs; \
		echo "nccs uninstalled successfully!"; \
	fi

# Clean build artifacts
.PHONY: clean
clean:
	@echo "Cleaning build artifacts..."
	@$(DOTNET) clean ./neo-devpack-dotnet.sln -c Release
	@rm -rf src/*/bin src/*/obj
	@rm -rf tests/*/bin tests/*/obj
	@echo "Clean completed!"

# Build Docker image
.PHONY: docker-build
docker-build:
	@echo "Building Docker image..."
	@docker build -t $(DOCKER_IMAGE):$(DOCKER_TAG) -f Dockerfile .
	@echo "Docker image built successfully!"

# Run Docker container
.PHONY: docker-run
docker-run:
	@echo "Running nccs in Docker container..."
	@docker run --rm -it -v $(PWD):/workspace $(DOCKER_IMAGE):$(DOCKER_TAG) /bin/bash

# Compile contract (usage: make compile FILE=path/to/contract.cs)
.PHONY: compile
compile: build
ifndef FILE
	@echo "Error: Please specify a file to compile"
	@echo "Usage: make compile FILE=path/to/contract.cs"
	@exit 1
else
	@echo "Compiling $(FILE)..."
	@$(DOTNET) run --project $(NCCS_PROJECT) -c Release -- $(FILE) -o ./out
	@echo "Compilation completed! Output in ./out directory"
endif

# Compile all example contracts
.PHONY: compile-all
compile-all: build
	@echo "Compiling all example contracts..."
	@for template in nep17 oracle owner; do \
		echo "Compiling $$template template..."; \
		$(DOTNET) new neocontract$$template -n Test$$template -o ./temp/$$template --force; \
		$(DOTNET) run --project $(NCCS_PROJECT) -c Release -- ./temp/$$template/Test$$template.csproj -o ./out/$$template; \
	done
	@rm -rf ./temp
	@echo "All example contracts compiled!"

# Format code
.PHONY: format
format:
	@echo "Formatting code..."
	@$(DOTNET) format ./neo-devpack-dotnet.sln
	@echo "Code formatting completed!"

# Run code analysis
.PHONY: lint
lint:
	@echo "Running code analysis..."
	@$(DOTNET) format ./neo-devpack-dotnet.sln --verify-no-changes --verbosity diagnostic
	@echo "Code analysis completed!"

# Create NuGet packages
.PHONY: package
package: build
	@echo "Creating NuGet packages..."
	@$(DOTNET) pack ./neo-devpack-dotnet.sln -c Release -o ./packages
	@echo "NuGet packages created in ./packages directory!"

# Publish self-contained executable
.PHONY: publish
publish:
	@echo "Publishing self-contained executable for $(RID)..."
	@$(DOTNET) publish $(NCCS_PROJECT) -c Release -r $(RID) --self-contained true \
		/p:PublishSingleFile=true /p:PublishReadyToRun=true -o ./publish/$(RID)
	@echo "Published to ./publish/$(RID)/"

# Publish for all platforms
.PHONY: publish-all
publish-all:
	@echo "Publishing for all platforms..."
	@for rid in win-x64 linux-x64 osx-x64 osx-arm64; do \
		echo "Publishing for $$rid..."; \
		$(DOTNET) publish $(NCCS_PROJECT) -c Release -r $$rid --self-contained true \
			/p:PublishSingleFile=true /p:PublishReadyToRun=true -o ./publish/$$rid; \
	done
	@echo "Published all platforms to ./publish/"

# Show version
.PHONY: version
version: build
	@$(DOTNET) run --project $(NCCS_PROJECT) -c Release -- --version

# Development helpers
.PHONY: watch
watch:
	@echo "Watching for changes..."
	@$(DOTNET) watch --project $(NCCS_PROJECT) run

# Quick compile helper for current directory
.PHONY: compile-here
compile-here: build
	@echo "Compiling all .cs files in current directory..."
	@for file in *.cs; do \
		if [ -f "$$file" ]; then \
			echo "Compiling $$file..."; \
			$(DOTNET) run --project $(NCCS_PROJECT) -c Release -- "$$file" -o ./out; \
		fi; \
	done

# Update dependencies
.PHONY: update
update:
	@echo "Updating dependencies..."
	@$(DOTNET) restore ./neo-devpack-dotnet.sln
	@git submodule update --init --recursive
	@echo "Dependencies updated!"

# Run specific test
.PHONY: test-unit
test-unit:
	@echo "Running unit tests..."
	@$(DOTNET) test ./tests/Neo.Compiler.CSharp.UnitTests -c Release

.PHONY: test-framework
test-framework:
	@echo "Running framework tests..."
	@$(DOTNET) test ./tests/Neo.SmartContract.Framework.UnitTests -c Release

.PHONY: test-template
test-template:
	@echo "Running template tests..."
	@$(DOTNET) test ./tests/Neo.SmartContract.Template.UnitTests -c Release

# Generate documentation
.PHONY: docs
docs:
	@echo "Generating documentation..."
	@$(DOTNET) tool restore
	@$(DOTNET) docfx docs/docfx.json --serve

# Benchmarks
.PHONY: benchmark
benchmark: build
	@echo "Running benchmarks..."
	@$(DOTNET) run --project tests/Neo.Compiler.CSharp.Benchmarks -c Release

# Security scan
.PHONY: security-scan
security-scan:
	@echo "Running security scan..."
	@$(DOTNET) list package --vulnerable --include-transitive

# Check outdated packages
.PHONY: outdated
outdated:
	@echo "Checking for outdated packages..."
	@$(DOTNET) list package --outdated

# Create contract from template
.PHONY: new-contract
new-contract:
ifndef NAME
	@echo "Error: Please specify a contract name"
	@echo "Usage: make new-contract NAME=MyContract TYPE=nep17"
	@exit 1
endif
ifndef TYPE
	$(eval TYPE := nep17)
endif
	@echo "Creating new $(TYPE) contract: $(NAME)..."
	@$(DOTNET) new neocontract$(TYPE) -n $(NAME) -o ./contracts/$(NAME)
	@echo "Contract created in ./contracts/$(NAME)/"

# Compile and deploy helper (requires neo-express)
.PHONY: deploy
deploy: compile
	@echo "Note: This requires neo-express to be installed"
	@echo "Deploy your compiled contract using:"
	@echo "neo-express contract deploy ./out/contract.nef"

.PHONY: list-templates
list-templates:
	@echo "Available contract templates:"
	@echo "  nep17  - NEP-17 token standard"
	@echo "  oracle - Oracle request contract"
	@echo "  owner  - Ownable contract pattern"