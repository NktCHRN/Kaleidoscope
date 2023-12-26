using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemovePostNormalizedName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_NormalizedName_NormalizedNameDuplicatesCount_BlogId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "NormalizedNameDuplicatesCount",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Posts",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NormalizedNameDuplicatesCount",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_NormalizedName_NormalizedNameDuplicatesCount_BlogId",
                table: "Posts",
                columns: new[] { "NormalizedName", "NormalizedNameDuplicatesCount", "BlogId" },
                unique: true);
        }
    }
}
