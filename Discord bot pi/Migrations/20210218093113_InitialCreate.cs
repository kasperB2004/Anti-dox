using Microsoft.EntityFrameworkCore.Migrations;

namespace Anti_Dox.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    GlobalCase = table.Column<double>(type: "REAL", nullable: false),
                    ServerCase = table.Column<double>(type: "REAL", nullable: false),
                    ServerId = table.Column<long>(type: "INTEGER", nullable: false),
                    IpposterId = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    Time = table.Column<string>(type: "TEXT", nullable: true),
                    MesasageContainingIp = table.Column<string>(type: "TEXT", nullable: true),
                    Punishment = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.GlobalCase);
                });

            migrationBuilder.CreateTable(
                name: "PrefixList",
                columns: table => new
                {
                    ServerId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerName = table.Column<string>(type: "TEXT", nullable: true),
                    Prefix = table.Column<char>(type: "TEXT", nullable: false),
                    SetById = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrefixList", x => x.ServerId);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    ServerId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerName = table.Column<string>(type: "TEXT", nullable: true),
                    LogsChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    SetById = table.Column<long>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    punishment = table.Column<int>(type: "INTEGER", nullable: false),
                    Discordlogs = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.ServerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "PrefixList");

            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
