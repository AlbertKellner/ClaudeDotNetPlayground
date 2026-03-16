FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

RUN apt-get update && apt-get install -y clang zlib1g-dev && rm -rf /var/lib/apt/lists/*

COPY ClaudeDotNetPlayground.csproj .
RUN dotnet restore ClaudeDotNetPlayground.csproj

COPY . .
RUN dotnet publish ClaudeDotNetPlayground.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish/ClaudeDotNetPlayground .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["./ClaudeDotNetPlayground"]
