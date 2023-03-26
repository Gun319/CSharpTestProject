using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Net;

namespace Jwt.Commons
{
    public class MyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _authorizationMiddleware = new();
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            // 当 token失效或者token不存在的时候 authorizeResult.Challenged 为True
            if (authorizeResult.Challenged)
            {
                // todo 拿到上下文user对象后 此处可以check token  区分token是否是过期了
                context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.Unauthorized);
                await context.Response.WriteAsJsonAsync(new ResponseModel(ResponseCode.UnAuthorized, "您未授权,请检查Token是否有效！"));
                return;
            }

            // 此时token 校验通过  但是访问的资源的没权限的话 authorizeResult.Forbidden 为true
            if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.Forbidden);
                await context.Response.WriteAsJsonAsync(new ResponseModel(ResponseCode.ForBidden, "您无权限访问！"));
                return;
            }
            await _authorizationMiddleware.HandleAsync(next, context, policy, authorizeResult);
        }
    }

    public class ResponseModel
    {
        public ResponseCode Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public ResponseModel(ResponseCode responseCode, string responseMessage = "", object data = null)
        {
            Code = responseCode;
            Message = responseMessage;
            Data = data;
        }
    }
}
