using GeneticAIApp.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace GeneticAIApp.Models
{
    public class CoursesViewModel
    {
        [Key]
        
        public int GradeId { get; set; }
        [Key]
        
        public int SubjectId { get; set; }
        [Key]
        
        public int Teacherid { get; set; }
        [MaxLength(2)]
        public int ForWeekId { get; set; }

        public virtual GradeViewModel Grade { get; set; }
        public virtual SubjectViewModel Subject { get; set; }
        public virtual TeacherViewModel Teacher { get; set; }

        public CoursesViewModel()
        {
            Grade = new GradeViewModel();
            Subject = new SubjectViewModel();
            Teacher = new TeacherViewModel();
            ForWeekId = 0;

        }

        public CoursesViewModel(int gradeId, int subjectId, int teacherId, ApplicationDbContext context)
        {

            Grade = context.GradeViewModel.Single(b => b.id == gradeId);
            Subject = context.SubjectViewModel.Single(b => b.id == subjectId);
            Teacher = context.TeacherViewModel.Single(b => b.id == teacherId);
            ForWeekId = 0;
        }

        public CoursesViewModel(int gradeId, int subjectId, int teacherId, int ForWeekId, GradeViewModel gwm, SubjectViewModel swm, TeacherViewModel twm)
        {
            GradeId = gradeId;
            SubjectId = subjectId;
            Teacherid = teacherId;
            Grade = new GradeViewModel(gwm.id, gwm.Number, gwm.Letter, gwm.Group, gwm.Courses);
            Subject = new SubjectViewModel(swm.id, swm.CourseName, swm.Courses);
            Teacher = new TeacherViewModel(twm.id, twm.Name,twm.Courses);
            this.ForWeekId = ForWeekId;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
