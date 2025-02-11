# DeveloperStore API

API REST desenvolvida em .NET 8 para gerenciamento de uma loja virtual.

## 🚀 Tecnologias

- .NET 8
- PostgreSQL
- Docker
- Entity Framework Core
- MediatR
- FluentValidation

## 📋 Pré-requisitos

- [Docker](https://www.docker.com/products/docker-desktop/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (opcional, apenas para desenvolvimento)

## 🏃 Como Executar

1. Clone o repositório

cd DeveloperStore

2. Execute os containers com Docker Compose

bash

docker-compose up -d


3. Acesse os serviços:
- API: http://localhost:5000/swagger
- pgAdmin: http://localhost:5050
  - Email: admin@admin.com
  - Senha: admin

## 🛠️ Configuração do pgAdmin

1. Acesse http://localhost:5050
2. Login com as credenciais acima
3. Clique em "Add New Server"
4. Na aba "General":
   - Name: DeveloperStore
5. Na aba "Connection":
   - Host: postgres
   - Port: 5432
   - Database: developerstore
   - Username: postgres
   - Password: postgres

## 🛑 Para Parar a Aplicação

bash

docker-compose down

## 📝 Licença

Este projeto está sob a licença MIT.
