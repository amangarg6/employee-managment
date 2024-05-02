using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Login_Register.Migrations
{
    /// <inheritdoc />
    public partial class apply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "applyjobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReciverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserStatus = table.Column<int>(type: "int", nullable: false),
                    UserActions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applyjobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_applyjobs_AspNetUsers_ReciverId",
                        column: x => x.ReciverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_applyjobs_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_applyjobs_ReciverId",
                table: "applyjobs",
                column: "ReciverId");

            migrationBuilder.CreateIndex(
                name: "IX_applyjobs_SenderId",
                table: "applyjobs",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applyjobs");
        }
    }
}
