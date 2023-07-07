using GeneticAIApp.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace GeneticAIApp.GenticAlgo
{
    public class SingleGradeDailyProgram
    {
        [BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfDocuments)]
        public Dictionary<int, CoursesViewModel> dayliProgram;
        public SingleGradeDailyProgram()
        {
            dayliProgram = new Dictionary<int, CoursesViewModel>();
        }

        public void Add(int randHourOfTheDay, CoursesViewModel course)
        {
            dayliProgram.Add(randHourOfTheDay, course);
        }

        public bool Check(int randHourOfTheDay) 
        {
            if (!dayliProgram.ContainsKey(randHourOfTheDay))
            {
                return true;
            }
            return false; 
        }
    }
}
