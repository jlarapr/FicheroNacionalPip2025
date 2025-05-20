using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FicheroNacionalPip.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedPasswordPolicyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RequiredUppercase",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                comment: "Cantidad de mayúsculas requeridas",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RequiredNumbers",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                comment: "Cantidad de números requeridos",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RequiredNonAlphanumeric",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                comment: "Cantidad de caracteres especiales requeridos",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedAt",
                table: "PasswordPolicies",
                type: "datetime2",
                nullable: false,
                comment: "Fecha de última modificación",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "MinPasswordLength",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                comment: "Longitud mínima de la contraseña",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaxPasswordAge",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                comment: "Edad máxima de la contraseña en días",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "PasswordPolicies",
                type: "bit",
                nullable: false,
                comment: "Indica si la política está activa",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PasswordPolicies",
                type: "datetime2",
                nullable: false,
                comment: "Fecha de creación",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "PasswordPolicies",
                columns: new[] { "Id", "CreatedAt", "IsActive", "MaxPasswordAge", "MinPasswordLength", "ModifiedAt", "RequiredNonAlphanumeric", "RequiredNumbers", "RequiredUppercase" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 90, 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PasswordPolicies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "RequiredUppercase",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Cantidad de mayúsculas requeridas");

            migrationBuilder.AlterColumn<int>(
                name: "RequiredNumbers",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Cantidad de números requeridos");

            migrationBuilder.AlterColumn<int>(
                name: "RequiredNonAlphanumeric",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Cantidad de caracteres especiales requeridos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedAt",
                table: "PasswordPolicies",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Fecha de última modificación");

            migrationBuilder.AlterColumn<int>(
                name: "MinPasswordLength",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Longitud mínima de la contraseña");

            migrationBuilder.AlterColumn<int>(
                name: "MaxPasswordAge",
                table: "PasswordPolicies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Edad máxima de la contraseña en días");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "PasswordPolicies",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indica si la política está activa");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PasswordPolicies",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Fecha de creación");
        }
    }
}
