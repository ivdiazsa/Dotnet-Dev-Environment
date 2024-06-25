# Dotnet-Dev-Environment.ps1

# Other ideas I need:
# Find test.
# Run test.

#############################
# Build and Setup the DevEnv
#############################

$scriptPath = $MyInvocation.MyCommand.Path
$sourcingPath = Split-Path $scriptPath -Parent
$devEnvEngPath = Join-Path $sourcingPath 'DevEnvEngine'
$devEnvApp = Join-Path $devEnvEngPath 'app' 'DevEnvEngine.exe'

# Build the .NET app here before anything else.
Write-Output 'Building the Dev Env...'
dotnet msbuild (Join-Path $devEnvEngPath "DevEnvEngine.csproj")

if ($LASTEXITCODE -ne 0) {
    Write-Output 'There was a problem building the dev environment.' `
                 + ' Check the C# failure.'
    exit -1
}

##############################
# Environment Variables Setup
##############################

$Env:DEV_REPOS=""
$Env:WORK_REPO=""
$Env:CORE_ROOT=""
$Env:TEST_ARTIFACTS=""

Write-Output "`nSetting architecture to DEV_ARCH..."
$Env:DEV_ARCH = & $devEnvApp "arch_setup"

Write-Output "DEV_ARCH environment variable was set to '$Env:DEV_ARCH'. You can" `
             + " always export a different value manually, should you require it."

Write-Output "`nSetting architecture to DEV_OS..."
$Env:DEV_OS = & $devEnvApp "os_setup"

Write-Output "DEV_OS environment variable was set to '$Env:DEV_OS'. You can" `
             + " always export a different value manually, should you require it."

if ($args.Length > 0) {
    Write-Output "`nSetting configuration '$($args[0])' to DEV_CONFIGURATION..."
    $Env:DEV_CONFIGURATION = $args[0]
}

############################
# DevEnv Calling Functions!
############################

function Get-DevEnvHelp {
    Write-Host 'DevEnv Help under construction!'
}

function Add-Repo([string[]]$Params) {
    $addRepoOutput = & $devEnvApp "add_repo" $Params
    $devEnvCode = $LASTEXITCODE

    if ($devEnvCode -ne 0) {
        Write-Output $addRepoOutput
        return $devEnvCode
    }

    if (!$Env:DEV_REPOS) {
        $Env:DEV_REPOS = $addRepoOutput
    }
    else {
        $Env:DEV_REPOS += ";$addRepoOutput"
    }

    if (!$Env:WORK_REPO) {
        $info = $addRepoOutput -Split ','
        $Env:WORK_REPO = $info[1]
    }

    if ($Env:DEV_CONFIGURATION) {
        $Env:TEST_ARTIFACTS =      `
          Join-Path $Env:WORK_REPO `
          "artifacts"              `
          "tests"                  `
          "coreclr"                `
          "$Env:DEV_OS.$Env:DEV_ARCH.$Env:DEV_CONFIGURATION"

        $Env:CORE_ROOT = Join-Path $Env:TEST_ARTIFACTS "Tests" "Core_Root"
    }
}

function List-Repos {
    & $devEnvApp "list_repos"
}

function Set-Repo([string[]]$Params) {
    $setRepoOutput = & $devEnvApp "set_repo" $Params
    $devEnvCode = $LASTEXITCODE

    if ($devEnvCode -ne 0) {
        Write-Output $setRepoOutput
        return $devEnvCode
    }

    $Env:WORK_REPO = $setRepoOutput
}

# Don't call this directly. Use the function that best suits your needs:
# - Build-Subsets
# - Generate-Layout
# - Build-ClrTests

function Build-RuntimeRepo([string[]]$Params) {
    $buildType = $Params[0]
    $buildArgs = $Params[1..$Params.Length]

    $buildCmdOutput = & $devEnvApp $buildType $buildArgs
    $devEnvCode = $LASTEXITCODE

    if ($devEnvCode -ne 0) {
        Write-Output $buildCmdOutput
        return $devEnvCode
    }
    & $buildCmdOutput
}

function Build-Subsets([string[]]$Params) {
    $buildParams = @("build_subsets") + $Params
    Build-RuntimeRepo -Params $buildParams
}

function Generate-Layout([string[]]$Params) {
    $buildParams = @("generate_layout") + $Params
    Build-RuntimeRepo -Params $buildParams
}

function Build-ClrTests([string[]]$Params) {
    $buildParams = @("build_clr_tests") + $Params
    Build-RuntimeRepo -Params $buildParams
}

function Set-ArtifactsPath {
    $Env:TEST_ARTIFACTS =      `
      Join-Path $Env:WORK_REPO `
      "artifacts"              `
      "tests"                  `
      "coreclr"                `
      "$Env:DEV_OS.$Env:DEV_ARCH.$Env:DEV_CONFIGURATION"

    $Env:CORE_ROOT = Join-Path $Env:TEST_ARTIFACTS "Tests" "Core_Root"
}

function Get-ActiveRepo {
    if ($Env:WORK_REPO) {
        Write-Output $Env:WORK_REPO
    }
    else {
        Write-Output 'No currently active repo.'
    }
}

function Clear-Workspace {
    $Env:DEV_REPOS = ""
    $Env:WORK_REPO = ""
    $Env:CORE_ROOT = ""
    $Env:TEST_ARTIFACTS = ""
}

function Cd-Repo {
    if ($Env:WORK_REPO) {
        Set-Location $Env:WORK_REPO
    }
    else {
        Write-Output 'No currently active repo.'
    }
}

function Cd-Tests {
    if ($Env:WORK_REPO) {
        Set-Location (Join-Path $Env:WORK_REPO "src" "tests")
    }
    else {
        Write-Output 'No currently active repo.'
    }
}
