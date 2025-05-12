using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FicheroNacionalPip.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AreasDeAcceso = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Locked = table.Column<bool>(type: "bit", nullable: true),
                    Disable = table.Column<bool>(type: "bit", nullable: true),
                    ForceChangePassword = table.Column<bool>(type: "bit", nullable: true),
                    StampDatePassword = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AreasDeAcceso", "Disable", "Email", "ForceChangePassword", "Locked", "PasswordHash", "SecurityStamp", "StampDatePassword", "UserId", "UserName" },
                values: new object[] { 1, "ALL", false, "admin@ficheironacionalpip.com", true, false, "AQAAAAIAAYagAAAAELbVqNJnC3Dw0v2Rzf8J0Xr8qQBfVBHzXmXZqhJeVxOtUXD+ZWGdPMK6GQpGXtGJrw==", "1712ef28-6211-43f1-a789-b4ed3101a946", "2025-05-12 23:43:32", new Guid("8e445865-a24d-4543-a6c6-9443d048cdb9"), "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserId",
                table: "Users",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
