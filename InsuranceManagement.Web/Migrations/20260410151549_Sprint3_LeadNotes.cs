using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InsuranceManagement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3_LeadNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "lead_assignments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);





            migrationBuilder.CreateTable(
                name: "lead_notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeadId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lead_notes_leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activities_LeadId",
                table: "activities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_OwnerEmployeeId",
                table: "accounts",
                column: "OwnerEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_lead_notes_LeadId",
                table: "lead_notes",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_employees_OwnerEmployeeId",
                table: "accounts",
                column: "OwnerEmployeeId",
                principalTable: "employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_activities_leads_LeadId",
                table: "activities",
                column: "LeadId",
                principalTable: "leads",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_employees_OwnerEmployeeId",
                table: "accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_activities_leads_LeadId",
                table: "activities");

            migrationBuilder.DropTable(
                name: "lead_notes");

            migrationBuilder.DropIndex(
                name: "IX_activities_LeadId",
                table: "activities");

            migrationBuilder.DropIndex(
                name: "IX_accounts_OwnerEmployeeId",
                table: "accounts");



            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "lead_assignments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
