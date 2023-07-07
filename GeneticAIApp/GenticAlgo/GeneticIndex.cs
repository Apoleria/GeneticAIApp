using Amazon.Runtime.Internal.Transform;
using GeneticAIApp.Data;
using GeneticAIApp.Models;
using GeneticAIApp.MongoModels;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using System.Collections.Generic;

namespace GeneticAIApp.GenticAlgo
{
    public class GeneticIndex
    {
        const int MaxDailyHours = 7;
        const int Days = 5;
        private readonly ApplicationDbContext _context;
        private readonly MongoDb MongoDb;
        string mongoDbName = "school_program";
        string mongoCollectionName = "Full_List_of_Parent_Programs";
        public List<FullListOfParentPrograms> FullListOfWPrograms { get; set; }
        IMongoDatabase mongoDatabase;
        List<SingleGradeWeekProgram> schoolProgram;
        int cnt = 0;
        int mutation = 10;
        int numberOfNewChild = 10000;

        public GeneticIndex(ApplicationDbContext context)
        {
            _context = context;
            MongoDb = new();
            mongoDatabase = MongoDb.mongoDbClient.GetDatabase(mongoDbName);
            FullListOfWPrograms = new();
            schoolProgram = new();
        }

        public void Start()
        {
            var mongoCollection = mongoDatabase.GetCollection<FullListOfParentPrograms>(mongoCollectionName);
            var fullCoursesCollection = mongoDatabase.GetCollection<SingleGradeWeekProgram>("fullCoursesCollection");

            for (int i = 0; i < 1000; i++)
            {
                var grades = new List<GradeViewModel>();
                List<SingleGradeWeekProgram> programForAllGradesList = new();
                Dictionary<GradeViewModel, SingleGradeWeekProgram> fullProgram = new Dictionary<GradeViewModel, SingleGradeWeekProgram>();

                if (_context.GradeViewModel is not null)
                {
                    grades = _context.GradeViewModel.ToList();
                }

                foreach (GradeViewModel grade in grades)
                {
                    //Генерираме седмична програма за всеки клас
                    var gradeCourses = new List<CoursesViewModel>();
                    if (_context.CoursesViewModel is not null)
                    {
                        gradeCourses = _context.CoursesViewModel.Where(g => g.GradeId == grade.id).Include(b => b.Subject).Include(b => b.Teacher).ToList();
                        if (!gradeCourses.Any())
                        {
                            continue;
                        }
                        SingleGradeWeekProgram singleGradeWeekProgram = new SingleGradeWeekProgram(gradeCourses, grade);
                        singleGradeWeekProgram.Generate();
                        programForAllGradesList.Add(singleGradeWeekProgram);
                    }
                }

                //Проверяваме релевантността на генерираната програма
                Fitness fitness = new(programForAllGradesList);
                int score = fitness.Check();

                //Записваме я в базата данни като обект
                mongoCollection.InsertOne(new FullListOfParentPrograms(score, programForAllGradesList));
                FullListOfWPrograms.Add(new FullListOfParentPrograms(score, programForAllGradesList));
            }
        }

