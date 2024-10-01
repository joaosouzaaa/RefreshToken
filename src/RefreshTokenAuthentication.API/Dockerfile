FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/RefreshTokenAuthentication.API/RefreshTokenAuthentication.API.csproj", "src/RefreshTokenAuthentication.API/"]
RUN dotnet restore "./src/RefreshTokenAuthentication.API/RefreshTokenAuthentication.API.csproj"
COPY . .
WORKDIR "/src/src/RefreshTokenAuthentication.API"
RUN dotnet build "./RefreshTokenAuthentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RefreshTokenAuthentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RefreshTokenAuthentication.API.dll"]