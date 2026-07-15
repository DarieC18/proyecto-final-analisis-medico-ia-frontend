using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedAnalyzer.Infraestructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumberIdentification",
                schema: "Identity",
                table: "Users",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberIdentification",
                schema: "Identity",
                table: "Users");
        }
    }
}
