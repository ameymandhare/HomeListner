using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Common
{
    public static class DBTableConstant
    {
        public const string Log = "Log";
        public const string RoomStatus = "RoomStatus";
        public const string APITrace = "RoomStatus";
    }

    public static class DBColumnConstant
    {
        public const string Id = "Id";
        public const string DateTime = "DateTime";
        public const string Information = "Information";
        public const string APITraceId = "TraceId";
        public const string APIType = "APIType";
        public const string APIRequestIPAddress = "RequestIPAddress";
        public const string APIRequestHeader = "RequestHeader";
        public const string APIRequestBody = "RequestBody";
        public const string APIResponseData = "ResponseData";
    }
}
