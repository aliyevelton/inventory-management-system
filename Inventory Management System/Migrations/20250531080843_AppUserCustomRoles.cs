using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AppUserCustomRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CustomUserRoles_UserRoleId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "UserRoleId1",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserRoleId1",
                table: "AspNetUsers",
                column: "UserRoleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CustomUserRoles_UserRoleId",
                table: "AspNetUsers",
                column: "UserRoleId",
                principalTable: "CustomUserRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CustomUserRoles_UserRoleId1",
                table: "AspNetUsers",
                column: "UserRoleId1",
                principalTable: "CustomUserRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CustomUserRoles_UserRoleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CustomUserRoles_UserRoleId1",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserRoleId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserRoleId1",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CustomUserRoles_UserRoleId",
                table: "AspNetUsers",
                column: "UserRoleId",
                principalTable: "CustomUserRoles",
                principalColumn: "Id");
        }
    }
}
