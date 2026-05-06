#!/bin/bash
(   
    pwd
    cd $(dirname "$0")
    pwd
    dotnet build --configuration Release --output /tmp/tony-output TonyRedis5.csproj
)

exec /tmp/tony-output/TonyRedis5 "$@"