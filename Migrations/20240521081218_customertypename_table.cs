using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallySoftware.Migrations
{
    /// <inheritdoc />
    public partial class customertypename_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerType_CustomerTypeId",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerType",
                table: "CustomerType");

            migrationBuilder.RenameTable(
                name: "CustomerType",
                newName: "CustomerTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerTypes",
                table: "CustomerTypes",
                column: "CustomerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerTypes_CustomerTypeId",
                table: "Customers",
                column: "CustomerTypeId",
                principalTable: "CustomerTypes",
                principalColumn: "CustomerTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerTypes_CustomerTypeId",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerTypes",
                table: "CustomerTypes");

            migrationBuilder.RenameTable(
                name: "CustomerTypes",
                newName: "CustomerType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerType",
                table: "CustomerType",
                column: "CustomerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerType_CustomerTypeId",
                table: "Customers",
                column: "CustomerTypeId",
                principalTable: "CustomerType",
                principalColumn: "CustomerTypeId");
        }
    }
}
