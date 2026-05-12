#!/bin/bash
# 4. [from linux .sh]: run [linux dotnet] build with custom path

echo "compiling tony's redis"

(   # change directory to where script is run ???
    cd $(dirname "$0")
    # DOTNETEXE="/mnt/c/Program Files/dotnet/dotnet.exe"
    dotnet build --output tonys-test-output-3 --configuration Release TonysRedis.csproj
    # dotnet --version                  # WORKS

)