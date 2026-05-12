#!bin/bash

clear

echo -e "\nRUN: \n"$0""

(
    cd $(dirname "$0")
    echo -e "\nDIR: \n$(pwd)\n"
    dotnet build --configuration Release --output /tmp/TonyRedis11Output/ TonyRedis11.csproj
)

exec /tmp/TonyRedis11Output/TonyRedis11 "$@"
