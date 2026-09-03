# ── Versión NO optimizada (una sola etapa) ──
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /src

COPY . .

RUN dotnet restore "MsProduccionAlimentos.Api/MsProduccionAlimentos.Api.csproj"
RUN dotnet publish "MsProduccionAlimentos.Api/MsProduccionAlimentos.Api.csproj" \
    -c Release -o /app/publish

EXPOSE 8080
ENTRYPOINT ["dotnet", "/app/publish/MsProduccionAlimentos.Api.dll"]