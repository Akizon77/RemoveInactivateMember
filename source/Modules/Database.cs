using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteInactiveMembers.Modules
{
    public class Database
    {
        public ISqlSugarClient DbClient;
        public Database() 
        {
            DbType dbType;
            string connectionString;
            switch (Env.DB_TYPE)
            {
                case "mysql":
                    connectionString = Env.DB_CONNECTION_STRING;
                    dbType = DbType.MySql;
                    break;
                case "sqlite":
                    connectionString = "datasource=" + Env.DB_FILE;
                    dbType = DbType.Sqlite;
                    break;
                default:
                    throw new ArgumentException($"不支持的数据库类型：{Env.DB_TYPE}");
            }
            var cf = new ConnectionConfig()
            {
                ConnectionString = connectionString,
                DbType = dbType,
                IsAutoCloseConnection = true
            };
            DbClient =  new SqlSugarClient(cf, db =>
            {
                db.Aop.OnError = (e) => Log.Error(e,"执行SQL出错");
            });

            if (!File.Exists(Env.DB_FILE) && Env.DB_TYPE == "sqlite")
            {
                Log.Warning("不存在 SQLITE 文件，正在新建 {0}",Env.DB_FILE);
                File.Create(Env.DB_FILE).Close();
            }

            DbClient.Open();
            Log.Information("成功连接到数据库");
        }
    }
}
