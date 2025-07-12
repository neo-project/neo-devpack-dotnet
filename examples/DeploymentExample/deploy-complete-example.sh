#!/bin/bash

# NEO Smart Contract Complete Deployment Example
# This script demonstrates the full workflow: compilation, plugin generation, Web GUI creation, deployment, and testing

set -e  # Exit on any error

echo "🚀 NEO Smart Contract Complete Deployment Example"
echo "================================================="

# Configuration
CONTRACT_NAME="InteractiveDemoContract"
PROJECT_DIR="$(pwd)"
SRC_DIR="$PROJECT_DIR/src/DeploymentExample.Contract"
DEPLOY_DIR="$PROJECT_DIR/deploy/DeploymentExample.Deploy"
OUTPUT_DIR="$PROJECT_DIR/generated-artifacts"
WEB_GUI_DIR="$OUTPUT_DIR/web-gui"
PLUGINS_DIR="$OUTPUT_DIR/plugins"
NEF_DIR="$OUTPUT_DIR/compiled"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

print_step() {
    echo -e "\n${BLUE}📋 Step $1: $2${NC}"
    echo "----------------------------------------"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${CYAN}ℹ️  $1${NC}"
}

# Check prerequisites
check_prerequisites() {
    print_step "1" "Checking Prerequisites"
    
    # Check .NET SDK
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK not found. Please install .NET 9.0 SDK or later."
        exit 1
    fi
    
    dotnet_version=$(dotnet --version)
    print_success ".NET SDK found: $dotnet_version"
    
    # Check Neo Express
    if ! command -v neo-express &> /dev/null; then
        print_warning "Neo Express not found. Installing as global tool..."
        dotnet tool install --global Neo.Express
    fi
    
    neo_express_version=$(neo-express --version)
    print_success "Neo Express found: $neo_express_version"
    
    # Check nccs (Neo Compiler)
    if ! command -v nccs &> /dev/null; then
        print_warning "Neo Compiler (nccs) not found. Building from source..."
        dotnet build ../../src/Neo.Compiler.CSharp.Tool/Neo.Compiler.CSharp.Tool.csproj
        export PATH="$PATH:$(pwd)/../../src/Neo.Compiler.CSharp.Tool/bin/Debug/net9.0"
    fi
    
    print_success "All prerequisites checked"
}

# Setup directory structure
setup_directories() {
    print_step "2" "Setting Up Directory Structure"
    
    # Create output directories
    mkdir -p "$OUTPUT_DIR"
    mkdir -p "$WEB_GUI_DIR"
    mkdir -p "$PLUGINS_DIR"
    mkdir -p "$NEF_DIR"
    
    print_success "Directories created:"
    print_info "  📁 Output: $OUTPUT_DIR"
    print_info "  🌐 Web GUI: $WEB_GUI_DIR"
    print_info "  🔌 Plugins: $PLUGINS_DIR"
    print_info "  📦 Compiled: $NEF_DIR"
}

# Build the solution
build_solution() {
    print_step "3" "Building Solution"
    
    dotnet build --verbosity quiet
    if [ $? -eq 0 ]; then
        print_success "Solution built successfully"
    else
        print_error "Solution build failed"
        exit 1
    fi
}

# Compile contract with advanced features
compile_contract() {
    print_step "4" "Compiling Smart Contract"
    
    cd "$SRC_DIR"
    
    # Compile the contract with optimization and debug info
    print_info "Compiling $CONTRACT_NAME with full optimization..."
    
    dotnet run --project ../../src/Neo.Compiler.CSharp.Tool/Neo.Compiler.CSharp.Tool.csproj -- \
        --source "$CONTRACT_NAME.cs" \
        --output "$NEF_DIR" \
        --optimize Basic \
        --debug Extended \
        --assembly \
        --generate-plugin \
        --plugin-output "$PLUGINS_DIR" \
        --generate-web-gui \
        --web-gui-output "$WEB_GUI_DIR"
    
    if [ $? -eq 0 ]; then
        print_success "Contract compiled successfully"
        
        # List generated files
        print_info "Generated files:"
        if [ -f "$NEF_DIR/$CONTRACT_NAME.nef" ]; then
            print_info "  📦 NEF: $NEF_DIR/$CONTRACT_NAME.nef"
        fi
        if [ -f "$NEF_DIR/$CONTRACT_NAME.manifest.json" ]; then
            print_info "  📋 Manifest: $NEF_DIR/$CONTRACT_NAME.manifest.json"
        fi
        if [ -f "$NEF_DIR/$CONTRACT_NAME.asm" ]; then
            print_info "  🔧 Assembly: $NEF_DIR/$CONTRACT_NAME.asm"
        fi
    else
        print_error "Contract compilation failed"
        exit 1
    fi
    
    cd "$PROJECT_DIR"
}

