using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GeneticAIApp.Models
{
    public class SubjectViewModel
    {
        [Key]
        public int id { get; set; }
        [NotNull]
        [Required]
        public string CourseName { get; set; }
        //[BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfArrays)]
        public virtual ICollection<CoursesViewModel> Courses { get; }

        public SubjectViewModel()
        {
            CourseName = string.Empty;
            Courses = new List<CoursesViewModel>();
        }

        public SubjectViewModel(int id, string cName, ICollection<CoursesViewModel> c)
        {
            this.id = id;
            CourseName = cName;
            Courses = c;
        }
    }
}
