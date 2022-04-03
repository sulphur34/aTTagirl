using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aTTagirl
{
    public static class Games
    {
        public static string AttaSwitch(int number)
        {
            switch (number)
            {
                case 1:
                    return "Молодец☺️";
                case 2:
                    return "Умничка😊";
                case 3:
                    return "Солнышко☀️";
                case 4:
                    return "🥇";
                case 5:
                    return "🏅";
                case 6:
                    return "🎖";
                default:
                    return "";
            }

        }
        public static async void GameNewRound(long playerID, int attNumber, string excuse, bool manualex)
        {            
            Score score = new Score();
            score.PlayerID = playerID;
            score.ScoreType = attNumber;
            score.ScoreName = AttaSwitch(attNumber);
            score.ScoreReasonManual = manualex;
            score.ScoreReason = excuse;
            score.DateRecieved = DateTime.Now;
            //score.ScoreID = attNumber.ToString() + " - " + playerID.ToString() + " - " + DateTime.Now.ToString();
            //if (score.scoresList == null)
            //{
            //    List<Score> tempscore = new List<Score>();
            //    tempscore.Add(score);
            //}
            //else 
            //{
            //    score.scoresList.Add(score);
            //}            
            await Program.ScoreStat.SaveScoreAsync(score);
        }
        //public async static void ShowStatistics(long playerID)
        //{
                
        //    //foreach (Score score in Player.AttaScore)
        //    //{
        //    //    statistics = "\n" + score.ScoreName + " - " + score.ScoreCount + " раз";
        //    //}
        //    //return player;
        //}
        }
        class Info
        {
            
        }

    }

