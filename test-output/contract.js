
// Configuration
const CONFIG = {
  "contract": {
    "name": "SampleHelloWorld",
    "methods": [
      {
        "name": "sayHello",
        "parameters": [],
        "returnType": "String",
        "safe": true
      }
    ],
    "events": []
  },
  "network": {
    "rpcEndpoint": "https://neo.coz.io:443",
    "magic": 860833102
  },
  "features": {
    "transactionHistory": true,
    "balanceMonitoring": true,
    "methodInvocation": true,
    "stateMonitoring": true,
    "eventMonitoring": true,
    "walletConnection": true
  },
  "ui": {
    "refreshInterval": 30,
    "darkTheme": false
  }
};

// Global state
const APP_STATE = {
    currentTab: 'overview',
    walletConnected: false,
    monitoring: false,
    transactions: [],
    events: [],
    balanceHistory: []
};


// Core utilities
const utils = {
    formatHash(hash) {
        if (!hash) return 'N/A';
        return hash.length > 10 ? hash.substring(0, 6) + '...' + hash.substring(hash.length - 4) : hash;
    },

    formatNumber(num, decimals = 8) {
        if (typeof num === 'string') num = parseFloat(num);
        return new Intl.NumberFormat('en-US', { 
            minimumFractionDigits: 0,
            maximumFractionDigits: decimals 
        }).format(num);
    },

    formatTimestamp(timestamp) {
        return new Date(timestamp).toLocaleString();
    },

    showLoading(element) {
        if (typeof element === 'string') element = document.getElementById(element);
        if (element) element.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Loading...';
    },

    showError(element, message) {
        if (typeof element === 'string') element = document.getElementById(element);
        if (element) element.innerHTML = `<div class="error"><i class="fas fa-exclamation-circle"></i> ${message}</div>`;
    },

    showSuccess(element, message) {
        if (typeof element === 'string') element = document.getElementById(element);
        if (element) element.innerHTML = `<div class="success"><i class="fas fa-check-circle"></i> ${message}</div>`;
    },

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
};

// Notification system
const notifications = {
    show(message, type = 'info', duration = 5000) {
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.innerHTML = `
            <i class="fas ${type === 'error' ? 'fa-exclamation-circle' : type === 'success' ? 'fa-check-circle' : 'fa-info-circle'}"></i>
            <span>${message}</span>
            <button onclick="this.parentElement.remove()" class="notification-close">×</button>
        `;
        
        document.body.appendChild(notification);
        
        if (duration > 0) {
            setTimeout(() => notification.remove(), duration);
        }
    },

    error(message) { this.show(message, 'error'); },
    success(message) { this.show(message, 'success'); },
    info(message) { this.show(message, 'info'); }
};


// Tab management
const tabManager = {
    init() {
        document.querySelectorAll('.tab-button').forEach(button => {
            button.addEventListener('click', (e) => {
                const tabName = e.target.dataset.tab;
                this.switchTab(tabName);
            });
        });
    },

    switchTab(tabName) {
        // Update buttons
        document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
        document.querySelector(`[data-tab="${tabName}"]`).classList.add('active');

        // Update content
        document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));
        document.getElementById(tabName).classList.add('active');

        APP_STATE.currentTab = tabName;

        // Trigger tab-specific initialization
        this.onTabSwitch(tabName);
    },

    onTabSwitch(tabName) {
        switch(tabName) {
            case 'balance':
                if (CONFIG.features.balanceMonitoring) balanceMonitor.refresh();
                break;
            case 'transactions':
                if (CONFIG.features.transactionHistory) transactionHistory.refresh();
                break;
            case 'events':
                if (CONFIG.features.eventMonitoring) eventMonitor.refresh();
                break;
            case 'state':
                if (CONFIG.features.stateMonitoring) stateMonitor.refresh();
                break;
        }
    }
};