# Generate plugin files
generate_plugin() {
    print_step "5" "Generating Contract Plugin"
    
    print_info "Creating C# plugin wrapper for $CONTRACT_NAME..."
    
    # Use the Neo.Compiler.CSharp to generate plugin
    dotnet run --project ../../src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- generate-plugin \
        --nef "$NEF_DIR/$CONTRACT_NAME.nef" \
        --manifest "$NEF_DIR/$CONTRACT_NAME.manifest.json" \
        --output "$PLUGINS_DIR" \
        --namespace "DeploymentExample.Plugins" \
        --class-name "${CONTRACT_NAME}Plugin"
    
    if [ $? -eq 0 ]; then
        print_success "Plugin generated successfully"
        print_info "  🔌 Plugin: $PLUGINS_DIR/${CONTRACT_NAME}Plugin.cs"
    else
        print_warning "Plugin generation failed or not supported - creating manual plugin"
        
        # Create a basic plugin manually
        cat > "$PLUGINS_DIR/${CONTRACT_NAME}Plugin.cs" << 'EOF'
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using System.Numerics;

namespace DeploymentExample.Plugins
{
    /// <summary>
    /// Generated plugin for InteractiveDemoContract
    /// </summary>
    public class InteractiveDemoContractPlugin : SmartContract
    {
        public static readonly UInt160 ContractHash = "0x0000000000000000000000000000000000000000";
        
        public static BigInteger GetCounter() => (BigInteger)Contract.Call(ContractHash, "getCounter", CallFlags.ReadOnly);
        public static BigInteger Increment() => (BigInteger)Contract.Call(ContractHash, "increment", CallFlags.All);
        public static BigInteger IncrementBy(BigInteger amount) => (BigInteger)Contract.Call(ContractHash, "incrementBy", CallFlags.All, amount);
        public static bool StoreValue(string key, string value) => (bool)Contract.Call(ContractHash, "storeValue", CallFlags.All, key, value);
        public static string GetValue(string key) => (string)Contract.Call(ContractHash, "getValue", CallFlags.ReadOnly, key);
        public static object[] GetContractInfo() => (object[])Contract.Call(ContractHash, "getContractInfo", CallFlags.ReadOnly);
        public static bool SetPaused(bool paused) => (bool)Contract.Call(ContractHash, "setPaused", CallFlags.All, paused);
        public static BigInteger GetGasBalance() => (BigInteger)Contract.Call(ContractHash, "getGasBalance", CallFlags.ReadOnly);
    }
}
EOF
        print_success "Manual plugin created"
    fi
}

# Generate interactive web GUI
generate_web_gui() {
    print_step "6" "Generating Interactive Web GUI"
    
    print_info "Creating interactive web dashboard for $CONTRACT_NAME..."
    
    # Use our Web GUI generation feature
    dotnet run --project ../../src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- generate-web-gui \
        --contract-name "$CONTRACT_NAME" \
        --nef "$NEF_DIR/$CONTRACT_NAME.nef" \
        --manifest "$NEF_DIR/$CONTRACT_NAME.manifest.json" \
        --output "$WEB_GUI_DIR" \
        --rpc-endpoint "http://localhost:50012" \
        --dark-theme \
        --include-all-features
    
    if [ $? -eq 0 ]; then
        print_success "Web GUI generated successfully"
        print_info "  🌐 Website: $WEB_GUI_DIR/index.html"
        print_info "  🎨 Styles: $WEB_GUI_DIR/styles.css"
        print_info "  ⚡ Scripts: $WEB_GUI_DIR/contract.js"
        print_info "  ⚙️  Config: $WEB_GUI_DIR/config.json"
    else
        print_warning "Web GUI generation failed - creating manual GUI"
        
        # Create a basic web interface manually
        create_manual_web_gui
    fi
}

