dotnet test -f netcoreapp3.1 /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Include="[Dommel]*"
reportgenerator "-reports:coverage.netcoreapp3.1.opencover.xml" "-targetdir:coveragereport"
start coveragereport\index.htm