// RPC communication
const rpc = {
    async call(method, params = []) {
        try {
            const response = await axios.post(CONFIG.network.rpcEndpoint, {
                jsonrpc: '2.0',
                method: method,
                params: params,
                id: Math.floor(Math.random() * 1000)
            });

            if (response.data.error) {
                throw new Error(response.data.error.message);
            }

            return response.data.result;
        } catch (error) {
            console.error('RPC Error:', error);
            throw error;
        }
    },

    async getBlockHeight() {
        return await this.call('getblockcount');
    },

    async getBlock(height) {
        return await this.call('getblock', [height, 1]);
    },

    async getTransaction(hash) {
        return await this.call('getrawtransaction', [hash, 1]);
    },

    async getContractState(hash) {
        return await this.call('getcontractstate', [hash]);
    },

    async getStorage(contractHash, key) {
        return await this.call('getstorage', [contractHash, key]);
    },

    async invokeFunction(contractHash, method, params = []) {
        return await this.call('invokefunction', [contractHash, method, params]);
    },

    async getApplicationLog(txHash) {
        return await this.call('getapplicationlog', [txHash]);
    },

    async getNep17Balances(address) {
        return await this.call('getnep17balances', [address]);
    }
};


// Method invocation
const methodInvoker = {
    methods: [{"name":"sayHello","parameters":[],"returnType":"String","safe":true}],

    init() {
        const methodSelect = document.getElementById('method-select');
        const invokeBtn = document.getElementById('invoke-method');
        const testInvokeBtn = document.getElementById('test-invoke');

        if (methodSelect) {
            methodSelect.addEventListener('change', () => this.updateParameterForm());
            this.updateParameterForm();
        }

        if (invokeBtn) {
            invokeBtn.addEventListener('click', () => this.invokeMethod(false));
        }

        if (testInvokeBtn) {
            testInvokeBtn.addEventListener('click', () => this.invokeMethod(true));
        }
    },

    updateParameterForm() {
        const methodSelect = document.getElementById('method-select');
        const parametersDiv = document.getElementById('method-parameters');
        
        if (!methodSelect || !parametersDiv) return;

        const selectedMethod = this.methods.find(m => m.name === methodSelect.value);
        if (!selectedMethod) return;

        let html = '<h4>Parameters</h4>';
        
        if (selectedMethod.parameters.length === 0) {
            html += '<p>No parameters required</p>';
        } else {
            selectedMethod.parameters.forEach((param, index) => {
                html += `
                    <div class="parameter-input">
                        <label for="param_${index}">${param.name} (${param.type}):</label>
                        <input type="text" id="param_${index}" placeholder="Enter ${param.type} value" />
                    </div>
                `;
            });
        }

        parametersDiv.innerHTML = html;
    },

    async invokeMethod(testOnly = false) {
        const methodSelect = document.getElementById('method-select');
        const resultDiv = document.getElementById('method-result');
        
        if (!methodSelect || !resultDiv) return;

        const selectedMethod = this.methods.find(m => m.name === methodSelect.value);
        if (!selectedMethod) return;

        utils.showLoading(resultDiv);

        try {
            const params = [];
            for (let i = 0; i < selectedMethod.parameters.length; i++) {
                const input = document.getElementById(`param_${i}`);
                if (input) {
                    params.push({
                        type: selectedMethod.parameters[i].type,
                        value: input.value
                    });
                }
            }

            let result;
            if (testOnly || selectedMethod.safe) {
                result = await rpc.invokeFunction(CONFIG.contract.hash, selectedMethod.name, params);
            } else {
                // For non-safe methods, we need wallet integration
                if (!APP_STATE.walletConnected) {
                    throw new Error('Wallet connection required for state-changing operations');
                }
                result = await this.sendInvocation(selectedMethod.name, params);
            }

            resultDiv.innerHTML = `
                <h4>Result</h4>
                <pre>${JSON.stringify(result, null, 2)}</pre>
            `;

            if (!testOnly && !selectedMethod.safe) {
                notifications.success('Transaction sent successfully');
            }
        } catch (error) {
            utils.showError(resultDiv, error.message);
            notifications.error(`Method invocation failed: ${error.message}`);
        }
    },

    async sendInvocation(method, params) {
        // This would integrate with wallet APIs (OneGate, Neon, etc.)
        // For now, return a placeholder
        return {
            state: 'HALT',
            gasconsumed: '1000000',
            stack: [{ type: 'Integer', value: '42' }],
            tx: {
                hash: '0x' + Array(64).fill(0).map(() => Math.floor(Math.random() * 16).toString(16)).join('')
            }
        };
    }
};


