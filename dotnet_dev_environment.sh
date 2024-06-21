#!/usr/bin/bash

export DEV_REPOS=""

SOURCING_PATH="$(cd -- \"$(dirname \"${BASH_SOURCE[0]}\")\" > /dev/null 2>&1 ; pwd -P)"
SCRIPT_PATH="$SOURCING_PATH/$(dirname ${BASH_SOURCE[0]})"
DEVENV_ENG_PATH="$SCRIPT_PATH/DevEnvEngine"
DEVENV_APP="$DEVENV_ENG_PATH/app/DevEnvEngine"

# Build the .NET app here before anything else.
dotnet msbuild "$DEVENV_ENG_PATH/DevEnvEngine.csproj" -t:BuildApp

function addrepo {
    repo_env_value=$($DEVENV_APP add_repo "$@")

    if [ -z "$DEV_REPOS" ]; then
        export DEV_REPOS="$repo_env_value"
    else
        export DEV_REPOS="$DEV_REPOS:$repo_env_value"
    fi
}

function listrepos {
    $DEVENV_APP list_repos
}

# function setrepo {
# }

# function buildruntime {
# }

# function buildtests {
# }

# function generatelayout {
# }

# function getcorerun {
# }

# function persistsettings {
# }
