# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Services/UserService/EasyOrderIdentity.Api/EasyOrderIdentity.Api.csproj \
     src/Services/UserService/EasyOrderIdentity.Api/
RUN dotnet restore src/Services/UserService/EasyOrderIdentity.Api/EasyOrderIdentity.Api.csproj

COPY . .
RUN dotnet publish \
    src/Services/UserService/EasyOrderIdentity.Api/EasyOrderIdentity.Api.csproj \
    -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet","EasyOrderIdentity.Api.dll"]
