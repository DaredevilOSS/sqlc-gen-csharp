# Starts MySQL on the Windows runner and loads the test schemas. Uses the MySQL preinstalled on the runner image;
# the image content changes over time, so when it is missing, falls back to installing it with Chocolatey.
$ErrorActionPreference = 'Stop'
$chocoBin = 'C:\tools\mysql\current\bin'
$bin = @(
  (Get-Command mysqld.exe -ErrorAction SilentlyContinue | ForEach-Object { Split-Path $_.Source }),
  (Get-ChildItem 'C:\Program Files\MySQL\MySQL Server *\bin\mysqld.exe' -ErrorAction SilentlyContinue | Sort-Object FullName -Descending | ForEach-Object { Split-Path $_.FullName })
) | Where-Object { $_ } | Select-Object -First 1
if ($bin) {
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

  & "$bin\mysqld.exe" "--defaults-file=$ini" --initialize-insecure --console
  if ($LASTEXITCODE -ne 0) { throw 'mysqld --initialize-insecure failed' }

  # a service (unlike a child process) survives the end of the workflow step
  & "$bin\mysqld.exe" --install MySQL "--defaults-file=$ini"
  if ($LASTEXITCODE -ne 0) { throw 'mysqld --install failed' }
  Start-Service MySQL
} else {
  Write-Host 'MySQL is not preinstalled on this runner image - installing with Chocolatey'
  choco install mysql --no-progress -y --params "/serviceName:MySQL"
  if ($LASTEXITCODE -ne 0) { throw 'choco install mysql failed' }
  $bin = $chocoBin
}

for ($i = 0; $i -lt 60; $i++) {
  & "$bin\mysqladmin.exe" -u root ping *> $null
  if ($LASTEXITCODE -eq 0) { break }
  Start-Sleep -Seconds 1
}
if ($LASTEXITCODE -ne 0) { throw 'mysqld did not start' }

& "$bin\mysql.exe" -u root -e "SET GLOBAL local_infile=1; CREATE DATABASE $env:TESTS_DB"
if ($LASTEXITCODE -ne 0) { throw 'create database failed' }
& "$bin\mysql.exe" -u root $env:TESTS_DB --execute="source examples/config/mysql/types/schema.sql; source examples/config/mysql/authors/schema.sql;"
if ($LASTEXITCODE -ne 0) { throw 'loading schemas failed' }
Write-Host 'mysql ready'
