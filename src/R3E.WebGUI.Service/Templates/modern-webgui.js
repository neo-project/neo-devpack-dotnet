// Modern WebGUI for Neo Smart Contracts
// Dynamically loads contract configuration and provides wallet integration

class ModernWebGUI {
    constructor() {
        this.contractConfig = null;
        this.wallet = null;
        this.isConnected = false;
        this.currentCategory = 'all';
        this.rpcClient = null;
        
        this.init();
    }

    async init() {
        try {
            await this.loadContractConfig();
            await this.setupWalletIntegration();
            this.setupEventListeners();
            this.applyTheme();
            this.renderUI();
            this.startPeriodicUpdates();
        } catch (error) {
            console.error('Failed to initialize WebGUI:', error);
            this.showError('Failed to load contract interface');
        } finally {
            this.hideLoading();
        }
    }

    async loadContractConfig() {
        // Extract contract address from URL or get it from current context
        const contractAddress = this.getContractAddressFromContext();
        
        if (!contractAddress) {
            throw new Error('Contract address not found');
        }

        const response = await fetch(`/api/webgui/${contractAddress}/config`);
        if (!response.ok) {
            throw new Error(`Failed to load contract config: ${response.status}`);
        }

        this.contractConfig = await response.json();
        
        // Initialize RPC client
        this.rpcClient = new NeoRpcClient(
            this.contractConfig.rpcEndpoints[this.contractConfig.network] || 
            this.contractConfig.rpcEndpoints.testnet
        );
    }

    getContractAddressFromContext() {
        // Try to get from URL, meta tags, or global variables
        const urlParams = new URLSearchParams(window.location.search);
        const fromUrl = urlParams.get('contract');
        
        if (fromUrl) return fromUrl;
        
        // Try from meta tag
        const metaTag = document.querySelector('meta[name="contract-address"]');
        if (metaTag) return metaTag.getAttribute('content');
        
        // Try from subdomain - extract from hostname
        const hostname = window.location.hostname;
        const parts = hostname.split('.');
        if (parts.length > 2 && parts[parts.length - 1] === 'localhost') {
            // For subdomain.localhost, we need to get contract address from config
            return this.getContractFromSubdomain(parts[0]);
        }
        
        return this.contractConfig?.contractAddress;
    }

    async getContractFromSubdomain(subdomain) {
        try {
            const response = await fetch(`/api/webgui/${subdomain}`);
            if (response.ok) {
                const data = await response.json();
                return data.contractAddress;
            }
        } catch (error) {
            console.error('Failed to get contract from subdomain:', error);
        }
        return null;
    }

    applyTheme() {
        if (!this.contractConfig?.theme) return;

        const theme = this.contractConfig.theme;
        const root = document.documentElement;

        root.style.setProperty('--primary-color', theme.primaryColor);
        root.style.setProperty('--secondary-color', theme.secondaryColor);
        root.style.setProperty('--accent-color', theme.accentColor);
        root.style.setProperty('--background-color', theme.backgroundColor);
        root.style.setProperty('--card-color', theme.cardColor);
        root.style.setProperty('--text-color', theme.textColor);
        root.style.setProperty('--border-radius', theme.borderRadius);
        
        if (theme.fontFamily) {
            document.body.style.fontFamily = theme.fontFamily;
        }
    }

    renderUI() {
        this.renderContractHeader();
        this.renderMethods();
        this.renderStatistics();
        this.updateNetworkInfo();
        this.loadBlockchainInfo();
    }

    renderContractHeader() {
        const config = this.contractConfig;
        
        document.getElementById('page-title').textContent = `${config.contractName} - Neo Smart Contract`;
        document.getElementById('header-title').textContent = config.contractName;
        document.getElementById('contract-name').textContent = config.contractName;
        document.getElementById('contract-description').textContent = 
            config.description || 'Smart contract interface';

        // Render contract metadata
        const metaContainer = document.getElementById('contract-meta');
        metaContainer.innerHTML = `
            <div class="meta-item">
                <div class="meta-label">Contract Address</div>
                <div class="meta-value">${this.truncateAddress(config.contractAddress)}</div>
            </div>
            <div class="meta-item">
                <div class="meta-label">Network</div>
                <div class="meta-value">${config.network.toUpperCase()}</div>
            </div>
            <div class="meta-item">
                <div class="meta-label">Deployed</div>
                <div class="meta-value">${this.formatDate(config.deployedAt)}</div>
            </div>
            <div class="meta-item">
                <div class="meta-label">Version</div>
                <div class="meta-value">${config.version}</div>
            </div>
            ${config.pluginDownloadUrl ? `
            <div class="meta-item">
                <div class="meta-label">Plugin</div>
                <div class="meta-value">
                    <a href="${config.pluginDownloadUrl}" class="plugin-download-btn" download>
                        <i class="ri-download-2-line"></i> Download Plugin
                    </a>
                </div>
            </div>
            ` : ''}
        `;

        // Update network indicator
        document.getElementById('network-name').textContent = config.network.toUpperCase();
    }

