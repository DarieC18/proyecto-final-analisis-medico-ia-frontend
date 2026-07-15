using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedAnalyzer.Infraestructure.Persistences.Migrations
{
    /// <inheritdoc />
    public partial class FixAppointmentDoctorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Patients_PatientId",
                table: "MedicalRecords");

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

            migrationBuilder.AddColumn<string>(
                name: "IdentificationType",
                table: "Patients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Patients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PatientType",
                table: "Patients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "DiagnosisInitial",
                table: "MedicalRecords",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

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
                name: "DoctorId",
                table: "Appointments",
                type: "character varying(450)",
                maxLength: 450,
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
                name: "FK_MedicalRecords_Patients_PatientId",
                table: "MedicalRecords",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_MedicalRecords_Patients_PatientId",
                table: "MedicalRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_VitalSigns_Patients_PatientId",
                table: "VitalSigns");

            migrationBuilder.DropColumn(
                name: "IdentificationType",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientType",
                table: "Patients");

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
                name: "DoctorId",
                table: "Appointments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<int>(
                name: "RequestedByUserId",
                table: "AiAnalyses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Patients_PatientId",
                table: "MedicalRecords",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
