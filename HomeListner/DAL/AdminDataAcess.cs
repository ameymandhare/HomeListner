using HomeListner.DBEntity;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HomeListner.DAL
{
    public sealed class AdminDataAcess
    {
        private static string DbPath;
        private static SQLiteConnection Connection;

        public AdminDataAcess()
        {
            DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "HomeAutomation.sqlite");
            Connection = new SQLiteConnection(new SQLitePlatformWinRT(), DbPath);
        }

        public IList<Log> GetLogs()
        {
            var result = new List<Log>();
            var logsQuery = Connection.Table<Log>();

            foreach (var log in logsQuery)
            {
                result.Add(new Log { Id = log.Id, Message = log.Message, Type = log.Type, Timestamp = log.Timestamp});
            }

            return result;
        }
    }
}
