# 🌩️ High-Throughput Order Processing System

![Architecture Diagram](docs/architecture.png)

A proof-of-concept **.NET** microservices solution that demonstrates how to handle **very high-demand order volumes** using **CQRS**, **Domain-Driven Design**, and industry-proven design patterns.  

---

## 🚀 Motivation

Online retailers and marketplaces often face extreme spikes in order volume (flash sales, product launches, Black Friday). Synchronous CRUD APIs struggle to keep up, leading to slow responses, timeouts, and lost revenue.  
This project shows how to:

- **Scale reads and writes independently** (CQRS)
- **Decouple services** with asynchronous messaging
- **Maintain data consistency** across microservices
- **Apply resilient design patterns** (Circuit Breaker, Retry, Saga, Factory, Repository, Unit of Work)

---

## 🏗️ Architecture

```text
┌───────────────┐      ┌──────────────┐      ┌─────────────────┐
│   API Gateway │◀────▶│ OrderService │◀────▶│ InventoryService│
│ (Ocelot)      │      │  (Writes)    │      │  (Reads & Stock)│
└───────────────┘      └──────────────┘      └─────────────────┘
        │                     │                       │
        │───────────▶────────▶└─▶─► Kafka/EventBus────┘
        │                            ▲
        │                            │
        │                ┌────────────────────────┐
        │                │ ShippingService (Saga) │
        │                └────────────────────────┘
        ▼
┌─────────────────┐
│   PaymentService│
│ (Idempotent API)│
└─────────────────┘