    renderMethods() {
        const methods = this.contractConfig.methods || [];
        
        // Get unique categories
        const categories = ['all', ...new Set(methods.map(m => m.category || 'General'))];
        
        // Render category tabs
        const categoriesContainer = document.getElementById('method-categories');
        categoriesContainer.innerHTML = categories.map(category => `
            <div class="category-tab ${category === this.currentCategory ? 'active' : ''}" 
                 data-category="${category}">
                ${category === 'all' ? 'All Methods' : category}
            </div>
        `).join('');

        // Filter and render methods
        const filteredMethods = this.currentCategory === 'all' 
            ? methods 
            : methods.filter(m => (m.category || 'General') === this.currentCategory);

        const methodsContainer = document.getElementById('methods-container');
        
        if (filteredMethods.length === 0) {
            methodsContainer.innerHTML = '<p class="text-muted">No methods found in this category.</p>';
            return;
        }

        methodsContainer.innerHTML = filteredMethods.map(method => this.renderMethod(method)).join('');
    }

    renderMethod(method) {
        const methodId = `method-${method.name}`;
        
        return `
            <div class="method" id="${methodId}">
                <div class="method-header" onclick="toggleMethod('${methodId}')">
                    <div class="method-info">
                        <span class="method-name">${method.displayName || method.name}</span>
                        <span class="method-type ${method.isReadOnly ? 'read-only' : 'write'}">
                            ${method.isReadOnly ? 'READ' : 'WRITE'}
                        </span>
                        ${method.gasEstimate ? `<span class="text-muted">~${method.gasEstimate} GAS</span>` : ''}
                    </div>
                    <i class="ri-arrow-down-s-line"></i>
                </div>
                <div class="method-content">
                    ${method.description ? `<p class="method-description">${method.description}</p>` : ''}
                    
                    <form id="form-${method.name}" onsubmit="return invokeMethod(event, '${method.name}')">
                        ${this.renderMethodParameters(method.parameters || [])}
                        
                        <div style="display: flex; gap: 1rem; margin-top: 1rem;">
                            <button type="submit" class="button button-primary">
                                <i class="ri-play-line"></i>
                                ${method.isReadOnly ? 'Query' : 'Invoke'}
                            </button>
                            
                            ${!method.isReadOnly ? `
                                <button type="button" class="button button-secondary" onclick="estimateGas('${method.name}')">
                                    <i class="ri-calculator-line"></i>
                                    Estimate Gas
                                </button>
                            ` : ''}
                        </div>
                    </form>
                    
                    <div id="result-${method.name}"></div>
                </div>
            </div>
        `;
    }

    renderMethodParameters(parameters) {
        if (parameters.length === 0) {
            return '<p class="text-muted">No parameters required.</p>';
        }

        return parameters.map(param => `
            <div class="form-group">
                <label class="form-label" for="param-${param.name}">
                    ${param.displayName || param.name}
                    ${param.required ? '*' : ''}
                    <span class="text-muted">(${param.type})</span>
                </label>
                <input 
                    type="text" 
                    id="param-${param.name}" 
                    name="${param.name}"
                    class="form-input" 
                    placeholder="${this.getParameterPlaceholder(param)}"
                    ${param.required ? 'required' : ''}
                    ${param.defaultValue ? `value="${param.defaultValue}"` : ''}
                />
                ${param.description ? `<div class="form-help">${param.description}</div>` : ''}
            </div>
        `).join('');
    }

    getParameterPlaceholder(param) {
        switch (param.type.toLowerCase()) {
            case 'string': return 'Enter text...';
            case 'integer': case 'int': return 'Enter number...';
            case 'boolean': case 'bool': return 'true or false';
            case 'bytearray': return 'Enter hex string (0x...)';
            case 'hash160': return 'Enter address or script hash';
            case 'hash256': return 'Enter transaction hash';
            default: return `Enter ${param.type}...`;
        }
    }

