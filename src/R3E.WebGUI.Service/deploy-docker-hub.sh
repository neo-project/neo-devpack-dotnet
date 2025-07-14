#!/bin/bash

# R3E WebGUI Service Docker Hub Deployment Script
# This script deploys the complete R3E WebGUI Service stack using images from Docker Hub

set -e

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuration
DEFAULT_BASE_DOMAIN="localhost"
DEFAULT_DB_PASSWORD="R3E_Strong_Pass_2024!"
COMPOSE_FILE="docker-compose.hub.yml"

# Functions
print_header() {
    echo -e "${BLUE}===================================================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}===================================================================${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ️  $1${NC}"
}

print_step() {
    echo -e "${PURPLE}🔄 $1${NC}"
}

check_prerequisites() {
    print_header "Checking Prerequisites"
    
    # Check if Docker is installed and running
    if ! command -v docker >/dev/null 2>&1; then
        print_error "Docker is not installed. Please install Docker first."
        exit 1
    fi
    
    if ! docker info >/dev/null 2>&1; then
        print_error "Docker daemon is not running. Please start Docker."
        exit 1
    fi
    print_success "Docker is installed and running"
    
    # Check if Docker Compose is available
    if ! command -v docker-compose >/dev/null 2>&1 && ! docker compose version >/dev/null 2>&1; then
        print_error "Docker Compose is not available. Please install Docker Compose."
        exit 1
    fi
    print_success "Docker Compose is available"
    
    # Check if the compose file exists
    if [ ! -f "$COMPOSE_FILE" ]; then
        print_error "Docker Compose file '$COMPOSE_FILE' not found in current directory."
        exit 1
    fi
    print_success "Docker Compose file found"
}

setup_environment() {
    print_header "Setting Up Environment"
    
    # Create .env file if it doesn't exist
    if [ ! -f ".env" ]; then
        print_step "Creating .env file..."
        cat > .env << EOF
# R3E WebGUI Service Configuration
BASE_DOMAIN=${BASE_DOMAIN:-$DEFAULT_BASE_DOMAIN}
DB_PASSWORD=${DB_PASSWORD:-$DEFAULT_DB_PASSWORD}

# Optional: Customize these settings
# NEO_RPC_TESTNET=https://test1.neo.coz.io:443
# NEO_RPC_MAINNET=https://mainnet1.neo.coz.io:443
# ADMIN_EMAIL=admin@your-domain.com
EOF
        print_success ".env file created with default values"
    else
        print_info ".env file already exists, using existing configuration"
    fi
    
    # Load environment variables
    if [ -f ".env" ]; then
        export $(cat .env | grep -v '^#' | xargs)
    fi
}

pull_images() {
    print_header "Pulling Docker Images"
    
    print_step "Pulling R3E WebGUI Service image..."
    docker pull r3enetwork/r3e-webgui-service:latest
    print_success "R3E WebGUI Service image pulled"
    
    print_step "Pulling supporting images..."
    docker pull mcr.microsoft.com/mssql/server:2022-latest
    docker pull nginx:alpine
    print_success "All images pulled successfully"
}

deploy_services() {
    print_header "Deploying Services"
    
    print_step "Starting services with Docker Compose..."
    
    # Use docker-compose or docker compose based on availability
    if command -v docker-compose >/dev/null 2>&1; then
        COMPOSE_CMD="docker-compose"
    else
        COMPOSE_CMD="docker compose"
    fi
    
    $COMPOSE_CMD -f $COMPOSE_FILE up -d
    
    print_success "Services started successfully"
}

wait_for_services() {
    print_header "Waiting for Services to be Ready"
    
    print_step "Waiting for SQL Server to be ready..."
    for i in {1..60}; do
        if docker exec r3e-webgui-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "${DB_PASSWORD:-$DEFAULT_DB_PASSWORD}" -Q "SELECT 1" -C >/dev/null 2>&1; then
            print_success "SQL Server is ready"
            break
        fi
        if [ $i -eq 60 ]; then
            print_error "SQL Server failed to start within 60 seconds"
            exit 1
        fi
        sleep 1
    done
    
    print_step "Waiting for R3E WebGUI Service to be ready..."
    for i in {1..60}; do
        if curl -f http://localhost:8888/health >/dev/null 2>&1; then
            print_success "R3E WebGUI Service is ready"
            break
        fi
        if [ $i -eq 60 ]; then
            print_error "R3E WebGUI Service failed to start within 60 seconds"
            exit 1
        fi
        sleep 1
    done
}