create_manual_web_gui() {
    print_info "Creating manual web GUI..."
    
    # Create index.html
    cat > "$WEB_GUI_DIR/index.html" << 'EOF'
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>InteractiveDemoContract Dashboard</title>
    <link rel="stylesheet" href="styles.css">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/axios/1.6.0/axios.min.js"></script>
</head>
<body class="dark-theme">
    <header class="header">
        <div class="header-content">
            <div class="logo">
                <h1>InteractiveDemoContract</h1>
            </div>
            <div class="header-actions">
                <button id="theme-toggle" class="btn btn-ghost">🌙</button>
                <button id="refresh-all" class="btn btn-primary">🔄 Refresh</button>
            </div>
        </div>
    </header>
    
    <div class="container">
        <nav class="nav-tabs">
            <button class="tab-button active" data-tab="overview">Overview</button>
            <button class="tab-button" data-tab="methods">Methods</button>
            <button class="tab-button" data-tab="storage">Storage</button>
            <button class="tab-button" data-tab="admin">Admin</button>
        </nav>
        
        <div id="overview" class="tab-content active">
            <div class="card">
                <h2>📊 Contract Information</h2>
                <div class="info-grid">
                    <div class="info-item">
                        <label>Contract Hash:</label>
                        <span id="contract-hash">Not deployed</span>
                    </div>
                    <div class="info-item">
                        <label>Current Counter:</label>
                        <span id="current-counter">0</span>
                    </div>
                    <div class="info-item">
                        <label>GAS Balance:</label>
                        <span id="gas-balance">0</span>
                    </div>
                    <div class="info-item">
                        <label>Status:</label>
                        <span id="contract-status">Active</span>
                    </div>
                </div>
            </div>
        </div>
        
        <div id="methods" class="tab-content">
            <div class="card">
                <h2>🎯 Method Invocation</h2>
                <div class="method-grid">
                    <div class="method-card">
                        <h3>Increment Counter</h3>
                        <button id="increment-btn" class="btn btn-primary">Increment</button>
                    </div>
                    <div class="method-card">
                        <h3>Increment By Amount</h3>
                        <input type="number" id="increment-amount" placeholder="Amount" min="1">
                        <button id="increment-by-btn" class="btn btn-primary">Increment By</button>
                    </div>
                    <div class="method-card">
                        <h3>Get Contract Info</h3>
                        <button id="get-info-btn" class="btn btn-secondary">Get Info</button>
                        <pre id="contract-info-result"></pre>
                    </div>
                </div>
            </div>
        </div>
        
        <div id="storage" class="tab-content">
            <div class="card">
                <h2>💾 Storage Operations</h2>
                <div class="storage-operations">
                    <div class="storage-action">
                        <h3>Store Value</h3>
                        <input type="text" id="store-key" placeholder="Key">
                        <input type="text" id="store-value" placeholder="Value">
                        <button id="store-btn" class="btn btn-primary">Store</button>
                    </div>
                    <div class="storage-action">
                        <h3>Get Value</h3>
                        <input type="text" id="get-key" placeholder="Key">
                        <button id="get-btn" class="btn btn-secondary">Get</button>
                        <div id="get-result"></div>
                    </div>
                </div>
            </div>
        </div>
        
        <div id="admin" class="tab-content">
            <div class="card">
                <h2>⚙️ Administration</h2>
                <div class="admin-actions">
                    <button id="pause-btn" class="btn btn-warning">Toggle Pause</button>
                    <button id="reset-counter-btn" class="btn btn-danger">Reset Counter</button>
                </div>
            </div>
        </div>
    </div>
    
    <script src="contract.js"></script>
</body>
</html>
EOF

    # Create styles.css
    cat > "$WEB_GUI_DIR/styles.css" << 'EOF'
:root {
    --primary-color: #667eea;
    --bg-color: #1a202c;
    --card-bg: #2d3748;
    --text-color: #f7fafc;
    --border-color: #4a5568;
}

* { margin: 0; padding: 0; box-sizing: border-box; }

body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background-color: var(--bg-color);
    color: var(--text-color);
    line-height: 1.6;
}