// Balance monitoring
const balanceMonitor = {
    chart: null,

    init() {
        this.refresh();
        this.initChart();
    },

    async refresh() {
        await Promise.all([
            this.updateGasBalance(),
            this.updateNeoBalance(),
            this.updateContractBalance()
        ]);
    },

    async updateGasBalance() {
        const element = document.getElementById('gas-balance');
        if (!element) return;

        try {
            utils.showLoading(element);
            // For demonstration, use placeholder data
            // In real implementation, this would query the actual balance
            const balance = (Math.random() * 1000).toFixed(8);
            element.textContent = utils.formatNumber(balance) + ' GAS';
        } catch (error) {
            utils.showError(element, 'Failed to load');
        }
    },

    async updateNeoBalance() {
        const element = document.getElementById('neo-balance');
        if (!element) return;

        try {
            utils.showLoading(element);
            const balance = Math.floor(Math.random() * 100);
            element.textContent = utils.formatNumber(balance) + ' NEO';
        } catch (error) {
            utils.showError(element, 'Failed to load');
        }
    },

    async updateContractBalance() {
        const element = document.getElementById('contract-balance');
        if (!element) return;

        try {
            utils.showLoading(element);
            const balance = (Math.random() * 500).toFixed(8);
            element.textContent = utils.formatNumber(balance) + ' GAS';
        } catch (error) {
            utils.showError(element, 'Failed to load');
        }
    },

    initChart() {
        const canvas = document.getElementById('balanceChart');
        if (!canvas) return;

        const ctx = canvas.getContext('2d');
        this.chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [],
                datasets: [{
                    label: 'GAS Balance',
                    data: [],
                    borderColor: '#667eea',
                    backgroundColor: 'rgba(102, 126, 234, 0.1)',
                    tension: 0.4
                }, {
                    label: 'NEO Balance',
                    data: [],
                    borderColor: '#28a745',
                    backgroundColor: 'rgba(40, 167, 69, 0.1)',
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });

        // Add some sample data
        this.updateChartData();
    },

    updateChartData() {
        if (!this.chart) return;

        const now = new Date();
        const labels = [];
        const gasData = [];
        const neoData = [];

        for (let i = 23; i >= 0; i--) {
            const time = new Date(now.getTime() - i * 60 * 60 * 1000);
            labels.push(time.toLocaleTimeString());
            gasData.push(Math.random() * 1000 + 500);
            neoData.push(Math.floor(Math.random() * 50) + 25);
        }

        this.chart.data.labels = labels;
        this.chart.data.datasets[0].data = gasData;
        this.chart.data.datasets[1].data = neoData;
        this.chart.update();
    }
};


