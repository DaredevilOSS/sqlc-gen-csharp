FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app
COPY *.sln .

# copying only project files - TODO this and below sections
COPY DebugHelper/*.csproj ./DebugHelper/
COPY SqlcProtobuf/*.csproj ./SqlcProtobuf/
COPY UtilsDriver/*.csproj ./UtilsDriver/
COPY CodeGenerator/*.csproj ./CodeGenerator/

COPY RoslynExtensions/*.csproj ./RoslynExtensions/
COPY StringExtensions/*.csproj ./StringExtensions/
COPY ListExtensions/*.csproj ./ListExtensions/

COPY MySqlConnectorExample/*.csproj ./MySqlConnectorExample/
COPY MySqlConnectorDriver/*.csproj ./MySqlConnectorDriver/
COPY NpgsqlExample/*.csproj ./NpgsqlExample/
COPY NpgsqlDriver/*.csproj ./NpgsqlDriver/

COPY SqlcGenCsharp/*.csproj ./SqlcGenCsharp/
COPY SqlcGenCsharpProcess/*.csproj ./SqlcGenCsharpProcess/
COPY SqlcGenCsharpWasm/*.csproj ./SqlcGenCsharpWasm/
COPY SqlcGenCsharpTests/*.csproj ./SqlcGenCsharpTests/

# copying the rest of the files
COPY DebugHelper/ ./DebugHelper/
COPY SqlcProtobuf/ ./SqlcProtobuf/
COPY CodeGenerator/ ./CodeGenerator/

COPY RoslynExtensions/ ./RoslynExtensions/
COPY StringExtensions/ ./StringExtensions/
COPY ListExtensions/ ./ListExtensions/

COPY UtilsDriver/ ./UtilsDriver/
COPY MySqlConnectorDriver/ ./MySqlConnectorDriver/
COPY NpgsqlDriver/ ./NpgsqlDriver/

COPY MySqlConnectorExample/ ./MySqlConnectorExample/
COPY NpgsqlExample/ ./NpgsqlExample/

COPY SqlcGenCsharp/ ./SqlcGenCsharp/
COPY SqlcGenCsharpProcess/ ./SqlcGenCsharpProcess/
COPY SqlcGenCsharpWasm/ ./SqlcGenCsharpWasm/
COPY SqlcGenCsharpTests/ ./SqlcGenCsharpTests/

RUN dotnet restore