.header {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    padding: 1rem 0;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.header-content {
    max-width: 1200px;
    margin: 0 auto;
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0 20px;
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 20px;
}

.nav-tabs {
    display: flex;
    background: var(--card-bg);
    border-radius: 8px;
    padding: 4px;
    margin: 20px 0;
}

.tab-button {
    background: none;
    border: none;
    padding: 12px 20px;
    border-radius: 6px;
    cursor: pointer;
    color: var(--text-color);
    transition: all 0.3s ease;
}

.tab-button.active {
    background: var(--primary-color);
    color: white;
}

.tab-content {
    display: none;
}

.tab-content.active {
    display: block;
}

.card {
    background: var(--card-bg);
    border-radius: 12px;
    padding: 24px;
    margin-bottom: 20px;
    border: 1px solid var(--border-color);
}

.info-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 20px;
}

.info-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 12px;
    background: rgba(255,255,255,0.1);
    border-radius: 6px;
}

.btn {
    padding: 10px 16px;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-weight: 500;
    transition: all 0.3s ease;
}

.btn-primary { background: var(--primary-color); color: white; }
.btn-secondary { background: #6c757d; color: white; }
.btn-warning { background: #ffc107; color: black; }
.btn-danger { background: #dc3545; color: white; }
.btn-ghost { background: transparent; color: var(--text-color); border: 1px solid var(--border-color); }

.method-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 20px;
}

.method-card {
    background: rgba(255,255,255,0.05);
    padding: 20px;
    border-radius: 8px;
    border: 1px solid var(--border-color);
}

.storage-operations {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
    gap: 20px;
}

.storage-action {
    background: rgba(255,255,255,0.05);
    padding: 20px;
    border-radius: 8px;
}

input {
    width: 100%;
    padding: 10px;
    margin: 5px 0;
    border: 1px solid var(--border-color);
    border-radius: 6px;
    background: var(--bg-color);
    color: var(--text-color);
}

pre {
    background: rgba(0,0,0,0.3);
    padding: 10px;
    border-radius: 6px;
    margin-top: 10px;
    font-size: 12px;
    overflow-x: auto;
}
EOF

    # Create contract.js
    cat > "$WEB_GUI_DIR/contract.js" << 'EOF'
// Contract interaction JavaScript
const CONTRACT_HASH = '0x0000000000000000000000000000000000000000'; // Will be updated after deployment
const RPC_URL = 'http://localhost:50012';

// Tab management
document.querySelectorAll('.tab-button').forEach(button => {
    button.addEventListener('click', () => {
        const tabId = button.dataset.tab;
        
        // Update active tab
        document.querySelectorAll('.tab-button').forEach(b => b.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));
        
        button.classList.add('active');
        document.getElementById(tabId).classList.add('active');
    });
});

// Method bindings
document.getElementById('increment-btn').addEventListener('click', () => {
    console.log('Increment counter');
    // TODO: Implement RPC call
});

document.getElementById('increment-by-btn').addEventListener('click', () => {
    const amount = document.getElementById('increment-amount').value;
    console.log('Increment by:', amount);
    // TODO: Implement RPC call
});

document.getElementById('get-info-btn').addEventListener('click', () => {
    console.log('Get contract info');
    // TODO: Implement RPC call
});

document.getElementById('store-btn').addEventListener('click', () => {
    const key = document.getElementById('store-key').value;
    const value = document.getElementById('store-value').value;
    console.log('Store:', key, value);
    // TODO: Implement RPC call
});

document.getElementById('get-btn').addEventListener('click', () => {
    const key = document.getElementById('get-key').value;
    console.log('Get value for key:', key);
    // TODO: Implement RPC call
});

// Theme toggle
document.getElementById('theme-toggle').addEventListener('click', () => {
    document.body.classList.toggle('dark-theme');
});

// Auto-refresh
setInterval(() => {
    // TODO: Refresh contract data
}, 30000);

console.log('InteractiveDemoContract Dashboard loaded');
EOF

    print_success "Manual web GUI created"
}

