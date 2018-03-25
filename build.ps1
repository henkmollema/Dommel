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

exec { & dotnet restore }

$versionSuffix = $(git rev-parse --short HEAD)
if ($env:APPVEYOR_BUILD_NUMBER) {
    $versionSuffix = $env:APPVEYOR_BUILD_NUMBER
}

echo "build: Package version suffix is $versionSuffix"

exec { & dotnet build Dommel.sln -c Release --version-suffix=$versionSuffix }

echo "Executing tests"
Push-Location -Path .\test\Dommel.Tests
exec { & dotnet test -c Release --no-build }
Pop-Location

echo "Creating NuGet package"
exec { & dotnet pack .\src\Dommel\Dommel.csproj -c Release -o .\artifacts --no-build --version-suffix=$versionSuffix }
