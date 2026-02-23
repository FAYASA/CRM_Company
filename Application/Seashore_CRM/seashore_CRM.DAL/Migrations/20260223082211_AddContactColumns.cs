using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddContactColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Contacts",
                newName: "Mobile");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Contacts",
                newName: "Designation");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Contacts",
                newName: "Contact_Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mobile",
                table: "Contacts",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "Designation",
                table: "Contacts",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Contact_Name",
                table: "Contacts",
                newName: "FirstName");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
