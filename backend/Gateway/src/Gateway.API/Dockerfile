FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Gateway/src ./Gateway/src
COPY shared ./shared

RUN dotnet restore "Gateway/src/Gateway.API/Gateway.API.csproj"
RUN dotnet publish "Gateway/src/Gateway.API/Gateway.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "Gateway.API.dll"]
