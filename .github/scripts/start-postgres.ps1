# Starts the PostgreSQL service preinstalled on the Windows runner image and loads the test schemas.
$ErrorActionPreference = 'Stop'
$psql = Join-Path $env:PGROOT 'bin\psql.exe'
function Invoke-Psql { & $psql -v ON_ERROR_STOP=1 @args; if ($LASTEXITCODE -ne 0) { throw "psql failed: $args" } }

Set-Service postgresql-x64-17 -StartupType Manual
Start-Service postgresql-x64-17

# The image sets the superuser password to 'root'; tests expect the one from .env
$env:PGPASSWORD = 'root'
Invoke-Psql -U postgres -c "CREATE DATABASE $env:TESTS_DB"
Invoke-Psql -U postgres -c "ALTER USER postgres PASSWORD '$env:POSTGRES_PASSWORD'"

$env:PGPASSWORD = $env:POSTGRES_PASSWORD
Invoke-Psql -U postgres -d $env:TESTS_DB -f 'examples/config/postgresql/types/schema.sql' -f 'examples/config/postgresql/authors/schema.sql'
Write-Host 'postgres ready'
