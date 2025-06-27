using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using EasyTemplate.Tool.Util;
using EasyTemplate.Service.Common;
using EasyTemplate.Tool.Entity;
using static EasyTemplate.Tool.PublicEnum;
using EasyTemplate.Tool;

namespace EasyTemplate.Service;

[DynamicController]
[ApiGroup(ApiGroupNames.System)]
public class SystemUserService
{
    private readonly SqlSugarRepository<SystemUser> _user;
    private readonly IHttpContextAccessor _contextAccessor;

    public SystemUserService(
        IHttpContextAccessor contextAccessor,
        SqlSugarRepository<SystemUser> user
        )
    {
        _user = user;
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    /// 多语言、单一接口的多种请求方式
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [HttpPost("/api/v1/[Controller]/[Action]")]
    [HttpGet("/api/v2/[controller]/[action]")]
    [AllowAnonymous]
    public async Task<object> Test()
    {
        //前端寫入語言
        _contextAccessor.HttpContext.Request.Headers.ContentLanguage = "en-US";

        //接口處理語言
        var list = _user.AsQueryable().ToList();
        list.ForEach(item => {
            item.Account = item.AreaCode?.ToLocal(_contextAccessor.HttpContext);
        });

        var aaa = _contextAccessor.HttpContext.Request.Headers.Authorization;
        return list;
    }

    /// <summary>
    /// 友好异常
    /// </summary>
    /// <returns></returns>
    public async Task<object> TestException()
    {
        _contextAccessor.HttpContext.Request.Headers.ContentLanguage = "en-US";
        throw Oops.Oh(Tool.PublicEnum.ErrorCode.E1000, _contextAccessor.HttpContext);
        throw Oops.Oh(ErrorCode.E1000);
        throw Oops.Oh("123123123123");
    }

    /// <summary>
    /// 黏土类型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<object> TestClay()
    {
        var obj = Clay.Object();
        obj.Id = 1;
        obj["Name"] = "Furion";
        obj.Data = new string[] { "Furion", "Fur" };
        return obj;
    }
}
