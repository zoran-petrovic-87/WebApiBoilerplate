using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebApi.Data.Migrations.App;

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
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Application = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Level = table.Column<string>(type: "text", nullable: true),
                Message = table.Column<string>(type: "text", nullable: true),
                Logger = table.Column<string>(type: "text", nullable: true),
                CallSite = table.Column<string>(type: "text", nullable: true),
                Exception = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Logs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ExternalId = table.Column<string>(type: "text", nullable: true),
                ExternalIdentityProvider = table.Column<string>(type: "text", nullable: true),
                Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                GivenName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                FamilyName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                RoleId = table.Column<Guid>(type: "uuid", nullable: true),
                LastLoginAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                LoginFailedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                LoginFailedCount = table.Column<int>(type: "integer", nullable: false),
                UnconfirmedEmail = table.Column<string>(type: "text", nullable: true),
                UnconfirmedEmailCreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UnconfirmedEmailCode = table.Column<string>(type: "text", nullable: true),
                UnconfirmedEmailCount = table.Column<int>(type: "integer", nullable: false),
                ResetPasswordCreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                ResetPasswordCount = table.Column<int>(type: "integer", nullable: false),
                ResetPasswordCode = table.Column<string>(type: "text", nullable: true),
                PasswordHash = table.Column<byte[]>(type: "bytea", nullable: true),
                PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false)
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
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Description = table.Column<string>(type: "text", nullable: true)
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