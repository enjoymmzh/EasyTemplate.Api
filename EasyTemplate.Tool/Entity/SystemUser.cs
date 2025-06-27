using EasyTemplate.Tool.Entity.Common;
using SqlSugar;

namespace EasyTemplate.Tool.Entity;

/// <summary>
/// 系统用户
/// </summary>
[SugarTable(null, "系统用户")]
public class SystemUser : EntityBase
{
    /// <summary>
    /// 账号
    /// </summary>
    [SugarColumn(ColumnDescription = "账号", Length = 32)]
    public string? Account { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(ColumnDescription = "密码", Length = 32)]
    public string? Password { get; set; }
    /// <summary>
    /// 区域编码，例如130300
    /// </summary>
    [SugarColumn(ColumnDescription = "区域编码")]
    public string? AreaCode { get; set; }
    /// <summary>
    /// 区域
    /// </summary>
    [SugarColumn(ColumnDescription = "区域")]
    public string? Area { get; set; }
}

public class SystemUserSeedData : ISeedData<SystemUser>
{
    public IEnumerable<SystemUser> Generate()
        =>
        [
            new SystemUser() { Id = 1, Account = "admin", Password = "123456", AreaCode = "130300", CreateTime = DateTime.Now },
            new SystemUser() { Id = 2, Account = "zhang", Password = "654321", AreaCode = "130300", CreateTime = DateTime.Now },
        ];
}
