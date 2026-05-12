#!/bin/bash

# test echo 
msg="hello from build_only.sh"
# change into dir where this file is run....
( 
    # pwd # /mnt/c/Users/tonyp/learn/c
    cd $(dirname "$0")
    # pwd # /mnt/c/Users/tonyp/learn/codecrafters-redis/tp/RedisSoln/TonyRedis3
    DOTNETEXE="/mnt/c/Program Files/dotnet/dotnet.exe"
    # "$DOTNETEXE" --version #ok
    "$DOTNETEXE" build --output ./tp-test-output-windows --configuration Release TonyRedis3.csproj
)
