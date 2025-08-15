# Build com SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia o csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante do código
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copia a pasta publicada do build
COPY --from=build /app/out ./

# Expõe a porta
EXPOSE 8080

# Roda a DLL publicada
ENTRYPOINT ["dotnet", "assinatura-digital.dll"]
