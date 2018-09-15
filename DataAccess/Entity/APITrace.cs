using DataAccess.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entity
{
    [Table(name: DBTableConstant.APITrace)]
    public class APITrace
    {
        [Column(name: DBColumnConstant.APITraceId)]
        public int Id { get; set; }

        [Column(name: DBColumnConstant.DateTime)]
        public int LogDateTime { get; set; }

        [Column(name: DBColumnConstant.APIType)]
        public APIType APIType { get; set; }

        [Column(name: DBColumnConstant.APIRequestHeader)]
        public string RequestHeader { get; set; }

        [Column(name: DBColumnConstant.APIRequestBody)]
        public string RequestBody { get; set; }

        [Column(name: DBColumnConstant.APIRequestIPAddress)]
        public string RequestIpAddress { get; set; }

        [Column(name: DBColumnConstant.APIResponseData)]
        public string ResponseData { get; set; }
    }
}
