# Stage 1: Build Environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory for building the application
WORKDIR /app

# Expose the application port
EXPOSE 8083

# Copy the source code to the working directory
COPY . .

# Set the working directory to the specific project folder
WORKDIR "/app/ExpenSpend.Web"

# Restore dependencies
RUN dotnet restore

# Publish the application
RUN dotnet publish ExpenSpend.Web.csproj -c Release -o out

# Stage 2: Runtime Environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-env

# Set the working directory for the runtime environment
WORKDIR /app

# Copy the published application from the build environment
COPY --from=build-env /app/ExpenSpend.Web/out .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "ExpenSpend.Web.dll"]