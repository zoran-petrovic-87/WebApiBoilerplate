using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Data.Migrations.DevApp;

/// <summary>
/// Initial migration.
/// </summary>
public partial class InitialCreate : Migration
{
    /// <summary>
    /// Creates Logs, Users and Roles tables.
    /// </summary>
    /// <param name="migrationBuilder">The migration builder.</param>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Logs",
            columns: table => new
            {
                Id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Application = table.Column<string>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                Level = table.Column<string>(type: "TEXT", nullable: true),
                Message = table.Column<string>(type: "TEXT", nullable: true),
                Logger = table.Column<string>(type: "TEXT", nullable: true),
                CallSite = table.Column<string>(type: "TEXT", nullable: true),
                Exception = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_Logs", x => x.Id); });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                ExternalId = table.Column<string>(type: "TEXT", nullable: true),
                ExternalIdentityProvider = table.Column<string>(type: "TEXT", nullable: true),
                Username = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                GivenName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                FamilyName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                Email = table.Column<string>(type: "TEXT", maxLength: 320, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                CreatedById = table.Column<Guid>(type: "TEXT", nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                UpdatedById = table.Column<Guid>(type: "TEXT", nullable: true),
                RoleId = table.Column<Guid>(type: "TEXT", nullable: true),
                LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                LoginFailedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                LoginFailedCount = table.Column<int>(type: "INTEGER", nullable: false),
                UnconfirmedEmail = table.Column<string>(type: "TEXT", nullable: true),
                UnconfirmedEmailCreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                UnconfirmedEmailCode = table.Column<string>(type: "TEXT", nullable: true),
                UnconfirmedEmailCount = table.Column<int>(type: "INTEGER", nullable: false),
                ResetPasswordCreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                ResetPasswordCount = table.Column<int>(type: "INTEGER", nullable: false),
                ResetPasswordCode = table.Column<string>(type: "TEXT", nullable: true),
                PasswordHash = table.Column<byte[]>(type: "BLOB", nullable: true),
                PasswordSalt = table.Column<byte[]>(type: "BLOB", nullable: true),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.ForeignKey(
                    name: "FK_Users_Users_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Users_Users_UpdatedById",
                    column: x => x.UpdatedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Roles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                UpdatedById = table.Column<Guid>(type: "TEXT", nullable: true),
                Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Roles", x => x.Id);
                table.ForeignKey(
                    name: "FK_Roles_Users_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Roles_Users_UpdatedById",
                    column: x => x.UpdatedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Roles_CreatedById",
            table: "Roles",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_Roles_Name",
            table: "Roles",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Roles_UpdatedById",
            table: "Roles",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "IX_Users_CreatedById",
            table: "Users",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_Users_IsActive",
            table: "Users",
            column: "IsActive");

        migrationBuilder.CreateIndex(
            name: "IX_Users_RoleId",
            table: "Users",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_Users_UpdatedById",
            table: "Users",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Username",
            table: "Users",
            column: "Username");

        migrationBuilder.AddForeignKey(
            name: "FK_Users_Roles_RoleId",
            table: "Users",
            column: "RoleId",
            principalTable: "Roles",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <summary>
    /// Drops Logs, Users and Roles tables.
    /// </summary>
    /// <param name="migrationBuilder">The migration builder.</param>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Roles_Users_CreatedById",
            table: "Roles");

        migrationBuilder.DropForeignKey(
            name: "FK_Roles_Users_UpdatedById",
            table: "Roles");

        migrationBuilder.DropTable(
            name: "Logs");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Roles");
    }
}