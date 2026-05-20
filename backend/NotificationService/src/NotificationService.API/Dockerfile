FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY NotificationService/src ./NotificationService/src
COPY shared ./shared

RUN dotnet restore "NotificationService/src/NotificationService.API/NotificationService.API.csproj"
RUN dotnet publish "NotificationService/src/NotificationService.API/NotificationService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5003
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
