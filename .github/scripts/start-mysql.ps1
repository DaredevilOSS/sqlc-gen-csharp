# Initializes and starts the MySQL server preinstalled on the Windows runner image and loads the test schemas.
$ErrorActionPreference = 'Stop'
$bin = 'C:\Program Files\MySQL\MySQL Server 8.0\bin'
$dataDir = 'C:\mysql-data'
$ini = "$env:RUNNER_TEMP\my.ini"
Set-Content -Path $ini -Value @"
[mysqld]
datadir=$($dataDir -replace '\\', '/')
local-infile=1
skip-log-bin
innodb-flush-log-at-trx-commit=0
innodb-doublewrite=0
"@

& "$bin\mysqld.exe" --initialize-insecure --console "--defaults-file=$ini"
if ($LASTEXITCODE -ne 0) { throw 'mysqld --initialize-insecure failed' }

# a service (unlike a child process) survives the end of the workflow step
& "$bin\mysqld.exe" --install MySQL "--defaults-file=$ini"
if ($LASTEXITCODE -ne 0) { throw 'mysqld --install failed' }
Start-Service MySQL

for ($i = 0; $i -lt 60; $i++) {
  & "$bin\mysqladmin.exe" -u root ping *> $null
  if ($LASTEXITCODE -eq 0) { break }
  Start-Sleep -Seconds 1
}
if ($LASTEXITCODE -ne 0) { throw 'mysqld did not start' }

& "$bin\mysql.exe" -u root -e "CREATE DATABASE $env:TESTS_DB"
if ($LASTEXITCODE -ne 0) { throw 'create database failed' }
& "$bin\mysql.exe" -u root $env:TESTS_DB --execute="source examples/config/mysql/types/schema.sql; source examples/config/mysql/authors/schema.sql;"
if ($LASTEXITCODE -ne 0) { throw 'loading schemas failed' }
Write-Host 'mysql ready'
