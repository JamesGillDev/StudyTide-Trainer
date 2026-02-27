FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["StudyTide Forge Web/StudyTide Forge Web.csproj", "StudyTide Forge Web/"]
RUN dotnet restore "StudyTide Forge Web/StudyTide Forge Web.csproj"

COPY . .
WORKDIR "/src/StudyTide Forge Web"
RUN dotnet publish "StudyTide Forge Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV FORGE_WEB_DB_PATH=/data/studytide-forge-web.db
ENV FORGE_IMPORT_SOURCE_FILE=/app/seed-source/legacy-source.cs

RUN mkdir -p /data
VOLUME ["/data"]

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "StudyTideForgeWeb.dll"]

