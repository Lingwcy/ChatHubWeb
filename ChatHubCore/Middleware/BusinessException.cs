namespace ChatHubApi.Middleware
{
    /// <summary>
    /// 业务异常类，用于区分业务逻辑错误和系统异常
    /// </summary>
    public class BusinessException : Exception
    {
        public string Code { get; }

        public BusinessException(string message, string code = "business_error") : base(message)
        {
            Code = code;
        }
    }
}
