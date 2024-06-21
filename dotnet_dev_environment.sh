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
    local repo_env_value=$($DEVENV_APP add_repo "$@")

    if [ -z "$DEV_REPOS" ]; then
        export DEV_REPOS="$repo_env_value"
    else
        export DEV_REPOS="$DEV_REPOS:$repo_env_value"
    fi
}

function listrepos {
    $DEVENV_APP list_repos
}

function setrepo {
    local repo_path=$($DEVENV_APP set_repo "$@")
    export WORK_REPO="$repo_path"
}

function buildruntime {
    local build_cmdline=$($DEVENV_APP build_runtime "$@")
    echo "$build_cmdline" | bash
}

function buildtests {
    echo 'Build_Tests is under construction!'
}

function generatelayout {
    echo 'Generate_Layout is under construction!'
}

function getcorerun {
    echo 'Get_Corerun is under construction!'
}

function persistsettings {
    echo 'Persist_Settings is under construction!'
}