// Transaction history
const transactionHistory = {
    currentPage: 1,
    pageSize: 10,
    filteredTransactions: [],

    init() {
        const filterInput = document.getElementById('tx-filter');
        const clearButton = document.getElementById('clear-filter');
        const prevButton = document.getElementById('prev-page');
        const nextButton = document.getElementById('next-page');

        if (filterInput) {
            filterInput.addEventListener('input', utils.debounce(() => this.filterTransactions(), 300));
        }

        if (clearButton) {
            clearButton.addEventListener('click', () => {
                filterInput.value = '';
                this.filterTransactions();
            });
        }

        if (prevButton) {
            prevButton.addEventListener('click', () => this.previousPage());
        }

        if (nextButton) {
            nextButton.addEventListener('click', () => this.nextPage());
        }

        this.refresh();
    },

    async refresh() {
        const listElement = document.getElementById('transaction-list');
        if (!listElement) return;

        try {
            utils.showLoading(listElement);
            
            // Generate sample transaction data
            APP_STATE.transactions = this.generateSampleTransactions(50);
            this.filteredTransactions = [...APP_STATE.transactions];
            this.currentPage = 1;
            
            this.renderTransactions();
        } catch (error) {
            utils.showError(listElement, error.message);
        }
    },

    generateSampleTransactions(count) {
        const methods = CONFIG.contract.methods.map(m => m.name);
        const transactions = [];

        for (let i = 0; i < count; i++) {
            transactions.push({
                hash: '0x' + Array(64).fill(0).map(() => Math.floor(Math.random() * 16).toString(16)).join(''),
                timestamp: Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000,
                method: methods[Math.floor(Math.random() * methods.length)],
                sender: '0x' + Array(40).fill(0).map(() => Math.floor(Math.random() * 16).toString(16)).join(''),
                gasConsumed: Math.floor(Math.random() * 10000000),
                success: Math.random() > 0.1
            });
        }

        return transactions.sort((a, b) => b.timestamp - a.timestamp);
    },

    filterTransactions() {
        const filterInput = document.getElementById('tx-filter');
        if (!filterInput) return;

        const filter = filterInput.value.toLowerCase();
        
        if (!filter) {
            this.filteredTransactions = [...APP_STATE.transactions];
        } else {
            this.filteredTransactions = APP_STATE.transactions.filter(tx =>
                tx.hash.toLowerCase().includes(filter) ||
                tx.method.toLowerCase().includes(filter) ||
                tx.sender.toLowerCase().includes(filter)
            );
        }

        this.currentPage = 1;
        this.renderTransactions();
    },

    renderTransactions() {
        const listElement = document.getElementById('transaction-list');
        const pageInfo = document.getElementById('page-info');
        
        if (!listElement) return;

        const startIndex = (this.currentPage - 1) * this.pageSize;
        const endIndex = startIndex + this.pageSize;
        const pageTransactions = this.filteredTransactions.slice(startIndex, endIndex);

        if (pageTransactions.length === 0) {
            listElement.innerHTML = '<div class="loading">No transactions found</div>';
            return;
        }

        const html = pageTransactions.map(tx => `
            <div class="transaction-item">
                <div class="transaction-header">
                    <span class="transaction-hash">${utils.formatHash(tx.hash)}</span>
                    <span class="transaction-time">${utils.formatTimestamp(tx.timestamp)}</span>
                </div>
                <div class="transaction-details">
                    <div><strong>Method:</strong> ${tx.method}</div>
                    <div><strong>Sender:</strong> ${utils.formatHash(tx.sender)}</div>
                    <div><strong>Gas:</strong> ${utils.formatNumber(tx.gasConsumed / 100000000)} GAS</div>
                    <div><strong>Status:</strong> <span class="${tx.success ? 'success' : 'error'}">${tx.success ? 'Success' : 'Failed'}</span></div>
                </div>
            </div>
        `).join('');

        listElement.innerHTML = html;

        if (pageInfo) {
            const totalPages = Math.ceil(this.filteredTransactions.length / this.pageSize);
            pageInfo.textContent = `Page ${this.currentPage} of ${totalPages}`;
        }

        this.updatePaginationButtons();
    },

    updatePaginationButtons() {
        const prevButton = document.getElementById('prev-page');
        const nextButton = document.getElementById('next-page');
        const totalPages = Math.ceil(this.filteredTransactions.length / this.pageSize);

        if (prevButton) {
            prevButton.disabled = this.currentPage <= 1;
        }

        if (nextButton) {
            nextButton.disabled = this.currentPage >= totalPages;
        }
    },

    previousPage() {
        if (this.currentPage > 1) {
            this.currentPage--;
            this.renderTransactions();
        }
    },

    nextPage() {
        const totalPages = Math.ceil(this.filteredTransactions.length / this.pageSize);
        if (this.currentPage < totalPages) {
            this.currentPage++;
            this.renderTransactions();
        }
    }
};


