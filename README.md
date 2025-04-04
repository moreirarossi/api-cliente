# 📝 Rommanel-API  - Clientes

Uma **API CRUD** desenvolvida em .NET, utilizando boas práticas como **FluentValidation**, **xUnit** para testes e **AutoMapper** para mapeamento de objetos.  

---

## 🛠️ Tecnologias Utilizadas

### [.NET 8+](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
Versão recente do framework .NET oferece desempenho aprimorado, suporte nativo a containers e recursos modernos como Minimal APIs. Ideal para a construção de aplicações web leves, modulares e altamente escaláveis.

### [FluentValidation](https://docs.fluentvalidation.net/)  
Biblioteca para validação fluente e desacoplada. Permite criar regras de validação limpas, reutilizáveis e com mensagens personalizadas, promovendo um código mais legível e sustentável em aplicações com lógica de validação complexa.
Foi implementado para validação da criação e alteração do cliente, com regras específicas para pessoa física o jurídica.

### [xUnit](https://xunit.net/)  
Framework moderno de testes para .NET. Suporta testes paralelos, data-driven testing com `[Theory]`, e é amplamente utilizado em pipelines de CI/CD por sua robustez e fácil integração com outras ferramentas.
Foram implementados testes unitários dos endpoints da API, incluindo testes das validações utilizadas com Fluent Validation

### [AutoMapper](https://docs.automapper.org/)  
Facilita o mapeamento entre objetos, como DTOs e entidades de domínio, que em nosso projeto fez a tradução entre as classes dos Handlers com a context, reduzindo o código repetitivo e melhorando a separação de responsabilidades entre as camadas da aplicação.

### [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)  
ORM oficial da Microsoft para .NET. Oferece suporte a migrações `code-first`, consultas usando LINQ, e compatibilidade com diversos bancos de dados relacionais. Promove produtividade sem sacrificar a performance.

### [CQRS (Command Query Responsibility Segregation)](https://martinfowler.com/bliki/CQRS.html)  
Padrão arquitetural que separa operações de leitura e escrita, proporcionando maior escalabilidade, organização e flexibilidade. Facilita a manutenção da lógica de negócios e permite otimizações específicas para cada tipo de operação.

### [MediatR (Mediator Pattern)](https://github.com/jbogard/MediatR)  
Implementamos o padrão Mediator para promover um baixo acoplamento entre componentes da aplicação. Utilizado como um barramento interno para lidar com comandos, consultas, eventos e notificações de forma clara e centralizada.
Dado a simplicidade do projeto, optamos por manter um arquivo contendo os Handlers de gravação e outro de Consulta, mas ainda sim permitindo futura segregação destas operações.

---

### 📌 Vantagens da Arquitetura Adotada

A combinação de CQRS com MediatR proporciona uma base sólida para aplicações robustas e escaláveis. Essa abordagem permite separar claramente responsabilidades, facilitando testes, manutenção e evolução do código. Além disso, promove um design limpo e orientado a comportamentos, onde cada operação (comando ou consulta) é tratada de forma independente e eficiente.

Para mais detalhes sobre essa arquitetura e boas práticas com .NET, consulte a [documentação oficial da Microsoft sobre arquitetura moderna](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/).


---

## 📌 Funcionalidades  

✅ **CRUD Completo de Cliente** (Create, Read, Update, Delete)  
✅ **Validação de Dados** com FluentValidation  
✅ **Testes Unitários** com xUnit  
✅ **DTOs e AutoMapper** para melhor organização  
✅ **Documentação com Swagger**  

---

## 🚀 Como Executar  

### Pré-requisitos  
- [.NET 8 SDK](https://dotnet.microsoft.com/download)  
- Banco de dados configurado SQL Server  

### Passo a Passo  

1. **Clone o repositório**  
   ```bash
   git clone https://github.com/moreirarossi/rommanel-api.git
   cd rommanel-api
