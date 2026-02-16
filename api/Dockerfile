FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SalesSystem.Api/SalesSystem.Api.csproj ./SalesSystem.Api/
COPY SalesSystem.Application/SalesSystem.Application.csproj ./SalesSystem.Application/
COPY SalesSystem.Domain/SalesSystem.Domain.csproj ./SalesSystem.Domain/
COPY SalesSystem.Infrastructure/SalesSystem.Infrastructure.csproj ./SalesSystem.Infrastructure/

RUN dotnet restore SalesSystem.Api/SalesSystem.Api.csproj

COPY . .
RUN dotnet publish SalesSystem.Api/SalesSystem.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "SalesSystem.Api.dll"]
