FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app
COPY *.sln .

# copying only project files - TODO this and below sections
COPY DebugHelper/*.csproj ./DebugHelper/
COPY SqlcProtobuf/*.csproj ./SqlcProtobuf/
COPY DriverCommon/*.csproj ./DriverCommon/
COPY CodeGenerator/*.csproj ./CodeGenerator/
COPY StringExtensions/*.csproj ./StringExtensions/
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
COPY DriverCommon/ ./DriverCommon/
COPY CodeGenerator/ ./CodeGenerator/
COPY StringExtensions/ ./StringExtensions/
COPY MySqlConnectorExample/ ./MySqlConnectorExample/
COPY MySqlConnectorDriver/ ./MySqlConnectorDriver/
COPY NpgsqlExample/ ./NpgsqlExample/
COPY NpgsqlDriver/ ./NpgsqlDriver/
COPY SqlcGenCsharp/ ./SqlcGenCsharp/
COPY SqlcGenCsharpProcess/ ./SqlcGenCsharpProcess/
COPY SqlcGenCsharpWasm/ ./SqlcGenCsharpWasm/
COPY SqlcGenCsharpTests/ ./SqlcGenCsharpTests/

RUN dotnet restore
