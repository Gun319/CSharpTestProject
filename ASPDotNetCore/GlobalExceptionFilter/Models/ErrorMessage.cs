namespace GlobalExceptionFilter.Models
{
    public class ErrorMessage
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// 堆栈跟踪信息
        /// </summary>
        public string? Stacktrace { get; set; }
    }
}
