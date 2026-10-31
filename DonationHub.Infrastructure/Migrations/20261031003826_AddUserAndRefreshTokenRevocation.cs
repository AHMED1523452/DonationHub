using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonationHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndRefreshTokenRevocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "preferredLanguage",
                table: "AspNetUsers",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "en",
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);

            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "preferredLanguage",
                table: "AspNetUsers",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldDefaultValue: "en");
        }
    }
}
