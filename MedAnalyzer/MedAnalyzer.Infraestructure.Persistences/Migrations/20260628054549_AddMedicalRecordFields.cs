using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedAnalyzer.Infraestructure.Persistences.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalRecordFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DiagnosisInitial",
                table: "MedicalRecords",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Antecedentes",
                table: "MedicalRecords",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesConsulta",
                table: "MedicalRecords",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Antecedentes",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "ObservacionesConsulta",
                table: "MedicalRecords");

            migrationBuilder.AlterColumn<string>(
                name: "DiagnosisInitial",
                table: "MedicalRecords",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }
    }
}
