using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeListner.DBEntity
{
    public sealed class Log
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }

    public enum LogType
    {
        Information,
        Warning,
        Error,
        Debug
    }
}
