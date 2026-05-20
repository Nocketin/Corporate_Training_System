FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY LearningService/src ./LearningService/src
COPY shared ./shared

RUN dotnet restore "LearningService/src/LearningService.API/LearningService.API.csproj"
RUN dotnet publish "LearningService/src/LearningService.API/LearningService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5002
ENTRYPOINT ["dotnet", "LearningService.API.dll"]
