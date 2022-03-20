using HomeListner.Common;
using HomeListner.Controller;
using HomeListner.DBEntity;
using HomeListner.Entity;
using Newtonsoft.Json;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Json;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HomeListner
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static HttpClient client = new HttpClient();
        private HttpServer _httpServer;
        private BackgroundTaskDeferral _deferral;
        private string _myRaspberryMacAddress;



        /// <remarks>
        /// If you start any asynchronous methods here, prevent the task
        /// from closing prematurely by using BackgroundTaskDeferral as
        /// described in http://aka.ms/backgroundtaskdeferral
        /// </remarks>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                //We need to create database tables to run application
                //SQLiteDBHelper.CreateDatabase();

                //DBLogger.Log(LogType.Information, "Application started");
                // This deferral should have an instance reference, if it doesn't... the GC will
                // come some day, see that this method is not active anymore and the local variable
                // should be removed. Which results in the application being closed.

                //Get raspeberryPi mac address
                FileLogger.Log(LogType.Information, "Application Started...");
                _myRaspberryMacAddress = Utility.GetMacAddress();//await GetMAC();

                _deferral = taskInstance.GetDeferral();
                var restRouteHandler = new RestRouteHandler();

                restRouteHandler.RegisterController<ParameterController>();

                var configuration = new HttpServerConfiguration()
                    .ListenOnPort(8800)
                    .RegisterRoute("api", restRouteHandler)
                    // .RegisterRoute(new StaticFileRouteHandler(@"Restup.DemoStaticFiles\Web"))
                    .EnableCors(); // allow cors requests on all origins
                                   //  .EnableCors(x => x.AddAllowedOrigin("http://specificserver:<listen-port>"));

                var httpServer = new HttpServer(configuration);
                _httpServer = httpServer;

                await httpServer.StartServerAsync();

                //DBLogger.Log(LogType.Information, "ServerStarted");

                //var startTimeSpan = TimeSpan.Zero;
                //var periodTimeSpan = TimeSpan.FromMinutes(1);

                //var stateTimer = new Timer(UpdateIpAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
                startTriggerIpUpdate();

                // Dont release deferral, otherwise app will stop
            }
            catch (Exception ex)
            {
                FileLogger.Log(LogType.Error, ex.Message);
            }
        }

        public void startTriggerIpUpdate()
        {
            var setTimer = new Timer(UpdateIpAsync, null, 0, 30000);
        }

        private async void UpdateIpAsync(object state)
        {
            //FileLogger.Log(Common.LogType.Information, "1");
            var publicIp = await new HttpClient().GetStringAsync("https://api.ipify.org/");
            //FileLogger.Log(Common.LogType.Information, "2");
            var device = new Device();
            //FileLogger.Log(Common.LogType.Information, "3");
            device.MacId = _myRaspberryMacAddress;
            device.IPAddress = publicIp;
            //FileLogger.Log(Common.LogType.Information, "4");
            //FileLogger.Log(LogType.Information, "IP and MacID:" + publicIp + " / " + device.MacId);

            // Update port # in the following line.
            client.BaseAddress = new Uri("http://www.AutoHome.somee.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            //FileLogger.Log(Common.LogType.Information, "5");
            try
            {
                // Update the device Ip
                var json = JsonConvert.SerializeObject(device);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(
                    $"api/Admin/UpdatePublicIp", stringContent);
                response.EnsureSuccessStatusCode();
                //FileLogger.Log(Common.LogType.Information, "6");
                // Deserialize the updated product from the response body.
                var result = await response.Content.ReadAsStringAsync();
                //FileLogger.Log(Common.LogType.Information, "7");
            }
            catch (Exception ex)
            {
                FileLogger.Log(LogType.Error, "UpdateIpAsync " + ex.Message);
            }
        }

        private async Task<String> GetMAC()
        {
            String MAC = null;
            StreamReader SR = await GetJsonStreamData("http://localhost:8080/api/networking/ipconfig");
            JsonObject ResultData = null;
            try
            {
                String JSONData;

                JSONData = SR.ReadToEnd();

                ResultData = (JsonObject)JsonObject.Parse(JSONData);
                JsonArray Adapters = ResultData.GetNamedArray("Adapters");

                //foreach (JsonObject Adapter in Adapters)   
                for (uint index = 0; index < Adapters.Count; index++)
                {
                    JsonObject Adapter = Adapters.GetObjectAt(index).GetObject();
                    String Type = Adapter.GetNamedString("Type");
                    if (Type.ToLower().CompareTo("ethernet") == 0)
                    {
                        MAC = ((JsonObject)Adapter).GetNamedString("HardwareAddress");
                        break;
                    }
                }
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine(E.Message);
            }

            return MAC;
        }

        private async Task<StreamReader> GetJsonStreamData(String URL)
        {
            HttpWebRequest wrGETURL = null;
            Stream objStream = null;
            StreamReader objReader = null;

            try
            {
                wrGETURL = (HttpWebRequest)WebRequest.Create(URL);
                wrGETURL.Credentials = new NetworkCredential("Administrator", "123456");
                HttpWebResponse Response = (HttpWebResponse)(await wrGETURL.GetResponseAsync());
                if (Response.StatusCode == HttpStatusCode.OK)
                {
                    objStream = Response.GetResponseStream();
                    objReader = new StreamReader(objStream);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetData " + e.Message);
            }
            return objReader;
        }

        private static IBackgroundTaskRegistration Register()
        {
            // get the entry point of the task. I'm reusing this as the task name in order to get an unique name
            var taskEntryPoint = typeof(StartupTask).FullName;
            var taskName = taskEntryPoint;

            // if the task is already registered, there is no need to register it again
            var registration = BackgroundTaskRegistration.AllTasks.Select(x => x.Value).FirstOrDefault(x => x.Name == taskName);
            if (registration != null) return registration;

            // register the task to run every 30 minutes if an internet connection is available
            var taskBuilder = new BackgroundTaskBuilder();
            taskBuilder.Name = taskName;
            taskBuilder.TaskEntryPoint = taskEntryPoint;
            taskBuilder.SetTrigger(new TimeTrigger(1, false));
            taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

            return taskBuilder.Register();
        }
    }
}
