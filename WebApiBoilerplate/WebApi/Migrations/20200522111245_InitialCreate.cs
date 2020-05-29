using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    /// <summary>
    /// The initial migration for User and Log models.
    /// </summary>
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// Creates initial Users and Logs tables in the database.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Application = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Level = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Logger = table.Column<string>(nullable: true),
                    CallSite = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    LastLoginAt = table.Column<DateTime>(nullable: true),
                    LoginFailedAt = table.Column<DateTime>(nullable: true),
                    LoginFailedCount = table.Column<int>(nullable: false),
                    UnconfirmedEmail = table.Column<string>(nullable: true),
                    UnconfirmedEmailCreatedAt = table.Column<DateTime>(nullable: true),
                    UnconfirmedEmailCode = table.Column<string>(nullable: true),
                    UnconfirmedEmailCount = table.Column<int>(nullable: false),
                    ResetPasswordCreatedAt = table.Column<DateTime>(nullable: true),
                    ResetPasswordCount = table.Column<int>(nullable: false),
                    ResetPasswordCode = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <summary>
        /// Drops Users and LOgs tables from the database.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
