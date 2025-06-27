using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Masuit.Tools;
using EasyTemplate.Tool.Entity;
using EasyTemplate.Tool.Util;

namespace EasyTemplate.Entry.Filter;

/// <summary>
/// 全局异常
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        Log.Error(context.Exception);
        var ex = context.Exception as FriendlyException;
        if (ex != null)
        {
            switch (ex.StatusCode)
            {
                //处理400状态码
                case StatusCodes.Status400BadRequest:
                    context.Result = new JsonResult(Result.Fail((HttpStatusCode)ex.StatusCode, ex.ValidationException ? ex.ErrorMessage.ToString() : "400 请求失败", false));
                    context.ExceptionHandled = true;
                    break;
                //处理401状态码
                case StatusCodes.Status401Unauthorized:
                    context.Result = new JsonResult(Result.Fail((HttpStatusCode)ex.StatusCode, "401 登录已过期，请重新登录", false));
                    context.ExceptionHandled = true;
                    break;
                //处理403状态码
                case StatusCodes.Status403Forbidden:
                    context.Result = new JsonResult(Result.Fail((HttpStatusCode)ex.StatusCode, "403 禁止访问，没有权限", false));
                    context.ExceptionHandled = true;
                    break;
                //处理404状态码
                case StatusCodes.Status404NotFound:
                    context.Result = new JsonResult(Result.Fail((HttpStatusCode)ex.StatusCode, "404 未找到", false));
                    context.ExceptionHandled = true;
                    break;
                //处理500状态码
                case StatusCodes.Status500InternalServerError:
                    context.Result = new JsonResult(Result.Fail((HttpStatusCode)ex.StatusCode, "500 内部服务器错误", false));
                    context.ExceptionHandled = true;
                    break;
                case 600:
                    context.Result = new JsonResult(Result.Fail(HttpStatusCode.BadRequest, $"{ex.ErrorCode} {ex.Message}", false));
                    context.ExceptionHandled = true;
                    break;
                default:
                    context.Result = new JsonResult(Result.Fail((HttpStatusCode)ex.StatusCode, $"{ex.StatusCode} 应用内错误", false));
                    context.ExceptionHandled = true;
                    break;
            }
        }
        else
        {
            context.Result = new JsonResult(Result.Fail(message: $"400 应用内错误", data: false));
            context.ExceptionHandled = true;
        }
    }
}

/// <summary>
/// 请求拦截
/// </summary>
public class GlobalActionFilter : IAsyncActionFilter
{

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //var result = context.Result;
        //var objectResult = result as ObjectResult;
        ////var resultLog = $"{DateTime.Now} 调用 {context.RouteData.Values["action"]} api 完成；执行结果：{Newtonsoft.Json.JsonConvert.SerializeObject(objectResult.Value)}";

        //var httpContext = context.HttpContext;
        //var httpRequest = httpContext.Request;
        //var actionContext = await next();

        ////判断是否请求成功（没有异常就是请求成功）
        //var isRequestSucceed = actionContext.Exception == null;
        //if (isRequestSucceed)
        //{

        //}
    }

}

/// <summary>
/// 实现自定义授权
/// </summary>
public class AuthorizeFilter : IAuthorizationFilter
{
    /// <summary>
    /// 请求验证，当前验证部分不要抛出异常，ExceptionFilter不会处理
    /// </summary>
    /// <param name="context"></param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
        {
            return;
        }

        if (context.HttpContext.Response.StatusCode == 200)
        {
            string token = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new JsonResult(Result.Fail(HttpStatusCode.Forbidden, "非法请求", false));
            }
            else
            {
                try
                {
                    token = token.Replace("Bearer ", "");
                    var info = Jwt.Deserialize(token, out DateTime expired);
                    if (expired.Subtract(DateTime.Now).TotalSeconds > 0)
                    {
                        context.HttpContext.Session.SetString("userid", info.UserId.ToString());
                        context.HttpContext.Session.SetString("username", info.Name);
                    }
                    else
                    {
                        context.Result = new JsonResult(Result.Fail(HttpStatusCode.Unauthorized, "token已过期", false));
                    }
                }
                catch
                {
                    context.Result = new JsonResult(Result.Fail(HttpStatusCode.Unauthorized, "token格式不正确", false));
                }
            }
        }
        else if (context.HttpContext.Response.StatusCode == 401)
        {
            context.Result = new JsonResult(Result.Fail(HttpStatusCode.Unauthorized, "请先登录", false));
        }
    }
}

/// <summary>
/// 
/// </summary>
public class ResultFilter : Attribute, IResultFilter
{
    public void OnResultExecuted(ResultExecutedContext context)
    {
        // 在结果执行之后调用的操作...
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is BadRequestObjectResult vresult)
        {
            if (vresult.Value is ValidationProblemDetails detail)
            {
                context.Result = new JsonResult(new Result() { Code = HttpStatusCode.BadRequest, Message = "字段验证失败", Data = detail.Errors.ToList() });
                return;
            }
        }

        if (Global.encryptResult)
        {
            var result = context.Result as OkObjectResult;
            if (result?.Value is Result res)
            {
                res.Data = res.Data is null ? res.Data : Crypto.Encrypt(res?.Data?.ToJson());
                context.Result = new OkObjectResult(res);
                return;
            }

            var jresult = context.Result as JsonResult;
            if (jresult?.Value is Result jres)
            {
                jres.Data = jres.Data is null ? jres.Data : Crypto.Encrypt(jres?.Data?.ToJson());
                context.Result = new OkObjectResult(jres);
                return;
            }
        }
        else
        {
            if (context.Result is ObjectResult result)
            {
                context.Result = new JsonResult(new Result() { Code = (HttpStatusCode)context.HttpContext.Response.StatusCode, Message = "成功", Data = result?.Value });
                return;
            }
        }
    }
}
