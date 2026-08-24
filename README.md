# Azure Playground

Projeto prático para estudo e demonstração de serviços do Microsoft Azure utilizando .NET 8.

O objetivo é construir uma aplicação evolutiva, explorando serviços Azure individualmente e aplicando boas práticas de desenvolvimento backend, arquitetura, persistência, testes, observabilidade e integração com serviços de infraestrutura.

> O projeto é desenvolvido inicialmente como um monólito modular, permitindo explorar os serviços Azure sem introduzir complexidade desnecessária de microservices.

---

## 🎯 Objetivos

- Praticar integração entre aplicações .NET e serviços Azure
- Compreender o funcionamento dos serviços Azure na prática
- Aplicar Clean Architecture
- Trabalhar com testes unitários e de integração
- Implementar observabilidade
- Utilizar Docker para infraestrutura local
- Explorar boas práticas de configuração e injeção de dependência
- Construir uma base que possa evoluir para cenários mais próximos de produção

---

## 🏗️ Arquitetura

O projeto utiliza uma estrutura baseada em Clean Architecture:

```text
AzurePlayground
│
├── src
│   ├── AzurePlayground.Api
│   ├── AzurePlayground.Application
│   ├── AzurePlayground.Domain
│   ├── AzurePlayground.Infra.Data
│   └── AzurePlayground.Infra.IoC
│
├── tests
│   ├── AzurePlayground.Application.Tests
│   ├── AzurePlayground.Domain.Tests
│   └── AzurePlayground.Infra.Data.Tests
│
└── infrastructure
    └── docker-compose.yml
```

Dependências entre projetos:

                 ┌─────────────────────┐
                 │ AzurePlayground.Api │
                 └──────────┬──────────┘
                            │
                            ▼
                 ┌─────────────────────┐
                 │      Infra.IoC      │
                 └──────┬────────┬─────┘
                        │        │
              ┌─────────▼───┐  ┌──▼─────────────┐
              │ Application │  │   Infra.Data   │
              └──────┬──────┘  └──────┬─────────┘
                     │                │
                     └───────┬────────┘
                             ▼
                     ┌────────────────┐
                     │     Domain     │
                     └────────────────┘

# ☁️ Módulos
## Modulo 1 — Azure Blob Storage

Primeiro módulo do projeto, dedicado ao armazenamento e gerenciamento de documentos utilizando Azure Blob Storage.

Para desenvolvimento local, o Azure Blob Storage é simulado utilizando Azurite, executado através de Docker.

### Funcionalidades
- Upload de documentos
- Download de documentos
- Exclusão de documentos
- Persistência dos metadados no SQL Server
- Validação de arquivos
- Integração com Azure Blob Storage
- Testes utilizando Azurite
- Regras de upload

### Regras de upload 

Atualmente são aceitos:
- PDF
- JPEG
- PNG

Limite máximo:
- 10 MB

### Fluxo de Upload

``` text
Client
  │
  ▼
DocumentController
  │
  ▼
DocumentService
  │
  ├──────────────► Azure Blob Storage
  │                     │
  │                     ▼
  │                  Blob
  │
  └──────────────► SQL Server
                       │
                       ▼
                   Document
```

A aplicação utiliza uma estratégia de compensação caso o upload do Blob seja concluído, mas a persistência dos metadados no banco falhe.

---

## 🗄️ Persistência

Os metadados dos documentos são armazenados no SQL Server.

A entidade Document contém informações como:
- Id
- OriginalFileName
- BlobName
- Container
- ContentType
- Size
- UploadedAt
- Status

O conteúdo físico do arquivo permanece no Blob Storage.

---

## 🐳 Infraestrutura local

O projeto utiliza Docker para executar o Azurite localmente.

```yaml
services:

  azurite:
    image: mcr.microsoft.com/azure-storage/azurite
    container_name: azurite
    command: azurite --blobHost 0.0.0.0
    ports:
      - "10000:10000"
    volumes:
      - ./azurite:/data
```

Executar:

```bash
docker compose up -d
```

Parar:

```bash
docker compose down
```

O Blob Storage local fica disponível em:

```text
http://127.0.0.1:10000
```

