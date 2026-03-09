using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seashore_CRM.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class addedinvCustomerinLead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IndividualCustomerId",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IndividualCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualCustomers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leads_IndividualCustomerId",
                table: "Leads",
                column: "IndividualCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_IndividualCustomers_IndividualCustomerId",
                table: "Leads",
                column: "IndividualCustomerId",
                principalTable: "IndividualCustomers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_IndividualCustomers_IndividualCustomerId",
                table: "Leads");

            migrationBuilder.DropTable(
                name: "IndividualCustomers");

            migrationBuilder.DropIndex(
                name: "IX_Leads_IndividualCustomerId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "IndividualCustomerId",
                table: "Leads");
        }
    }
}
