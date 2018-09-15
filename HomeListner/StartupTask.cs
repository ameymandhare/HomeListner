using HomeListner.Entity;
using Newtonsoft.Json;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Windows.ApplicationModel.Background;
using Serilog;
using Windows.Storage;
using HomeListner.Common;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HomeListner
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static HttpClient client = new HttpClient();

        private HttpServer _httpServer;

        private BackgroundTaskDeferral _deferral;

        private StorageFile sampleFile = null;
        private StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        /// <remarks>
        /// If you start any asynchronous methods here, prevent the task
        /// from closing prematurely by using BackgroundTaskDeferral as
        /// described in http://aka.ms/backgroundtaskdeferral
        /// </remarks>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                // Create sample file; replace if exists.

                sampleFile = await storageFolder.CreateFileAsync("sampleLog.txt", CreationCollisionOption.OpenIfExists);

                // This deferral should have an instance reference, if it doesn't... the GC will
                // come some day, see that this method is not active anymore and the local variable
                // should be removed. Which results in the application being closed.
                _deferral = taskInstance.GetDeferral();
                var restRouteHandler = new RestRouteHandler();

                restRouteHandler.RegisterController<ParameterController>();
                //restRouteHandler.RegisterController<FromContentControllerSample>();
                //restRouteHandler.RegisterController<PerCallControllerSample>();
                //restRouteHandler.RegisterController<SimpleParameterControllerSample>();
                //restRouteHandler.RegisterController<SingletonControllerSample>();
                //restRouteHandler.RegisterController<ThrowExceptionControllerSample>();
                //restRouteHandler.RegisterController<WithResponseContentControllerSample>();

                var configuration = new HttpServerConfiguration()
                    .ListenOnPort(8800)
                    .RegisterRoute("api", restRouteHandler)
                    // .RegisterRoute(new StaticFileRouteHandler(@"Restup.DemoStaticFiles\Web"))
                    .EnableCors(); // allow cors requests on all origins
                                   //  .EnableCors(x => x.AddAllowedOrigin("http://specificserver:<listen-port>"));

                var httpServer = new HttpServer(configuration);
                _httpServer = httpServer;

                await httpServer.StartServerAsync();

                await FileIO.WriteTextAsync(sampleFile, "\n\n\n"+"FileLocation:" + storageFolder.Path.ToString() +"\n"+"TimeStamp: "+DateTime.Now.ToString());
                //var startTimeSpan = TimeSpan.Zero;
                //var periodTimeSpan = TimeSpan.FromMinutes(1);

                //var timer = new System.Threading.Timer((e) =>
                //{
                UpdateIpAsync();
                //}, null, startTimeSpan, periodTimeSpan);

                // Dont release deferral, otherwise app will stop
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }
        }

        private async void UpdateIpAsync()
        {

            var publicIp = await new HttpClient().GetStringAsync("https://api.ipify.org/");
            //var firstMacAddress = NetworkInterface
            //                         .GetAllNetworkInterfaces()
            //                         .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            //                         .Select(nic => nic.GetPhysicalAddress().ToString())
            //                         .FirstOrDefault();

            var macId = "b8:27:eb:1e:8b:51";

            await FileIO.WriteTextAsync(sampleFile, "\n\n\n" + "IP and MacID:" + publicIp + "/"+ macId + "\n" + "TimeStamp: " + DateTime.Now.ToString());

            var device = new Device();
            device.MacId = macId;
            device.IPAddress = publicIp;

            // Update port # in the following line.
            client.BaseAddress = new Uri("http://kosustestsite.somee.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Update the device Ip
                var json = JsonConvert.SerializeObject(device);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(
                    $"api/Admin/UpdatePublicIp", stringContent);
                response.EnsureSuccessStatusCode();

                // Deserialize the updated product from the response body.
                var result = await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                Log.Error(e, "Something went wrong");
            }
        }
    }
}
