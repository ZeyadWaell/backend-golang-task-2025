using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyOrderProduct.Infrastructure.Migrations.WriteDb
{
    /// <inheritdoc />
    public partial class G_G : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_ProductItem_ProductItemId",
                table: "Inventory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventory",
                table: "Inventory");

            migrationBuilder.RenameTable(
                name: "Inventory",
                newName: "inventories");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_ProductItemId",
                table: "inventories",
                newName: "IX_inventories_ProductItemId");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "inventories",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_inventories",
                table: "inventories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_inventories_ProductItem_ProductItemId",
                table: "inventories",
                column: "ProductItemId",
                principalTable: "ProductItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventories_ProductItem_ProductItemId",
                table: "inventories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_inventories",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "inventories");

            migrationBuilder.RenameTable(
                name: "inventories",
                newName: "Inventory");

            migrationBuilder.RenameIndex(
                name: "IX_inventories_ProductItemId",
                table: "Inventory",
                newName: "IX_Inventory_ProductItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventory",
                table: "Inventory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_ProductItem_ProductItemId",
                table: "Inventory",
                column: "ProductItemId",
                principalTable: "ProductItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
