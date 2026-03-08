using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class updatedLead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadItems_Leads_LeadId",
                table: "LeadItems");

            migrationBuilder.RenameColumn(
                name: "LeadId",
                table: "LeadItems",
                newName: "OpportunityId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadItems_LeadId",
                table: "LeadItems",
                newName: "IX_LeadItems_OpportunityId");

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Opportunities",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionDate",
                table: "Opportunities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Contact_Name",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "UserLeadRights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    CanView = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLeadRights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLeadRights_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLeadRights_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLeadRights_LeadId",
                table: "UserLeadRights",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLeadRights_UserId",
                table: "UserLeadRights",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadItems_Opportunities_OpportunityId",
                table: "LeadItems",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadItems_Opportunities_OpportunityId",
                table: "LeadItems");

            migrationBuilder.DropTable(
                name: "UserLeadRights");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "DecisionDate",
                table: "Opportunities");

            migrationBuilder.RenameColumn(
                name: "OpportunityId",
                table: "LeadItems",
                newName: "LeadId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadItems_OpportunityId",
                table: "LeadItems",
                newName: "IX_LeadItems_LeadId");

            migrationBuilder.AlterColumn<string>(
                name: "Contact_Name",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadItems_Leads_LeadId",
                table: "LeadItems",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
