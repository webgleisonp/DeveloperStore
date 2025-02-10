using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeveloperStore.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCartEntityCollumnActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "public",
                table: "carts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                schema: "public",
                table: "carts");
        }
    }
}
