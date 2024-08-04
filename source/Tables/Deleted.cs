using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteInactiveMembers.Tables
{
    [SugarTable(tableName:"Deleted")]
    public class Deleted : Member
    {
    }
}
