using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CAS.DB.Migrations
{
    /// <inheritdoc />
    public partial class addLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Location",
                columns: new[] { "Id", "AgencyId", "CreatedById", "ExpiryDate", "JustinCode", "Name", "ParentLocationId", "RegionId", "Timezone", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 7, "SS7", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Vancouver Island Virtual Registry (VIVR)", null, 1, "America/Vancouver", null, null },
                    { 8, "SS8", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Virtual Bail", null, 1, "America/Vancouver", null, null },
                    { 9, "SS9", new Guid("00000000-0000-0000-0000-000000000001"), null, null, "Vancouver Island Regional HQ", null, 1, "America/Vancouver", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Location",
                keyColumn: "Id",
                keyValue: 9);
        }
    }
}
