using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DeveloperStore.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Latitude = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: true),
                    Longitude = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: true),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    PostCode = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    Street = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users",
                schema: "public");
        }
    }
}
