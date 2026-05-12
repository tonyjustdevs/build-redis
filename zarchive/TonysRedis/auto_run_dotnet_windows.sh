#!/bin/bash

echo "compiling tony's redis"

(   # change directory to where script is run ???
    cd $(dirname "$0")
    DOTNETEXE="/mnt/c/Program Files/dotnet/dotnet.exe"
    "$DOTNETEXE" build --output tonys-test-output-2 --configuration Release TonysRedis.csproj
)

q