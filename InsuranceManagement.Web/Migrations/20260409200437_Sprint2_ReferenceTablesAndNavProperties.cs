using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InsuranceManagement.Web.Migrations
{
    /// <inheritdoc />
    public partial class Sprint2_ReferenceTablesAndNavProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "ExpenseType",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "ContactStatus",
                table: "activities");

            migrationBuilder.DropColumn(
                name: "OutcomeStatus",
                table: "activities");

            migrationBuilder.AddColumn<int>(
                name: "ProductTypeId",
                table: "sales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LeadSourceTypeId",
                table: "leads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LeadStatusTypeId",
                table: "leads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpenseTypeId",
                table: "expenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContactStatusTypeId",
                table: "activities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OutcomeStatusTypeId",
                table: "activities",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "activity_contact_status_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_contact_status_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activity_outcome_status_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_outcome_status_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "expense_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "insurance_product_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_insurance_product_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lead_assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeadId = table.Column<int>(type: "integer", nullable: false),
                    AssignedEmployeeId = table.Column<int>(type: "integer", nullable: false),
                    AssignedByUserId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignmentNote = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lead_assignments_employees_AssignedEmployeeId",
                        column: x => x.AssignedEmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lead_assignments_leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lead_assignments_users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lead_source_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_source_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lead_status_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_status_types", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_EmployeeId",
                table: "users",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_sales_AccountId",
                table: "sales",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_sales_ActivityId",
                table: "sales",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_sales_EmployeeId",
                table: "sales",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_sales_ProductTypeId",
                table: "sales",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_leads_AssignedEmployeeId",
                table: "leads",
                column: "AssignedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_leads_LeadSourceTypeId",
                table: "leads",
                column: "LeadSourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_leads_LeadStatusTypeId",
                table: "leads",
                column: "LeadStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_EmployeeId",
                table: "expenses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_ExpenseTypeId",
                table: "expenses",
                column: "ExpenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_activities_AccountId",
                table: "activities",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_activities_ContactStatusTypeId",
                table: "activities",
                column: "ContactStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_activities_EmployeeId",
                table: "activities",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_activities_OutcomeStatusTypeId",
                table: "activities",
                column: "OutcomeStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_lead_assignments_AssignedByUserId",
                table: "lead_assignments",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_lead_assignments_AssignedEmployeeId",
                table: "lead_assignments",
                column: "AssignedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_lead_assignments_LeadId",
                table: "lead_assignments",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_activities_accounts_AccountId",
                table: "activities",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_activities_activity_contact_status_types_ContactStatusTypeId",
                table: "activities",
                column: "ContactStatusTypeId",
                principalTable: "activity_contact_status_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_activities_activity_outcome_status_types_OutcomeStatusTypeId",
                table: "activities",
                column: "OutcomeStatusTypeId",
                principalTable: "activity_outcome_status_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_activities_employees_EmployeeId",
                table: "activities",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_expenses_employees_EmployeeId",
                table: "expenses",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_expenses_expense_types_ExpenseTypeId",
                table: "expenses",
                column: "ExpenseTypeId",
                principalTable: "expense_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_leads_employees_AssignedEmployeeId",
                table: "leads",
                column: "AssignedEmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_leads_lead_source_types_LeadSourceTypeId",
                table: "leads",
                column: "LeadSourceTypeId",
                principalTable: "lead_source_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_leads_lead_status_types_LeadStatusTypeId",
                table: "leads",
                column: "LeadStatusTypeId",
                principalTable: "lead_status_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_accounts_AccountId",
                table: "sales",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_activities_ActivityId",
                table: "sales",
                column: "ActivityId",
                principalTable: "activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_employees_EmployeeId",
                table: "sales",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_insurance_product_types_ProductTypeId",
                table: "sales",
                column: "ProductTypeId",
                principalTable: "insurance_product_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_employees_EmployeeId",
                table: "users",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_activities_accounts_AccountId",
                table: "activities");

            migrationBuilder.DropForeignKey(
                name: "FK_activities_activity_contact_status_types_ContactStatusTypeId",
                table: "activities");

            migrationBuilder.DropForeignKey(
                name: "FK_activities_activity_outcome_status_types_OutcomeStatusTypeId",
                table: "activities");

            migrationBuilder.DropForeignKey(
                name: "FK_activities_employees_EmployeeId",
                table: "activities");

            migrationBuilder.DropForeignKey(
                name: "FK_expenses_employees_EmployeeId",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_expenses_expense_types_ExpenseTypeId",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_leads_employees_AssignedEmployeeId",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_leads_lead_source_types_LeadSourceTypeId",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_leads_lead_status_types_LeadStatusTypeId",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_accounts_AccountId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_activities_ActivityId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_employees_EmployeeId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_insurance_product_types_ProductTypeId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_users_employees_EmployeeId",
                table: "users");

            migrationBuilder.DropTable(
                name: "activity_contact_status_types");

            migrationBuilder.DropTable(
                name: "activity_outcome_status_types");

            migrationBuilder.DropTable(
                name: "expense_types");

            migrationBuilder.DropTable(
                name: "insurance_product_types");

            migrationBuilder.DropTable(
                name: "lead_assignments");

            migrationBuilder.DropTable(
                name: "lead_source_types");

            migrationBuilder.DropTable(
                name: "lead_status_types");

            migrationBuilder.DropIndex(
                name: "IX_users_EmployeeId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_sales_AccountId",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sales_ActivityId",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sales_EmployeeId",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sales_ProductTypeId",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_leads_AssignedEmployeeId",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_LeadSourceTypeId",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_LeadStatusTypeId",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_expenses_EmployeeId",
                table: "expenses");

            migrationBuilder.DropIndex(
                name: "IX_expenses_ExpenseTypeId",
                table: "expenses");

            migrationBuilder.DropIndex(
                name: "IX_activities_AccountId",
                table: "activities");

            migrationBuilder.DropIndex(
                name: "IX_activities_ContactStatusTypeId",
                table: "activities");

            migrationBuilder.DropIndex(
                name: "IX_activities_EmployeeId",
                table: "activities");

            migrationBuilder.DropIndex(
                name: "IX_activities_OutcomeStatusTypeId",
                table: "activities");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "LeadSourceTypeId",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "LeadStatusTypeId",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "ExpenseTypeId",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "ContactStatusTypeId",
                table: "activities");

            migrationBuilder.DropColumn(
                name: "OutcomeStatusTypeId",
                table: "activities");

            migrationBuilder.AddColumn<string>(
                name: "ProductType",
                table: "sales",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "leads",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "leads",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpenseType",
                table: "expenses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactStatus",
                table: "activities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OutcomeStatus",
                table: "activities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
