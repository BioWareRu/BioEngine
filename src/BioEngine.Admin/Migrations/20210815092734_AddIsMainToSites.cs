using Microsoft.EntityFrameworkCore.Migrations;

namespace BioEngine.Admin.Migrations
{
    public partial class AddIsMainToSites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "Sites",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Blocks",
                table: "Sections",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'",
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Blocks",
                table: "Posts",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'",
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Blocks",
                table: "Pages",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'",
                oldClrType: typeof(string),
                oldType: "jsonb");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "Sites");

            migrationBuilder.AlterColumn<string>(
                name: "Blocks",
                table: "Sections",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldDefaultValueSql: "'[]'");

            migrationBuilder.AlterColumn<string>(
                name: "Blocks",
                table: "Posts",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldDefaultValueSql: "'[]'");

            migrationBuilder.AlterColumn<string>(
                name: "Blocks",
                table: "Pages",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldDefaultValueSql: "'[]'");
        }
    }
}
