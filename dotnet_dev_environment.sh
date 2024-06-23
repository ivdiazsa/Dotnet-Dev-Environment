#!/usr/bin/bash

export DEV_REPOS=""
export WORK_REPO=""

SOURCING_PATH="$(cd -- \"$(dirname \"${BASH_SOURCE[0]}\")\" > /dev/null 2>&1 ; pwd -P)"
SCRIPT_PATH="$SOURCING_PATH/$(dirname ${BASH_SOURCE[0]})"
DEVENV_ENG_PATH="$SCRIPT_PATH/DevEnvEngine"
DEVENV_APP="$DEVENV_ENG_PATH/app/DevEnvEngine"

# Build the .NET app here before anything else.
dotnet msbuild "$DEVENV_ENG_PATH/DevEnvEngine.csproj" -t:BuildApp

# ENHANCEMENT ITEMS:
#
# - Add a short but comprehensive help message about all the commands.
# - Make 'addrepo' also call 'setrepo' if $WORK_REPO is empty or unset.

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
        export DEV_REPOS="$DEV_REPOS:$addrepo_output"
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

function buildsubsets {
    local buildsubsets_output;
    buildsubsets_output=$($DEVENV_APP build_subsets "$@")
    echo $buildsubsets_output | bash
}

function buildtests {
    echo 'Build_Tests under construction!'
}

function generatelayout {
    local generatelayout_output;
    generatelayout_output=$($DEVENV_APP build_subsets "$@")
    echo $generatelayout_output | bash
}
