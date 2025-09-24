# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/AuthSystem.Api/*.csproj", "AuthSystem.Api/"]
COPY ["src/AuthSystem.Core/*.csproj", "AuthSystem.Core/"]
COPY ["src/AuthSystem.Infrastructure/*.csproj", "AuthSystem.Infrastructure/"]
RUN dotnet restore "AuthSystem.Api/AuthSystem.Api.csproj"

# Copy all files and build
COPY src/ .
RUN dotnet build "AuthSystem.Api/AuthSystem.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AuthSystem.Api/AuthSystem.Api.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "AuthSystem.Api.dll"]
