using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedLeadEnitiesHistory5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadHistories_Activities_RelatedActivityId",
                table: "LeadHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadStatusActivities_Activities_ActivityId",
                table: "LeadStatusActivities");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_LeadStatusActivities_ActivityId",
                table: "LeadStatusActivities");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "LeadStatusActivities");

            migrationBuilder.RenameColumn(
                name: "RelatedActivityId",
                table: "LeadHistories",
                newName: "RelatedLeadStatusActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadHistories_RelatedActivityId",
                table: "LeadHistories",
                newName: "IX_LeadHistories_RelatedLeadStatusActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadHistories_LeadStatusActivities_RelatedLeadStatusActivityId",
                table: "LeadHistories",
                column: "RelatedLeadStatusActivityId",
                principalTable: "LeadStatusActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadHistories_LeadStatusActivities_RelatedLeadStatusActivityId",
                table: "LeadHistories");

            migrationBuilder.RenameColumn(
                name: "RelatedLeadStatusActivityId",
                table: "LeadHistories",
                newName: "RelatedActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadHistories_RelatedLeadStatusActivityId",
                table: "LeadHistories",
                newName: "IX_LeadHistories_RelatedActivityId");

            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "LeadStatusActivities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    LeadId = table.Column<int>(type: "int", nullable: true),
                    ActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NextFollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Companies_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activities_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activities_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadStatusActivities_ActivityId",
                table: "LeadStatusActivities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CreatedById",
                table: "Activities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CustomerId",
                table: "Activities",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_LeadId",
                table: "Activities",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadHistories_Activities_RelatedActivityId",
                table: "LeadHistories",
                column: "RelatedActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadStatusActivities_Activities_ActivityId",
                table: "LeadStatusActivities",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id");
        }
    }
}