    renderStatistics() {
        const methods = this.contractConfig.methods || [];
        const events = this.contractConfig.events || [];
        
        const readOnlyCount = methods.filter(m => m.isReadOnly).length;
        const writeCount = methods.filter(m => !m.isReadOnly).length;

        document.getElementById('total-methods').textContent = methods.length;
        document.getElementById('readonly-methods').textContent = readOnlyCount;
        document.getElementById('write-methods').textContent = writeCount;
        document.getElementById('total-events').textContent = events.length;
    }

    setupEventListeners() {
        // Wallet connection
        document.getElementById('wallet-connect').addEventListener('click', () => {
            if (this.isConnected) {
                this.disconnectWallet();
            } else {
                this.connectWallet();
            }
        });

        // Category tabs
        document.addEventListener('click', (e) => {
            if (e.target.classList.contains('category-tab')) {
                const category = e.target.dataset.category;
                this.switchCategory(category);
            }
        });

        // Copy contract address
        document.addEventListener('click', (e) => {
            if (e.target.closest('.meta-value')) {
                const text = e.target.textContent;
                if (text.startsWith('0x') || text.length === 42) {
                    this.copyToClipboard(this.contractConfig.contractAddress);
                    this.showNotification('Contract address copied!', 'success');
                }
            }
        });
    }

    async setupWalletIntegration() {
        // Check if NeoLine is available
        if (typeof window.NEOLine !== 'undefined') {
            this.wallet = window.NEOLine;
        } else if (typeof window.o3dapi !== 'undefined') {
            this.wallet = window.o3dapi.NEO;
        } else {
            console.warn('No compatible wallet found');
            return;
        }

        // Try to restore previous connection
        try {
            const account = await this.wallet.getAccount();
            if (account) {
                this.onWalletConnected(account);
            }
        } catch (error) {
            // User not connected or denied access
            console.log('Wallet not connected');
        }
    }

    async connectWallet() {
        if (!this.wallet) {
            this.showNotification('No compatible wallet found. Please install NeoLine or O3.', 'error');
            return;
        }

        try {
            const account = await this.wallet.getAccount();
            this.onWalletConnected(account);
            this.showNotification('Wallet connected successfully!', 'success');
        } catch (error) {
            console.error('Failed to connect wallet:', error);
            this.showNotification('Failed to connect wallet', 'error');
        }
    }

    disconnectWallet() {
        this.isConnected = false;
        this.wallet = null;
        
        const walletButton = document.getElementById('wallet-connect');
        walletButton.innerHTML = '<i class="ri-wallet-3-line"></i><span>Connect Wallet</span>';
        walletButton.classList.remove('wallet-connected');
        
        document.getElementById('wallet-info').style.display = 'none';
        
        this.showNotification('Wallet disconnected', 'warning');
    }

    onWalletConnected(account) {
        this.isConnected = true;
        
        const walletButton = document.getElementById('wallet-connect');
        walletButton.innerHTML = `<i class="ri-wallet-3-line"></i><span>Connected</span>`;
        walletButton.classList.add('wallet-connected');
        
        // Show wallet info
        const walletInfo = document.getElementById('wallet-info');
        walletInfo.style.display = 'block';
        
        document.getElementById('wallet-address').textContent = 
            `${account.address.substring(0, 10)}...${account.address.substring(account.address.length - 8)}`;
        
        // Load wallet balance
        this.loadWalletBalance(account.address);
    }

    async loadWalletBalance(address) {
        try {
            // This would typically call the RPC to get wallet balance
            // For now, we'll show placeholder
            document.getElementById('wallet-balance').innerHTML = `
                <div class="balance-item">
                    <span class="balance-label">NEO</span>
                    <span class="balance-amount">Loading...</span>
                </div>
                <div class="balance-item">
                    <span class="balance-label">GAS</span>
                    <span class="balance-amount">Loading...</span>
                </div>
            `;
            
            // TODO: Implement actual balance loading
            // const balance = await this.rpcClient.getBalance(address);
        } catch (error) {
            console.error('Failed to load wallet balance:', error);
        }
    }

