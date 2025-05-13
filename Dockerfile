# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
EXPOSE 5000

COPY ./*.csproj ./product.service/
RUN dotnet restore ./product.service/*.csproj

COPY . ./product.service/
WORKDIR /source/product.service
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT [ "dotnet", "product.service.dll" ]