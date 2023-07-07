using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneticAIApp.Data.Migrations
{
    public partial class @base : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GradeViewModel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Letter = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Number = table.Column<int>(type: "int", maxLength: 2, nullable: false),
                    Group = table.Column<int>(type: "int", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeViewModel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectViewModel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectViewModel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TeacherViewModel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherViewModel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CoursesViewModel",
                columns: table => new
                {
                    //id = table.Column<int>(type: "int", nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Teacherid = table.Column<int>(type: "int", nullable: false),
                    ForWeekId = table.Column<int>(type: "int", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesViewModel", x => new { x.SubjectId, x.GradeId, x.Teacherid });
                    //table.PrimaryKey("PK_CoursesViewModel2", x => new { x.id});
                    table.ForeignKey(
                        name: "FK_CoursesViewModel_GradeViewModel_GradeId",
                        column: x => x.GradeId,
                        principalTable: "GradeViewModel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursesViewModel_SubjectViewModel_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "SubjectViewModel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursesViewModel_TeacherViewModel_Teacherid",
                        column: x => x.Teacherid,
                        principalTable: "TeacherViewModel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursesViewModel_GradeId",
                table: "CoursesViewModel",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesViewModel_Teacherid",
                table: "CoursesViewModel",
                column: "Teacherid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursesViewModel");

            migrationBuilder.DropTable(
                name: "GradeViewModel");

            migrationBuilder.DropTable(
                name: "SubjectViewModel");

            migrationBuilder.DropTable(
                name: "TeacherViewModel");
        }
    }
}
