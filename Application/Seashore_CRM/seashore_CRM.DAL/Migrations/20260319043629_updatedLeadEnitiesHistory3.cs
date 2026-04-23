using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedLeadEnitiesHistory3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeadId1",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_LeadId1",
                table: "Activities",
                column: "LeadId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Leads_LeadId1",
                table: "Activities",
                column: "LeadId1",
                principalTable: "Leads",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Leads_LeadId1",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_LeadId1",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "LeadId1",
                table: "Activities");
        }
    }
}