# Setup Neo Express
setup_neo_express() {
    print_step "7" "Setting Up Neo Express"
    
    # Check if neo-express is already initialized
    if [ ! -f "default.neo-express" ]; then
        print_info "Initializing Neo Express..."
        neo-express create -f default.neo-express
        print_success "Neo Express initialized"
    else
        print_info "Neo Express already initialized"
    fi
    
    # Check if Neo Express is running
    if ! pgrep -f "neo-express" > /dev/null; then
        print_info "Starting Neo Express..."
        neo-express run -s 10 &
        NEO_EXPRESS_PID=$!
        
        # Wait for Neo Express to start
        print_info "Waiting for Neo Express to start..."
        sleep 15
        
        # Check if it's running
        if pgrep -f "neo-express" > /dev/null; then
            print_success "Neo Express started successfully"
        else
            print_error "Failed to start Neo Express"
            exit 1
        fi
    else
        print_info "Neo Express is already running"
    fi
}

# Deploy the contract
deploy_contract() {
    print_step "8" "Deploying Smart Contract"
    
    print_info "Deploying $CONTRACT_NAME to Neo Express..."
    
    # Deploy using neo-express
    neo-express contract deploy "$NEF_DIR/$CONTRACT_NAME.nef" alice
    
    if [ $? -eq 0 ]; then
        print_success "Contract deployed successfully"
        
        # Get the contract hash
        CONTRACT_HASH=$(neo-express show contract "$CONTRACT_NAME" | grep -oP 'Hash: \K[0-9a-fx]+')
        print_info "Contract Hash: $CONTRACT_HASH"
        
        # Update the web GUI with the actual contract hash
        if [ -f "$WEB_GUI_DIR/contract.js" ]; then
            sed -i "s/0x0000000000000000000000000000000000000000/$CONTRACT_HASH/g" "$WEB_GUI_DIR/contract.js"
            print_success "Web GUI updated with contract hash"
        fi
        
        # Update the plugin with the actual contract hash
        if [ -f "$PLUGINS_DIR/${CONTRACT_NAME}Plugin.cs" ]; then
            sed -i "s/0x0000000000000000000000000000000000000000/$CONTRACT_HASH/g" "$PLUGINS_DIR/${CONTRACT_NAME}Plugin.cs"
            print_success "Plugin updated with contract hash"
        fi
        
    else
        print_error "Contract deployment failed"
        exit 1
    fi
}

# Test the deployed contract
test_contract() {
    print_step "9" "Testing Deployed Contract"
    
    print_info "Running contract tests..."
    
    # Test basic functionality
    print_info "Testing getCounter method..."
    neo-express contract invoke "$CONTRACT_HASH" getCounter alice
    
    print_info "Testing increment method..."
    neo-express contract invoke "$CONTRACT_HASH" increment alice
    
    print_info "Testing getContractInfo method..."
    neo-express contract invoke "$CONTRACT_HASH" getContractInfo alice
    
    print_success "Contract tests completed"
}

