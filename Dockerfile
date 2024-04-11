# Use the official Microsoft .NET SDK image to build the solution
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory inside the container
WORKDIR /app

# Copy the csproj and restore any dependencies (via NuGet)
COPY *.sln .

# copying only project files
COPY DebugHelper/*.csproj ./DebugHelper/
COPY CodegenProtobuf/*.csproj ./CodegenProtobuf/
COPY DriverCommon/*.csproj ./DriverCommon/
COPY CodeGenerator/*.csproj ./CodeGenerator/
COPY StringExtensions/*.csproj ./StringExtensions/
COPY MySqlConnectorExample/*.csproj ./MySqlConnectorExample/
COPY MySqlConnectorDriver/*.csproj ./MySqlConnectorDriver/
COPY NpgsqlExample/*.csproj ./NpgsqlExample/
COPY NpgsqlDriver/*.csproj ./NpgsqlDriver/
COPY SqlcGenCsharp/*.csproj ./SqlcGenCsharp/
COPY SqlcGenCsharpTests/*.csproj ./SqlcGenCsharpTests/

# copying the rest of the files
COPY DebugHelper/ ./DebugHelper/
COPY CodegenProtobuf/ ./CodegenProtobuf/
COPY DriverCommon/ ./DriverCommon/
COPY CodeGenerator/ ./CodeGenerator/
COPY StringExtensions/ ./StringExtensions/
COPY MySqlConnectorExample/ ./MySqlConnectorExample/
COPY MySqlConnectorDriver/ ./MySqlConnectorDriver/
COPY NpgsqlExample/ ./NpgsqlExample/
COPY NpgsqlDriver/ ./NpgsqlDriver/
COPY SqlcGenCsharp/ ./SqlcGenCsharp/
COPY SqlcGenCsharpTests/ ./SqlcGenCsharpTests/

RUN dotnet restore