// Event monitoring
const eventMonitor = {
    isMonitoring: false,
    intervalId: null,

    init() {
        const startBtn = document.getElementById('start-monitoring');
        const stopBtn = document.getElementById('stop-monitoring');
        const clearBtn = document.getElementById('clear-events');

        if (startBtn) {
            startBtn.addEventListener('click', () => this.startMonitoring());
        }

        if (stopBtn) {
            stopBtn.addEventListener('click', () => this.stopMonitoring());
        }

        if (clearBtn) {
            clearBtn.addEventListener('click', () => this.clearEvents());
        }

        this.refresh();
    },

    async refresh() {
        this.renderEvents();
    },

    startMonitoring() {
        if (this.isMonitoring) return;

        this.isMonitoring = true;
        APP_STATE.monitoring = true;

        document.getElementById('start-monitoring').disabled = true;
        document.getElementById('stop-monitoring').disabled = false;

        // Simulate event monitoring
        this.intervalId = setInterval(() => {
            this.simulateEvent();
        }, 5000 + Math.random() * 10000);

        notifications.success('Event monitoring started');
    },

    stopMonitoring() {
        if (!this.isMonitoring) return;

        this.isMonitoring = false;
        APP_STATE.monitoring = false;

        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }

        document.getElementById('start-monitoring').disabled = false;
        document.getElementById('stop-monitoring').disabled = true;

        notifications.info('Event monitoring stopped');
    },

    simulateEvent() {
        const events = CONFIG.contract.events;
        if (events.length === 0) return;

        const randomEvent = events[Math.floor(Math.random() * events.length)];
        const eventData = {};

        randomEvent.parameters.forEach(param => {
            switch (param.type) {
                case 'String':
                    eventData[param.name] = 'Sample string value';
                    break;
                case 'Integer':
                    eventData[param.name] = Math.floor(Math.random() * 1000000);
                    break;
                case 'ByteArray':
                    eventData[param.name] = '0x' + Array(40).fill(0).map(() => Math.floor(Math.random() * 16).toString(16)).join('');
                    break;
                default:
                    eventData[param.name] = 'Sample value';
            }
        });

        const event = {
            name: randomEvent.name,
            data: eventData,
            timestamp: Date.now(),
            blockHeight: Math.floor(Math.random() * 1000000) + 5000000,
            txHash: '0x' + Array(64).fill(0).map(() => Math.floor(Math.random() * 16).toString(16)).join('')
        };

        APP_STATE.events.unshift(event);

        // Keep only last 100 events
        if (APP_STATE.events.length > 100) {
            APP_STATE.events = APP_STATE.events.slice(0, 100);
        }

        this.renderEvents();
    },

    clearEvents() {
        APP_STATE.events = [];
        this.renderEvents();
        notifications.info('Events cleared');
    },

    renderEvents() {
        const listElement = document.getElementById('event-list');
        if (!listElement) return;

        if (APP_STATE.events.length === 0) {
            listElement.innerHTML = '<div class="loading">No events recorded</div>';
            return;
        }

        const html = APP_STATE.events.map(event => `
            <div class="event-item">
                <div class="event-header">
                    <span class="event-name">${event.name}</span>
                    <span class="event-time">${utils.formatTimestamp(event.timestamp)}</span>
                </div>
                <div class="event-details">
                    <div><strong>Block:</strong> ${event.blockHeight}</div>
                    <div><strong>Transaction:</strong> ${utils.formatHash(event.txHash)}</div>
                </div>
                <div class="event-data">${JSON.stringify(event.data, null, 2)}</div>
            </div>
        `).join('');

        listElement.innerHTML = html;
    }
};


