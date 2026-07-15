using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedAnalyzer.Infraestructure.Persistences.Migrations
{
    /// <inheritdoc />
    public partial class FixAiAnalysisRequestedByUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"AiAnalyses\" ALTER COLUMN \"RequestedByUserId\" TYPE varchar(450) USING \"RequestedByUserId\"::text;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"AiAnalyses\" ALTER COLUMN \"RequestedByUserId\" TYPE integer USING \"RequestedByUserId\"::integer;");
        }
    }
}
