# APICalendario

Este projeto é uma API para gerenciamento de lembretes, desenvolvida utilizando .NET 7.0. Ele suporta a criação, leitura, atualização e exclusão (CRUD) de lembretes com diferentes frequências (diária, semanal, mensal, anual). Além disso, a API inclui paginação, filtros, métodos assíncronos, autenticação, autorização e suporte a tokens JWT.

## Tecnologias Utilizadas

- **.NET 7.0: Framework utilizado para construir a API.**
- **Entity Framework Core: Ferramenta de mapeamento objeto-relacional (ORM) para interação com o banco de dados MySQL**
- **MySQL: Banco de dados utilizado para armazenar os lembretes.**
- **AutoMapper: Biblioteca para mapeamento automático entre objetos.**
- **Swagger: Ferramenta para documentação interativa da API.**
- **JWT (JSON Web Token): Utilizado para autenticação e autorização de usuários na API.**

## Pré-requisitos

- .NET SDK 7.0 ou superior
- MySQL
- Visual Studio 2022 ou outro IDE de sua preferência

## Configuração do Projeto

### 1. Clonando o Repositório

```bash
git clone https://github.com/mjpa10/.NET_Calendario.git
```

```bash
cd .NET_Calendario
```

### 2. Instalando Dependências

Use o comando abaixo para restaurar as dependências do projeto:

```bash
dotnet restore
```
```bash
cd APICalendario
```
### 3. Configurando o Banco de Dados e o autenticador de tokens

- Crie as configurações de conexão no arquivo `appsettings.json` dentro da pasta `\APICalendario`, seguindo esse modelo:
  
```json
 {
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=calendariodb;Uid=seu_usuario;Password=sua_senha;"
  },
  "JWT": {
    "ValidAudiance": "http://localhost:7066",
    "ValidIssuer": "http://localhost:5066",
    "SecretKey": "SuaChavePrivada",
    "TokenValidInMinutes": 30,
    "RefreshTokenValidyInMinutes": 60
  }
} //Substitua seu_usuario e sua_senha pelos valores de login do seu banco de dados.
```

- Crie as configurações de conexão no arquivo `appsettings.Development.json` dentro da pasta `\APICalendario`, seguindo esse modelo:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "Console": {
      "includescopes": true
    }
  },
  "Authentication": {
    "Schemes": {
      "Bearer": {
        "ValidAudiences": [
          "http://localhost:35800",
          "https://localhost:44340",
          "http://localhost:5289",
          "https://localhost:7186"
        ],
        "ValidIssuer": "dotnet-user-jwts"
      }
    }
  }
}
```

- Execute as migrações para configurar o banco de dados:

```bash
dotnet ef migrations add MigracaoInicial
```
```bash
dotnet ef database update
```

### 4. Compilando o Projeto

Compile o projeto utilizando o comando:

```bash
dotnet build
```

### 5. Executando o Projeto

Execute o projeto utlizando o comando :

```bash
dotnet watch run
```

## Estrutura do Projeto

- **Controllers**: Contém os controladores que gerenciam as requisições HTTP.
- **Models**: Define as classes de modelo utilizadas na aplicação.
- **Repositories**: Implementa os padrões de repositório e unidade de trabalho.
- **Services**: Contém os serviços responsáveis pela lógica de criação de lembretes.
- **DTOs**: Data Transfer Objects utilizados para transferência de dados entre as camadas.
- **Pagination**: Contém as classes responsáveis pela paginação e filtros de lembretes.
- 
