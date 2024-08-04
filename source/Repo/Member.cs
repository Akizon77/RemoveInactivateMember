using Mamo233Lib.SqlSugar;
using SqlSugar;

namespace DeleteInactiveMembers.Repo
{
    public class Member: BaseRepository<Tables.Member>
    {
        public Member(ISqlSugarClient context) : base(context)
        {

        }
    }
}