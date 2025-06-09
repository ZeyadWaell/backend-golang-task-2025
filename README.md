# E-Commerce CQRS + Kafka Overview


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
