// Neo Wallet Adapter
// Provides unified interface for different Neo wallets

class NeoWalletAdapter {
    constructor() {
        this.wallets = new Map();
        this.currentWallet = null;
        this.isConnected = false;
        this.account = null;
        
        this.detectWallets();
    }

    detectWallets() {
        // NeoLine wallet
        if (typeof window.NEOLine !== 'undefined') {
            this.wallets.set('neoline', {
                name: 'NeoLine',
                icon: 'ri-wallet-line',
                instance: window.NEOLine,
                type: 'neoline'
            });
        }

        // O3 wallet
        if (typeof window.o3dapi !== 'undefined') {
            this.wallets.set('o3', {
                name: 'O3 Wallet',
                icon: 'ri-wallet-2-line',
                instance: window.o3dapi.NEO,
                type: 'o3'
            });
        }

        // WalletConnect (if available)
        if (typeof window.WalletConnect !== 'undefined') {
            this.wallets.set('walletconnect', {
                name: 'WalletConnect',
                icon: 'ri-wireless-charging-line',
                instance: null, // Will be initialized on connect
                type: 'walletconnect'
            });
        }

        console.log(`Detected ${this.wallets.size} wallet(s):`, Array.from(this.wallets.keys()));
    }

    getAvailableWallets() {
        return Array.from(this.wallets.entries()).map(([key, wallet]) => ({
            id: key,
            name: wallet.name,
            icon: wallet.icon,
            available: true
        }));
    }

    async connect(walletId = null) {
        if (walletId && this.wallets.has(walletId)) {
            return await this.connectSpecificWallet(walletId);
        }

        // Try to connect to the first available wallet
        for (const [id, wallet] of this.wallets) {
            try {
                return await this.connectSpecificWallet(id);
            } catch (error) {
                console.warn(`Failed to connect to ${wallet.name}:`, error);
                continue;
            }
        }

        throw new Error('No wallet available or user rejected connection');
    }

    async connectSpecificWallet(walletId) {
        const wallet = this.wallets.get(walletId);
        if (!wallet) {
            throw new Error(`Wallet ${walletId} not found`);
        }

        this.currentWallet = wallet;

        try {
            let account;
            
            switch (wallet.type) {
                case 'neoline':
                    account = await wallet.instance.getAccount();
                    break;
                    
                case 'o3':
                    await wallet.instance.isReady();
                    account = await wallet.instance.getAccount();
                    break;
                    
                case 'walletconnect':
                    // TODO: Implement WalletConnect integration
                    throw new Error('WalletConnect not yet implemented');
                    
                default:
                    throw new Error(`Unknown wallet type: ${wallet.type}`);
            }

            this.isConnected = true;
            this.account = account;

            return {
                address: account.address,
                label: account.label || 'Connected Account',
                walletName: wallet.name,
                walletType: wallet.type
            };

        } catch (error) {
            this.currentWallet = null;
            throw new Error(`Failed to connect to ${wallet.name}: ${error.message}`);
        }
    }

    disconnect() {
        this.currentWallet = null;
        this.isConnected = false;
        this.account = null;
    }

    async getAccount() {
        if (!this.isConnected || !this.currentWallet) {
            throw new Error('No wallet connected');
        }

        return this.account;
    }

    async getBalance() {
        if (!this.isConnected) {
            throw new Error('No wallet connected');
        }

        try {
            switch (this.currentWallet.type) {
                case 'neoline':
                    return await this.currentWallet.instance.getBalance({
                        params: [{ address: this.account.address, assets: ['NEO', 'GAS'] }]
                    });
                    
                case 'o3':
                    return await this.currentWallet.instance.getBalance();
                    
                default:
                    throw new Error(`Balance not supported for ${this.currentWallet.type}`);
            }
        } catch (error) {
            throw new Error(`Failed to get balance: ${error.message}`);
        }
    }

