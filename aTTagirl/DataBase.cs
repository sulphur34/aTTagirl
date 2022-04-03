using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace aTTagirl
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection db;

        public DataBase(string connectionString)
        {
            db = new SQLiteAsyncConnection(connectionString);

            db.CreateTableAsync<Score>().Wait();
        }
        public Task<List<Score>> GetScoresList()
        {
            return db.Table<Score>().ToListAsync();
        }
        public Task<Score> GetScore(long ID)
        {
            return db.Table<Score>().Where(i => i.PlayerID == ID).FirstOrDefaultAsync();
        }
        public Task<int> SaveScoreAsync(Score score)
        {
            var op = db.Table<Score>().Where(i => i.ScoreID == score.ScoreID).FirstOrDefaultAsync();
            //if (db.Table<Score>().Where(i => i.ScoreID == score.ScoreID).FirstOrDefaultAsync() != null)
            //{
            //    return db.UpdateAsync(score);
            //}
            //else            
            //{
                return db.InsertAsync(score);
            //}
        }
        public Task<int> DeleteScoreAsync(Score player)
        {
            return db.DeleteAsync(player);
        }
    }
}
