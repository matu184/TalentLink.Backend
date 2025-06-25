using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryImageUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobComment_Jobs_JobId",
                table: "JobComment");

            migrationBuilder.DropForeignKey(
                name: "FK_JobComment_Users_AuthorId",
                table: "JobComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobComment",
                table: "JobComment");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Jobs");

            migrationBuilder.RenameTable(
                name: "JobComment",
                newName: "JobComments");

            migrationBuilder.RenameIndex(
                name: "IX_JobComment_JobId",
                table: "JobComments",
                newName: "IX_JobComments_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobComment_AuthorId",
                table: "JobComments",
                newName: "IX_JobComments_AuthorId");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Jobs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobComments",
                table: "JobComments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JobCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CategoryId",
                table: "Jobs",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobComments_Jobs_JobId",
                table: "JobComments",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobComments_Users_AuthorId",
                table: "JobComments",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobCategories_CategoryId",
                table: "Jobs",
                column: "CategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobComments_Jobs_JobId",
                table: "JobComments");

            migrationBuilder.DropForeignKey(
                name: "FK_JobComments_Users_AuthorId",
                table: "JobComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobCategories_CategoryId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "JobCategories");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CategoryId",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobComments",
                table: "JobComments");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Jobs");

            migrationBuilder.RenameTable(
                name: "JobComments",
                newName: "JobComment");

            migrationBuilder.RenameIndex(
                name: "IX_JobComments_JobId",
                table: "JobComment",
                newName: "IX_JobComment_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobComments_AuthorId",
                table: "JobComment",
                newName: "IX_JobComment_AuthorId");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobComment",
                table: "JobComment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobComment_Jobs_JobId",
                table: "JobComment",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobComment_Users_AuthorId",
                table: "JobComment",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
