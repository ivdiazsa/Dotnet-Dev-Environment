#!/usr/bin/bash

export DEV_REPOS=""
export WORK_REPO=""

SOURCING_PATH="$(cd -- \"$(dirname \"${BASH_SOURCE[0]}\")\" > /dev/null 2>&1 ; pwd -P)"
SCRIPT_PATH="$SOURCING_PATH/$(dirname ${BASH_SOURCE[0]})"
DEVENV_ENG_PATH="$SCRIPT_PATH/DevEnvEngine"
DEVENV_APP="$DEVENV_ENG_PATH/app/DevEnvEngine"

# Build the .NET app here before anything else.
dotnet msbuild "$DEVENV_ENG_PATH/DevEnvEngine.csproj" -t:BuildApp

function devenvhelp {
    echo 'DevEnv Help under construction!'
}

function addrepo {
    local addrepo_output;
    local devenv_code;

    addrepo_output=$($DEVENV_APP add_repo "$@")
    devenv_code=$?

    if [[ "$devenv_code" != "0" ]]; then
        echo $addrepo_output
        return -1
    fi

    if [[ -z "$DEV_REPOS" ]]; then
        export DEV_REPOS="$addrepo_output"
    else
        export DEV_REPOS="$DEV_REPOS;$addrepo_output"
    fi

    if [[ -z "$WORK_REPO" ]]; then
        local info=($(echo $addrepo_output | tr "," "\n"))
        export WORK_REPO="${info[1]}"
    fi
}

function listrepos {
    $DEVENV_APP list_repos
}

function setrepo {
    local setrepo_output;
    local devenv_code;

    setrepo_output=$($DEVENV_APP set_repo "$@")
    devenv_code=$?

    if [[ "$devenv_code" != "0" ]]; then
        echo $setrepo_output
        return -1
    fi

    export WORK_REPO="$setrepo_output"
}

function clearworkspace {
    export DEV_REPOS=""
    export WORK_REPO=""
}
