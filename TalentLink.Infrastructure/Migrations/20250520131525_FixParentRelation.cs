using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixParentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifiedStudents",
                table: "Parents");

            migrationBuilder.CreateTable(
                name: "VerifiedStudents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StudentId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifiedStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerifiedStudents_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VerifiedStudents_ParentId",
                table: "VerifiedStudents",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerifiedStudents");

            migrationBuilder.AddColumn<string>(
                name: "VerifiedStudents",
                table: "Parents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
