using Microsoft.EntityFrameworkCore.Migrations;

namespace BioEngine.Admin.Migrations
{
    public partial class AddForumPropertiesToSiteAndPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForumId",
                table: "Sites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ForumPostId",
                table: "Posts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ForumTopicId",
                table: "Posts",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForumId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ForumPostId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ForumTopicId",
                table: "Posts");
        }
    }
}
