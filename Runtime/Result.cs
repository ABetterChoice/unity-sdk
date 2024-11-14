namespace AbcSDKSpace
{

    public class Result
    {
        public StatusCode statusCode;
        public string message;

        public Result(StatusCode statusCode, string message)
        {
            this.statusCode = statusCode;
            this.message = message;
        }

        public Result(StatusCode statusCode)
        {
            this.statusCode = statusCode;
            this.message = "";
        }
    }

    public enum StatusCode
    {
        Success = 0,
        InvalidParamErr = -1,
        ReInitErr = -2,
    }
}