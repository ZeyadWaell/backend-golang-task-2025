https://ibb.co/zTQWnM48


# E-Commerce Microservices Platform  
**.NET · CQRS · Kafka · Ocelot API Gateway**

---

## 📖 Overview  
This repository illustrates a sample e-commerce platform built on a microservices architecture in .NET. It uses CQRS (Command–Query Responsibility Segregation) to split write and read models, and Apache Kafka as the event bus so downstream services can react asynchronously. All client traffic (Blazor, MVC, React, Angular) is funneled through an Ocelot API Gateway.

---

## 🏗 Architecture Diagram  
![High-level Architecture](./docs/architecture.png)  
*(See `/docs/architecture.png` for a sketch of service boundaries, DBs, and Kafka topics.)*

---

## 🔧 Components  

1. **API Gateway**  
   - **Ocelot** routes all incoming HTTP traffic from WebApps → appropriate microservice.

2. **Command-Side Services**  
   Each service owns its own **WriteDB** and exposes only command (CUD) endpoints.  
   - **Order.Command.API**  
     - Handles `CreateOrder`, `CancelOrder`, etc.  
     - Writes to **Order_WriteDB**  
     - Publishes domain events to **Kafka** (topic: `order-events`)  
   - **Product.Command.API**  
     - Manages product CRUD, inventory thresholds  
     - Writes to **Product_WriteDB**  
     - (No Kafka emission)  
   - **Customer.Command.API**  
     - Manages customer profiles, addresses  
     - Writes to **Customer_WriteDB**  
   - **Basket.Command.API**  
     - Manages shopping carts  
     - Writes to **Basket_WriteDB**  
   - **Discount.gRPC**  
     - Applies discount rules  
     - Writes to **Discount_WriteDB**  

3. **Query-Side Services**  
   Each service owns its own **ReadDB** and subscribes to one or more Kafka topics to keep its read model up to date.  
   - **Order.Query.API**  
     - Serves order lookups by customer, status, date range  
     - Maintains **Order_ReadDB** via projections of `OrderCreated`, `OrderConfirmed`, etc.  
   - **Inventory.Service**  
     - Subscribes to `order-events`  
     - Decrements/increments stock in **Inventory_ReadDB**  
     - Emits `InventoryLow` when thresholds breach  
   - **Notification.Service**  
     - Subscribes to `order-events`, `payment-events`, `inventory-events`  
     - Sends emails/SMS/push for order confirmations, payment successes/failures, low-stock alerts  
   - **AuditLogs.Service**  
     - Subscribes to **all** domain events  
     - Appends an immutable audit trail in **AuditLogs_ReadDB**  

4. **Event Bus**  
   - **Apache Kafka** cluster with topics:  
     - `order-events`  
     - `payment-events`  
     - `inventory-events`  
     - *(extendable for shipping-events, notification-events, etc.)*

---

## 🔄 Typical Event Flow  

1. **Client** calls `POST /orders` → **Order.Command.API**  
2. **Order.Command.API**  
   - Persists to **Order_WriteDB**  
   - Publishes `OrderCreated` → **Kafka: order-events**  
3. **Subscribers** react:  
   - **Inventory.Service**  
     - On `OrderCreated` → decrement stock → persist to **Inventory_ReadDB**  
     - If low, publish `InventoryLow`  
   - **Notification.Service**  
     - On `OrderCreated` → send confirmation email  
   - **AuditLogs.Service**  
     - On every event → append to **AuditLogs_ReadDB**  
4. **Payment.Service**  
   - (optionally) consumes `OrderCreated` → attempt charge → publish `PaymentSucceeded`/`PaymentFailed`  
5. **Order.Command.API**  
   - Consumes `PaymentSucceeded` → update **Order_WriteDB** status → publish `OrderConfirmed`  
6. **Order.Query.API**  
   - Consumes `OrderConfirmed` → update **Order_ReadDB**  
   - Serves `GET /orders?customerId=…`  

---

## 🚀 Getting Started  

1. **Prerequisites**  
   - .NET 7 SDK  
   - Docker & Docker Compose  
   - Kafka cluster (you can spin up a local one via `docker-compose up kafka zookeeper`)  
   - SQL Server instances (Docker containers or Azure SQL)  

2. **Configuration**  
   - Each microservice reads its DB connection string from `appsettings.json` or environment vars.  
   - Kafka broker address configured in `KafkaSettings:BootstrapServers`.  
   - Ocelot gateway routes defined in `ocelot.json`.  

3. **Run Locally**  
   ```bash
   # Start Kafka & Zookeeper
   docker-compose up -d zookeeper kafka

   # Launch microservices
   cd src/Order.Command.API && dotnet run
   cd src/Order.Query.API   && dotnet run
   cd src/Inventory.Service  && dotnet run
   # …and so on for Product, Customer, Basket, Notification, AuditLogs

   # Start API Gateway
   cd src/ApiGateway && dotnet run
