using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetMeet.Data.Migrations.MainDb
{
    public partial class MainAppMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    emailAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    stage = table.Column<int>(type: "int", nullable: true),
                    profileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    identityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DayFrees",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<int>(type: "int", nullable: false),
                    startHour = table.Column<int>(type: "int", nullable: false),
                    endHour = table.Column<int>(type: "int", nullable: false),
                    UserInfoid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayFrees", x => x.id);
                    table.ForeignKey(
                        name: "FK_DayFrees_UserInfos_UserInfoid",
                        column: x => x.UserInfoid,
                        principalTable: "UserInfos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SupervisionInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    supervisorid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    extendTimes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisionInfo", x => x.id);
                    table.ForeignKey(
                        name: "FK_SupervisionInfo_UserInfos_studentid",
                        column: x => x.studentid,
                        principalTable: "UserInfos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_SupervisionInfo_UserInfos_supervisorid",
                        column: x => x.supervisorid,
                        principalTable: "UserInfos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    totalTimeHoure = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    startHour = table.Column<int>(type: "int", nullable: false),
                    endHour = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isPresent = table.Column<bool>(type: "bit", nullable: false),
                    SupervisionInfoid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.id);
                    table.ForeignKey(
                        name: "FK_Meetings_SupervisionInfo_SupervisionInfoid",
                        column: x => x.SupervisionInfoid,
                        principalTable: "SupervisionInfo",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "MeetingTasks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    decription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    isCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Meetingid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingTasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_MeetingTasks_Meetings_Meetingid",
                        column: x => x.Meetingid,
                        principalTable: "Meetings",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayFrees_UserInfoid",
                table: "DayFrees",
                column: "UserInfoid");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_SupervisionInfoid",
                table: "Meetings",
                column: "SupervisionInfoid");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingTasks_Meetingid",
                table: "MeetingTasks",
                column: "Meetingid");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisionInfo_studentid",
                table: "SupervisionInfo",
                column: "studentid");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisionInfo_supervisorid",
                table: "SupervisionInfo",
                column: "supervisorid");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_emailAddress",
                table: "UserInfos",
                column: "emailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_identityId",
                table: "UserInfos",
                column: "identityId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayFrees");

            migrationBuilder.DropTable(
                name: "MeetingTasks");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropTable(
                name: "SupervisionInfo");

            migrationBuilder.DropTable(
                name: "UserInfos");
        }
    }
}
