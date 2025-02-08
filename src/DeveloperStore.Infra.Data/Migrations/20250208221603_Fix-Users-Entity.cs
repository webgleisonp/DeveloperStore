using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeveloperStore.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixUsersEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "public",
                table: "users",
                type: "character varying(12)",
                unicode: false,
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldUnicode: false,
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "public",
                table: "users",
                type: "character varying(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "public",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "public",
                table: "users",
                type: "character varying(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldUnicode: false,
                oldMaxLength: 12);
        }
    }
}
