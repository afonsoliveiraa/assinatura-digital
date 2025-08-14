# Imagem base do .NET SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante do código
COPY . ./

# Publica a aplicação (build otimizado)
RUN dotnet publish -c Release -o out

# Imagem runtime para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copia os arquivos publicados
COPY --from=build /app/out ./

# Expõe a porta que a aplicação usa
EXPOSE 5000

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "assinatura-digital.dll"]
