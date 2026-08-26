using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrienteeringService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameMapVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Visability",
                table: "Maps",
                newName: "Visibility");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Visibility",
                table: "Maps",
                newName: "Visability");
        }
    }
}
