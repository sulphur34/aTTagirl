using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace aTTagirl
{
    //public class Player
    //{
    //    //public long playerID;
        

    //    public List<Score> scoresList;
    //    //public void WriteNewScore(Score score)
    //    //{            
    //    //    scoresList.Add(score);
    //    //    //int i = scoresList.FindIndex((item) => item.ScoreType == score.ScoreType);
    //    //    //if (i != -1)
    //    //    //{

    //    //    //    scoresList.ElementAt(i).ScoreCount++;
    //    //    //}
    //    //    //else
    //    //    //{
    //    //    //    scoresList.Add(score);
    //    //    //    scoresList.ElementAt(i).ScoreCount++;
    //    //    //}
    //    //}
    //    //public List<Score> ReadScores()
    //    //{
    //    //    return scoresList;
    //    //}
    //}
    public class Score
    {
        [PrimaryKey, AutoIncrement]
        public int ScoreID { get; set; }
        [Indexed]
        public long PlayerID { get; set; }
        public int ScoreType { get; set; }
        public string ScoreName { get; set; }
        public bool ScoreReasonManual { get; set; }
        public string ScoreReason { get; set; }
        public DateTime DateRecieved { get; set; }
    }
    public class Shouts
    {
        public string First;
        public string Second;
        public string Third;
    }
    

}
