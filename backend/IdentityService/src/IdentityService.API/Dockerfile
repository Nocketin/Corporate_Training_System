
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY IdentityService/src ./IdentityService/src
COPY shared ./shared

RUN dotnet restore "IdentityService/src/IdentityService.API/IdentityService.API.csproj"
RUN dotnet publish "IdentityService/src/IdentityService.API/IdentityService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENTRYPOINT ["dotnet", "IdentityService.API.dll"]
