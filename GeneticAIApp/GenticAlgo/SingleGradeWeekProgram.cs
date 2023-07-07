using Amazon.Runtime.Internal.Transform;
using GeneticAIApp.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace GeneticAIApp.GenticAlgo
{
    public class SingleGradeWeekProgram
    {
        [BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfDocuments)]
        public Dictionary<int, SingleGradeDailyProgram> singleGradeWeekProgram;
        //[BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfDocuments)]
        public List<CoursesViewModel> coursesList;
        //[BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfArrays)]
        //readonly List<string> dayOfWeeks = new () { "понеделник", "вторник", "сряда", "четвъртък", "петък", "събота", "неделя" };
        public GradeViewModel Grade;
        public SingleGradeDailyProgram monday;
        public SingleGradeDailyProgram tuesday;
        public SingleGradeDailyProgram wednesday;
        public SingleGradeDailyProgram thursday;
        public SingleGradeDailyProgram friday;

        public SingleGradeWeekProgram(List<CoursesViewModel> cList, GradeViewModel grade)
        {
            singleGradeWeekProgram = new Dictionary<int, SingleGradeDailyProgram>();
            monday = new SingleGradeDailyProgram();
            tuesday = new SingleGradeDailyProgram();
            wednesday = new SingleGradeDailyProgram();
            thursday = new SingleGradeDailyProgram();
            friday = new SingleGradeDailyProgram();
            coursesList = cList;
            Grade = grade;
        }

        public void Generate()
        {
            int maxCourses = 0;
            foreach (var course in coursesList)
            {
                //Добавяме в програмата всеки курс по толкова пъти, по колкото класът има за седмица
                for (int i = course.ForWeekId; i > 0; i--)
                {
                    maxCourses++;  

                    int randDayOfWeek = 0;
                    int randHourOfTheDay = 0;
                    bool isFree = false;

                    while (!isFree)
                    {
                        randDayOfWeek = GenRandValues(1, 6);
                        randHourOfTheDay = GenRandValues(1, 8);
                        //if (maxCourses >= 30) randHourOfTheDay = 7;
                        if (maxCourses >= 35) { break; }
                        //Добавяме курса към списъка с часове за деня, като проверяваме да няма припокриване на вече записан час за този ден и час.
                        switch (randDayOfWeek)
                        {
                            case 1:
                                if(monday.Check(randHourOfTheDay)) {
                                    monday.Add(randHourOfTheDay, course);
                                    isFree = true;
                                }
                                break;
                            case 2:
                                if (tuesday.Check(randHourOfTheDay))
                                {
                                    tuesday.Add(randHourOfTheDay, course);
                                    isFree = true;
                                }
                                break;
                            case 3:
                                if (wednesday.Check(randHourOfTheDay))
                                {
                                    wednesday.Add(randHourOfTheDay, course);
                                    isFree = true;
                                }
                                break;
                            case 4:
                                if (thursday.Check(randHourOfTheDay))
                                {
                                    thursday.Add(randHourOfTheDay, course);
                                    isFree = true;
                                }
                                break;
                            case 5:
                                if (friday.Check(randHourOfTheDay))
                                {
                                    friday.Add(randHourOfTheDay, course);
                                    isFree = true;
                                }
                                break;
                            default:
                                if (monday.Check(randHourOfTheDay))
                                {
                                    monday.Add(randHourOfTheDay, course);
                                    isFree = true;
                                }
                                break;
                        }
                        
                    }
                    if (maxCourses >= 35) { break; }
                }
                if (maxCourses >= 35) { break; }

            }

            singleGradeWeekProgram.Add(1, OrderDayProgram(monday));
            singleGradeWeekProgram.Add(2, OrderDayProgram(tuesday));
            singleGradeWeekProgram.Add(3, OrderDayProgram(wednesday));
            singleGradeWeekProgram.Add(4, OrderDayProgram(thursday));
            singleGradeWeekProgram.Add(5, OrderDayProgram(friday));

        }

        public SingleGradeDailyProgram OrderDayProgram(SingleGradeDailyProgram sgdp) {

            
            bool isFinish = false;
            while (isFinish != true)
            {
                List<int> missingKeyList = new List<int>();
                int lastKey = sgdp.dayliProgram.Keys.Max();

                for (int i = 1; i <= 7; i++)
                {
                    if (!sgdp.dayliProgram.ContainsKey(i) && i < lastKey)
                    {
                        missingKeyList.Add(i);
                    }
                    
                }
                
                if (missingKeyList.Count > 0 && lastKey > 0 && missingKeyList[0] < lastKey)
                {
                    CoursesViewModel lastElement = new(sgdp.dayliProgram[lastKey].GradeId, sgdp.dayliProgram[lastKey].SubjectId, sgdp.dayliProgram[lastKey].Teacherid, sgdp.dayliProgram[lastKey].ForWeekId, sgdp.dayliProgram[lastKey].Grade, sgdp.dayliProgram[lastKey].Subject, sgdp.dayliProgram[lastKey].Teacher);
                    sgdp.Add(missingKeyList[0], lastElement);
                    sgdp.dayliProgram.Remove(lastKey);
                }
                else
                {
                    isFinish = true;
                }
            }

            return sgdp;
        }


        public int GenRandValues(int min, int max)
        {
            Random rand = new Random();
            int value = rand.Next(min, max);
            return value;
        }
    }
}
