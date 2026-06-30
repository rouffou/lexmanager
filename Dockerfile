# syntax=docker/dockerfile:1

# ──────────────────────────────────────────────────────────────────────────────
# Stage 1 — Frontend build (Angular)
# Enabled once `frontend/` is scaffolded. Produces static assets copied into the
# backend's wwwroot in stage 2 so the monolith ships as a single image (SRD §6.3).
# ──────────────────────────────────────────────────────────────────────────────
FROM node:22-alpine AS frontend
WORKDIR /app/frontend
# Build only when a package.json is present; otherwise emit an empty wwwroot.
COPY frontend/package*.json ./
RUN if [ -f package.json ]; then npm ci; fi
COPY frontend/ ./
RUN mkdir -p /app/wwwroot && \
    if [ -f package.json ]; then npm run build && cp -r dist/*/browser/* /app/wwwroot/ 2>/dev/null || true; fi

# ──────────────────────────────────────────────────────────────────────────────
# Stage 2 — Backend build & publish (.NET 10)
# ──────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (better layer caching): copy only project + central-package files.
COPY *.slnx Directory.Build.props Directory.Packages.props global.json ./
COPY src/ ./src/
RUN dotnet restore src/Bootstrapper/LexManager.Api/LexManager.Api.csproj

# Bring in the Angular assets produced in stage 1.
COPY --from=frontend /app/wwwroot ./src/Bootstrapper/LexManager.Api/wwwroot

RUN dotnet publish src/Bootstrapper/LexManager.Api/LexManager.Api.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ──────────────────────────────────────────────────────────────────────────────
# Stage 3 — Runtime (lightweight ASP.NET)
# Bundles Tesseract OCR (FR + NL) so the DMS can run full-text OCR locally and for
# free — confidential documents never leave the container (SRD §3 Module 3, §7.2).
# ──────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        tesseract-ocr \
        tesseract-ocr-fra \
        tesseract-ocr-nld \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "LexManager.Api.dll"]
