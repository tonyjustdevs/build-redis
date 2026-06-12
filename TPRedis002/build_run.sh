#!/bin/bash

clear

(
    pwd
    cd $(dirname "$0")
    pwd
    dotnet build --configuration Release --output /tmp/tpredis002 TPRedis002.csproj
    # dotnet build --configuration Release --output /tmp/tpredis002 TPRedis002.csproj -v diag
)
exec /tmp/tpredis002/TPRedis002 "$@"

