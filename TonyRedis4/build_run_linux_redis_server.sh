#!bin/bash

(
    pwd
    cd $(dirname $0) # cd `dirname $0`
    pwd
    dotnet build --configuration Release --output /tmp/TonyRedis4 TonyRedis4.csproj
)
