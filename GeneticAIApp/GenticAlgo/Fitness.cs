using GeneticAIApp.Models;
using System.Security.Cryptography.X509Certificates;

namespace GeneticAIApp.GenticAlgo
{
    public class Fitness
    {
        const int MaxDailyHours = 7;
        const int Days = 5;

        List<SingleGradeWeekProgram> FullProgram;
        public int Score = 0;
        public Fitness(List<SingleGradeWeekProgram> fullProgram)
        {
            FullProgram = fullProgram;
        }

        public int Check()
        {
            //Въртим програмата за всеки клас
            for (int i = 0; i < FullProgram.Count; i++)
            {
                //въртим дните от 0 до 4 за да вземем обектите с дневна програма
                //CoursesViewModel basicObject;
                for (int a = 1; a <= Days; a++)
                {
                    //Проверяваме има ли записани часове за конкретния ден и ако има вемаме обекта с програмата за деня
                    if (FullProgram[i] is not null) {
                        
                        //Въртим часовете от 1 до 7 за да сравним всеки обект със същата позиция в програмата на другите класове
                        for (int b = 1; b <= MaxDailyHours; b++)
                        {
                            //basicObject = currentDayliGradeProgram.dayliProgram[b];
                            if (i + 1 < FullProgram.Count)
                            {
                                //Въртим програмата на oстаналите класове
                                for (int c = i + 1; c < FullProgram.Count; c++)
                                {
                                    //Проверяваме има ли записана дневна програма за каласа с който сравняваме 
                                    if(FullProgram[c] is not null)
                                    {   

                                        //Проверяваме има ли записан час в двата обекта за сравнение
                                        if (FullProgram[i].singleGradeWeekProgram[a].dayliProgram.ContainsKey(b) && FullProgram[c].singleGradeWeekProgram[a].dayliProgram.ContainsKey(b))
                                        {
                                            if(FullProgram[i].singleGradeWeekProgram[a].dayliProgram[b] is not null && FullProgram[c].singleGradeWeekProgram[a].dayliProgram[b] is not null)
                                            {
                                                //сравняваме обектите
                                                bool isEqual = Compare(FullProgram[i].singleGradeWeekProgram[a].dayliProgram[b], FullProgram[c].singleGradeWeekProgram[a].dayliProgram[b]);

                                                //ако има съвпадение увеличаваме фитнес резултата
                                                if (isEqual) Score++;
                                            }
                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }   
            }


            return Score;
        }

        public bool Compare(CoursesViewModel basicObject, CoursesViewModel objectToCompareWith)
        {
            if (basicObject.SubjectId == objectToCompareWith.SubjectId && basicObject.Teacherid == objectToCompareWith.Teacherid)
            {
                return true;
            }
            return false;
        }

    }
}
