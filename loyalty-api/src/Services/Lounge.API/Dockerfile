#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
RUN apt-get update
RUN apt-get install vim -y
WORKDIR /app
ADD /src/Services/Lounge.API/Attachs /app/Attachs
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Services/Lounge.API/Lounge.API.csproj", "src/Services/Lounge.API/"]
COPY ["src/BuildingBlocks/Utils/Utils/Utils.csproj", "src/BuildingBlocks/Utils/Utils/"]
RUN dotnet restore "src/Services/Lounge.API/Lounge.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Lounge.API"
RUN dotnet build "Lounge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lounge.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lounge.API.dll"]