    async invoke(params) {
        if (!this.isConnected) {
            throw new Error('No wallet connected');
        }

        try {
            let result;
            
            switch (this.currentWallet.type) {
                case 'neoline':
                    result = await this.currentWallet.instance.invoke(params);
                    break;
                    
                case 'o3':
                    result = await this.currentWallet.instance.invoke(params);
                    break;
                    
                default:
                    throw new Error(`Invoke not supported for ${this.currentWallet.type}`);
            }

            return result;
            
        } catch (error) {
            throw new Error(`Transaction failed: ${error.message}`);
        }
    }

    async invokeRead(params) {
        if (!this.isConnected) {
            throw new Error('No wallet connected');
        }

        try {
            let result;
            
            switch (this.currentWallet.type) {
                case 'neoline':
                    result = await this.currentWallet.instance.invokeRead(params);
                    break;
                    
                case 'o3':
                    result = await this.currentWallet.instance.invokeRead(params);
                    break;
                    
                default:
                    throw new Error(`InvokeRead not supported for ${this.currentWallet.type}`);
            }

            return result;
            
        } catch (error) {
            throw new Error(`Read invocation failed: ${error.message}`);
        }
    }

    async signMessage(message) {
        if (!this.isConnected) {
            throw new Error('No wallet connected');
        }

        try {
            switch (this.currentWallet.type) {
                case 'neoline':
                    return await this.currentWallet.instance.signMessage({
                        message: message
                    });
                    
                case 'o3':
                    return await this.currentWallet.instance.signMessage({
                        message: message
                    });
                    
                default:
                    throw new Error(`Message signing not supported for ${this.currentWallet.type}`);
            }
        } catch (error) {
            throw new Error(`Failed to sign message: ${error.message}`);
        }
    }

    async getNetworks() {
        if (!this.isConnected) {
            throw new Error('No wallet connected');
        }

        try {
            switch (this.currentWallet.type) {
                case 'neoline':
                    return await this.currentWallet.instance.getNetworks();
                    
                case 'o3':
                    return await this.currentWallet.instance.getNetworks();
                    
                default:
                    throw new Error(`Networks not supported for ${this.currentWallet.type}`);
            }
        } catch (error) {
            throw new Error(`Failed to get networks: ${error.message}`);
        }
    }

    async switchNetwork(network) {
        if (!this.isConnected) {
            throw new Error('No wallet connected');
        }

        try {
            switch (this.currentWallet.type) {
                case 'neoline':
                    // NeoLine doesn't support programmatic network switching
                    throw new Error('Please switch network manually in NeoLine wallet');
                    
                case 'o3':
                    return await this.currentWallet.instance.switchNetwork({ network });
                    
                default:
                    throw new Error(`Network switching not supported for ${this.currentWallet.type}`);
            }
        } catch (error) {
            throw new Error(`Failed to switch network: ${error.message}`);
        }
    }

    // Event listeners for wallet events
    onAccountChanged(callback) {
        if (!this.isConnected) return;

        try {
            switch (this.currentWallet.type) {
                case 'neoline':
                    // NeoLine doesn't provide account change events directly
                    // We can poll for changes or listen to window events
                    setInterval(async () => {
                        try {
                            const currentAccount = await this.currentWallet.instance.getAccount();
                            if (currentAccount.address !== this.account.address) {
                                this.account = currentAccount;
                                callback(currentAccount);
                            }
                        } catch (error) {
                            // Account likely disconnected
                            this.disconnect();
                            callback(null);
                        }
                    }, 5000);
                    break;
                    
                case 'o3':
                    this.currentWallet.instance.addEventListener('accountChanged', callback);
                    break;
            }
        } catch (error) {
            console.error('Failed to set up account change listener:', error);
        }
    }

    onNetworkChanged(callback) {
        if (!this.isConnected) return;

        try {
            switch (this.currentWallet.type) {
                case 'neoline':
                    // NeoLine network changes require manual handling
                    break;
                    
                case 'o3':
                    this.currentWallet.instance.addEventListener('networkChanged', callback);
                    break;
            }
        } catch (error) {
            console.error('Failed to set up network change listener:', error);
        }
    }

