# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /App

# Copy the project file and restore dependencies
COPY OrderManagementService.csproj ./OrderManagementService.csproj
RUN dotnet restore ./OrderManagementService.csproj

# Copy all application files and publish the application
COPY . ./
RUN dotnet publish ./OrderManagementService.csproj -c Release -o out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /App
COPY --from=build-env /App/out .

# Specify the entry point to run the application
ENTRYPOINT ["dotnet", "OrderManagementService.dll"]

