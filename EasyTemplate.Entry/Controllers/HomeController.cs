using EasyTemplate.Entry.Common;
using EasyTemplate.Service.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTemplate.Entry;

/// <summary>
/// 如果不适应Service写法，这里提供了基于controller的基本写法
/// </summary>
[ApiGroup(ApiGroupNames.Test), AllowAnonymous]
public class HomeController : TheBaseController
{
    /// <summary>
    /// 自定义注入在此实例化
    /// </summary>
    private readonly IInjectionSample _inj;
    public HomeController(
        IInjectionSample inj
    )
    {
        _inj = inj;
    }

    /// <summary>
    /// 测试用
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("/api/v2/[controller]/[action]")]
    public async Task<string> Test()
        => _inj.ExecuteFunction();
}
