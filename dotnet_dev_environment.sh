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
        return $devenv_code
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
        return $devenv_code
    fi

    export WORK_REPO="$setrepo_output"
}

# Don't call this directly. Use the function that best suits your needs:
# - buildsubsets
# - generatelayout
# - buildclrtests

function buildruntimerepo {
    local buildcmd_output;
    local devenv_code;

    local build_args=($@)
    local build_type="${build_args[0]}"
    build_args=("${build_args[@]:1}")

    buildcmd_output=$($DEVENV_APP $build_type "${build_args[@]}")
    devenv_code=$?

    if [[ "$devenv_code" != "0" ]]; then
        echo $buildcmd_output
        return $devenv_code
    fi

    echo $buildcmd_output | bash
}

function buildsubsets {
    buildruntimerepo "build_subsets" "$@"
}

function generatelayout {
    buildruntimerepo "generate_layout" "$@"
}

function buildclrtests {
    buildruntimerepo "build_clr_tests" "$@"
}

function activerepo {
    if [[ -z "$WORK_REPO" ]]; then
        echo 'No currently active repo.'
    else
        echo $WORK_REPO
    fi
}

function clearworkspace {
    export DEV_REPOS=""
    export WORK_REPO=""
}

function cdrepo {
    if [[ -z "$WORK_REPO" ]]; then
        echo 'No currently active repo.'
    else
        cd $WORK_REPO
    fi
}

function cdtests {
    if [[ -z "$WORK_REPO" ]]; then
        echo 'No currently active repo.'
    else
        cd $WORK_REPO/src/tests
    fi
}
