# Base image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Expose ports for HTTP and HTTPS
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy the project file and restore dependencies
COPY ["Capi.Management/Capi.Management.csproj", "Capi.Management/"]
RUN dotnet restore "Capi.Management/Capi.Management.csproj"
# Copy the rest of the source code
COPY . .
WORKDIR "/src/Capi.Management"
# Build the project
RUN dotnet build "Capi.Management.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
# Publish the project
RUN dotnet publish "Capi.Management.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
# Copy the published output from the publish stage
COPY --from=publish /app/publish .
# Set the entry point for the container
ENTRYPOINT ["dotnet", "Capi.Management.dll"]
