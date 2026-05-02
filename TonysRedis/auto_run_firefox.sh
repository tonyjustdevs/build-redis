#!/bin/bash

echo "compiling tony's redis"

(   # change directory to where script is run ???
    cd $(dirname "$0")
    # dotnet compile --
    # lets try open firefox from shell
    ff="/mnt/c/Program Files/Mozilla Firefox/firefox.exe"
    "$ff"
)

