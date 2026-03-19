FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

RUN apt-get update && apt-get install -y clang zlib1g-dev ca-certificates && rm -rf /var/lib/apt/lists/*

# Suporte a CA customizada para ambientes com proxy TLS (ex: sandbox de CI)
ARG EXTRA_CA_CERT=""
RUN if [ -n "${EXTRA_CA_CERT}" ]; then \
    echo "${EXTRA_CA_CERT}" | base64 -d > /usr/local/share/ca-certificates/proxy-ca.crt && \
    update-ca-certificates; \
  else \
    touch /usr/local/share/ca-certificates/proxy-ca.crt; \
  fi

COPY Directory.Build.props .
COPY Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj Albert.Playground.ECS.AOT.Api/
RUN dotnet restore Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj

COPY Albert.Playground.ECS.AOT.Api/ Albert.Playground.ECS.AOT.Api/
RUN dotnet publish Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj \
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
COPY --from=build /usr/local/share/ca-certificates/proxy-ca.crt /usr/local/share/ca-certificates/proxy-ca.crt

# Copiar cert individual do proxy para /etc/ssl/certs/ e regenerar hash symlinks.
# O OpenSSL usa hash symlinks para lookup de CA na verificação de cadeia TLS.
# Sem o hash symlink, a primeira conexão falha com PartialChain e depende do retry do Polly.
RUN if [ -s /usr/local/share/ca-certificates/proxy-ca.crt ]; then \
      cp /usr/local/share/ca-certificates/proxy-ca.crt /etc/ssl/certs/proxy-ca.crt && \
      c_rehash /etc/ssl/certs/ 2>/dev/null; \
    fi || true

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["./Albert.Playground.ECS.AOT.Api"]
