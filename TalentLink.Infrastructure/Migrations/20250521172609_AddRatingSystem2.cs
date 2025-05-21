using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingSystem2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Users_FromUserId",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Users_ToUserId",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.RenameTable(
                name: "Rating",
                newName: "Ratings");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_ToUserId",
                table: "Ratings",
                newName: "IX_Ratings_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_FromUserId",
                table: "Ratings",
                newName: "IX_Ratings_FromUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Users_FromUserId",
                table: "Ratings",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Users_ToUserId",
                table: "Ratings",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Users_FromUserId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Users_ToUserId",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "Rating");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_ToUserId",
                table: "Rating",
                newName: "IX_Rating_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_FromUserId",
                table: "Rating",
                newName: "IX_Rating_FromUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Users_FromUserId",
                table: "Rating",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Users_ToUserId",
                table: "Rating",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
