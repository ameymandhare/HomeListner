using HomeListner.Common;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeListner
{
    [RestController(InstanceCreationType.Singleton)]
    public sealed class ParameterController
    {
        private DBLogger dbLogger;

        public ParameterController()
        {
            dbLogger = new DBLogger();
        }

        [UriFormat("/Parameter/{id}/property/{propName}")]
        public IGetResponse GetWithSimpleParameters(int id, string propName)
        {
            dbLogger.Log(DBEntity.LogType.Information, "Received request GetWithSimpleParameters() on: " + DateTime.Now.ToLocalTime());

            return new GetResponse(
              GetResponse.ResponseStatus.OK,
              new DataReceived() { ID = id, PropName = propName });
        }

        [UriFormat("/SamplePOST")]
        public IPostResponse PostWithSimpleParameter([FromContent]DataReceived user)
        {
            dbLogger.Log(DBEntity.LogType.Information, "Received request PostWithSimpleParameter() on: " + DateTime.Now.ToLocalTime());

            var obj = new SampleData { iD = user.ID, FirstName = "ABC", LastName = "POD", PropName = user.PropName };
            return new PostResponse(PostResponse.ResponseStatus.Created, "", obj);
        }
    }
}
