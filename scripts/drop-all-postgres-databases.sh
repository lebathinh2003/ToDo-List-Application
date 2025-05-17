#!/bin/bash

. ./scripts/lib.sh

project_root=$(pwd)

drop_database() {
    local service_path=$1
    local service_name=$2

    echo -e "\e[96mDropping $service_name database ....\e[0m"
    cd $service_path  
    dotnet ef database drop -f
    cd "$project_root"
}

drop_database "./app/server/IdentityService/src/IdentityService.Infrastructure" "Identity" &&
drop_database "./app/server/TaskService/src/TaskService.Infrastructure" "Task" &&
drop_database "./app/server/UserService/src/UserService.Infrastructure" "User"
