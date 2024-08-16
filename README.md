# APICalendario

Este projeto é uma API para gerenciamento de lembretes, desenvolvida utilizando .NET 7.0. Ele suporta a criação, leitura, atualização e exclusão (CRUD) de lembretes com diferentes frequências (diária, semanal, mensal, anual).

## Tecnologias Utilizadas

- **.NET 7.0**
- **Entity Framework Core**
- **MySQL**
- **AutoMapper**
- **Swagger**

## Pré-requisitos

- .NET SDK 7.0 ou superior
- MySQL
- Visual Studio 2022 ou outro IDE de sua preferência

## Configuração do Projeto

### 1. Clonando o Repositório

```bash
git clone https://github.com/mjpa10/.NET_Calendario.git
cd .Net_Calendario
cd APICalendario
```

### 2. Instalando Dependências

Use o comando abaixo para restaurar as dependências do projeto:

```bash
dotnet restore
```

### 3. Configurando o Banco de Dados

- Crie as configurações de conexão no arquivo `appsettings.json` seguindo esse modelo:
  
  ```bash
  {
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=calendariodb;User Id=seu_usuario;Password=sua_senha;"
  },
  "AllowedHosts": "*"
}
```

- Execute as migrações para configurar o banco de dados:

```bash
dotnet ef migrations add calendariodb
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
