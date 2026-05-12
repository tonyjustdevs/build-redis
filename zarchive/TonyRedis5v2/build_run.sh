#!/bin/bash
(   
    pwd
    cd $(dirname "$0")
    pwd
    dotnet build --configuration Release --output /tmp/tony-output TonyRedis5v2.csproj
)

exec /tmp/tony-output/TonyRedis5v2 "$@"