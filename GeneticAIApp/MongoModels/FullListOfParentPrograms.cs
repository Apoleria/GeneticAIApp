using GeneticAIApp.GenticAlgo;
using GeneticAIApp.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace GeneticAIApp.MongoModels
{
    public class FullListOfParentPrograms
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int fitnesScore { get; set; }
        //[BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfArrays)]
        public List<SingleGradeWeekProgram> fullProgram;
        //public Dictionary<GradeViewModel, SingleGradeWeekProgram> fullProgram;
        public FullListOfParentPrograms(int fscore, List<SingleGradeWeekProgram> full)
        {
            fullProgram = full;
            fitnesScore = fscore;
        }
    }
}
