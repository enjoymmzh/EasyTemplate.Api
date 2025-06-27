using Masuit.Tools.Logging;
using System.Diagnostics;

namespace EasyTemplate.Entry.Common;

/// <summary>
/// 中间件
/// 记录请求和响应数据
/// 暂时不注册
/// </summary>
public class RequestMiddleware
{
    private readonly RequestDelegate _next;

    private Stopwatch _stopwatch;

    public RequestMiddleware(RequestDelegate next)
    {
        _stopwatch = new Stopwatch();
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 过滤，只有接口
        if (context.Request.Path.Value.ToLower().Contains("api"))
        {
            context.Request.EnableBuffering();
            Stream originalBody = context.Response.Body;

            _stopwatch.Restart();

            // 获取 Api 请求内容
            var requestContent = await GetRequesContent(context);


            // 获取 Api 返回内容
            using (var ms = new MemoryStream())
            {
                context.Response.Body = ms;

                await _next(context);
                ms.Position = 0;

                await ms.CopyToAsync(originalBody);
            }

            context.Response.Body = originalBody;

            _stopwatch.Stop();

            LogManager.Info($"请求返回为：{requestContent}");
        }
        else
        {
            await _next(context);
        }
    }

    private async Task<string> GetRequesContent(HttpContext context)
    {
        var request = context.Request;
        var sr = new StreamReader(request.Body);

        var content = $"{await sr.ReadToEndAsync()}";

        if (!string.IsNullOrEmpty(content))
        {
            request.Body.Position = 0;
        }

        return content;
    }
}

public class RequestLocalizationMiddleware
{
    private readonly RequestDelegate _next;
    /// <summary>
    /// 构造 Http 请求中间件
    /// </summary>
    /// <param name="next"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="cacheService"></param>
    public RequestLocalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        var aaa = context.Request.Headers.ContentLanguage;
        context.Request.EnableBuffering();
    }
}
