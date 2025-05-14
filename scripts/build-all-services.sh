#!/bin/bash

. ./scripts/lib.sh

project_root=$(pwd)

build_service() {
  local service_path=$1
  local service_name=$2
  echo -e "${PURPLE}Building $service_name service ...${NC}"

  dotnet build --no-incremental --packages "$project_root/data/nuget" $service_path 2>&1 |
    sed -E \
      -e "/(warning|warn|wrn)/I s/.*/$(printf "${WARNING}&${NC}")/" \
      -e "/(error|err)/I s/.*/$(printf "${DANGER}&${NC}")/"
}

[[ "$PLATFORM" != "windows" ]] && sudo chmod 777 data -R && echo -e "${GREEN}Run chmod 777 for ./data directory successfully${NC}"

# Publishing Contract solution
echo -e "${PURPLE}Publishing Contract solution ...${NC}"
dotnet publish --packages "$project_root/data/nuget" -o ./app/server/Contract/Contract/Published ./app/server/Contract/Contract

build_service "./app/server/Contract/Contract" "contract" &&
build_service "./app/server/IdentityService/src/IdentityService.API" "identity" &&
build_service "./app/server/UserService/src/UserService.API" "user" &&
build_service "./app/server/TaskService/src/TaskService.API" "task"