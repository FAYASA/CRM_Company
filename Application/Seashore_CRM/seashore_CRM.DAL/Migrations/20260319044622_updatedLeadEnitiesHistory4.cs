using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedLeadEnitiesHistory4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "LeadItem",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LeadItem",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