// State monitoring
const stateMonitor = {
    init() {
        const getStorageBtn = document.getElementById('get-storage');
        const dumpStateBtn = document.getElementById('dump-state');

        if (getStorageBtn) {
            getStorageBtn.addEventListener('click', () => this.getStorageValue());
        }

        if (dumpStateBtn) {
            dumpStateBtn.addEventListener('click', () => this.dumpState());
        }
    },

    async refresh() {
        // Refresh any cached state data
    },

    async getStorageValue() {
        const keyInput = document.getElementById('storage-key');
        const resultDiv = document.getElementById('storage-result');

        if (!keyInput || !resultDiv) return;

        const key = keyInput.value.trim();
        if (!key) {
            utils.showError(resultDiv, 'Please enter a storage key');
            return;
        }

        try {
            utils.showLoading(resultDiv);

            if (!CONFIG.contract.hash) {
                throw new Error('Contract hash not available');
            }

            const value = await rpc.getStorage(CONFIG.contract.hash, key);
            
            if (value) {
                resultDiv.innerHTML = `
                    <h4>Storage Value</h4>
                    <div><strong>Key:</strong> ${key}</div>
                    <div><strong>Value:</strong> ${value}</div>
                    <div><strong>Decoded:</strong> ${this.decodeStorageValue(value)}</div>
                `;
            } else {
                resultDiv.innerHTML = `
                    <h4>Storage Value</h4>
                    <div>No value found for key: ${key}</div>
                `;
            }
        } catch (error) {
            utils.showError(resultDiv, error.message);
        }
    },

    async dumpState() {
        const resultDiv = document.getElementById('state-dump-result');
        if (!resultDiv) return;

        try {
            utils.showLoading(resultDiv);

            if (!CONFIG.contract.hash) {
                throw new Error('Contract hash not available');
            }

            // Simulate state dump (in real implementation, this would iterate through storage)
            const sampleState = {
                'totalSupply': '1000000000000000000',
                'owner': '0x' + Array(40).fill(0).map(() => Math.floor(Math.random() * 16).toString(16)).join(''),
                'paused': false,
                'version': '1.0.0'
            };

            resultDiv.innerHTML = `
                <h4>Contract State Dump</h4>
                <pre>${JSON.stringify(sampleState, null, 2)}</pre>
                <p><em>Note: This is sample data. Real implementation would query actual contract storage.</em></p>
            `;
        } catch (error) {
            utils.showError(resultDiv, error.message);
        }
    },

    decodeStorageValue(hexValue) {
        try {
            // Try to decode as string
            const bytes = hexValue.match(/.{2}/g).map(byte => parseInt(byte, 16));
            const string = String.fromCharCode(...bytes);
            if (string.match(/^[a-zA-Z0-9\s\.\-_]+$/)) {
                return `String: ${string}`;
            }
        } catch (e) {
            // Ignore decode errors
        }

        try {
            // Try to decode as integer
            const num = parseInt(hexValue, 16);
            return `Integer: ${num}`;
        } catch (e) {
            // Ignore decode errors
        }

        return 'Raw hex data';
    }
};


