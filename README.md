# E-Commerce CQRS + Kafka Overview
https://private-user-images.githubusercontent.com/80164976/241560447-337c599f-5138-4c17-b92d-98f6a4ed1a2a.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NDk0MjIzMjIsIm5iZiI6MTc0OTQyMjAyMiwicGF0aCI6Ii84MDE2NDk3Ni8yNDE1NjA0NDctMzM3YzU5OWYtNTEzOC00YzE3LWI5MmQtOThmNmE0ZWQxYTJhLnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNTA2MDglMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjUwNjA4VDIyMzM0MlomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPTZhMDBhNGY3OThlZjNlMWE2ZWQwMTcwZDU4M2ZkNWQzYmExZjUwNGNkZmIyNTg4YjkwMDVkMDkwYmUyYTEzNzYmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0In0.bHB7VPQTXi120wr13E4Edlv_mZmqJbioLYwZNhNIx28

![Architecture Diagram](https://ibb.co/zTQWnM48)

A minimalist .NET microservices design using CQRS and Kafka for order-driven workflows.

## 🚀 Services

- **User**  
  - _Command API_ → Register/login customers & admins  
  - _Query API_ → Read user profiles  

- **Product**  
  - _Command API_ → Manage products & inventory rules  
  - _Query API_ → Read product catalog  

- **Order**  
  - _Command API_ → Create/cancel orders → writes to **Order_WriteDB** + publishes `OrderCreated` to Kafka  
  - _Query API_ → Read orders by status, customer → built from event projections  

- **Payment**  
  - Subscribes to `OrderCreated` → processes payment → emits `PaymentSucceeded`/`PaymentFailed`  

- **Inventory**  
  - Subscribes to `OrderCreated` → decrements stock → emits `InventoryLow`  

- **Notification**  
  - Subscribes to order/payment/inventory events → sends emails/SMS/push  

- **AuditLog**  
  - Subscribes to all domain events → append-only log in **AuditLogs_ReadDB**  

## 🔄 Event Flow

1. **Create Order** → write DB & `OrderCreated` → Kafka  
2. **Inventory** ⤷ update stock  
3. **Payment** ⤷ charge customer  
4. **Notification** ⤷ send alerts  
5. **AuditLog** ⤷ persist event  
6. **Query APIs** ⤷ project read models  

---

> Publish once, subscribe many—decoupled, scalable, and audit-friendly.  
