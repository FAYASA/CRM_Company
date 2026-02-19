using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Migration05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Leads",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionDate",
                table: "Leads",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsQualified",
                table: "Leads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Probability",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QualificationNotes",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "DecisionDate",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "IsQualified",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Probability",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "QualificationNotes",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "QualifiedById",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "QualifiedOn",
                table: "Leads");
        }
    }
}
