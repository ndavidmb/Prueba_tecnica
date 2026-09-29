using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocalTeamId = table.Column<int>(type: "integer", nullable: false),
                    VisitorTeamId = table.Column<int>(type: "integer", nullable: false),
                    LocalGoals = table.Column<int>(type: "integer", nullable: true),
                    VisitorGoals = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.CheckConstraint("CK_Match_DifferentTeams", "\"LocalTeamId\" <> \"VisitorTeamId\"");
                    table.ForeignKey(
                        name: "FK_Matches_Teams_LocalTeamId",
                        column: x => x.LocalTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_VisitorTeamId",
                        column: x => x.VisitorTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    LocalGoals = table.Column<int>(type: "integer", nullable: false),
                    VisitorGoals = table.Column<int>(type: "integer", nullable: false),
                    PointsEarned = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bets", x => x.Id);
                    table.CheckConstraint("CK_Bet_PointsEarned", "\"PointsEarned\" IN (0, 1, 3)");
                    table.ForeignKey(
                        name: "FK_Bets_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Real Madrid" },
                    { 2, "Bayern München" },
                    { 3, "Paris Saint-Germain" },
                    { 4, "Inter de Milán" },
                    { 5, "Manchester City" },
                    { 6, "FC Barcelona" },
                    { 7, "Borussia Dortmund" },
                    { 8, "Arsenal" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "password", "Role" },
                values: new object[,]
                {
                    { 1, "player1@player.com", "Juan Díaz", "$2b$11$DaqubqB.PVWKKtzFqBknAuef4ywFvt5XO5EtRsM.8/4p/IpCud2Ca", "Player" },
                    { 2, "admin@admin.com", "Luisa Perez", "$2b$11$DaqubqB.PVWKKtzFqBknAuef4ywFvt5XO5EtRsM.8/4p/IpCud2Ca", "Admin" },
                    { 3, "player@player.com", "Mario Torres", "$2b$11$DaqubqB.PVWKKtzFqBknAuef4ywFvt5XO5EtRsM.8/4p/IpCud2Ca", "Player" }
                });

            migrationBuilder.InsertData(
                table: "Matches",
                columns: new[] { "Id", "LocalGoals", "LocalTeamId", "Status", "VisitorGoals", "VisitorTeamId" },
                values: new object[,]
                {
                    { 8, null, 5, "UpcomingMatch", null, 6 },
                    { 9, null, 7, "UpcomingMatch", null, 8 },
                    { 10, null, 5, "UpcomingMatch", null, 7 },
                    { 11, null, 6, "UpcomingMatch", null, 8 },
                    { 12, null, 8, "UpcomingMatch", null, 5 },
                    { 13, null, 6, "UpcomingMatch", null, 7 },
                    { 14, null, 1, "UpcomingMatch", null, 2 },
                    { 15, null, 3, "UpcomingMatch", null, 4 },
                    { 16, null, 1, "UpcomingMatch", null, 3 },
                    { 17, null, 2, "UpcomingMatch", null, 4 },
                    { 18, null, 4, "UpcomingMatch", null, 1 },
                    { 19, null, 2, "UpcomingMatch", null, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bets_MatchId",
                table: "Bets",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Bets_UserId_MatchId",
                table: "Bets",
                columns: new[] { "UserId", "MatchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_LocalTeamId",
                table: "Matches",
                column: "LocalTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_VisitorTeamId",
                table: "Matches",
                column: "VisitorTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bets");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