    async loadBlockchainInfo() {
        try {
            // Load basic blockchain information
            document.getElementById('block-height').innerHTML = '<div class="loading"></div>';
            document.getElementById('gas-price').innerHTML = '<div class="loading"></div>';
            
            // TODO: Implement actual blockchain info loading
            // const blockCount = await this.rpcClient.getBlockCount();
            // const gasPrice = await this.rpcClient.getGasPrice();
            
            // Placeholder values
            setTimeout(() => {
                document.getElementById('block-height').textContent = '1,234,567';
                document.getElementById('gas-price').textContent = '0.00001 GAS';
                document.getElementById('network-info').textContent = this.contractConfig.network.toUpperCase();
            }, 1000);
            
        } catch (error) {
            console.error('Failed to load blockchain info:', error);
            document.getElementById('block-height').textContent = 'Error';
            document.getElementById('gas-price').textContent = 'Error';
        }
    }

    switchCategory(category) {
        this.currentCategory = category;
        
        // Update active tab
        document.querySelectorAll('.category-tab').forEach(tab => {
            tab.classList.toggle('active', tab.dataset.category === category);
        });
        
        // Re-render methods
        this.renderMethods();
    }

    updateNetworkInfo() {
        const network = this.contractConfig.network;
        document.getElementById('network-name').textContent = network.toUpperCase();
        
        // Update network dot color based on network
        const networkDot = document.querySelector('.network-dot');
        switch (network.toLowerCase()) {
            case 'mainnet':
                networkDot.style.background = 'var(--success-color)';
                break;
            case 'testnet':
                networkDot.style.background = 'var(--warning-color)';
                break;
            default:
                networkDot.style.background = 'var(--text-muted)';
        }
    }

    startPeriodicUpdates() {
        // Update blockchain info every 30 seconds
        setInterval(() => {
            this.loadBlockchainInfo();
        }, 30000);
    }

    // Utility methods
    truncateAddress(address) {
        if (!address || address.length < 10) return address;
        return `${address.substring(0, 6)}...${address.substring(address.length - 6)}`;
    }

    formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString();
    }

    hideLoading() {
        const overlay = document.getElementById('loading-overlay');
        if (overlay) {
            overlay.style.display = 'none';
        }
    }

    showNotification(message, type = 'info') {
        const container = document.getElementById('notification-container');
        const notification = document.createElement('div');
        
        notification.className = `notification notification-${type}`;
        notification.innerHTML = `
            <div style="display: flex; align-items: center; gap: 0.5rem;">
                <i class="ri-${this.getNotificationIcon(type)}-line"></i>
                <span>${message}</span>
            </div>
        `;
        
        container.appendChild(notification);
        
        // Show notification
        setTimeout(() => notification.classList.add('show'), 100);
        
        // Hide after 5 seconds
        setTimeout(() => {
            notification.classList.remove('show');
            setTimeout(() => notification.remove(), 300);
        }, 5000);
    }

    getNotificationIcon(type) {
        switch (type) {
            case 'success': return 'check-circle';
            case 'error': return 'error-warning';
            case 'warning': return 'alert-circle';
            default: return 'information';
        }
    }

    showError(message) {
        this.showNotification(message, 'error');
    }

    copyToClipboard(text) {
        navigator.clipboard.writeText(text).catch(() => {
            // Fallback for older browsers
            const textArea = document.createElement('textarea');
            textArea.value = text;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
        });
    }
}

// Global functions for method interaction
window.toggleMethod = function(methodId) {
    const method = document.getElementById(methodId);
    method.classList.toggle('expanded');
    
    const arrow = method.querySelector('.ri-arrow-down-s-line');
    arrow.style.transform = method.classList.contains('expanded') ? 'rotate(180deg)' : 'rotate(0deg)';
};

window.invokeMethod = async function(event, methodName) {
    event.preventDefault();
    
    const webgui = window.webguiInstance;
    if (!webgui) return false;
    
    const method = webgui.contractConfig.methods.find(m => m.name === methodName);
    if (!method) {
        webgui.showError('Method not found');
        return false;
    }
    
    const resultContainer = document.getElementById(`result-${methodName}`);
    resultContainer.innerHTML = '<div class="result-box result-loading">Executing method...</div>';
    
    try {
        // Collect parameters
        const formData = new FormData(event.target);
        const parameters = [];
        
        for (const param of method.parameters || []) {
            const value = formData.get(param.name);
            if (param.required && !value) {
                throw new Error(`Parameter ${param.name} is required`);
            }
            if (value) {
                parameters.push({
                    name: param.name,
                    type: param.type,
                    value: value
                });
            }
        }
        
        let result;
        if (method.isReadOnly) {
            result = await webgui.invokeReadMethod(method, parameters);
        } else {
            if (!webgui.isConnected) {
                throw new Error('Please connect your wallet first');
            }
            result = await webgui.invokeWriteMethod(method, parameters);
        }
        
        resultContainer.innerHTML = `<div class="result-box result-success">${JSON.stringify(result, null, 2)}</div>`;
        
    } catch (error) {
        console.error('Method invocation failed:', error);
        resultContainer.innerHTML = `<div class="result-box result-error">Error: ${error.message}</div>`;
    }
    
    return false;
};

