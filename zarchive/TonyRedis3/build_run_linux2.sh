#!/bin/bash
set -e
# test echo 
msg="hello from build_run_linux.sh"

( 
    cd $(dirname "$0")
    dotnet build --output /tmp/tony-redis3-output --configuration Release TonyRedis3.csproj
)

exec /tmp/tony-redis3-output/TonyRedis3 "$@"
