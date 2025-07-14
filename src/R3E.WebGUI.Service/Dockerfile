# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/R3E.WebGUI.Service/R3E.WebGUI.Service.csproj", "src/R3E.WebGUI.Service/"]
RUN dotnet restore "src/R3E.WebGUI.Service/R3E.WebGUI.Service.csproj"

# Copy source code
COPY . .
WORKDIR "/src/src/R3E.WebGUI.Service"

# Build the application
RUN dotnet build "R3E.WebGUI.Service.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "R3E.WebGUI.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Create directories and set permissions
RUN mkdir -p /app/webgui-storage && \
    mkdir -p /app/logs && \
    mkdir -p /app/Templates && \
    chown -R appuser:appuser /app

# Copy published application
COPY --from=publish /app/publish .

# Copy template files
COPY --chown=appuser:appuser src/R3E.WebGUI.Service/Templates/ /app/Templates/

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

# Entry point
ENTRYPOINT ["dotnet", "R3E.WebGUI.Service.dll"]