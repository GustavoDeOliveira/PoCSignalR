using Microsoft.EntityFrameworkCore.Migrations;

namespace TesteSignalR.Migrations
{
    public partial class AddHubConnectionKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HubConnections_UserAId",
                table: "HubConnections");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HubConnections",
                table: "HubConnections",
                columns: new[] { "UserAId", "UserBId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HubConnections",
                table: "HubConnections");

            migrationBuilder.CreateIndex(
                name: "IX_HubConnections_UserAId",
                table: "HubConnections",
                column: "UserAId");
        }
    }
}
