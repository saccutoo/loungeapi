FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
RUN apt-get update
RUN apt-get install vim -y
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Services/Voucher.API/Voucher.API.csproj src/Services/Voucher.API/
COPY src/BuildingBlocks/Utils/Utils/Utils.csproj src/BuildingBlocks/Utils/Utils/
RUN dotnet restore src/Services/Voucher.API/Voucher.API.csproj
COPY . .
WORKDIR /src/src/Services/Voucher.API
RUN dotnet build Voucher.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Voucher.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Voucher.API.dll"]
