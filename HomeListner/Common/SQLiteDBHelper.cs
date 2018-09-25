using HomeListner.DBEntity;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeListner.Common
{

    public static class SQLiteDBHelper
    {
        private static string DbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "HomeAutomation.sqlite");
        private static SQLiteConnection Connection = new SQLiteConnection(new SQLitePlatformWinRT(), DbPath);

        //This method will create database tables required for application to run
        public static void CreateDatabase()
        {
            Connection.CreateTable<Log>();
        }

        public static bool InsertIntoTable(object row)
        {
            var rowCount = Connection.Insert(row);
            return rowCount > 0;
        }
    }
}
