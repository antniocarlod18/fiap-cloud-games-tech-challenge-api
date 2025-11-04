# 🎮 FIAP Cloud Games Tech Challenge API

API RESTful desenvolvida em **.NET 8** para gerenciamento de **usuários, jogos, pedidos e promoções**.  
Este projeto foi criado como parte do **FIAP Tech Challenge**, aplicando conceitos de **DDD (Domain-Driven Design)**, **Clean Architecture**, **Entity Framework Core** e **Autenticação JWT** com **MySQL** como banco de dados relacional.

---

## 🚀 Tecnologias Utilizadas

- **.NET 8**
- **MySQL**
- **JWT Authentication**
- **xUnit** (testes unitários)
- **Swagger / OpenAPI**
- **FluentValidation**
- **Dependency Injection**
- **Minimal APIs**

---

## 🧩 Estrutura do Projeto

src/
├── fiap-cloud-games-tech-challenge-api # Camada de apresentação (Minimal API)
├── Application # Casos de uso, DTOs, Handlers
├── Domain # Entidades e regras de negócio (DDD)
├── Infrastructure # Repositórios e persistência (EF Core / MySQL)
└── Tests # Testes unitários (xUnit)

## ⚙️ Pré-requisitos

Antes de rodar o projeto, verifique se possui instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)
- [MySQL Workbench (opcional)](https://dev.mysql.com/downloads/workbench/)
- [Visual Studio 2022 ou Insiders(2026)](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

---

## 🛠️ Configuração para rodar local

1. Crie um banco de dados MySQL:
   ```sql
   CREATE DATABASE FiapCloudGames;

2. Configure o arquivo appsettings.Development.json localizado em \FiapCloudGamesTechChallenge.Api\appsettings.Development.json:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "MySQL": "Server=localhost;Database=FiapCloudGames;Uid=root;Pwd=admin;"
  },
  "Authentication": {
    "Key": "HvPmUQ7j9ygSJ5DYDyOOPFjj0DRC6RDCi03GUR0uo8GZQEWACVqrY1xEqfdA",
    "Issuer": "https://localhost:7121/autentication",
    "Audience": "https://localhost:7121/"
  }
}
```
3. Aplique as migrations do Entity Framework Core com os comandos:

dotnet ef database update

4. Execute o projeto pelo Visual Studio ou por linha de comando.
