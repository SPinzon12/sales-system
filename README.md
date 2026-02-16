# Sistema de Productos y Ventas

Sistema End-to-End para gestion de productos y ventas con autenticacion JWT.

## Tecnologias

- Backend: .NET 8 (Clean Architecture)
- Frontend: Vue 3 + Vite + Pinia
- Base de datos: SQLite
- ORM: Entity Framework Core
- Autenticacion: JWT
- Contenedores: Docker Compose
- Logs: Serilog (JSON)
- Resiliencia: Polly
- Tests: xUnit + Moq

## Estructura API

```
SalesSystem.Api/        -> Controllers, Middleware
SalesSystem.Application/ -> UseCases, DTOs, Interfaces
SalesSystem.Domain/     -> Entities
SalesSystem.Infrastructure/ -> Repositories, DbContext
SalesSystem.Tests/      -> Unit Tests
```

## Entidades

- **User**: Id, Username, Email, PasswordHash, CreatedAt
- **Product**: Id, Name, Price, Stock, ImageUrl, IsActive
- **Sale**: Id, UserId, SaleDate, Status, Total
- **SaleItem**: Id, SaleId, ProductId, Quantity, UnitPrice, SubTotal

## Relacion

```
User 1 --> * Sale
         Sale 1 --> * SaleItem
                   SaleItem * --> 1 Product
```

## Ejecucion con Docker

```bash
docker compose up -d
```

| Servicio | URL |
|----------|-----|
| Frontend | http://localhost:3000 |
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |

**Credenciales:** admin / admin123

## Ejecucion Local

```bash
# Backend
cd api/SalesSystem.Api
dotnet restore
dotnet run

# Frontend
cd frontend
npm install
npm run dev
```

## Endpoints API

### Auth
- POST `/api/auth/login` - Iniciar sesion

### Productos
- GET `/api/products` - Listar productos
- GET `/api/products/{id}` - Obtener producto por ID
- POST `/api/products` - Crear producto
- PUT `/api/products/{id}` - Actualizar producto
- DELETE `/api/products/{id}` - Eliminar producto
- PATCH `/api/products/{id}/reactivate` - Reactivar producto

### Ventas
- POST `/api/sales` - Registrar venta
- GET `/api/sales/{id}` - Ver detalles de venta
- GET `/api/sales/report` - Reporte de ventas por rango de fechas con paginacion

## Tests

```bash
cd api
dotnet test
```

8 pruebas unitarias: AuthService (3) + ProductService (5)
