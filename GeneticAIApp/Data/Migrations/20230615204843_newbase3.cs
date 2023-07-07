using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneticAIApp.Data.Migrations
{
    public partial class newbase3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesViewModel",
                table: "CoursesViewModel");

            migrationBuilder.DropIndex(
                name: "IX_CoursesViewModel_SubjectId",
                table: "CoursesViewModel");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CoursesViewModel");*/

/*            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesViewModel",
                table: "CoursesViewModel",
                columns: new[] { "SubjectId", "GradeId", "Teacherid" });*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
/*            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesViewModel",
                table: "CoursesViewModel");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "CoursesViewModel",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesViewModel",
                table: "CoursesViewModel",
                column: "CourseId");*/

/*            migrationBuilder.CreateIndex(
                name: "IX_CoursesViewModel_SubjectId",
                table: "CoursesViewModel",
                column: "SubjectId");*/
        }
    }
}
