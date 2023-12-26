#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN groupadd -r app && useradd -r -g app -m -d /app app
USER app
WORKDIR /app
EXPOSE 8080
# EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TrampolineCenterAPI/TrampolineCenterAPI.csproj", "."]
RUN dotnet restore "./././TrampolineCenterAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./TrampolineCenterAPI/TrampolineCenterAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TrampolineCenterAPI/TrampolineCenterAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrampolineCenterAPI.dll"]