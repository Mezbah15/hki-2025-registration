using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hki_2025_registration.Migrations
{
    /// <inheritdoc />
    public partial class Added_CreatePayment_Properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "Participants",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Participants",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Participants");
        }
    }
}
