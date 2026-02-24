using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserEn2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Users",
                newName: "Region");

            migrationBuilder.AddColumn<string>(
                name: "Contact",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportToUserId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReportToUserId",
                table: "Users",
                column: "ReportToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_ReportToUserId",
                table: "Users",
                column: "ReportToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_ReportToUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ReportToUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Contact",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReportToUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Users",
                newName: "Phone");
        }
    }
}
