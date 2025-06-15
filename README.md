# 🛒 Concurrent Order Processing System

A high-performance, scalable **e-commerce backend** built with **.NET 8**, featuring **microservices**, **CQRS**, **Clean Architecture**, **gRPC**, **JWT Authentication**, and **Docker**.

---

## 🧱 Architecture Overview

### ⚙️ Microservices

| Service                     | Description                                           | Pattern        |
| --------------------------- | ----------------------------------------------------- | -------------- |
| **OrderService**            | Handles order creation, updates, and cancellations    | ✅ CQRS + Clean |
| **ProductInventoryService** | Manages product stock, reservations, and availability | ✅ CQRS + Clean |
| **IdentityService**         | Manages user registration, login, JWT, and roles      | ❌ No CQRS      |

---

### 🧩 Folder Structure

```
BackendSolution
│
├── src
│   └── Services
│       ├── OrderService
│       │   ├── EasyOrderOrder.Api
│       │   ├── EasyOrderOrder.Application.Contracts
│       │   ├── EasyOrderOrder.Application.Commands
│       │   ├── EasyOrderOrder.Application.Queries
│       │   ├── EasyOrderOrder.Domain
│       │   └── EasyOrderOrder.Infrastructure
│       │
│       ├── ProductInventoryService
│       │   ├── EasyOrderInventory.Api
│       │   ├── EasyOrderInventory.Application.Contracts
│       │   ├── EasyOrderInventory.Application.Commands
│       │   ├── EasyOrderInventory.Application.Queries
│       │   ├── EasyOrderInventory.Domain
│       │   └── EasyOrderInventory.Infrastructure
│       │
│       └── IdentityService
│           ├── EasyOrderIdentity.Api
│           ├── EasyOrderIdentity.Application
│           ├── EasyOrderIdentity.Domain
│           └── EasyOrderIdentity.Infrastructure   
│
├── API Gateway
│   └── Ocelot configuration (Swagger merge, routing, rate limiting)
│
├── docker
│   └── docker-compose.yml, Dockerfiles, .env
│
└── tests
    ├── OrderServiceTests
    ├── InventoryServiceTests
    └── IdentityServiceTests
```

---

## 🚀 Key Features

* 🔄 **CQRS + Clean Architecture** for Order & Inventory services
* 🔐 **JWT-based Authentication** via IdentityService
* 📦 **gRPC Communication** for service-to-service calls
* ⚙️ **Dockerized** deployment for consistent environments
* 🛠️ **Hangfire** for error logging and background processing
* 📘 **Swagger** for API documentation
* 🧪 **xUnit** for robust unit/integration testing

---

## 🧠 Design Principles

### ✅ CQRS (Order & Inventory)

* **Write Models**: Commands handle create/update/delete via MediatR
* **Read Models**: Queries return optimized projections for fast reads
* **Separation of Concerns**: Handlers isolated for maintainability and testability

### 🧱 Clean Architecture

* `Api`: Controllers, gRPC endpoints, middleware
* `Application.Contracts`: DTOs, interfaces, Protos
* `Application.Commands`: Command handlers (writes)
* `Application.Queries`: Query handlers (reads)
* `Domain`: Entities, value objects, aggregates
* `Infrastructure`: Repos, EF DbContext, service implementations

### ❌ IdentityService: Clean Architecture (without CQRS)

* Layered CRUD for user and role management
* JWT issuance, validation, and refresh flows
* Role-based authorization via policies

---

## 🎁 Order Incentive Handling

To support promotional incentives (discount codes, loyalty points) when creating orders, we leverage the CQRS microservices and gRPC patterns:

1. **Why CQRS?**

   * Incentive logic often involves complex business rules and multiple steps (validate code, calculate discount, update loyalty balances).
   * Separating **Command** and **Query** models lets us keep write-side operations (order + incentive creation) decoupled from read-side projections (order history dashboards).

2. **How it Works**

   * **OrderCommand**: The client submits an `ApplyIncentiveCommand` alongside `CreateOrderCommand` via MediatR in the OrderService.
   * **Validation**: The command handler invokes a gRPC call to the ProductInventoryService (or a dedicated IncentiveService stub) to validate stock and incentive eligibility in a single transaction.
   * **Domain Event**: Once validated, the handler emits an `OrderCreatedWithIncentive` domain event.
   * **Event Projection**: A read-model projection updates the `OrderReadModel` and `IncentiveUsageReadModel` for reporting.

3. **gRPC Communication**

   * The OrderService calls ProductInventoryService over gRPC to reserve stock and to resolve incentive details (e.g., discount amount).
   * Responses include confirmation tokens and updated inventory levels.

4. **Why Not RabbitMQ/Rebus Yet?**

   * **Time Constraints**: Implementing a full message bus (RabbitMQ + Rebus) adds infrastructure complexity, CI/CD changes, and operational overhead.
   * **Scope**: For MVP, synchronous gRPC calls combined with MediatR command dispatch meet performance and reliability needs.
   * **Next Steps**: Asynchronous event publishing via a message broker will be added in Phase 2 for better decoupling and resilience.

---

## 📬 gRPC Communication

| From → To                       | Purpose                           |
| ------------------------------- | --------------------------------- |
| OrderService → InventoryService | Reserve/Release stock             |
| InventoryService → OrderService | Confirm availability              |
| OrderService → IdentityService  | Verify user roles and permissions |

---

## 📦 Docker & CI/CD

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Docker & Docker Compose](https://www.docker.com)

### Getting Started

```bash
# Clone repository
git clone https://github.com/your-org/BackendSolution.git
cd BackendSolution

# Copy and configure environment variables
cp docker/.env.example docker/.env

# Build and run all services
docker-compose up --build
```

### Running Tests

```bash
# From solution root
dotnet test tests/OrderServiceTests
dotnet test tests/InventoryServiceTests
dotnet test tests/IdentityServiceTests
```


---

## 📜 License

Licensed under the MIT License. See [LICENSE](LICENSE) for details.