        public void GenerateChild()
        {
            var orderedPrograms = FullListOfWPrograms.OrderBy(a => a.fitnesScore).Take(300).ToList();

            if (orderedPrograms[0].fitnesScore >= 17)
            {
                cnt++;
                Dictionary<GradeViewModel, List<SingleGradeWeekProgram>> hromosomes = new();
                Random rand = new Random();
                
                //От всички родителски програми вземаме седмичните програми за всеки клас и ги слагаме в една колекция
                foreach (FullListOfParentPrograms Program in orderedPrograms)
                {
                    for (int i = 0; i < Program.fullProgram.Count; i++)
                    {
                        if (hromosomes.ContainsKey(Program.fullProgram[i].Grade))
                        {
                            hromosomes[Program.fullProgram[i].Grade].Add(Program.fullProgram[i]);
                        }
                        else
                        {
                            List<SingleGradeWeekProgram> gradeWeekProgTmpList = new();
                            gradeWeekProgTmpList.Add(Program.fullProgram[i]);
                            hromosomes.Add(Program.fullProgram[i].Grade, gradeWeekProgTmpList);
                        }
                    }
                }

                FullListOfWPrograms.Clear();

                //Генерираме определен брой програми наследници от родителските хромозоми
                for (int i = 0; i < numberOfNewChild; i++)
                {
                    List<SingleGradeWeekProgram> programForAllGradesList = new();

                    //За всеки клас вземаме списъка с хромозоми и на рандом избираме един от листа с хромозомии го слагаме в програмата.
                    foreach (KeyValuePair<GradeViewModel, List<SingleGradeWeekProgram>> singleGradeHromosomes in hromosomes)
                    {
                        //Вземаме рандъм от 0 до дължината на Списъка с хромозоми за съответния клас
                        int randValue = rand.Next(0, singleGradeHromosomes.Value.Count());
                        
                        programForAllGradesList.Add(makeMutation(singleGradeHromosomes.Value[randValue]));

                    }

                    //Проверяваме релевантността на генерираната програма от наследници
                    Fitness fitness = new(programForAllGradesList);
                    int score = fitness.Check();
                    FullListOfWPrograms.Add(new FullListOfParentPrograms(score, programForAllGradesList));
                }

                GenerateChild();
            } 
            else if (orderedPrograms[0].fitnesScore < 17 && orderedPrograms[0].fitnesScore > 0)
            {
                finishProgram(orderedPrograms[0]);
                Fitness fitness = new(schoolProgram);
                int score = fitness.Check();
                var flpp = new FullListOfParentPrograms(score, schoolProgram);
                var mCollection = mongoDatabase.GetCollection<FullListOfParentPrograms>("final_program");
                mCollection.DeleteMany(Builders<FullListOfParentPrograms>.Filter.Empty);
                mCollection.InsertOne(flpp);
            } else {
                var mCollection = mongoDatabase.GetCollection<FullListOfParentPrograms>("final_program");
                mCollection.DeleteMany(Builders<FullListOfParentPrograms>.Filter.Empty);
                mCollection.InsertOne(orderedPrograms[0]);
            }
        }

        public SingleGradeWeekProgram makeMutation(SingleGradeWeekProgram sgwp)
        {
            if (sgwp is not null)
            {
                Random rand = new Random();
                for (int i = 0; i < mutation; i++)
                {
                    bool isFisrtPresent = false;
                    int dayFirst = 0;
                    int hourFirst = 0;
                    while (isFisrtPresent == false)
                    {
                        dayFirst = rand.Next(1, 6);
                        hourFirst = rand.Next(1, 8);
                        if (sgwp.singleGradeWeekProgram[dayFirst].dayliProgram.ContainsKey(hourFirst))
                        {
                            //sgwp.singleGradeWeekProgram[dayFirst].dayliProgram[hourFirst];
                            isFisrtPresent = true;
                        }
                        
                    }

                    bool isSecondPresent = false;
                    int daySecond = 0;
                    int hourSecond = 0;
                    while (isSecondPresent == false)
                    {
                        daySecond = rand.Next(1, 6);
                        hourSecond = rand.Next(1, 8);
                        if (sgwp.singleGradeWeekProgram[daySecond].dayliProgram.ContainsKey(hourSecond))
                        {
                            if(daySecond != dayFirst) isSecondPresent = true;
                            // sgwp.singleGradeWeekProgram[daySecond].dayliProgram[hourSecond];
                        }
                    }

                    CoursesViewModel firstTmp = new(sgwp.singleGradeWeekProgram[dayFirst].dayliProgram[hourFirst].GradeId, 
                        sgwp.singleGradeWeekProgram[dayFirst].dayliProgram[hourFirst].SubjectId, sgwp.singleGradeWeekProgram[dayFirst].dayliProgram[hourFirst].Teacherid, 
                        sgwp.singleGradeWeekProgram[dayFirst].dayliProgram[hourFirst].ForWeekId, sgwp.singleGradeWeekProgram[dayFirst].dayliProgram[hourFirst].Grade, 
                        sgwp.singleGradeWeekProgram[dayFirst].dayliProgram[hourFirst].Subject, sgwp.singleGradeWeekProgram[dayFirst].dayliProgram[hourFirst].Teacher);

                    CoursesViewModel secondTmp = new(sgwp.singleGradeWeekProgram[daySecond].dayliProgram[hourSecond].GradeId, 
                        sgwp.singleGradeWeekProgram[daySecond].dayliProgram[hourSecond].SubjectId, sgwp.singleGradeWeekProgram[daySecond].dayliProgram[hourSecond].Teacherid, 
                        sgwp.singleGradeWeekProgram[daySecond].dayliProgram[hourSecond].ForWeekId, sgwp.singleGradeWeekProgram[daySecond].dayliProgram[hourSecond].Grade, 
                        sgwp.singleGradeWeekProgram[daySecond].dayliProgram[hourSecond].Subject, sgwp.singleGradeWeekProgram[daySecond].dayliProgram[hourSecond].Teacher);

                    sgwp.singleGradeWeekProgram[dayFirst].dayliProgram.Remove(hourFirst);
                    sgwp.singleGradeWeekProgram[daySecond].dayliProgram.Remove(hourSecond);
                    sgwp.singleGradeWeekProgram[dayFirst].dayliProgram.Add(hourFirst, secondTmp);
                    sgwp.singleGradeWeekProgram[daySecond].dayliProgram.Add(hourSecond, firstTmp);

                } 
            }
            return sgwp;
        }

