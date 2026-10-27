# Starts SQL Server Express LocalDB on the Windows runner and points the tests at it (GlobalSetup creates the database).
$ErrorActionPreference = 'Stop'

if (-not (Get-Command sqllocaldb -ErrorAction SilentlyContinue)) {
  choco install sqllocaldb -y --no-progress
  if ($LASTEXITCODE -ne 0) { throw 'installing LocalDB failed' }
  $env:Path = [Environment]::GetEnvironmentVariable('Path', 'Machine') + ';' + [Environment]::GetEnvironmentVariable('Path', 'User')
}

sqllocaldb create MSSQLLocalDB -s
if ($LASTEXITCODE -ne 0) {
  sqllocaldb start MSSQLLocalDB
  if ($LASTEXITCODE -ne 0) { throw 'starting LocalDB failed' }
}
sqllocaldb info MSSQLLocalDB

# later steps read it; it wins over the .env value (EndToEndCommon does not overwrite existing variables)
"MSSQL_CONNECTION_STRING=Server=(localdb)\MSSQLLocalDB;Database=$env:TESTS_DB;Integrated Security=true;TrustServerCertificate=True" |
  Out-File -FilePath $env:GITHUB_ENV -Append -Encoding utf8
Write-Host 'mssql ready'
