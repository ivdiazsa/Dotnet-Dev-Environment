#!/usr/bin/bash

export DEV_REPOS=""
export WORK_REPO=""

SOURCING_PATH="$(cd -- \"$(dirname \"${BASH_SOURCE[0]}\")\" > /dev/null 2>&1 ; pwd -P)"
SCRIPT_PATH="$SOURCING_PATH/$(dirname ${BASH_SOURCE[0]})"
DEVENV_ENG_PATH="$SCRIPT_PATH/DevEnvEngine"
DEVENV_APP="$DEVENV_ENG_PATH/app/DevEnvEngine"

# Build the .NET app here before anything else.
dotnet msbuild "$DEVENV_ENG_PATH/DevEnvEngine.csproj" -t:BuildApp

function addrepo {
    local new_repo_kvp;
    local devenv_code;

    new_repo_kvp=$($DEVENV_APP add_repo "$@")
    devenv_code=$?

    if [[ "$devenv_code" != "0" ]]; then
        echo $new_repo_kvp
        return -1
    fi

    if [[ -z "$DEV_REPOS" ]]; then
        export DEV_REPOS="$new_repo_kvp"
    else
        export DEV_REPOS="$DEV_REPOS:$new_repo_kvp"
    fi
}

function listrepos {
    $DEVENV_APP list_repos
}

function setrepo {
    echo 'Set_Repo under construction!'
}

function buildsubsets {
    echo 'Build_Subsets under construction!'
}

function buildtests {
    echo 'Build_Tests under construction!'
}

function generatelayout {
    echo 'Generate_Layout under construction!'
}
