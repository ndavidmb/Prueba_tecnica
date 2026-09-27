FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Domain/Domain.csproj",                  "src/Domain/"]
COPY ["src/Application/Application.csproj",         "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj",   "src/Infrastructure/"]
COPY ["src/WebApi/WebApi.csproj",                   "src/WebApi/"]
RUN dotnet restore "src/WebApi/WebApi.csproj"

COPY . .
RUN dotnet publish "src/WebApi/WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS final
WORKDIR /app

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# App publicada
COPY --from=build /app/publish .

# Código fuente + caché de NuGet para que dotnet-ef pueda compilar y generar migraciones
COPY --from=build /root/.nuget /root/.nuget
COPY --from=build /src/src ./src

# Restaura usando el caché local (sin red)
RUN dotnet restore src/WebApi/WebApi.csproj

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "WebApi.dll"]