show_status() {
    print_header "Deployment Status"
    
    echo -e "${CYAN}🐳 Container Status:${NC}"
    docker ps --filter "name=r3e-webgui" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
    echo ""
    
    echo -e "${CYAN}📊 Service Health:${NC}"
    echo "• R3E WebGUI Service: http://localhost:8888/health"
    echo "• Main Interface: http://localhost:8888"
    echo "• API Documentation: http://localhost:8888/swagger"
    echo "• SQL Server: localhost:1433 (sa/${DB_PASSWORD:-$DEFAULT_DB_PASSWORD})"
    echo ""
    
    echo -e "${CYAN}📝 Access Information:${NC}"
    echo "• WebGUI Service URL: http://${BASE_DOMAIN:-$DEFAULT_BASE_DOMAIN}:8888"
    echo "• Contract deployment: Use the /api/webgui/deploy-from-manifest endpoint"
    echo "• Documentation: Check README.md for complete usage instructions"
    echo ""
}

cleanup() {
    print_header "Cleanup"
    
    read -p "Do you want to stop and remove all containers? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        print_step "Stopping and removing containers..."
        
        if command -v docker-compose >/dev/null 2>&1; then
            docker-compose -f $COMPOSE_FILE down
        else
            docker compose -f $COMPOSE_FILE down
        fi
        
        print_success "Containers stopped and removed"
        
        read -p "Do you want to remove volumes (data will be lost)? (y/N): " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            print_step "Removing volumes..."
            
            if command -v docker-compose >/dev/null 2>&1; then
                docker-compose -f $COMPOSE_FILE down -v
            else
                docker compose -f $COMPOSE_FILE down -v
            fi
            
            print_success "Volumes removed"
        fi
    fi
}

show_help() {
    echo "R3E WebGUI Service Docker Hub Deployment Script"
    echo ""
    echo "Usage: $0 [COMMAND] [OPTIONS]"
    echo ""
    echo "Commands:"
    echo "  deploy    Deploy the complete R3E WebGUI Service stack (default)"
    echo "  status    Show the current status of deployed services"
    echo "  logs      Show logs from all services"
    echo "  restart   Restart all services"
    echo "  stop      Stop all services"
    echo "  cleanup   Stop and remove all containers and optionally volumes"
    echo "  help      Show this help message"
    echo ""
    echo "Environment Variables:"
    echo "  BASE_DOMAIN     Base domain for the service (default: localhost)"
    echo "  DB_PASSWORD     Database password (default: R3E_Strong_Pass_2024!)"
    echo ""
    echo "Examples:"
    echo "  $0 deploy"
    echo "  BASE_DOMAIN=mysite.com DB_PASSWORD=mypass $0 deploy"
    echo "  $0 status"
    echo "  $0 logs"
    echo ""
}

# Main execution
main() {
    case "${1:-deploy}" in
        "deploy")
            print_header "R3E WebGUI Service Docker Hub Deployment"
            echo -e "${CYAN}Deploying R3E WebGUI Service from Docker Hub...${NC}"
            echo ""
            
            check_prerequisites
            setup_environment
            pull_images
            deploy_services
            wait_for_services
            show_status
            
            print_header "🎉 Deployment Complete! 🎉"
            echo -e "${GREEN}R3E WebGUI Service is now running and ready to use!${NC}"
            ;;
        "status")
            show_status
            ;;
        "logs")
            if command -v docker-compose >/dev/null 2>&1; then
                docker-compose -f $COMPOSE_FILE logs -f
            else
                docker compose -f $COMPOSE_FILE logs -f
            fi
            ;;
        "restart")
            print_step "Restarting services..."
            if command -v docker-compose >/dev/null 2>&1; then
                docker-compose -f $COMPOSE_FILE restart
            else
                docker compose -f $COMPOSE_FILE restart
            fi
            print_success "Services restarted"
            ;;
        "stop")
            print_step "Stopping services..."
            if command -v docker-compose >/dev/null 2>&1; then
                docker-compose -f $COMPOSE_FILE stop
            else
                docker compose -f $COMPOSE_FILE stop
            fi
            print_success "Services stopped"
            ;;
        "cleanup")
            cleanup
            ;;
        "help"|"-h"|"--help")
            show_help
            ;;
        *)
            print_error "Unknown command: $1"
            echo ""
            show_help
            exit 1
            ;;
    esac
}

# Run the main function with all arguments
main "$@"