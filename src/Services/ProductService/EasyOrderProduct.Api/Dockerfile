# ─── Build Stage ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Copy each .csproj so restore knows all projects
COPY src/Services/ProductService/EasyOrderProduct.Api/EasyOrderProduct.Api.csproj \
     src/Services/ProductService/EasyOrderProduct.Api/
COPY src/Services/ProductService/EasyOrderProduct.Domain/EasyOrderProduct.Domain.csproj \
     src/Services/ProductService/EasyOrderProduct.Domain/
COPY src/Services/ProductService/EasyOrderProduct.Application.Contracts/EasyOrderProduct.Application.Contract.csproj \
     src/Services/ProductService/EasyOrderProduct.Application.Contracts/
COPY src/Services/ProductService/EasyOrderProduct.Infrastructure/EasyOrderProduct.Infrastructure.csproj \
     src/Services/ProductService/EasyOrderProduct.Infrastructure/

# 2) Copy the Protos folder (Windows is case‐insensitive, Linux is not)
COPY src/Services/ProductService/EasyOrderProduct.Application.Contracts/Protos \
     src/Services/ProductService/EasyOrderProduct.Application.Contracts/Protos/

# 3) Fix filename case: Inventory.proto → inventory.proto
RUN mv \
    src/Services/ProductService/EasyOrderProduct.Application.Contracts/Protos/Inventory.proto \
    src/Services/ProductService/EasyOrderProduct.Application.Contracts/Protos/inventory.proto

# 4) Restore packages (now sees the .proto)
RUN dotnet restore src/Services/ProductService/EasyOrderProduct.Api/EasyOrderProduct.Api.csproj

# 5) Copy all remaining source files
COPY src/Services/ProductService/EasyOrderProduct.Api/ \
     src/Services/ProductService/EasyOrderProduct.Api/
COPY src/Services/ProductService/EasyOrderProduct.Domain/ \
     src/Services/ProductService/EasyOrderProduct.Domain/
COPY src/Services/ProductService/EasyOrderProduct.Application.Contracts/ \
     src/Services/ProductService/EasyOrderProduct.Application.Contracts/
COPY src/Services/ProductService/EasyOrderProduct.Infrastructure/ \
     src/Services/ProductService/EasyOrderProduct.Infrastructure/

# 6) Publish the API
RUN dotnet publish \
    src/Services/ProductService/EasyOrderProduct.Api/EasyOrderProduct.Api.csproj \
    -c Release -o /app/publish

# ─── Runtime Stage ───────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet","EasyOrderProduct.Api.dll"]
