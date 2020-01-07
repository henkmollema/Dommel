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

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

exec { & dotnet restore }

$branch = @{ $true = $env:APPVEYOR_REPO_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$env:APPVEYOR_REPO_BRANCH -ne $NULL];
$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision"}[$branch -eq "master" -and $revision -ne "local"]
$commitHash = $(git rev-parse --short HEAD)
$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]
echo "build: Build version suffix is $buildSuffix"

exec { & dotnet build Dommel.sln -c Release --version-suffix=$buildSuffix /p:CI=true }

echo "build: Executing tests"
#exec { & dotnet test test/Dommel.Tests -c Release --no-build }
#exec { & dotnet test test/Dommel.IntegrationTests -c Release --no-build }
#exec { & dotnet test test/Dommel.Json.Tests -c Release --no-build }
#exec { & dotnet test test/Dommel.Json.IntegrationTests -c Release --no-build }

echo "build: Calculating code coverage metrics"

# Create the first coverage in the coverlet JSON format to allow merging
exec { & dotnet test test/Dommel.Tests -c Release -f netcoreapp3.1 --no-build /p:CollectCoverage=true }

# Merge this coverage output with the previous coverage output, this time
# create a report using the opencover format which codecov can parse
Push-Location -Path "test/Dommel.IntegrationTests"
exec { & dotnet test -c Release -f netcoreapp3.1 --no-build /p:CollectCoverage=true /p:MergeWith="..\Dommel.Tests\coverage.netcoreapp3.1.json" /p:CoverletOutputFormat=opencover }
if ($env:APPVEYOR_BUILD_NUMBER) {
    exec { & codecov -f "coverage.netcoreapp3.1.opencover.xml" }
}
Pop-Location

if ($env:APPVEYOR_BUILD_NUMBER) {
    $versionSuffix = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10)
}
else {
    $versionSuffix = $suffix
}

echo "build: Creating NuGet package with suffix $versionSuffix"
exec { & dotnet pack .\src\Dommel\Dommel.csproj -c Release -o .\artifacts --no-build --version-suffix=$versionSuffix }
exec { & dotnet pack .\src\Dommel.Json\Dommel.Json.csproj -c Release -o .\artifacts --no-build --version-suffix=$versionSuffix }
