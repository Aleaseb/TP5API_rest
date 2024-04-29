#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Le Dockerfile généré auto a des problèmes :
#   Il copie pas trop du bon endroit. Sauf si on met le compose et les autres dossiers avec le .sln mais c'est moche
#   Plus inverser COPY et WORKDIR sinon il trouve pas le main
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
#COPY ["ChepoAPI/ChepoAPI.csproj", "ChepoAPI/"]
COPY ["ChepoAPI.csproj", "ChepoAPI/"]
RUN dotnet restore "ChepoAPI/ChepoAPI.csproj"
#COPY . .
#WORKDIR "/src/ChepoAPI"
WORKDIR "/src/ChepoAPI"
COPY . .
RUN dotnet build "ChepoAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChepoAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChepoAPI.dll"]