using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAssignedToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAssigned",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAssigned",
                table: "Jobs");
        }
    }
}
