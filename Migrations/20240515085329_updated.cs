using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallySoftware.Migrations
{
    /// <inheritdoc />
    public partial class updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Staffs",
                keyColumn: "StaffId",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Enquiries");

            migrationBuilder.InsertData(
                table: "Staffs",
                columns: new[] { "StaffId", "ConfirmPassword", "IsDeleted", "Password", "StaffName", "StaffType" },
                values: new object[] { 1, "snapy@2020", false, "snapy@2020", "admin", "Admin" });
        }
    }
}
