# Use .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and all project files
COPY ../../*.sln ./
COPY ../API/*.csproj ./API/
COPY ../Application/*.csproj ./Application/
COPY ../Domain/*.csproj ./Domain/
COPY ../Infrastructure/*.csproj ./Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY ../API/. ./API/
COPY ../Application/. ./Application/
COPY ../Domain/. ./Domain/
COPY ../Infrastructure/. ./Infrastructure/

# Build and publish
WORKDIR /src/API
RUN dotnet publish -c Release -o /app/publish

# Use .NET runtime for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Create directory for SQLite database
RUN mkdir -p /app/data

# Expose port
EXPOSE 8080

# Set environment variables - Railway will provide $PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}

# Run the application
ENTRYPOINT ["dotnet", "API.dll"]
