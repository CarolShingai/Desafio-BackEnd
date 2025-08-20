# 🏍️ Sistema de Locação de Motocicletas

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue.svg)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 📋 Sobre o Projeto

Sistema completo de locação de motocicletas desenvolvido em .NET 9, implementando Clean Architecture com Domain-Driven Design (DDD). O sistema oferece gerenciamento completo de motos, entregadores e locações, com sistema de notificações em tempo real via RabbitMQ.

### 🎯 Principais Características

- **Arquitetura Limpa**: Separação clara de responsabilidades
- **Domain-Driven Design**: Foco na lógica de negócio
- **Event-Driven**: Sistema de eventos com RabbitMQ
- **Containerizado**: Deploy facilitado com Docker
- **Documentação Completa**: XML Documentation + Swagger
- **Testes Abrangentes**: Unitários e de Integração

## 🚀 Stack Tecnológica

### Backend
- **.NET 9** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **RabbitMQ** - Message Broker
- **Swagger/OpenAPI** - Documentação da API

### DevOps & Infraestrutura
- **Docker & Docker Compose** - Containerização
- **Nginx** - Proxy reverso e load balancer
- **xUnit** - Framework de testes

## 🏗️ Arquitetura do Sistema

```
📁 RentalApi/
├── 🎯 Domain/              # Camada de Domínio
│   ├── Entities/          # Entidades de negócio
│   ├── Interfaces/        # Contratos do domínio
│   ├── ValueObjects/      # Objetos de valor
│   └── Events/           # Eventos de domínio
├── 🔧 Application/         # Camada de Aplicação
│   ├── Services/         # Serviços de aplicação
│   └── DTOs/            # Data Transfer Objects
├── 🏗️ Infrastructure/      # Camada de Infraestrutura
│   ├── Data/            # Contexto do banco
│   ├── Repositories/    # Implementações dos repositórios
│   ├── Messaging/       # Integração RabbitMQ
│   └── Background/      # Serviços em background
└── 🌐 Controllers/        # Camada de apresentação
```

## 🔧 Configuração e Execução

### 📋 Pré-requisitos

