using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallySoftware.Migrations
{
    /// <inheritdoc />
    public partial class addedenquiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnquiryEntity_Customers_CustomerId",
                table: "EnquiryEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnquiryEntity",
                table: "EnquiryEntity");

            migrationBuilder.RenameTable(
                name: "EnquiryEntity",
                newName: "Enquiries");

            migrationBuilder.RenameIndex(
                name: "IX_EnquiryEntity_CustomerId",
                table: "Enquiries",
                newName: "IX_Enquiries_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enquiries",
                table: "Enquiries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_Customers_CustomerId",
                table: "Enquiries",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_Customers_CustomerId",
                table: "Enquiries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enquiries",
                table: "Enquiries");

            migrationBuilder.RenameTable(
                name: "Enquiries",
                newName: "EnquiryEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Enquiries_CustomerId",
                table: "EnquiryEntity",
                newName: "IX_EnquiryEntity_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnquiryEntity",
                table: "EnquiryEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EnquiryEntity_Customers_CustomerId",
                table: "EnquiryEntity",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
