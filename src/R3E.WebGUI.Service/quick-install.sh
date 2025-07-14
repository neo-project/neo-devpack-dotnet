#!/bin/bash

# ============================================================================
# R3E WebGUI Service - Quick Install Script
# 
# One-line installation command:
# curl -sSL https://raw.githubusercontent.com/neo-project/neo-devpack-dotnet/r3e/src/R3E.WebGUI.Service/quick-install.sh | sudo bash
# ============================================================================

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${BLUE}=== R3E WebGUI Service Quick Installer ===${NC}"
echo ""
echo -e "${YELLOW}This will install the complete R3E WebGUI Service on your server.${NC}"
echo -e "${YELLOW}Requirements: Ubuntu/Debian/CentOS, 4GB+ RAM, ports 80,443,8888 available${NC}"
echo ""

# Download and run the full setup script
curl -sSL https://raw.githubusercontent.com/neo-project/neo-devpack-dotnet/r3e/src/R3E.WebGUI.Service/setup-r3e-webgui.sh -o /tmp/setup-r3e-webgui.sh
chmod +x /tmp/setup-r3e-webgui.sh
/tmp/setup-r3e-webgui.sh

# Cleanup
rm -f /tmp/setup-r3e-webgui.sh

echo -e "${GREEN}Installation complete!${NC}"