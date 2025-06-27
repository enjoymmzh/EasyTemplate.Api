using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTemplate.Entry.Common;

/// <summary>
/// 自定义路由模版
/// 用于解决swagger文档No operations defined in spec!问题
/// </summary>
///[Authorize]
#if DEBUG
[AllowAnonymous]
#endif
[Route("api/[controller]/[action]")]
[ApiController]
public class TheBaseController : ControllerBase { }
