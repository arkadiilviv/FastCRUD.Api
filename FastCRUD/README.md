# FastCRUD

FastCRUD is a lightweight library for generating CRUD (Create, Read, Update, Delete) endpoints in ASP.NET Core Minimal APIs. It provides extension methods to quickly scaffold RESTful endpoints for your repository classes, with optional authentication support.

## Features
- Automatic endpoint generation for CRUD operations
- Support for custom repositories via `IFastRepository<T, IdKey>`
- Optional JWT authentication integration
- Minimal API style for fast development
- Different groups for same model repositories

## Usage
1. Implement `IFastRepository<T, IdKey>` for your model.
2. Call `app.GenerateCRUD()` or `app.GenerateCRUDWithAuth()` in your `Program.cs` to register endpoints.

Example:
```csharp
var humanRepository = new HumanRepository();
app.GenerateCRUD(humanRepository);
```

## Installation
Nuget
```
dotnet add package FastCRUD.Api
```
Or manualy clone and build
## Endpoints Generated
- `GET /{RepoName}/{ModelName}s` - Get all items
- `GET /{RepoName}/{ModelName}s/{id}` - Get item by ID
- `POST /{RepoName}/{ModelName}s` - Create item
- `PUT /{RepoName}/{ModelName}s/{id}` - Update item
- `DELETE /{RepoName}/{ModelName}s/{id}` - Delete item

## Authentication
Use `GenerateCRUDWithAuth` to require JWT authentication for all endpoints.

## License
MIT

