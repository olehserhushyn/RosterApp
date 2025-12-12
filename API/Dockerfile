# ===================== BUILD STAGE =====================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy everything from the API folder into the container
COPY . .

# Restore only the Web API project (entry point)
RUN dotnet restore RosterApp.API/RosterApp.API.csproj

# Publish the Web API
WORKDIR /src/RosterApp.API
RUN dotnet publish -c Release -o /app/publish

# ===================== RUNTIME STAGE =====================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish ./

# Kestrel will listen on port 5000 inside the container
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# This DLL name matches your folder/project: RosterApp.API
ENTRYPOINT ["dotnet", "RosterApp.API.dll"]
