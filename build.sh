dotnet restore
dotnet build
dotnet test test/Dommel.Tests --no-build
dotnet test test/Dommel.IntegrationTests --no-build
dotnet test test/Dommel.Json.Tests --no-build
dotnet test test/Dommel.Json.IntegrationTests --no-build