# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ../MassTransit.Worker /app/MassTransit.Worker
COPY ../MassTransit.Core /app/MassTransit.Core
COPY MassTransit.Worker/Certs_tls-gen/client_atlantico03753.p12 /app/Certs_tls-gen/

RUN dotnet restore "/app/MassTransit.Core/MassTransit.Core.csproj" --disable-parallel
RUN dotnet restore "/app/MassTransit.Worker/MassTransit.Worker.csproj" --disable-parallel
RUN dotnet publish "/app/MassTransit.Worker/MassTransit.Worker.csproj" -c release -o /app/MassTransit.Worker/publish --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/MassTransit.Worker/publish .

ENTRYPOINT ["dotnet", "MassTransit.Worker.dll"]