// Wallet connection
const walletManager = {
    wallet: null,

    init() {
        const connectBtn = document.getElementById('connect-wallet');
        const disconnectBtn = document.getElementById('disconnect-wallet');

        if (connectBtn) {
            connectBtn.addEventListener('click', () => this.connectWallet());
        }

        if (disconnectBtn) {
            disconnectBtn.addEventListener('click', () => this.disconnectWallet());
        }

        this.checkWalletStatus();
    },

    async checkWalletStatus() {
        // Check if wallet is already connected
        if (typeof window.NEOLine !== 'undefined') {
            this.updateWalletInfo('NEOLine wallet detected');
        } else if (typeof window.o3 !== 'undefined') {
            this.updateWalletInfo('O3 wallet detected');
        } else {
            this.updateWalletInfo('No wallet detected');
        }
    },

    async connectWallet() {
        try {
            let account;

            if (typeof window.NEOLine !== 'undefined') {
                account = await window.NEOLine.getAccount();
                this.wallet = { provider: 'NEOLine', account };
            } else if (typeof window.o3 !== 'undefined') {
                account = await window.o3.getAccount();
                this.wallet = { provider: 'O3', account };
            } else {
                // Simulate wallet connection
                account = {
                    address: 'N' + Array(33).fill(0).map(() => 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'[Math.floor(Math.random() * 62)]).join(''),
                    label: 'Demo Wallet'
                };
                this.wallet = { provider: 'Demo', account };
            }

            APP_STATE.walletConnected = true;
            this.updateConnectedState(account);
            notifications.success('Wallet connected successfully');

        } catch (error) {
            notifications.error(`Failed to connect wallet: ${error.message}`);
        }
    },

    disconnectWallet() {
        this.wallet = null;
        APP_STATE.walletConnected = false;
        this.updateDisconnectedState();
        notifications.info('Wallet disconnected');
    },

    updateWalletInfo(message) {
        const infoElement = document.getElementById('wallet-info');
        if (infoElement) {
            infoElement.textContent = message;
        }
    },

    updateConnectedState(account) {
        const infoElement = document.getElementById('wallet-info');
        const addressElement = document.getElementById('wallet-address');
        const balanceElement = document.getElementById('wallet-balance');
        const connectBtn = document.getElementById('connect-wallet');
        const disconnectBtn = document.getElementById('disconnect-wallet');
        const detailsDiv = document.querySelector('.wallet-details');

        if (infoElement) {
            infoElement.textContent = `Connected to ${this.wallet.provider}`;
        }

        if (addressElement) {
            addressElement.textContent = account.address;
        }

        if (balanceElement) {
            this.updateWalletBalance();
        }

        if (connectBtn) {
            connectBtn.style.display = 'none';
        }

        if (disconnectBtn) {
            disconnectBtn.style.display = 'inline-flex';
        }

        if (detailsDiv) {
            detailsDiv.style.display = 'block';
        }
    },

    updateDisconnectedState() {
        const infoElement = document.getElementById('wallet-info');
        const connectBtn = document.getElementById('connect-wallet');
        const disconnectBtn = document.getElementById('disconnect-wallet');
        const detailsDiv = document.querySelector('.wallet-details');

        if (infoElement) {
            infoElement.textContent = 'No wallet connected';
        }

        if (connectBtn) {
            connectBtn.style.display = 'inline-flex';
        }

        if (disconnectBtn) {
            disconnectBtn.style.display = 'none';
        }

        if (detailsDiv) {
            detailsDiv.style.display = 'none';
        }
    },

    async updateWalletBalance() {
        const balanceElement = document.getElementById('wallet-balance');
        if (!balanceElement || !this.wallet) return;

        try {
            // In real implementation, query actual balance
            const gasBalance = (Math.random() * 1000).toFixed(8);
            const neoBalance = Math.floor(Math.random() * 100);
            
            balanceElement.innerHTML = `${gasBalance} GAS, ${neoBalance} NEO`;
        } catch (error) {
            balanceElement.textContent = 'Error loading balance';
        }
    }
};


// Theme management
const themeManager = {
    init() {
        const toggleBtn = document.getElementById('theme-toggle');
        if (toggleBtn) {
            toggleBtn.addEventListener('click', () => this.toggleTheme());
        }

        // Set initial theme
        if (CONFIG.ui.darkTheme) {
            this.setTheme('dark');
        } else {
            this.setTheme('light');
        }
    },

    toggleTheme() {
        const isDark = document.body.classList.contains('dark-theme');
        this.setTheme(isDark ? 'light' : 'dark');
    },

    setTheme(theme) {
        const body = document.body;
        const toggleBtn = document.getElementById('theme-toggle');

        if (theme === 'dark') {
            body.classList.add('dark-theme');
            if (toggleBtn) {
                toggleBtn.innerHTML = '<i class="fas fa-sun"></i>';
            }
        } else {
            body.classList.remove('dark-theme');
            if (toggleBtn) {
                toggleBtn.innerHTML = '<i class="fas fa-moon"></i>';
            }
        }

        // Update charts if they exist
        if (balanceMonitor.chart) {
            balanceMonitor.chart.update();
        }
    }
};


// Auto-refresh functionality
const autoRefresh = {
    intervalId: null,
    interval: 30000,

    init() {
        const refreshBtn = document.getElementById('refresh-all');
        if (refreshBtn) {
            refreshBtn.addEventListener('click', () => this.refreshAll());
        }

        this.start();
    },

    start() {
        if (this.intervalId) return;

        this.intervalId = setInterval(() => {
            this.refreshCurrent();
        }, this.interval);
    },

    stop() {
        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }
    },

    async refreshAll() {
        const refreshBtn = document.getElementById('refresh-all');
        if (refreshBtn) {
            refreshBtn.disabled = true;
            refreshBtn.innerHTML = '<i class="fas fa-sync-alt fa-spin"></i> Refreshing...';
        }

        try {
            await Promise.all([
                CONFIG.features.balanceMonitoring ? balanceMonitor.refresh() : Promise.resolve(),
                CONFIG.features.transactionHistory ? transactionHistory.refresh() : Promise.resolve(),
                CONFIG.features.eventMonitoring ? eventMonitor.refresh() : Promise.resolve(),
                CONFIG.features.stateMonitoring ? stateMonitor.refresh() : Promise.resolve()
            ]);

            notifications.success('All data refreshed');
        } catch (error) {
            notifications.error('Refresh failed: ' + error.message);
        } finally {
            if (refreshBtn) {
                refreshBtn.disabled = false;
                refreshBtn.innerHTML = '<i class="fas fa-sync-alt"></i> Refresh';
            }
        }
    },

    async refreshCurrent() {
        try {
            switch (APP_STATE.currentTab) {
                case 'balance':
                    if (CONFIG.features.balanceMonitoring) await balanceMonitor.refresh();
                    break;
                case 'transactions':
                    if (CONFIG.features.transactionHistory) await transactionHistory.refresh();
                    break;
                case 'events':
                    if (CONFIG.features.eventMonitoring) await eventMonitor.refresh();
                    break;
                case 'state':
                    if (CONFIG.features.stateMonitoring) await stateMonitor.refresh();
                    break;
            }
        } catch (error) {
            console.error('Auto-refresh error:', error);
        }
    }
};


