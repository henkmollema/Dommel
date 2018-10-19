function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

if(Test-Path .\src\Dommel\artifacts) { Remove-Item .\src\Dommel\artifacts -Force -Recurse }

exec { & dotnet --info }
exec { & dotnet restore }

$branch = @{ $true = $env:APPVEYOR_REPO_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$env:APPVEYOR_REPO_BRANCH -ne $NULL];
$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision"}[$branch -eq "master" -and $revision -ne "local"]
$commitHash = $(git rev-parse --short HEAD)
$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]
echo "build: Build version suffix is $buildSuffix"

exec { & dotnet build Dommel.sln -c Release --version-suffix=$buildSuffix /p:CI=true }

echo "build: Executing tests"
Push-Location -Path .\test\Dommel.Tests
exec { & dotnet test -c Release --no-build }
Pop-Location

if ($env:APPVEYOR_BUILD_NUMBER) {
    $versionSuffix = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10)
}
else {
    $versionSuffix = $suffix
}

echo "build: Creating NuGet package with suffix $versionSuffix"
exec { & dotnet pack .\src\Dommel\Dommel.csproj -c Release -o .\artifacts --no-build --version-suffix=$versionSuffix }
