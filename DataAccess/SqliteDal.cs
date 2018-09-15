using System;
using System.Data;
using System.Data.SQLite;

namespace DataAccess
{
    public static class SqliteDal
    {
        public static string cnstr = "Data Source=homeautomation.sqlite;Version=3;";

        public static void CreateDatabase()
        {
            SQLiteConnection.CreateFile("homeautomation.sqlite");

            SQLiteConnection m_dbConnection = new SQLiteConnection(cnstr);
            m_dbConnection.Open();

            string sql = "create table highscores (name varchar(20), score int)";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            sql = "insert into highscores (name, score) values ('Me', 9001)";

            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            m_dbConnection.Close();
        }

        public static void InitializeDatabase()
        {
            using (SQLiteConnection db =
                new SQLiteConnection(cnstr))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
                    "Text_Entry NVARCHAR(2048) NULL)";

                SQLiteCommand createTable = new SQLiteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        /// <summary> 
        /// Returns datatbale for given sql query. 
        /// </summary> 
        /// <param name="sql"></param> 
        /// <returns></returns> 
        public static DataTable getData(string sql)
        {
            SQLiteConnection cn = new SQLiteConnection(cnstr);
            cn.Open();

            try
            {
                SQLiteCommand cm = new SQLiteCommand(sql, cn);
                SQLiteDataReader dr = cm.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                cn.Close();

                return dt;
            }
            catch (Exception ex)
            {
                cn.Close();
                throw ex;
            }
        }

        /// <summary> 
        /// Returns count of executed insert, update, delete statement. 
        /// </summary> 
        /// <param name="sql"></param> 
        /// <returns></returns> 
        public static int execNQ(string sql)
        {
            SQLiteConnection cn = new SQLiteConnection(cnstr);
            cn.Open();
            SQLiteCommand cm = new SQLiteCommand(sql, cn);
            int rows;
            rows = cm.ExecuteNonQuery();
            cn.Close();
            return rows;
        }

        /// <summary> 
        /// Returns scalar for given sql query. 
        /// </summary> 
        /// <param name="sql"></param> 
        /// <returns></returns> 
        public static object execSC(string sql)
        {
            SQLiteConnection cn = new SQLiteConnection(cnstr);
            cn.Open();
            SQLiteCommand cm = new SQLiteCommand(sql, cn);
            object rows;
            rows = cm.ExecuteScalar();
            cn.Close();
            if (rows == null)
            {
                rows = 0;
            }
            return rows;
        }

    }
}
