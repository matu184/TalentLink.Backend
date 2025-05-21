using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTippsAndTricks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Tips",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Tips_CreatedById",
                table: "Tips",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Tips_Users_CreatedById",
                table: "Tips",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tips_Users_CreatedById",
                table: "Tips");

            migrationBuilder.DropIndex(
                name: "IX_Tips_CreatedById",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Tips");
        }
    }
}
