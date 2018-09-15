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
        [UriFormat("/Parameter/{id}/property/{propName}")]
        public IGetResponse GetWithSimpleParameters(int id, string propName)
        {
            return new GetResponse(
              GetResponse.ResponseStatus.OK,
              new DataReceived() { ID = id, PropName = propName });
        }

        [UriFormat("/SamplePOST")]
        public IPostResponse PostWithSimpleParameter([FromContent]DataReceived user)
        {
            var obj = new SampleData { iD = user.ID, FirstName = "ABC", LastName = "POD", PropName = user.PropName };
            return new PostResponse(PostResponse.ResponseStatus.Created, "", obj);
        }
    }
}