--- 

## 📊 Observabilidade

O projeto utiliza Serilog para logging estruturado.

Os principais fluxos da aplicação possuem logs estruturados:

- Upload
- Download
- Delete
- Falhas de persistência
- Operações de compensação

Exemplo:

```text
[INF] Starting document upload. FileName: document.pdf
[INF] Document uploaded to storage successfully.
[INF] Document metadata persisted successfully.
```

Os logs utilizam propriedades estruturadas, como:

- DocumentId
- BlobName
- FileName
- ContentType
- Size

Isso permite que os logs possam futuramente ser integrados a ferramentas como Application Insights, Seq ou outros sistemas de observabilidade.

---

## ❤️ Health Checks

A aplicação possui endpoints separados para liveness e readiness.

### Liveness

```http
GET /health/live
```

Verifica se a aplicação está em execução.

### Readiness

```http
GET /health/ready
```

Verifica se a aplicação está pronta para operar e suas principais dependências estão disponíveis.

Atualmente:

```
SQL Server
Azure Blob Storage / Azurite
```

Exemplo:

```
{
  "status": "Healthy",
  "entries": {
    "sqlserver": {
      "status": "Healthy"
    },
    "azure_blob": {
      "status": "Healthy"
    }
  }
}
```

---

## 🧪 Testes

O projeto utiliza testes automatizados para validar as regras de negócio e integrações.

### Testes unitários

Utilizados principalmente para:
- Domain
- Application
- DocumentService
- regras de validação
- fluxos de sucesso e falha

Tecnologias:
- xUnit
- Moq
- AutoMapper
- Testes de integração

O <code>AzurePlayground.Infra.Data.Tests</code> utiliza o Azurite para testar a integração real com o Blob Storage.

Exemplo:

```text
Application
     │
     ▼
AzureBlobDocumentStorage
     │
     ▼
Azurite
     │
     ▼
Blob
```

---

## 🛠️ Tecnologias
### Backend
- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- AutoMapper

### Azure
- Azure Blob Storage
- Azurite

### Banco de dados
- SQL Server

### Infraestrutura
- Docker
- Docker Compose

### Observabilidade
- Serilog
- ASP.NET Core Health Checks

### Testes
- xUnit
- Moq

---

## 🚀 Executando o projeto

### 1. Clonar o repositório

```bash
git clone <repository-url>
cd AzurePlayground
```

### 2. Iniciar o Azurite

```bash
docker compose -f infrastructure/docker-compose.yml up -d
```

### 3. Configurar o banco

Configure a connection string do SQL Server em:

```text
src/AzurePlayground.Api/appsettings.json
```


### 4. Executar a aplicação

```bash
dotnet run --project src/AzurePlayground.Api
```

A API estará disponível através das URLs exibidas pelo ASP.NET Core.

### 5. Swagger

Em ambiente de desenvolvimento:

```text
/swagger
```

---

## 📌 Roadmap

O projeto será expandido gradualmente com novos módulos relacionados ao ecossistema Azure.

### Módulo 1 — Storage
- ☑ Azure Blob Storage
- ☑ Azurite
- ☑ Upload
- ☑ Download
- ☑ Delete
- ☑ Validação de arquivos
- ☑ Testes
- ☑ Serilog
- ☑ Health Checks
- ☑ Liveness / Readiness
  
### Próximos módulos
- ⬜ Azure Queue Storage
- ⬜ Azure Key Vault
- ⬜ Azure Service Bus
- ⬜ Azure Functions
- ⬜ Azure App Service
- ⬜ Application Insights
- ⬜ Managed Identity

> Novos módulos serão adicionados progressivamente ao projeto.

---

## 📚 Propósito

Este projeto faz parte de uma jornada prática de aprofundamento em desenvolvimento backend com .NET e Microsoft Azure.

Mais do que demonstrar chamadas isoladas para serviços Azure, o objetivo é explorar como esses serviços podem ser incorporados a uma aplicação real considerando:

- Arquitetura
- Segurança
- Persistência
- Resiliência
- Observabilidade
- Testabilidade
- Configuração
- Infraestrutura
- Boas práticas de desenvolvimento
