using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CAS.DB.Migrations
{
    /// <inheritdoc />
    public partial class changeCourtRoomDisplayColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LookupType",
                keyColumn: "Id",
                keyValue: 1,
                column: "DisplayColor",
                value: "#6C3BAA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LookupType",
                keyColumn: "Id",
                keyValue: 1,
                column: "DisplayColor",
                value: "#ffb007");
        }
    }
}
