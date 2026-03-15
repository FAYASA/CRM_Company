using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedLeadmodule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadItems_Opportunities_OpportunityId",
                table: "LeadItems");

            migrationBuilder.RenameColumn(
                name: "OpportunityId",
                table: "LeadItems",
                newName: "LeadId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadItems_OpportunityId",
                table: "LeadItems",
                newName: "IX_LeadItems_LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadItems_Leads_LeadId",
                table: "LeadItems",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddForeignKey(
                name: "FK_LeadItems_Opportunities_OpportunityId",
                table: "LeadItems",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
