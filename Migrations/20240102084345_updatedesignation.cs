using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Login_Register.Migrations
{
    /// <inheritdoc />
    public partial class updatedesignation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "designations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_designations_EmployeeId",
                table: "designations",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_designations_employees_EmployeeId",
                table: "designations",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_designations_employees_EmployeeId",
                table: "designations");

            migrationBuilder.DropIndex(
                name: "IX_designations_EmployeeId",
                table: "designations");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "designations");
        }
    }
}
