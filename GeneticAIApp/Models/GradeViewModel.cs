using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace GeneticAIApp.Models
{
    public class GradeViewModel
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(2)]
        public string Number { get; set; }
        [Required]
        [MaxLength(1)]
        public string Letter  { get; set; }
        [MaxLength(1)]
        public string Group { get; set; }

        //[BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfArrays)]
        public virtual ICollection<CoursesViewModel> Courses { get; }


        public GradeViewModel()
        {
            Courses = new List<CoursesViewModel>();
        }

        public GradeViewModel(int id, string number, string letter, string group, ICollection<CoursesViewModel> c)
        {
            this.id = id;
            Number = number;
            Letter = letter;
            Group = group;
            Courses = c;
        }

        public string FullName()
        {
            return Number + Letter.ToUpper();
        }

        public string LableLatterBg()
        {
            return "Буква";
        }

        public string LableNumberBg()
        {
            return "Номер";
        }

        public string LableGroupBg()
        {
            return "Групи";
        }
    }
}