// Application initialization
document.addEventListener('DOMContentLoaded', function() {
    console.log('Initializing Neo Contract Dashboard');

    // Initialize all modules
    tabManager.init();
    themeManager.init();

    if (CONFIG.features.methodInvocation) {
        methodInvoker.init();
    }

    if (CONFIG.features.balanceMonitoring) {
        balanceMonitor.init();
    }

    if (CONFIG.features.transactionHistory) {
        transactionHistory.init();
    }

    if (CONFIG.features.eventMonitoring) {
        eventMonitor.init();
    }

    if (CONFIG.features.stateMonitoring) {
        stateMonitor.init();
    }

    if (CONFIG.features.walletConnection) {
        walletManager.init();
    }

    if (CONFIG.ui.refreshInterval > 0) {
        autoRefresh.init();
    }

    console.log('Neo Contract Dashboard initialized successfully');
    notifications.success('Dashboard loaded successfully');
});

// Handle page visibility changes
document.addEventListener('visibilitychange', function() {
    if (document.hidden) {
        if (autoRefresh) autoRefresh.stop();
        if (eventMonitor && eventMonitor.isMonitoring) eventMonitor.stopMonitoring();
    } else {
        if (autoRefresh) autoRefresh.start();
    }
});

// Handle errors globally
window.addEventListener('error', function(event) {
    console.error('Global error:', event.error);
    notifications.error('An unexpected error occurred');
});

// Add notification styles if not present
if (!document.querySelector('style[data-notifications]')) {
    const style = document.createElement('style');
    style.setAttribute('data-notifications', 'true');
    style.textContent = `
        .notification {
            position: fixed;
            top: 20px;
            right: 20px;
            background: var(--card-bg);
            border: 1px solid var(--border-color);
            border-radius: 8px;
            padding: 16px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
            z-index: 1000;
            display: flex;
            align-items: center;
            gap: 10px;
            max-width: 400px;
            animation: slideIn 0.3s ease;
        }
        
        .notification-error {
            border-left: 4px solid var(--danger-color);
        }
        
        .notification-success {
            border-left: 4px solid var(--success-color);
        }
        
        .notification-info {
            border-left: 4px solid var(--primary-color);
        }
        
        .notification-close {
            background: none;
            border: none;
            font-size: 18px;
            cursor: pointer;
            color: var(--text-muted);
            margin-left: auto;
        }
        
        @keyframes slideIn {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
    `;
    document.head.appendChild(style);
}

