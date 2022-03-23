using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aTTagirl
{
    struct Player
    {
        string playerID;
        List<Score> scores;             
    }
    struct Score
    {
        string ScoreName;
        int ScoreCount;
    }
}
