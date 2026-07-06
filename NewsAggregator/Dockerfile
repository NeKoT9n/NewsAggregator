FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["NewsAggregator/NewsAggregator/NewsAggregator.csproj", "NewsAggregator/NewsAggregator/"]
COPY ["NewsAggregator/NewsAggregator.DataAccess/NewsAggregator.DataAccess.csproj", "NewsAggregator/NewsAggregator.DataAccess/"]
COPY ["NewsAggregator/NewsAggregator.Domain/NewsAggregator.Domain.csproj", "NewsAggregator/NewsAggregator.Domain/"]
COPY ["NewsAggregator/NewsAggregator.Infostructure/NewsAggregator.Infostructure.csproj", "NewsAggregator/NewsAggregator.Infostructure/"]
COPY ["NewsApi/Shared/Shared.csproj", "NewsApi/Shared/"]

RUN dotnet restore "NewsAggregator/NewsAggregator/NewsAggregator.csproj"

COPY ["NewsAggregator/NewsAggregator", "NewsAggregator/NewsAggregator/"]
COPY ["NewsAggregator/NewsAggregator.DataAccess", "NewsAggregator/NewsAggregator.DataAccess/"]
COPY ["NewsAggregator/NewsAggregator.Domain", "NewsAggregator/NewsAggregator.Domain/"]
COPY ["NewsAggregator/NewsAggregator.Infostructure", "NewsAggregator/NewsAggregator.Infostructure/"]
COPY ["NewsApi/Shared", "NewsApi/Shared/"]

WORKDIR "/src/NewsAggregator/NewsAggregator"
RUN dotnet build "NewsAggregator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NewsAggregator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NewsAggregator.dll"]