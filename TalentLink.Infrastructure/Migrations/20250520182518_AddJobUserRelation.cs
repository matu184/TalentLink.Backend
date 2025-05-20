using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Seniors_SeniorId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_SeniorId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SeniorId",
                table: "Jobs");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CreatedById",
                table: "Jobs",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_CreatedById",
                table: "Jobs",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_CreatedById",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CreatedById",
                table: "Jobs");

            migrationBuilder.AddColumn<Guid>(
                name: "SeniorId",
                table: "Jobs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_SeniorId",
                table: "Jobs",
                column: "SeniorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Seniors_SeniorId",
                table: "Jobs",
                column: "SeniorId",
                principalTable: "Seniors",
                principalColumn: "Id");
        }
    }
}
