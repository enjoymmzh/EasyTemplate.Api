using System.Net;

namespace EasyTemplate.Tool.Entity
{
    public class Result
    {
        public HttpStatusCode Code { get; set; }
        public bool IsSuccess { get { return Code is HttpStatusCode.OK; } }
        public string Message { get; set; }
        public object Data { get; set; }
        public object Extra { get; set; }
        public long Timestamp { get { return DateTimeOffset.Now.ToUnixTimeMilliseconds(); } }

        public static Result Success(string message = "成功", object data = null)
            => new Result { Code = HttpStatusCode.OK, Message = message, Data = data };

        public static Result Fail(HttpStatusCode code = HttpStatusCode.BadRequest, string message = "失败", object data = null)
            => new Result { Code = code, Message = message, Data = data };
    }
}