        public void finishProgram(FullListOfParentPrograms finalProgram)
        {
            this.schoolProgram = finalProgram.fullProgram;
            Random rand = new Random();
            //Въртим програмата за всеки клас
            for (int i = 0; i < schoolProgram.Count; i++)
            {
                // въртим дните от 1 до 5 за да вземем обектите с дневна програма
                //CoursesViewModel basicObject;
                for (int a = 1; a <= Days; a++)
                {
                    //Проверяваме има ли записани часове за конкретния ден и ако има вемаме обекта с програмата за деня
                    if (schoolProgram[i] is not null)
                    {
                        //Въртим часовете от 1 до 7 за да сравним всеки обект със същата позиция в програмата на другите класове
                        for (int b = 1; b <= MaxDailyHours; b++) 
                        { 

                            //Проверяваме дали не е последният клас, ако е, няма с какво повече да сравняваме
                            if (i + 1 < schoolProgram.Count)
                            {
                                //Въртим програмата на oстаналите класове
                                for (int c = i + 1; c < schoolProgram.Count; c++)
                                {
                                    //Проверяваме има ли записана дневна програма за класа с който сравняваме 
                                    if (schoolProgram[c] is not null)
                                    {
                                        //Проверяваме има ли записан час в двата обекта за сравнение
                                        if (schoolProgram[i].singleGradeWeekProgram[a].dayliProgram.ContainsKey(b) && schoolProgram[c].singleGradeWeekProgram[a].dayliProgram.ContainsKey(b))
                                        {
                                            if (schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b] is not null && schoolProgram[c].singleGradeWeekProgram[a].dayliProgram[b] is not null)
                                            {
                                                if (schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].SubjectId == schoolProgram[c].singleGradeWeekProgram[a].dayliProgram[b].SubjectId
                                                    && schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].Teacherid == schoolProgram[c].singleGradeWeekProgram[a].dayliProgram[b].Teacherid)
                                                {
                                                    //schoolProgram[i].singleGradeWeekProgram[a];
                                                    
                                                    bool isMatchAgainFirstPlace = true;
                                                    bool isMatchAgainSecondPlace = true;
                                                    int randDayOfWeek = 0;
                                                    int randHourOfTheDay = 0;
                                                    int cnt3 = 0;
                                                    bool makeChanges = true;
                                                    while (true)
                                                    {
                                                        //рандъм за нов ден и час
                                                        randDayOfWeek = rand.Next(1, 6);
                                                        randHourOfTheDay = rand.Next(1, 7);

                                                        //Проверка дали обекта с който ще сменяме не е празен
                                                        if (schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram.ContainsKey(randHourOfTheDay) && schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay] is not null)
                                                        {
                                                            //Проверка дали не се опитваме да сменим със същия обект
                                                            if (randHourOfTheDay != b && randDayOfWeek != a)
                                                            {
                                                                //Проверка отново за първата позиция с новия час
                                                                isMatchAgainFirstPlace = chechIsItWhrongClassesAgain(i, a, b, randDayOfWeek, randHourOfTheDay);

                                                                //Проверка отново за втората позиция с новия час
                                                                isMatchAgainSecondPlace = chechIsItWhrongClassesAgain(i, randDayOfWeek, randHourOfTheDay, a, b);

                                                                //Ако всичко е наред прекъсваме цикъла и преминаваме към размяната на местата
                                                                if (isMatchAgainFirstPlace == false && isMatchAgainSecondPlace == false &&
                                                                    schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram.ContainsKey(randHourOfTheDay)) break;

                                                            }
                                                            //Ако много пъти не успее да намери съответстващо място прекъсваме цикъла, като несъответствието остава и не правим размяната
                                                            if (cnt3 > 10000)
                                                            {
                                                                makeChanges = false;
                                                                break;
                                                            }
                                                            cnt3++;
                                                        }
                                                    }

                                                    //Сменяме местата на часовете, като вече е проверено, че на новото място няма съвпадение с чаосвете на друг клас.
                                                    if (makeChanges)
                                                    {
                                                        if (schoolProgram[i].singleGradeWeekProgram[a].dayliProgram.ContainsKey(b) && schoolProgram[c].singleGradeWeekProgram[randDayOfWeek].dayliProgram.ContainsKey(randHourOfTheDay))
                                                        {
                                                            CoursesViewModel firstTmp = new(schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].GradeId, schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].SubjectId, schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].Teacherid, schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].ForWeekId, schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].Grade, schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].Subject, schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].Teacher);
                                                            CoursesViewModel secondTmp = new(schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay].GradeId, schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay].SubjectId, schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay].Teacherid, schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay].ForWeekId, schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay].Grade, schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay].Subject, schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay].Teacher);
                                                            schoolProgram[i].singleGradeWeekProgram[a].dayliProgram.Remove(b);
                                                            schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram.Remove(randHourOfTheDay);
                                                            schoolProgram[i].singleGradeWeekProgram[a].dayliProgram.Add(b, secondTmp);
                                                            schoolProgram[i].singleGradeWeekProgram[randDayOfWeek].dayliProgram.Add(randHourOfTheDay, firstTmp);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        public bool chechIsItWhrongClassesAgain(int i, int a, int b, int randDayOfWeek, int randHourOfTheDay)
        {
            bool isMatchInNewPosition = false;
            //Въртим програмата на oстаналите класове
            for (int c = 0; c < schoolProgram.Count; c++)
            {
                //Проверяваме има ли записана дневна програма за каласа с който сравняваме 
                if (schoolProgram[c] is not null)
                {
                    //Ако се падне същия клас да не проверява сам себе си
                    if(schoolProgram[i].Grade.id != schoolProgram[c].Grade.id)
                    {
                        //Проверяваме има ли записан час в двата обекта за сравнение
                        if (schoolProgram[i].singleGradeWeekProgram[a].dayliProgram.ContainsKey(b) && schoolProgram[c].singleGradeWeekProgram[randDayOfWeek].dayliProgram.ContainsKey(randHourOfTheDay))
                        {
                            if (schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b] is not null && schoolProgram[c].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay] is not null)
                            {
                                if (schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].SubjectId == schoolProgram[c].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay].SubjectId
                                    && schoolProgram[i].singleGradeWeekProgram[a].dayliProgram[b].Teacherid == schoolProgram[c].singleGradeWeekProgram[randDayOfWeek].dayliProgram[randHourOfTheDay].Teacherid)
                                {
                                    isMatchInNewPosition = true;
                                }
                            }
                        }
                    }
                }
            }
            return isMatchInNewPosition;
        }
        
        public FullListOfParentPrograms? getFinalProgram()
        {
            
            var mCollection = mongoDatabase.GetCollection<FullListOfParentPrograms>("final_program");

            var filter = Builders<FullListOfParentPrograms>.Filter.Empty;
            //if (mCollection.Find(filter).SingleOrDefault() is not null)
                return mCollection.Find(filter).SingleOrDefault();

            //return null;
        }

    }
}
