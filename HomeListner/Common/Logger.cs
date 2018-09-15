﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HomeListner.Common
{
    public enum LogType
    {
        Information,
        Error,
        Warning,
        Debug
    }

    public static class FileLogger
    {
        private static StorageFile sampleFile = null;
        private static readonly StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        public async static void Log(LogType logType, string msg)
        {
            // Create sample file; replace if exists.
            sampleFile = await storageFolder.CreateFileAsync("sampleLog.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(sampleFile, Environment.NewLine + "======================= " + logType.ToString() + " ======================= ");
            await FileIO.AppendTextAsync(sampleFile, Environment.NewLine + Environment.NewLine + msg + Environment.NewLine + "TimeStamp: " + DateTime.Now.ToString());
            await FileIO.AppendTextAsync(sampleFile, Environment.NewLine + "======================= Log End =======================");
        }
    }
}
