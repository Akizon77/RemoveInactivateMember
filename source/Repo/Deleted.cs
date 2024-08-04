using Mamo233Lib.SqlSugar;
using SqlSugar;

namespace DeleteInactiveMembers.Repo
{
    public class Deleted : BaseRepository<Tables.Deleted>
    {
        public Deleted(ISqlSugarClient context) : base(context)
        {

        }
    }
}