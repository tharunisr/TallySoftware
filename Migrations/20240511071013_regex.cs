using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallySoftware.Migrations
{
    /// <inheritdoc />
    public partial class regex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmPassword",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Staffs",
                keyColumn: "StaffId",
                keyValue: 1,
                column: "ConfirmPassword",
                value: "snapy@2020");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmPassword",
                table: "Staffs");
        }
    }
}
