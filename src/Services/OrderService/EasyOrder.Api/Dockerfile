# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Services/OrderService/EasyOrder.Api/EasyOrder.Api.csproj \
     src/Services/OrderService/EasyOrder.Api/
RUN dotnet restore src/Services/OrderService/EasyOrder.Api/EasyOrder.Api.csproj

COPY . .
RUN dotnet publish \
    src/Services/OrderService/EasyOrder.Api/EasyOrder.Api.csproj \
    -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet","EasyOrder.Api.dll"]
