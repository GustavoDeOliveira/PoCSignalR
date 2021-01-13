using Microsoft.EntityFrameworkCore.Migrations;

namespace TesteSignalR.Migrations
{
    public partial class AddHubConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_CoordinateTargetId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CoordinateTargetId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CoordinateTargetId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "HubConnections",
                columns: table => new
                {
                    UserAId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserBId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_HubConnections_AspNetUsers_UserAId",
                        column: x => x.UserAId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_HubConnections_AspNetUsers_UserBId",
                        column: x => x.UserBId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HubConnections_UserAId",
                table: "HubConnections",
                column: "UserAId");

            migrationBuilder.CreateIndex(
                name: "IX_HubConnections_UserBId",
                table: "HubConnections",
                column: "UserBId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HubConnections");

            migrationBuilder.AddColumn<string>(
                name: "CoordinateTargetId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CoordinateTargetId",
                table: "AspNetUsers",
                column: "CoordinateTargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_CoordinateTargetId",
                table: "AspNetUsers",
                column: "CoordinateTargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
