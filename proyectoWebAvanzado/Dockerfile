# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["proyectoWebAvanzado.csproj", "./"]
RUN dotnet restore "./proyectoWebAvanzado.csproj"

COPY . .
RUN dotnet publish "./proyectoWebAvanzado.csproj" \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM runtime AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "proyectoWebAvanzado.dll"]
