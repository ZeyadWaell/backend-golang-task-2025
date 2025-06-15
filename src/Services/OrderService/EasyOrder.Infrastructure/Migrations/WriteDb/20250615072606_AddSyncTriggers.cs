using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyOrder.Infrastructure.Migrations.WriteDb
{
    public partial class AddSyncTriggers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- Orders trigger ---
            migrationBuilder.Sql(@"
CREATE OR ALTER TRIGGER dbo.trg_SyncOrders
ON dbo.Orders
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Allow identity insert on read‐side
    EXEC('SET IDENTITY_INSERT [OrderReadDb].dbo.Orders ON;');

    -- Delete removed rows
    DELETE R
    FROM [OrderReadDb].dbo.Orders AS R
    LEFT JOIN inserted AS i ON R.Id = i.Id
    WHERE i.Id IS NULL
      AND EXISTS (SELECT 1 FROM deleted AS d WHERE d.Id = R.Id);

    -- Upsert new/updated rows
    MERGE [OrderReadDb].dbo.Orders AS target
    USING inserted AS src
      ON target.Id = src.Id
    WHEN MATCHED THEN
      UPDATE SET
         Status      = src.Status,
         TotalAmount = src.TotalAmount,
         Currency    = src.Currency,
         PlacedAt    = src.PlacedAt,
         PaidAt      = src.PaidAt,
         CancelledAt = src.CancelledAt
    WHEN NOT MATCHED BY TARGET THEN
      INSERT (Id, Status, TotalAmount, Currency, PlacedAt, PaidAt, CancelledAt)
      VALUES (src.Id, src.Status, src.TotalAmount, src.Currency, src.PlacedAt, src.PaidAt, src.CancelledAt);

    -- Turn identity insert back off
    EXEC('SET IDENTITY_INSERT [OrderReadDb].dbo.Orders OFF;');
END;
");

            // --- OrderItems trigger ---
            migrationBuilder.Sql(@"
CREATE OR ALTER TRIGGER dbo.trg_SyncOrderItems
ON dbo.OrderItems
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    EXEC('SET IDENTITY_INSERT [OrderReadDb].dbo.OrderItems ON;');

    DELETE R
    FROM [OrderReadDb].dbo.OrderItems AS R
    LEFT JOIN inserted AS i ON R.Id = i.Id
    WHERE i.Id IS NULL
      AND EXISTS (SELECT 1 FROM deleted AS d WHERE d.Id = R.Id);

    MERGE [OrderReadDb].dbo.OrderItems AS target
    USING inserted AS src
      ON target.Id = src.Id
    WHEN MATCHED THEN
      UPDATE SET
         OrderId       = src.OrderId,
         ProductItemId = src.ProductItemId,
         Quantity      = src.Quantity,
         UnitPrice     = src.UnitPrice,
         SubTotal      = src.SubTotal
    WHEN NOT MATCHED BY TARGET THEN
      INSERT (Id, OrderId, ProductItemId, Quantity, UnitPrice, SubTotal)
      VALUES (src.Id, src.OrderId, src.ProductItemId, src.Quantity, src.UnitPrice, src.SubTotal);

    EXEC('SET IDENTITY_INSERT [OrderReadDb].dbo.OrderItems OFF;');
END;
");

            // --- Payments trigger ---
            migrationBuilder.Sql(@"
CREATE OR ALTER TRIGGER dbo.trg_SyncPayments
ON dbo.Payments
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Payments uses OrderId as PK, so no identity‐insert needed here 
    DELETE R
    FROM [OrderReadDb].dbo.Payments AS R
    LEFT JOIN inserted AS i ON R.OrderId = i.OrderId
    WHERE i.OrderId IS NULL
      AND EXISTS (SELECT 1 FROM deleted AS d WHERE d.OrderId = R.OrderId);

    MERGE [OrderReadDb].dbo.Payments AS target
    USING inserted AS src
      ON target.OrderId = src.OrderId
    WHEN MATCHED THEN
      UPDATE SET
         TransactionId = src.TransactionId,
         Amount        = src.Amount,
         Currency      = src.Currency,
         Method        = src.Method,
         Status        = src.Status,
         ProcessedAt   = src.ProcessedAt
    WHEN NOT MATCHED BY TARGET THEN
      INSERT (OrderId, TransactionId, Amount, Currency, Method, Status, ProcessedAt)
      VALUES (src.OrderId, src.TransactionId, src.Amount, src.Currency, src.Method, src.Status, src.ProcessedAt);
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS dbo.trg_SyncOrders;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS dbo.trg_SyncOrderItems;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS dbo.trg_SyncPayments;");
        }
    }
}
