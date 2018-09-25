using HomeListner.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HomeListner.Common
{
    public static class FileLogger
    {
        private static StorageFile sampleFile = null;
        private static readonly StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        public async static void Log(LogType logType, string msg)
        {
            // Create sample file; replace if exists.
            sampleFile = await storageFolder.CreateFileAsync("sampleLog.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(sampleFile, Environment.NewLine + "======================= " + logType.ToString() + " ======================= "
                                        + Environment.NewLine + Environment.NewLine + msg + Environment.NewLine + "TimeStamp: " + DateTime.Now.ToString()
                                        + Environment.NewLine + "======================= Log End ======================="
                                        + Environment.NewLine);
        }
    }

    public static class DBLogger
    {
        public static void Log(LogType logType, string msg)
        {
            var isInserted = SQLiteDBHelper.InsertIntoTable(new Log()
            {
                Type = logType.ToString(),
                Message = msg,
                Timestamp = DateTime.Now.ToString()
            });
        }
    }
}
