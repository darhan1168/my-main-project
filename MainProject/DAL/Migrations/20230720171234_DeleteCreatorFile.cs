using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCreatorFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_creator_id",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_creator_id",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "creator_id",
                table: "Files");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "creator_id",
                table: "Files",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Files_creator_id",
                table: "Files",
                column: "creator_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_creator_id",
                table: "Files",
                column: "creator_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
