using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FicheroNacionalPip.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordPolicyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaxPasswordAge = table.Column<int>(type: "int", nullable: false),
                    MinPasswordLength = table.Column<int>(type: "int", nullable: false),
                    RequiredNumbers = table.Column<int>(type: "int", nullable: false),
                    RequiredNonAlphanumeric = table.Column<int>(type: "int", nullable: false),
                    RequiredUppercase = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordPolicies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordPolicies");
        }
    }
}
