using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Login_Register.Migrations
{
    /// <inheritdoc />
    public partial class updatecompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "companies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "companies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
