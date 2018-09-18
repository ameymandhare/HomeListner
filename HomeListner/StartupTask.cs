using HomeListner.Common;
using HomeListner.DBEntity;
using HomeListner.Entity;
using Newtonsoft.Json;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Json;
using Windows.Storage;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HomeListner
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static HttpClient client = new HttpClient();

        private HttpServer _httpServer;

        private BackgroundTaskDeferral _deferral;



        /// <remarks>
        /// If you start any asynchronous methods here, prevent the task
        /// from closing prematurely by using BackgroundTaskDeferral as
        /// described in http://aka.ms/backgroundtaskdeferral
        /// </remarks>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                // This deferral should have an instance reference, if it doesn't... the GC will
                // come some day, see that this method is not active anymore and the local variable
                // should be removed. Which results in the application being closed.
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


                //var startTimeSpan = TimeSpan.Zero;
                //var periodTimeSpan = TimeSpan.FromMinutes(1);
                var stateTimer = new Timer(UpdateIpAsync,
                                   null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

                // Dont release deferral, otherwise app will stop
            }
            catch (Exception ex)
            {
                FileLogger.Log(LogType.Error, ex.Message);
            }
        }

        private async void UpdateIpAsync(object state)
        {
            //FileLogger.Log(Common.LogType.Information, "1");
            var publicIp = await new HttpClient().GetStringAsync("https://api.ipify.org/");
            //FileLogger.Log(Common.LogType.Information, "2");
            var device = new Device();
            //FileLogger.Log(Common.LogType.Information, "3");
            device.MacId = await GetMAC();
            device.IPAddress = publicIp;
            //FileLogger.Log(Common.LogType.Information, "4");
            FileLogger.Log(LogType.Information, "IP and MacID:" + publicIp + " / " + device.MacId);

            // Update port # in the following line.
            client.BaseAddress = new Uri("http://kosustestsite.somee.com/");
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
                wrGETURL.Credentials = new NetworkCredential("Administrator", "123");
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
    }
}
