using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListedLinks.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Author = table.Column<string>(type: "TEXT", nullable: false),
                    Blurb = table.Column<string>(type: "TEXT", nullable: true),
                    GenreName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => new { x.Title, x.Author });
                    table.ForeignKey(
                        name: "FK_Books_Genre_GenreName",
                        column: x => x.GenreName,
                        principalTable: "Genre",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_GenreName",
                table: "Books",
                column: "GenreName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
