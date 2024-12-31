FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Falc.Communications.Api/Falc.Communications.Api.csproj", "Falc.Communications.Api/"]
COPY ./NuGet.Config .
RUN dotnet restore "Falc.Communications.Api/Falc.Communications.Api.csproj"

COPY . .
WORKDIR "/src/Falc.Communications.Api"
RUN dotnet build "Falc.Communications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Falc.Communications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Falc.Communications.Api.dll"]
