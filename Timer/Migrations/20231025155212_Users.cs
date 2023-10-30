using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timer.Migrations
{
    /// <inheritdoc />
    public partial class Users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Timers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timers_UserId",
                table: "Timers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timers_AspNetUsers_UserId",
                table: "Timers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timers_AspNetUsers_UserId",
                table: "Timers");

            migrationBuilder.DropIndex(
                name: "IX_Timers_UserId",
                table: "Timers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Timers");
        }
    }
}
