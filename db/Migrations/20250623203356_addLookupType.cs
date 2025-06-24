using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CAS.DB.Migrations
{
    /// <inheritdoc />
    public partial class addLookupType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateSequence<int>(
                name: "LookupType_Code_seq",
                startValue: 100L,
                incrementBy: 1
            );
            migrationBuilder.CreateTable(
                name: "LookupType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"LookupType_Code_seq\"')"),
                    SortOrder = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Abbreviation = table.Column<string>(type: "text", nullable: true),
                    DisplayColor = table.Column<string>(type: "text", nullable: true),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LookupType_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LookupType_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "LookupType",
                columns: new[] { "Id", "Abbreviation", "Category", "Code", "CreatedById", "CreatedOn", "Description", "DisplayColor", "ExpiryDate", "IsSystem", "Name", "SortOrder", "UpdatedById", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "CR", 0, 0, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8930), new TimeSpan(0, 0, 0, 0, 0)), "Court Room", "#ffb007", null, true, "CourtRoom", 1, null, null },
                    { 2, "CA", 0, 1, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8933), new TimeSpan(0, 0, 0, 0, 0)), "Court Assignment", "#189fd4", null, null, "CourtRole", 2, null, null },
                    { 3, "J", 0, 2, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8935), new TimeSpan(0, 0, 0, 0, 0)), "Jail Assignment", "#A22BB9", null, null, "JailRole", 3, null, null },
                    { 4, "T", 0, 3, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8936), new TimeSpan(0, 0, 0, 0, 0)), "Transport Assignment", "#ffb007", null, null, "EscortRun", 4, null, null },
                    { 5, "O", 0, 4, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8938), new TimeSpan(0, 0, 0, 0, 0)), "Other Assignment", "#7a4528", null, null, "OtherAssignment", 5, null, null },
                    { 6, null, 1, 5, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8939), new TimeSpan(0, 0, 0, 0, 0)), "Leave Type", null, null, null, "LeaveType", 1, null, null },
                    { 7, null, 2, 6, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8941), new TimeSpan(0, 0, 0, 0, 0)), "Training Type", null, null, null, "TrainingType", 1, null, null },
                    { 8, null, 3, 7, new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8942), new TimeSpan(0, 0, 0, 0, 0)), "Court Admin Rank", null, null, null, "CourtAdminRank", 1, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LookupType_Code",
                table: "LookupType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LookupType_CreatedById",
                table: "LookupType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LookupType_Name",
                table: "LookupType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LookupType_UpdatedById",
                table: "LookupType",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LookupType");
        }
    }
}
