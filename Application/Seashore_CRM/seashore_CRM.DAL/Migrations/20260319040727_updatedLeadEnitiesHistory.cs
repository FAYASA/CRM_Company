using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatedLeadEnitiesHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadItems_Leads_LeadId",
                table: "LeadItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadItems_Products_ProductId",
                table: "LeadItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_IndividualCustomers_IndividualCustomerId",
                table: "Leads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeadItems",
                table: "LeadItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IndividualCustomers",
                table: "IndividualCustomers");

            migrationBuilder.RenameTable(
                name: "LeadItems",
                newName: "LeadItem");

            migrationBuilder.RenameTable(
                name: "IndividualCustomers",
                newName: "IndividualCustomer");

            migrationBuilder.RenameIndex(
                name: "IX_LeadItems_ProductId",
                table: "LeadItem",
                newName: "IX_LeadItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadItems_LeadId",
                table: "LeadItem",
                newName: "IX_LeadItem_LeadId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivityDate",
                table: "LeadStatusActivities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // make ActivityId nullable
            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "LeadStatusActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "LeadStatusActivities",
                type: "int",
                nullable: true);

            // make LeadId nullable to avoid default 0 duplicate keys when applied to existing data
            migrationBuilder.AddColumn<int>(
                name: "LeadId",
                table: "LeadStatusActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextFollowUpDate",
                table: "LeadStatusActivities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActivityName",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "LeadItem",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LeadItem",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeadItem",
                table: "LeadItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IndividualCustomer",
                table: "IndividualCustomer",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LeadHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldStatusId = table.Column<int>(type: "int", nullable: true),
                    OldStatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewStatusId = table.Column<int>(type: "int", nullable: true),
                    NewStatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedById = table.Column<int>(type: "int", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RelatedActivityId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadHistories_Activities_RelatedActivityId",
                        column: x => x.RelatedActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LeadHistories_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadHistories_Users_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadStatusActivities_ActivityId",
                table: "LeadStatusActivities",
                column: "ActivityId");

            // create non-unique index on LeadId to avoid duplicate key error
            migrationBuilder.CreateIndex(
                name: "IX_LeadStatusActivities_LeadId",
                table: "LeadStatusActivities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadHistories_ChangedAt",
                table: "LeadHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LeadHistories_ChangedById",
                table: "LeadHistories",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_LeadHistories_LeadId",
                table: "LeadHistories",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadHistories_RelatedActivityId",
                table: "LeadHistories",
                column: "RelatedActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadItem_Leads_LeadId",
                table: "LeadItem",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadItem_Products_ProductId",
                table: "LeadItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_IndividualCustomer_IndividualCustomerId",
                table: "Leads",
                column: "IndividualCustomerId",
                principalTable: "IndividualCustomer",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadStatusActivities_Activities_ActivityId",
                table: "LeadStatusActivities",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadStatusActivities_Leads_LeadId",
                table: "LeadStatusActivities",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadItem_Leads_LeadId",
                table: "LeadItem");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadItem_Products_ProductId",
                table: "LeadItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_IndividualCustomer_IndividualCustomerId",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadStatusActivities_Activities_ActivityId",
                table: "LeadStatusActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadStatusActivities_Leads_LeadId",
                table: "LeadStatusActivities");

            migrationBuilder.DropTable(
                name: "LeadHistories");

            migrationBuilder.DropIndex(
                name: "IX_LeadStatusActivities_ActivityId",
                table: "LeadStatusActivities");

            migrationBuilder.DropIndex(
                name: "IX_LeadStatusActivities_LeadId",
                table: "LeadStatusActivities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeadItem",
                table: "LeadItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IndividualCustomer",
                table: "IndividualCustomer");

            migrationBuilder.DropColumn(
                name: "ActivityDate",
                table: "LeadStatusActivities");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "LeadStatusActivities");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "LeadStatusActivities");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "LeadStatusActivities");

            migrationBuilder.DropColumn(
                name: "NextFollowUpDate",
                table: "LeadStatusActivities");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "ActivityName",
                table: "Activities");

            migrationBuilder.RenameTable(
                name: "LeadItem",
                newName: "LeadItems");

            migrationBuilder.RenameTable(
                name: "IndividualCustomer",
                newName: "IndividualCustomers");

            migrationBuilder.RenameIndex(
                name: "IX_LeadItem_ProductId",
                table: "LeadItems",
                newName: "IX_LeadItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_LeadItem_LeadId",
                table: "LeadItems",
                newName: "IX_LeadItems_LeadId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "LeadItems",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LeadItems",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeadItems",
                table: "LeadItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IndividualCustomers",
                table: "IndividualCustomers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadItems_Leads_LeadId",
                table: "LeadItems",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadItems_Products_ProductId",
                table: "LeadItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_IndividualCustomers_IndividualCustomerId",
                table: "Leads",
                column: "IndividualCustomerId",
                principalTable: "IndividualCustomers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
