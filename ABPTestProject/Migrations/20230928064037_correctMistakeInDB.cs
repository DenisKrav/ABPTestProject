using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABPTestProject.Migrations
{
    /// <inheritdoc />
    public partial class correctMistakeInDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteVisitors",
                columns: table => new
                {
                    Device_token = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Button_color = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Price = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SiteVisi__5A171AE3B633681C", x => x.Device_token);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteVisitors");
        }
    }
}
