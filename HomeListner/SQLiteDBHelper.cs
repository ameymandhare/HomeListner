using HomeListner.DBEntity;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeListner
{

    public class SQLiteDBHelper : IDisposable
    {
        public string DbPath { get; set; }
        public SQLiteConnection Connection { get; set; }

        public SQLiteDBHelper()
        {
            DbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "HomeAutomation.sqlite");
            Connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DbPath);
        }

        //This method will create database tables required for application to run
        public void CreateDatabase()
        {
            Connection.CreateTable<Log>();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        //I was trying to return connection but not working due to following error:
        /*
        Method 'HomeListner.SQLiteDBHelper.Connection.get()' has a parameter of type 'SQLite.Net.SQLiteConnection' in its signature.  
        Although this type is not a valid Windows Runtime type, it implements interfaces that are valid Windows Runtime types.  
        Consider changing the method signature to use one of the following types instead: 'System.IDisposable'.
        */
        //public SQLiteConnection GetConnection()
        //{
        //    return connection;
        //}
    }
}
