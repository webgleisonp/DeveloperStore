using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DeveloperStore.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateEntityProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    Category = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    Image = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products",
                schema: "public");
        }
    }
}
