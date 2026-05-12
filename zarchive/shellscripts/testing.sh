#!/bin/bash

  # cd "$(dirname "$0")" # Ensure compile steps are run within the repository directory

pwd
cd $(dirname "$0")
pwd

# "$0" ---> $0 output how script was run:
# $0 can be:
# - testing.sh
# - ./testing.sh
# - /../../testing.sh