# Use .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build

# Copy everything
COPY . .

# Debug: List what we have
RUN ls -la
RUN ls -la src/ || echo "src folder not found"
RUN ls -la src/API/ || echo "src/API folder not found"

# Restore dependencies
RUN dotnet restore ProjectManager.sln

# Build and publish
WORKDIR /build/src/API
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
