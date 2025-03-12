using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SageSupervisor.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalHT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalHT",
                table: "DocumentChangeDtos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalHT",
                table: "DocumentChangeDtos");
        }
    }
}