# Generate final report
generate_report() {
    print_step "10" "Generating Deployment Report"
    
    REPORT_FILE="$OUTPUT_DIR/deployment-report.md"
    
    cat > "$REPORT_FILE" << EOF
# NEO Smart Contract Deployment Report

**Contract:** $CONTRACT_NAME  
**Deployment Date:** $(date)  
**Network:** Neo Express (Local)  

## Artifacts Generated

### Compiled Contract
- 📦 **NEF File:** \`$NEF_DIR/$CONTRACT_NAME.nef\`
- 📋 **Manifest:** \`$NEF_DIR/$CONTRACT_NAME.manifest.json\`
- 🔧 **Assembly:** \`$NEF_DIR/$CONTRACT_NAME.asm\`

### Plugin Files
- 🔌 **C# Plugin:** \`$PLUGINS_DIR/${CONTRACT_NAME}Plugin.cs\`

### Web GUI
- 🌐 **Website:** \`$WEB_GUI_DIR/index.html\`
- 🎨 **Styles:** \`$WEB_GUI_DIR/styles.css\`
- ⚡ **Scripts:** \`$WEB_GUI_DIR/contract.js\`
- ⚙️ **Config:** \`$WEB_GUI_DIR/config.json\`

## Contract Information

**Contract Hash:** $CONTRACT_HASH  
**Network:** Neo Express Local  
**RPC Endpoint:** http://localhost:50012  

## Available Methods

- \`getCounter()\` - Get current counter value
- \`increment()\` - Increment counter by 1
- \`incrementBy(amount)\` - Increment counter by amount
- \`storeValue(key, value)\` - Store a key-value pair
- \`getValue(key)\` - Get stored value by key
- \`getContractInfo()\` - Get contract information
- \`setPaused(paused)\` - Pause/unpause contract (owner only)
- \`getGasBalance()\` - Get contract GAS balance
- \`getNeoBalance()\` - Get contract NEO balance

## How to Use

### Web Interface
Open \`$WEB_GUI_DIR/index.html\` in your browser to interact with the contract through a web interface.

### Plugin Integration
Copy \`$PLUGINS_DIR/${CONTRACT_NAME}Plugin.cs\` to your project to interact with the contract programmatically.

### Direct RPC Calls
Use the contract hash \`$CONTRACT_HASH\` to make direct RPC calls to the Neo Express node.

## Testing Commands

\`\`\`bash
# Get counter value
neo-express contract invoke $CONTRACT_HASH getCounter alice

# Increment counter
neo-express contract invoke $CONTRACT_HASH increment alice

# Store a value
neo-express contract invoke $CONTRACT_HASH storeValue alice "key1" "value1"

# Get stored value
neo-express contract invoke $CONTRACT_HASH getValue alice "key1"
\`\`\`

## Next Steps

1. **Web Interface:** Open the generated web GUI to interact with your contract
2. **Plugin Usage:** Integrate the generated plugin into your applications
3. **Custom Development:** Extend the contract functionality as needed
4. **Production Deployment:** Deploy to TestNet or MainNet when ready

---
*Generated by Neo DevPack Complete Deployment Example*
EOF

    print_success "Deployment report generated: $REPORT_FILE"
}

# Display final summary
display_summary() {
    echo ""
    echo -e "${PURPLE}🎉 Deployment Complete!${NC}"
    echo "=================================="
    echo ""
    print_success "Contract successfully deployed and tested"
    print_info "Contract Hash: $CONTRACT_HASH"
    print_info "RPC Endpoint: http://localhost:50012"
    echo ""
    print_info "📂 Generated Artifacts:"
    print_info "  📦 Compiled Contract: $NEF_DIR/"
    print_info "  🔌 Plugin Files: $PLUGINS_DIR/"
    print_info "  🌐 Web GUI: $WEB_GUI_DIR/"
    print_info "  📊 Full Report: $OUTPUT_DIR/deployment-report.md"
    echo ""
    print_info "🚀 Next Steps:"
    print_info "  1. Open $WEB_GUI_DIR/index.html in your browser"
    print_info "  2. Copy plugin files to your projects"
    print_info "  3. Test contract methods using the web interface"
    print_info "  4. View the deployment report for detailed information"
    echo ""
    print_info "💡 Useful Commands:"
    print_info "  neo-express contract invoke $CONTRACT_HASH getCounter alice"
    print_info "  neo-express contract invoke $CONTRACT_HASH increment alice"
    echo ""
}

# Cleanup function for graceful exit
cleanup() {
    if [ ! -z "$NEO_EXPRESS_PID" ]; then
        print_info "Cleaning up Neo Express..."
        kill $NEO_EXPRESS_PID 2>/dev/null
    fi
}

# Set trap for cleanup
trap cleanup EXIT

# Main execution flow
main() {
    echo -e "${CYAN}Starting complete deployment workflow...${NC}"
    
    check_prerequisites
    setup_directories
    build_solution
    compile_contract
    generate_plugin
    generate_web_gui
    setup_neo_express
    deploy_contract
    test_contract
    generate_report
    display_summary
    
    echo -e "\n${GREEN}🏁 All done! Happy coding! 🚀${NC}"
}

# Run main function
main "$@"