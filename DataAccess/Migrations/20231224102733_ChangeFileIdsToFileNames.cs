using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFileIdsToFileNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "PostItems",
                newName: "LocalFileName");

            migrationBuilder.RenameColumn(
                name: "AvatarId",
                table: "Blogs",
                newName: "AvatarLocalFileName");

            migrationBuilder.RenameColumn(
                name: "AvatarId",
                table: "AspNetUsers",
                newName: "AvatarLocalFileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocalFileName",
                table: "PostItems",
                newName: "FileId");

            migrationBuilder.RenameColumn(
                name: "AvatarLocalFileName",
                table: "Blogs",
                newName: "AvatarId");

            migrationBuilder.RenameColumn(
                name: "AvatarLocalFileName",
                table: "AspNetUsers",
                newName: "AvatarId");
        }
    }
}
