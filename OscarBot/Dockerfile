FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /app

COPY OscarBot.csproj .
RUN dotnet restore "./OscarBot.csproj"
COPY . .
RUN dotnet build "OscarBot.csproj" -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "OscarBot.dll"]
