using Microsoft.AspNetCore.Mvc.Filters;

namespace GlobalExceptionFilter
{
    public class MyActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Console.WriteLine("ActionFilter 前代码");
            ActionExecutedContext result = await next();
            if (result.Exception != null)
            {
                Console.WriteLine("ActionFilter：发生异常");
            }
            else
            {
                Console.WriteLine("ActionFilter：执行成功");
            }
        }
    }
}
