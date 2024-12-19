# Lightweight Microservice Example

This is an example of a minimalistic approach to building microservices using ASP.NET Core Minimal APIs.

The service has no convoluted business logic and only a small - yet meaningful - set of validation rules,
thus it employs a very lean stack of frameworks including the following:
- Middleware: ASP.NET Core Minimal APIs
- Dependency injection: ASP.NET Core built-in DI container
- Validation: FluentValidation
- Persistence: Dapper
- Database: Sqlite (in-memory for demonstration purposes)
- Testing: xUnit integration tests

While miniature and fast, this stack is still quite extendable and can accomodate additions of business layer, 
more database entities and relations etc.

However, as the complexity grows it may become beneficial to switch to a full-fledged layered architecture; 
an example may include the following:
- Hexagonal architecture approach with fully isolated core (business logic), presentation (REST API) and persistence layers
- ASP.NET Controller-based APIs for presentation layer
- .NET Entity Framework for persistence layer
- AutoMapper for layers isolation
- Autofac for modular dependency injection and testing