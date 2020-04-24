using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class Intial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "genre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                        .Annotation("MySql:Collation", "utf8_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genre", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "genre");
        }
    }
}
