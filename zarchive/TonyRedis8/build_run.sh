#!bin/bash

clear

echo -e "\nRUN: \n"$0""

(
    cd $(dirname "$0")
    echo -e "\nDIR: \n$(pwd)\n"
    dotnet build --configuration Release --output /tmp/TonyRedis8Output/ TonyRedis8.csproj
)

exec /tmp/TonyRedis8Output/TonyRedis8 "$@"
