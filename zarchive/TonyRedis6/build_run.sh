#!bin/bash

(
    pwd
    cd $(dirname "$0")
    pwd
    dotnet build --configuration Release --output /tmp/TonyRedis6/tp-output-dir TonyRedis6.csproj
)

exec /tmp/TonyRedis6/tp-output-dir/TonyRedis6 $@