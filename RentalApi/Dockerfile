FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dev
WORKDIR /app
EXPOSE 8080

# Copia apenas arquivos essenciais para restaurar dependências
COPY RentalApi.csproj ./
RUN dotnet restore

# Copia o restante do código para o container
COPY . .

# Variável de ambiente para modo Development
ENV ASPNETCORE_ENVIRONMENT=Development

# Permite hot reload e mapeia volume do código
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8080"]
