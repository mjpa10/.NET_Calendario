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
    "ValidAudience": "http://localhost:7016", //Representa o usuario válido para o token JWT
    "ValidIssuer": "http://localhost:5066", //Indica o emissor válido do token JWT, a fonte
    "SecretKey": "Sua@Super#Secreta&Chave*Privada!2024%", //Essa é a chave secreta utilizada para assinar e validar o token
    "TokenValidityInMinutes": 30, //Define a duração de validade do token JWT em minutos
    "RefreshTokenValidyInMinutes": 60 //Especifica por quanto tempo um refresh token é válido
  }
}
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

### 4. Abra o Projeto na IDE:

Inicie sua IDE e abra o arquivo de solução (.sln) que corresponde ao projeto, No caso o `APICalendario.sln`.

### 5. Compile o Projeto:

Na IDE, localize e selecione a opção para compilar o projeto. Normalmente, isso pode ser feito clicando com o botão direito no arquivo de solução ou projeto no Gerenciador de Soluções e selecionando "Compilar" ou "Build".

### 6. Inicie o Projeto:

pós a compilação bem-sucedida, inicie o projeto usando a opção "Iniciar" ou "Run" na IDE. Pode ser usado o Atalaho F5 Também

## Estrutura do Projeto

- **Controllers**: Contém os controladores que gerenciam as requisições HTTP.
- **Models**: Define as classes de modelo utilizadas na aplicação.
- **Repositories**: Implementa os padrões de repositório e unidade de trabalho.
- **Services**: Contém os serviços responsáveis pela lógica de criação de lembretes.
- **DTOs**: Data Transfer Objects utilizados para transferência de dados entre as camadas.
- **Pagination**: Contém as classes responsáveis pela paginação e filtros de lembretes.
- 
