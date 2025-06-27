using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using EasyTemplate.Tool.Util;
using EasyTemplate.Service.Common;
using EasyTemplate.Tool.Entity;
using EasyTemplate.Tool.Entity.Dto.UserDto;
using EasyTemplate.Tool.Entity.Common;

namespace EasyTemplate.Service;

[DynamicController]
[AllowAnonymous]
[ApiGroup(ApiGroupNames.Auth)]
public class AuthService
{
    private readonly SqlSugarRepository<SystemUser> _user;
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthService(
        IHttpContextAccessor contextAccessor,
        SqlSugarRepository<SystemUser> user
        )
    {
        _user = user;
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    /// 登录
    /// {"username":"admin","password":"123456"}
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <remarks><code>{"username":"admin","password":"123456"}</code></remarks>
    [HttpPost]
    public async Task<object> Login(LoginInput input)
    {
        var user = await _user.AsQueryable()
            .Where(x => x.Account.Equals(input.username) && x.Password.Equals(input.password))
            .FirstAsync();
        _ = user ?? throw Oops.Oh(Tool.PublicEnum.ErrorCode.E1000);

        //生成Token令牌
        var token = Jwt.Serialize(new TokenModelJwt
        {
            UserId = user.Id,
            Name = user.Account
        });
        _contextAccessor.HttpContext.Response.Headers["access-token"] = token;
        return token;
    }
}
