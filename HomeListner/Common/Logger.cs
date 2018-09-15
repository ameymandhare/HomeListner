using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HomeListner.Common
{
    internal static class Logger
    {
        private static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        private static StorageFile sampleFile = null;

        public static async void LogToFile(string Message)
        {
            sampleFile = await storageFolder.CreateFileAsync("sampleLog.txt", CreationCollisionOption.OpenIfExists);

            await FileIO.WriteTextAsync(sampleFile, "FileLocation:" + storageFolder.Path.ToString() + "\t" + "TimeStamp: " + DateTime.Now.ToString()+ "\r\n");
            await FileIO.WriteLinesAsync(sampleFile, new string[] { "\r\n" });

        }
    }
}
