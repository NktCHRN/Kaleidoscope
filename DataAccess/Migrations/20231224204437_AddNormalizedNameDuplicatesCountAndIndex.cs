using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizedNameDuplicatesCountAndIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_NormalizedName_NormalizedNameDuplicatesCount_BlogId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "NormalizedNameDuplicatesCount",
                table: "Posts");
        }
    }
}
