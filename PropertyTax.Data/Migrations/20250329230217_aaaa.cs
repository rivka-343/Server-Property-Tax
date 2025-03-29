using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyTax.Data.Migrations
{
    /// <inheritdoc />
    public partial class aaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Requests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FName",
                table: "Requests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Gmail",
                table: "Requests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HomeNumber",
                table: "Requests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LFName",
                table: "Requests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Requests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "FName",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Gmail",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "HomeNumber",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "LFName",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Requests");
        }
    }
}
