using Microsoft.Build.Framework;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GeneticAIApp.Models
{
    public class TeacherViewModel
    {
        [Key]
        public int id { get; set; }
        [NotNull]
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; }
        //[BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfArrays)]
        public virtual ICollection<CoursesViewModel> Courses { get; }

        public TeacherViewModel()
        {
            Name = string.Empty;
            Courses = new List<CoursesViewModel>();
        }

        public TeacherViewModel(int id, string tName, ICollection<CoursesViewModel> c)
        {
            this.id = id;
            Name = tName;
            Courses = c;
        }
    }
}