    // Utility methods
    formatBalance(balance) {
        if (!balance || !Array.isArray(balance)) return {};

        const formatted = {};
        for (const asset of balance) {
            const symbol = asset.assethash === '0xd2a4cff31913016155e38e474a2c06d08be276cf' ? 'GAS' : 
                          asset.assethash === '0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5' ? 'NEO' : 
                          asset.symbol || 'Unknown';
            
            formatted[symbol] = {
                amount: asset.amount || '0',
                symbol: symbol,
                hash: asset.assethash
            };
        }

        return formatted;
    }

    isWalletAvailable(walletId) {
        return this.wallets.has(walletId);
    }

    getCurrentWallet() {
        return this.currentWallet ? {
            id: this.currentWallet.type,
            name: this.currentWallet.name,
            connected: this.isConnected
        } : null;
    }
}

// Enhanced error handling
class WalletError extends Error {
    constructor(message, code = 'UNKNOWN_ERROR', walletType = null) {
        super(message);
        this.name = 'WalletError';
        this.code = code;
        this.walletType = walletType;
    }
}

// Common error codes
const WalletErrorCodes = {
    USER_REJECTED: 'USER_REJECTED',
    WALLET_NOT_FOUND: 'WALLET_NOT_FOUND',
    NETWORK_ERROR: 'NETWORK_ERROR',
    INSUFFICIENT_FUNDS: 'INSUFFICIENT_FUNDS',
    TRANSACTION_FAILED: 'TRANSACTION_FAILED',
    CONNECTION_FAILED: 'CONNECTION_FAILED',
    UNKNOWN_ERROR: 'UNKNOWN_ERROR'
};

// Export for use in other files
if (typeof window !== 'undefined') {
    window.NeoWalletAdapter = NeoWalletAdapter;
    window.WalletError = WalletError;
    window.WalletErrorCodes = WalletErrorCodes;
}

// Transaction builder helper
class NeoTransactionBuilder {
    constructor(walletAdapter) {
        this.wallet = walletAdapter;
    }

    buildInvokeParams(contractHash, method, parameters = [], systemFee = '0', networkFee = '0') {
        return {
            scriptHash: contractHash,
            operation: method,
            args: parameters.map(param => this.convertParameter(param)),
            fee: systemFee,
            networkFee: networkFee,
            broadcastOverride: false,
            signers: [{
                account: this.wallet.account.address,
                scope: 'CalledByEntry'
            }]
        };
    }

    convertParameter(param) {
        switch (param.type.toLowerCase()) {
            case 'string':
                return {
                    type: 'String',
                    value: param.value
                };
                
            case 'integer':
            case 'int':
                return {
                    type: 'Integer',
                    value: param.value.toString()
                };
                
            case 'boolean':
            case 'bool':
                return {
                    type: 'Boolean',
                    value: param.value === 'true' || param.value === true
                };
                
            case 'bytearray':
                return {
                    type: 'ByteArray',
                    value: param.value.startsWith('0x') ? param.value.slice(2) : param.value
                };
                
            case 'hash160':
                return {
                    type: 'Hash160',
                    value: this.addressToScriptHash(param.value)
                };
                
            case 'hash256':
                return {
                    type: 'Hash256',
                    value: param.value.startsWith('0x') ? param.value.slice(2) : param.value
                };
                
            default:
                return {
                    type: 'String',
                    value: param.value.toString()
                };
        }
    }

    addressToScriptHash(address) {
        // This would normally use a proper Neo SDK function
        // For now, return as-is if it looks like a script hash
        if (address.startsWith('0x') && address.length === 42) {
            return address.slice(2);
        }
        
        // If it's an address, we would need to convert it
        // This is a placeholder - in production, use proper Neo SDK
        throw new Error('Address to script hash conversion not implemented');
    }
}

if (typeof window !== 'undefined') {
    window.NeoTransactionBuilder = NeoTransactionBuilder;
}