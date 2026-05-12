#!/bin/bash

(
    pwd
    cd $(dirname "$0")
    pwd
    dotnet build --configuration Release --output /tmp/tpredis002 TPRedis002.csproj
)
exec /tmp/tpredis002/TPRedis002 "$@"

