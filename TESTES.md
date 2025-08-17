# 🧪 Comandos de Teste

Este documento descreve como executar os diferentes tipos de teste no projeto.

## 🚀 Comandos Disponíveis

### **Aplicação Principal**
```bash
# Subir apenas a aplicação (sem testes)
docker compose up

# Subir aplicação com rebuild
docker compose up --build
```

### **Testes Unitários**
```bash
# Rodar apenas testes unitários (sem dependências externas)
docker compose --profile test-unit up test-unit

# Ou usando run (remove container após execução)
docker compose run --rm test-unit
```

### **Testes de Repositório/Banco**
```bash
# Rodar apenas testes de repositório com InMemory DB
docker compose --profile test-db up test-db

# Ou usando run
docker compose run --rm test-db
```

### **Testes de Integração RabbitMQ**
```bash
# Rodar testes que dependem do RabbitMQ
docker compose --profile test-rabbitmq up test-rabbitmq

# Ou usando run
docker compose run --rm test-rabbitmq
```

### **Todos os Testes**
```bash
# Rodar todos os testes (com dependências completas)
docker compose --profile test up tests

# Ou usando run
docker compose run --rm tests
```

### **Testes de Integração Avançados (Testcontainers)**
```bash
# Rodar testes com containers próprios (PostgreSQL + RabbitMQ)
docker compose --profile integration-test up integration-test

# Ou usando run
docker compose run --rm integration-test
```

## 📊 Tipos de Teste

| Comando | Descrição | Dependências |
|---------|-----------|--------------|
| `test-unit` | Testes unitários rápidos | Nenhuma |
| `test-db` | Testes de repositório | InMemory DB |
| `test-rabbitmq` | Testes de mensageria | RabbitMQ |
| `tests` | Todos os testes | DB + RabbitMQ |
| `integration-test` | Testes isolados | Testcontainers |

## 🐛 Desenvolvimento Local

Para rodar testes localmente (sem Docker):

```bash
# Testes unitários
dotnet test --filter "Category!=Integration"

# Testes específicos do RabbitMQ
dotnet test --filter "FullyQualifiedName~RabbitMq"

# Todos os testes
dotnet test
```

## 🔧 Troubleshooting

- **RabbitMQ Connection Refused**: Normal quando RabbitMQ não está rodando. Use `test-db` para testes sem dependências.
- **EF Core Provider Conflict**: Use testes específicos por categoria para evitar conflitos.
- **Docker Socket Issues**: Certifique-se que Docker está rodando para `integration-test`.

## 📝 Exemplos

```bash
# Desenvolvimento rápido - apenas testes de lógica
docker compose run --rm test-unit

# Validar mensageria - com RabbitMQ
docker compose run --rm test-rabbitmq

# CI/CD completo - todos os testes
docker compose run --rm tests
```