- [Docker](https://www.docker.com/get-started) 20.10+
- [Docker Compose](https://docs.docker.com/compose/) 2.0+
- [Git](https://git-scm.com/) (para clonar o repositório)

### 🚀 Execução com Docker (Recomendado)

1. **Clone o repositório:**
```bash
git clone https://github.com/CarolShingai/Desafio-BackEnd.git
cd Desafio-BackEnd
```

2. **Configure as variáveis de ambiente:**
```bash
# Copie o arquivo de exemplo
cp .env.example .env

# Configure conforme necessário (opcional - valores padrão funcionam)
```

3. **Execute a aplicação:**
```bash
# Subir todos os serviços
docker compose up --build

# Para executar em background
docker compose up -d --build
```

4. **Configure o banco de dados (primeira execução):**
```bash
# Execute as migrations
docker exec -it desafio-backend-rentalapi-1 dotnet ef migrations add InitialCreate
docker exec -it desafio-backend-rentalapi-1 dotnet ef database update
```

### 🌐 Acessos da Aplicação

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **API Principal** | http://localhost:8080 | Endpoints da API |
| **Swagger UI** | http://localhost:8080/swagger | Documentação interativa |
| **Nginx (Proxy)** | http://localhost | Proxy reverso |
| **RabbitMQ Management** | http://localhost:15672 | Interface do RabbitMQ |

**Credenciais RabbitMQ:**

### 🛠️ Desenvolvimento Local

<details>
<summary>Clique para expandir instruções de desenvolvimento local</summary>

#### Pré-requisitos Adicionais
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/) 15+

#### Configuração
```bash
# 1. Configure PostgreSQL e RabbitMQ (use credenciais seguras)
docker run --name postgres \
  -e POSTGRES_PASSWORD=sua_senha_segura \
  -e POSTGRES_DB=RentalDB \
  -p 5432:5432 -d postgres:15

docker run --name rabbitmq \
  -e RABBITMQ_DEFAULT_USER=seu_usuario \
  -e RABBITMQ_DEFAULT_PASS=sua_senha_segura \
  -p 5672:5672 -p 15672:15672 -d rabbitmq:3-management

# 2. Configure a aplicação
cd RentalApi
dotnet restore

# 3. Configure string de conexão (appsettings.Development.json)
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=RentalDB;Username=postgres;Password=sua_senha_segura"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "seu_usuario",
    "Password": "sua_senha_segura"
  }
}

# 4. Execute migrations e rode a aplicação
dotnet ef database update
dotnet run
```

> ⚠️ **Segurança**: Substitua `sua_senha_segura` e `seu_usuario` por credenciais reais e seguras.
> Para desenvolvimento rápido, você pode usar `postgres/postgres` e `guest/guest`, mas nunca em produção.
</details>

## 📚 Funcionalidades do Sistema

### 🏍️ Gestão de Motocicletas
| Funcionalidade | Status | Descrição |
|----------------|--------|-----------|
| ✅ Cadastro | Completo | Registro de novas motocicletas |
| ✅ Listagem | Completo | Visualização de todas as motos |
| ✅ Filtro por Placa | Completo | Busca específica por placa |
| ✅ Atualização | Completo | Modificação da placa |
| ✅ Remoção | Completo | Exclusão (apenas não locadas) |

### 👨‍💼 Gestão de Entregadores  
| Funcionalidade | Status | Descrição |
|----------------|--------|-----------|
| ✅ Cadastro | Completo | Registro com validação CNPJ/CNH |
| ✅ Upload CNH | Completo | Imagem da CNH em base64 |
| ✅ Validações | Completo | CNPJ, CNH e tipos válidos |

### 📋 Sistema de Locação
| Funcionalidade | Status | Descrição |
|----------------|--------|-----------|
| ✅ Criar Locação | Completo | Múltiplos planos disponíveis |
| ✅ Consultar | Completo | Visualização de locações |
| ✅ Devolver | Completo | Cálculo automático de multas |
| ✅ Planos Flexíveis | Completo | 7, 15, 30, 45 e 50 dias |

### 🔔 Sistema de Notificações
| Funcionalidade | Status | Descrição |
|----------------|--------|-----------|
| ✅ Eventos | Completo | Cadastro de motos via RabbitMQ |
| ✅ Processamento | Completo | Handling assíncrono |
| ✅ Persistência | Completo | Armazenamento no banco |

## 🗂️ Documentação da API

### 🏍️ Endpoints - Motocicletas
```http
GET    /api/motos                 # Listar todas as motos
GET    /api/motos?placa=ABC-1234  # Filtrar por placa
POST   /api/motos                 # Cadastrar nova moto
PUT    /api/motos/{id}/placa      # Atualizar placa
DELETE /api/motos/{id}            # Remover moto
```

### 👨‍💼 Endpoints - Entregadores
```http
POST   /api/entregadores          # Cadastrar entregador  
POST   /api/entregadores/{id}/cnh # Upload imagem CNH
```

### 📋 Endpoints - Locação
```http
POST   /api/locacao               # Criar locação
GET    /api/locacao/{id}          # Consultar locação
PUT    /api/locacao/{id}/devolucao # Informar devolução
```

### 📊 Planos de Locação Disponíveis

| Plano | Dias | Valor/Dia | Multa Antecipada | Multa Atraso |
|-------|------|-----------|------------------|--------------|
| **Básico** | 7 | R$ 30,00 | 20% sobre dias não efetivados | R$ 50,00/dia |
| **Padrão** | 15 | R$ 28,00 | 40% sobre dias não efetivados | R$ 50,00/dia |
| **Mensal** | 30 | R$ 22,00 | 40% sobre dias não efetivados | R$ 50,00/dia |
| **Estendido** | 45 | R$ 20,00 | 40% sobre dias não efetivados | R$ 50,00/dia |
| **Corporativo** | 50 | R$ 18,00 | 40% sobre dias não efetivados | R$ 50,00/dia |

## 🧪 Testes

### 🏃‍♂️ Executando Testes

```bash
# Todos os testes
dotnet test

# Testes por categoria
dotnet test --filter "Category=Unit"      # Testes unitários
dotnet test --filter "Category=Integration" # Testes de integração

# Com relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### 📊 Cobertura de Testes

| Camada | Cobertura | Tipos de Teste |
|--------|-----------|----------------|
| **Domain** | 95%+ | Unitários |
| **Application** | 90%+ | Unitários + Integração |
| **Infrastructure** | 85%+ | Integração |
| **Controllers** | 90%+ | Integração |

## 🔐 Configuração de Ambiente

### 📝 Variáveis de Ambiente (.env)

```bash
# Ambiente
ASPNETCORE_ENVIRONMENT=Development

# Banco de Dados
DB_CONNECTION=Host=db;Database=RentalDB;Username=postgres;Password=postgres
POSTGRES_DB=RentalDB
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

# RabbitMQ
RABBITMQ_HOST=rabbitmq
RABBITMQ_USER=guest
RABBITMQ_PASS=guest
```

> ⚠️ **Importante**: As credenciais acima são apenas para desenvolvimento local. 
> Em produção, use sempre credenciais seguras e variáveis de ambiente apropriadas.

### 🔒 Configuração de Produção

Para ambientes de produção, configure as seguintes variáveis de ambiente:

```bash
# Banco de Dados - Use credenciais seguras
DB_CONNECTION=Host=seu-host;Database=sua-db;Username=seu-usuario;Password=sua-senha-segura

# RabbitMQ - Configure usuário específico
RABBITMQ_HOST=seu-rabbitmq-host
RABBITMQ_USER=seu-usuario-rabbitmq
RABBITMQ_PASS=sua-senha-segura-rabbitmq

# Configurações de Segurança
JWT_SECRET=sua-chave-jwt-muito-segura
ENCRYPTION_KEY=sua-chave-de-criptografia
```

## 🛠️ Padrões de Desenvolvimento

### 🏗️ Arquiteturas e Padrões
- **Clean Architecture** - Separação de responsabilidades
- **Domain-Driven Design** - Foco no domínio de negócio
- **Repository Pattern** - Abstração da camada de dados
- **Service Layer** - Lógica de aplicação
- **Event Sourcing** - Eventos de domínio
- **Dependency Injection** - Inversão de controle

### 📝 Convenções de Código
- **XML Documentation** - Documentação em inglês
- **Async/Await** - Operações assíncronas
- **Exception Handling** - Tratamento robusto de erros
- **Validation** - Validação de entrada robusta

### 📋 Padrão de Commits

```
feat: nova funcionalidade
fix: correção de bug
docs: atualização de documentação
test: adição ou correção de testes
refactor: refatoração de código
style: formatação e estilo
ci: integração contínua
```

## 📚 Documentação Adicional

| Documento | Descrição |
|-----------|-----------|
| [TESTES.md](./TESTES.md) | Guia de testes manuais |

## 📄 Licença

Este projeto está licenciado sob a [Licença MIT](LICENSE) - veja o arquivo LICENSE para detalhes.

## 👨‍💻 Desenvolvido por

**Caroline Shingai**
- 💼 [LinkedIn](https://linkedin.com/in/caroline-shingai)
- 🐙 [GitHub](https://github.com/CarolShingai)
- 📧 Email: caroline.shingai@email.com

---

**Desenvolvido com ❤️ e ☕ usando .NET 9**

</div>