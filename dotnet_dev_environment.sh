#!/usr/bin/bash

# Other ideas I need:
# Find test.
# Run test.

#############################
# Build and Setup the DevEnv
#############################

SOURCING_PATH="$(cd -- \"$(dirname \"${BASH_SOURCE[0]}\")\" > /dev/null 2>&1 ; pwd -P)"
SCRIPT_PATH="$SOURCING_PATH/$(dirname ${BASH_SOURCE[0]})"
DEVENV_ENG_PATH="$SCRIPT_PATH/DevEnvEngine"
DEVENV_APP="$DEVENV_ENG_PATH/app/DevEnvEngine"

# Build the .NET app here before anything else.
echo 'Building the Dev Env...'
dotnet msbuild "$DEVENV_ENG_PATH/DevEnvEngine.csproj" -t:BuildApp

if [[ "$?" != "0" ]]; then
    echo 'There was a problem building the dev environment. Check the C# failure.'
    return -1
fi

##############################
# Environment Variables Setup
##############################

export DEV_REPOS=""
export WORK_REPO=""
export CORE_ROOT=""
export TEST_ARTIFACTS=""

echo -e '\nSetting architecture to DEV_ARCH...'
export DEV_ARCH=$($DEVENV_APP arch_setup)

echo "DEV_ARCH environment variable was set to '$DEV_ARCH'. You can always export \
a different value manually should you require it."

echo -e '\nSetting operating system to DEV_OS...'
export DEV_OS=$($DEVENV_APP os_setup)

echo "DEV_OS environment variable was set to '$DEV_OS'. You can always export \
a different value manually should you require it."

if [[ ! -z "$1" ]]; then
    echo -e "\nSetting configuration '$1' to DEV_CONFIGURATION..."
    export DEV_CONFIGURATION="$1"
fi

############################
# DevEnv Calling Functions!
############################

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

    if [[ ! -z "$DEV_CONFIGURATION" ]]; then
        export TEST_ARTIFACTS="$WORK_REPO/artifacts/tests/coreclr/$DEV_OS.$DEV_ARCH.$DEV_CONFIGURATION"
        export CORE_ROOT="$TEST_ARTIFACTS/Tests/Core_Root"
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

function repomainscript {
    buildruntimerepo "repo_main_script" "$@"
}

function generatelayout {
    buildruntimerepo "generate_layout" "$@"
}

function testsbuildscript {
    buildruntimerepo "tests_build_script" "$@"
}

function setartifactspath {
    export TEST_ARTIFACTS="$WORK_REPO/artifacts/tests/coreclr/$DEV_OS.$DEV_ARCH.$DEV_CONFIGURATION"
    export CORE_ROOT="$TEST_ARTIFACTS/Tests/Core_Root"
}

function findtest {
    $DEVENV_APP find_test "$@"
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
    export CORE_ROOT=""
    export TEST_ARTIFACTS=""
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
