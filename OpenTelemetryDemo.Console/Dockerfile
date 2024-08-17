FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OpenTelemetryDemo.Console/OpenTelemetryDemo.Console.csproj", "OpenTelemetryDemo.Console/"]
RUN dotnet restore "OpenTelemetryDemo.Console/OpenTelemetryDemo.Console.csproj"
COPY . .
WORKDIR "/src/OpenTelemetryDemo.Console"
RUN dotnet build "OpenTelemetryDemo.Console.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OpenTelemetryDemo.Console.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetryDemo.Console.dll"]
