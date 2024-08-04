using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteInactiveMembers.Tables
{
    public class Member : Mamo233Lib.SqlSugar.SqlReopBase
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnDataType = "TEXT")]
        public string Name { get; set; } = null!;

        public DateTime LastActiveTime { get; set; }
        public long LastActiveMessage { get; set; }
        public Member(long id,string name,long messageID = 0)
        {
            Id = id;
            Name = name;
            LastActiveTime = DateTime.UtcNow;
            LastActiveMessage = messageID;
        }
        public Member()
        {
            
        }
    }
}
