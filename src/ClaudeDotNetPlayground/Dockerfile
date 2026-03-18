FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

RUN apt-get update && apt-get install -y clang zlib1g-dev ca-certificates && rm -rf /var/lib/apt/lists/*

# Suporte a CA customizada para ambientes com proxy TLS (ex: sandbox de CI)
ARG EXTRA_CA_CERT=""
RUN if [ -n "${EXTRA_CA_CERT}" ]; then \
    echo "${EXTRA_CA_CERT}" | base64 -d > /usr/local/share/ca-certificates/proxy-ca.crt && \
    update-ca-certificates; \
  fi

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

COPY --from=build /app/publish/ .

# Copiar bundle de certificados CA do estágio de build.
# Inclui qualquer CA customizada instalada via EXTRA_CA_CERT (ex: proxy TLS de sandbox).
# Necessário para que chamadas HTTPS externas (Open-Meteo) funcionem em ambientes com inspeção TLS.
COPY --from=build /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/ca-certificates.crt

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["./ClaudeDotNetPlayground"]
