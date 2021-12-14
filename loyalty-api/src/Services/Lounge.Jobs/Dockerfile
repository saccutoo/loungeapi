FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY SHBMicroServiceCoreFW.sln ./
COPY src/Services/Lounge.Jobs/Lounge.Jobs.csproj src/Services/Lounge.Jobs/
COPY src/BuildingBlocks/Utils/Utils/Utils.csproj src/BuildingBlocks/Utils/Utils/
RUN dotnet restore "src/Services/Lounge.Jobs/Lounge.Jobs.csproj"
COPY . .
WORKDIR /src/src/Services/Lounge.Jobs
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Lounge.Jobs.dll"]
