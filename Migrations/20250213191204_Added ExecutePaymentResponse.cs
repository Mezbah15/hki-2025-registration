using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hki_2025_registration.Migrations
{
    /// <inheritdoc />
    public partial class AddedExecutePaymentResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExecutePaymentResponse",
                table: "Participants",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutePaymentResponse",
                table: "Participants");
        }
    }
}
