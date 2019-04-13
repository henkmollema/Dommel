dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Include="[Dommel]*"
reportgenerator "-reports:coverage.opencover.xml" "-targetdir:coveragereport"
start coveragereport\index.htm
