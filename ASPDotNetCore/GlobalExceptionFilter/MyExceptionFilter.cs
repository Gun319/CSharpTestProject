using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GlobalExceptionFilter
{
    public class MyExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MyExceptionFilter(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            // context.Exception 代表异常信息对象
            // 如果给 context.ExceptionHandled 赋值为 true，则其他 ExceptionFilter 不再执行
            // context.Result 的值会被返回给客户端

            // 判断是否为开发模式
            string message = _webHostEnvironment.IsDevelopment() ? context.Exception.ToString() : "服务器发生未处理异常";
            
            ObjectResult result = new(new
            {
                code = 500,
                message = message
            });
            context.Result = result;
            context.ExceptionHandled = true;

            return Task.CompletedTask;
        }
    }
}
