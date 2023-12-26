using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TextPostItemFormattings");

            migrationBuilder.DropTable(
                name: "TextPostItemLinks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TextPostItemFormattings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextPostItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    End = table.Column<int>(type: "int", nullable: false),
                    Formatting = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Start = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextPostItemFormattings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextPostItemFormattings_PostItems_TextPostItemId",
                        column: x => x.TextPostItemId,
                        principalTable: "PostItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TextPostItemLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextPostItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    End = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextPostItemLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextPostItemLinks_PostItems_TextPostItemId",
                        column: x => x.TextPostItemId,
                        principalTable: "PostItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TextPostItemFormattings_TextPostItemId",
                table: "TextPostItemFormattings",
                column: "TextPostItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TextPostItemLinks_TextPostItemId",
                table: "TextPostItemLinks",
                column: "TextPostItemId");
        }
    }
}
