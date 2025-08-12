# Multi-stage Dockerfile for Neo Smart Contract Compiler (nccs)

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy solution and project files
COPY *.sln .
COPY src/ ./src/
COPY tests/ ./tests/
COPY neo/ ./neo/

# Restore dependencies
RUN dotnet restore

# Build the compiler
RUN dotnet build src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -c Release

# Publish self-contained executable
RUN dotnet publish src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:PublishReadyToRun=true \
    -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0
WORKDIR /workspace

# Install additional tools
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    git \
    make \
    && rm -rf /var/lib/apt/lists/*

# Copy the compiled binary from build stage
COPY --from=build /app/nccs /usr/local/bin/nccs
RUN chmod +x /usr/local/bin/nccs

# Set up working directory
VOLUME ["/workspace"]

# Default command
CMD ["/bin/bash"]