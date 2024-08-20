using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logger.DataAccess.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RollbackField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserIds",
                table: "Logs",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Logs",
                newName: "UserIds");
        }
    }
}
