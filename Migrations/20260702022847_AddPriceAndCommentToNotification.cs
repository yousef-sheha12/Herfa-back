using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Herfa_back.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceAndCommentToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Notifications",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Notifications");
        }
    }
}
