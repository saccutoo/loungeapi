#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
RUN apt-get update
RUN apt-get install vim -y
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Services/Promotion.API/Promotion.API.csproj", "src/Services/Promotion.API/"]
COPY ["src/BuildingBlocks/Utils/Utils/Utils.csproj", "src/BuildingBlocks/Utils/Utils/"]
RUN dotnet restore "src/Services/Promotion.API/Promotion.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Promotion.API"
RUN dotnet build "Promotion.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Promotion.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Promotion.API.dll"]
