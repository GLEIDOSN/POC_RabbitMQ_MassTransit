# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
WORKDIR /app
RUN apk add --no-cache icu-libs icu-data-full
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY ../MassTransit.Web.API /app/MassTransit.Web.API
COPY ../MassTransit.Core /app/MassTransit.Core
COPY MassTransit.Web.API/Certs_tls-gen/client_atlantico03753.p12 /app/Certs_tls-gen/

RUN dotnet restore "/app/MassTransit.Core/MassTransit.Core.csproj" --disable-parallel
RUN dotnet restore "/app/MassTransit.Web.API/MassTransit.Web.API.csproj" --disable-parallel
RUN dotnet publish "/app/MassTransit.Web.API/MassTransit.Web.API.csproj" -c release -o /app/MassTransit.Web.API/publish --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/MassTransit.Web.API/publish .

ENTRYPOINT ["dotnet", "MassTransit.Web.API.dll"]