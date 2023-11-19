using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace GlobalExceptionFilter
{
    /// <summary>
    /// 请求限流器
    /// </summary>
    public class RateLimitActionFilter : IAsyncActionFilter
    {
        private readonly IMemoryCache _memoryCache;

        public RateLimitActionFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string removeIP = context.HttpContext.Connection.RemoteIpAddress!.ToString();
            string cachKey = $"LastVisitTick_{removeIP}";
            long? lastTick = _memoryCache.Get<long?>(cachKey);

            if (lastTick is null || Environment.TickCount64 - lastTick > 1000)
            {
                _memoryCache.Set(cachKey, Environment.TickCount64, TimeSpan.FromSeconds(5));
                return next();
            }

            context.Result = new ContentResult { StatusCode = StatusCodes.Status429TooManyRequests };
            return Task.CompletedTask;
        }
    }
}
