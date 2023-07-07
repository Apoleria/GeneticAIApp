using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GeneticAIApp.Models;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;

namespace GeneticAIApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CoursesViewModel>()
                .HasKey(c => new { c.SubjectId, c.GradeId, c.Teacherid });


            /* modelBuilder.Entity<IdentityUserLogin<string>>()
                 .HasKey(c => new { c.LoginProvider, c.ProviderKey });*/

            modelBuilder.Entity<GradeViewModel>()
                .HasMany(e => e.Courses)
                .WithOne(e => e.Grade)
                .HasForeignKey(e => e.GradeId)
                .HasPrincipalKey(e => e.id); ;

            modelBuilder.Entity<TeacherViewModel>()
                .HasMany(e => e.Courses)
                .WithOne(e => e.Teacher)
                .HasForeignKey(e => e.Teacherid)
                .HasPrincipalKey(e => e.id);

            modelBuilder.Entity<SubjectViewModel>()
                .HasMany(e => e.Courses)
                .WithOne(e => e.Subject)
                .HasForeignKey(e => e.SubjectId)
                .HasPrincipalKey(e => e.id);
        }
        public DbSet<GeneticAIApp.Models.CoursesViewModel>? CoursesViewModel { get; set; }

        public DbSet<GeneticAIApp.Models.GradeViewModel>? GradeViewModel { get; set; }

        public DbSet<GeneticAIApp.Models.TeacherViewModel>? TeacherViewModel { get; set; }

        public DbSet<GeneticAIApp.Models.SubjectViewModel>? SubjectViewModel { get; set; }


    }
}