window.estimateGas = async function(methodName) {
    const webgui = window.webguiInstance;
    if (!webgui) return;
    
    // TODO: Implement gas estimation
    webgui.showNotification('Gas estimation: ~0.05 GAS', 'info');
};

// Simple RPC Client for Neo
class NeoRpcClient {
    constructor(endpoint) {
        this.endpoint = endpoint;
    }

    async call(method, params = []) {
        const response = await fetch(this.endpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                jsonrpc: '2.0',
                method: method,
                params: params,
                id: 1
            })
        });

        const data = await response.json();
        if (data.error) {
            throw new Error(data.error.message);
        }

        return data.result;
    }

    async getBlockCount() {
        return await this.call('getblockcount');
    }

    async invokeFunction(scriptHash, operation, params = []) {
        return await this.call('invokefunction', [scriptHash, operation, params]);
    }

    async sendRawTransaction(hex) {
        return await this.call('sendrawtransaction', [hex]);
    }
}

// Add method implementations to ModernWebGUI
ModernWebGUI.prototype.invokeReadMethod = async function(method, parameters) {
    try {
        const params = parameters.map(p => ({
            type: this.mapParameterType(p.type),
            value: p.value
        }));

        const result = await this.rpcClient.invokeFunction(
            this.contractConfig.contractAddress,
            method.name,
            params
        );

        if (result.state === 'FAULT') {
            throw new Error(result.exception || 'Contract execution failed');
        }

        return this.parseResult(result.stack[0], method.returnType);
    } catch (error) {
        throw new Error(`Read method failed: ${error.message}`);
    }
};

ModernWebGUI.prototype.invokeWriteMethod = async function(method, parameters) {
    if (!this.wallet || !this.isConnected) {
        throw new Error('Wallet not connected');
    }

    try {
        const params = parameters.map(p => ({
            type: this.mapParameterType(p.type),
            value: p.value
        }));

        // Use wallet to create and send transaction
        const result = await this.wallet.invoke({
            scriptHash: this.contractConfig.contractAddress,
            operation: method.name,
            args: params,
            fee: '0.05',
            networkFee: '0.001'
        });

        // Add to transaction history
        this.addTransaction(result.txid, method.name, 'pending');

        return result;
    } catch (error) {
        throw new Error(`Write method failed: ${error.message}`);
    }
};

ModernWebGUI.prototype.mapParameterType = function(type) {
    const typeMap = {
        'string': 'String',
        'integer': 'Integer',
        'int': 'Integer',
        'boolean': 'Boolean',
        'bool': 'Boolean',
        'bytearray': 'ByteArray',
        'hash160': 'Hash160',
        'hash256': 'Hash256'
    };
    return typeMap[type.toLowerCase()] || 'String';
};

ModernWebGUI.prototype.parseResult = function(stackItem, returnType) {
    if (!stackItem) return null;

    switch (stackItem.type) {
        case 'ByteString':
            return atob(stackItem.value);
        case 'Integer':
            return parseInt(stackItem.value);
        case 'Boolean':
            return stackItem.value;
        default:
            return stackItem.value;
    }
};

ModernWebGUI.prototype.addTransaction = function(txid, method, status) {
    const container = document.getElementById('transactions-container');
    const existing = container.querySelector('p.text-muted');
    if (existing) existing.remove();

    const txElement = document.createElement('div');
    txElement.className = 'transaction-item';
    txElement.innerHTML = `
        <div class="transaction-hash">${this.truncateAddress(txid)}</div>
        <div class="transaction-details">
            <span>${method}</span>
            <span class="text-${status === 'pending' ? 'warning' : status === 'confirmed' ? 'success' : 'error'}">${status}</span>
        </div>
    `;

    container.insertBefore(txElement, container.firstChild);

    // Keep only last 5 transactions
    const transactions = container.querySelectorAll('.transaction-item');
    if (transactions.length > 5) {
        transactions[transactions.length - 1].remove();
    }
};

// Initialize WebGUI when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.webguiInstance = new ModernWebGUI();
});