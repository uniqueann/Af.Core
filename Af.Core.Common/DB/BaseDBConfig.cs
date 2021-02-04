using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Af.Core.Common.DB
{
    public class BaseDBConfig
    {
        public static (List<MultiDBOperate> allDbs, List<MultiDBOperate> slaveDbs) MultiConnectionString => MultiInitConn();

        private static (List<MultiDBOperate> allDbs, List<MultiDBOperate> slaveDbs) MultiInitConn()
        {
            List<MultiDBOperate> listDatabase = Appsettings.app<MultiDBOperate>("DBS")
                .Where(i => i.Enabled).ToList();
            foreach (var item in listDatabase)
            {
                SpecialDbString(item);
            }
            List<MultiDBOperate> listDatabaseSimpleDB = new List<MultiDBOperate>();//单库
            List<MultiDBOperate> listDatabaseSlaveDB = new List<MultiDBOperate>();//从库

            // 单库 不开启读写分离 只保留一个
            if (!Appsettings.app(new string[] { "CQRSEnabled" }).ObjToBool() && !Appsettings.app(new string[] { "MultiDBEnabled" }).ObjToBool())
            {
                if (listDatabase.Count==1)
                {
                    return (listDatabase, listDatabaseSlaveDB);
                }
                else
                {
                    var dbFirst = listDatabase.FirstOrDefault(a => a.ConnId == Appsettings.app(new string[] { "MainDB" }).ObjToString());
                    if (dbFirst==null)
                    {
                        dbFirst = listDatabase.FirstOrDefault();
                    }
                    listDatabaseSimpleDB.Add(dbFirst);
                    return (listDatabaseSimpleDB,listDatabaseSlaveDB);
                }
            }

            // 读写分离 且必须单库 获取从库
            if (Appsettings.app(new string[] { "CQRSEnabled" }).ObjToBool() && !Appsettings.app(new string[] { "MultiDBEnabled" }).ObjToBool())
            {
                if (listDatabase.Count>1)
                {
                    listDatabaseSlaveDB = listDatabase.Where(a=>a.ConnId!= Appsettings.app(new string[] { "MainDB" }).ObjToString()).ToList();
                }
            }

            return (listDatabase, listDatabaseSlaveDB);
        }

        private static MultiDBOperate SpecialDbString(MultiDBOperate item)
        {
            if (item.DbType==DataBaseType.Sqlite)
            {
                item.Connection = $"DataSource=" + Path.Combine(Environment.CurrentDirectory, item.Connection);
            }
            else if (item.DbType==DataBaseType.SqlServer)
            {
                item.Connection = item.Connection;
            }
            else if (item.DbType==DataBaseType.MySql)
            {
                item.Connection = $"";
            }
            else if (item.DbType==DataBaseType.PostgreSQL)
            {
                item.Connection = $"";
            }
            else if (item.DbType==DataBaseType.Oracle)
            {
                item.Connection = $"";
            }

            return item;
        }

        private static string DifDBConnOfSecurity(params string[] conn)
        {
            foreach (var item in conn)
            {
                try
                {
                    if (File.Exists(item))
                    {
                        return File.ReadAllText(item).Trim();
                    }
                }
                catch (System.Exception) { }
            }

            return conn[conn.Length - 1];
        }

    }

    public class MultiDBOperate
    {
        public bool Enabled { get; set; }
        public string ConnId { get; set; }
        public int HiRate { get; set; }
        public string Connection { get; set; }
        public DataBaseType DbType { get; set; }
    }
    public enum DataBaseType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3,
        PostgreSQL = 4
    }
}
