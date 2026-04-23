using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedLeadEnities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConverted",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "QualifiedById",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "QualifiedOn",
                table: "Leads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConverted",
                table: "Leads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "QualifiedById",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QualifiedOn",
                table: "Leads",
                type: "datetime2",
                nullable: true);
        }
    }
}
