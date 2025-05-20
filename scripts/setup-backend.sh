#!/bin/bash
set -e

GREEN='\033[0;32m'
RED='\033[0;31m'
CYAN='\033[0;36m'
NC='\033[0m'

# Phát hiện hệ điều hành
IS_WINDOWS=false
if grep -qi microsoft /proc/version 2>/dev/null || [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
  IS_WINDOWS=true
fi

# Kiểm tra dotnet
check_dotnet() {
  if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}[ERROR] .NET SDK not found.${NC}"
    if $IS_WINDOWS; then
      echo -e "${CYAN}Opening .NET SDK download page for Windows...${NC}"
      start "" "https://dotnet.microsoft.com/en-us/download/dotnet/8.0"
    else
      echo -e "${CYAN}Installing .NET SDK on Linux...${NC}"
      wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
      chmod +x dotnet-install.sh
      ./dotnet-install.sh --channel 8.0 --install-dir $HOME/.dotnet
      export DOTNET_ROOT=$HOME/.dotnet
      export PATH=$DOTNET_ROOT:$PATH
      echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
      echo 'export PATH=$DOTNET_ROOT:$PATH' >> ~/.bashrc
    fi
    echo -e "${RED}Please restart the terminal and run the script again.${NC}"
    exit 1
  else
    echo -e "${GREEN}[OK] .NET SDK is installed.${NC}"
  fi
}

# Kiểm tra dotnet-ef
check_dotnet_ef() {
  if ! dotnet tool list -g | grep -q dotnet-ef; then
    echo -e "${CYAN}Installing dotnet-ef...${NC}"
    dotnet tool install -g dotnet-ef
    export PATH="$PATH:$HOME/.dotnet/tools"
    echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.bashrc
  else
    echo -e "${GREEN}[OK] dotnet-ef is installed.${NC}"
  fi
}

# Đợi container healthy
wait_for_healthy() {
  local container=$1
  echo -e "${CYAN}Waiting for $container to become healthy...${NC}"
  for _ in {1..30}; do
    status=$(docker inspect --format='{{.State.Health.Status}}' "$container" 2>/dev/null || echo "no-container")
    if [ "$status" == "healthy" ]; then
      echo -e "${GREEN}[$container] is healthy.${NC}"
      return
    fi
    if [ "$status" == "unhealthy" ]; then
      echo -e "${RED}[$container] is unhealthy.${NC}"
      exit 1
    fi
    sleep 2
  done
  echo -e "${RED}Timeout waiting for $container to become healthy.${NC}"
  exit 1
}

# Bắt đầu setup
echo -e "${CYAN}=== Starting Backend Setup ===${NC}"

check_dotnet
check_dotnet_ef
. ./scripts/lib.sh && check_docker

echo -e "${CYAN}Starting docker-compose...${NC}"
docker-compose up -d
printf "\n\t*** ${GREEN}DONE RUNNING CONTAINER${NC} ***\n\n"

# wait_for_healthy "sqlserver"
wait_for_healthy "service-bus"

./scripts/build-all-services.sh
printf "\n\t*** ${GREEN}DONE BUILDING ALL SERVICES${NC} ***\n\n"

./scripts/apply-all-migrations.sh
printf "\n\t*** ${GREEN}DONE APPLY ALL MIGRATIONS${NC} ***\n\n"

echo -e "${GREEN}=== Backend Setup Done ===${NC}"
