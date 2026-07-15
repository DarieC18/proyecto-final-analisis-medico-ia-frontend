using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedAnalyzer.Infraestructure.Persistences.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VitalSigns_Patients_PatientId",
                table: "VitalSigns");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "VitalSigns",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "MedicalRecords",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedByUserId",
                table: "MedicalDocuments",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AuditLogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "RequestedByUserId",
                table: "AiAnalyses",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "AiAnalyses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AiAnalyses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_VitalSigns_Patients_PatientId",
                table: "VitalSigns",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VitalSigns_Patients_PatientId",
                table: "VitalSigns");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "AiAnalyses");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AiAnalyses");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "VitalSigns",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByUserId",
                table: "MedicalRecords",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UploadedByUserId",
                table: "MedicalDocuments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "AuditLogs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "RequestedByUserId",
                table: "AiAnalyses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_VitalSigns_Patients_PatientId",
                table: "VitalSigns",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
