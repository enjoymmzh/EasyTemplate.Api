using EasyTemplate.Tool.Entity;
using Microsoft.Extensions.Configuration;

namespace EasyTemplate.Tool.Util;

public class Global
{
    /// <summary>
    /// 
    /// </summary>
    public static string? configId { get { return Setting.Get<string>("dbConnection:connectionConfigs:0:configId"); } }
    /// <summary>
    /// 
    /// </summary>
    public static string? connectionString { get { return Setting.Get<string>("dbConnection:connectionConfigs:0:connectionString"); } }
    /// <summary>
    /// 
    /// </summary>
    public static string? redisconnectionString { get { return $"{Setting.Get<string>("Cache:RedisConnectionString")},prefix={Setting.Get<string>("Cache:InstanceName")}"; } }
    /// <summary>
    /// 
    /// </summary>
    public static bool enableInitTable { get { return Setting.Get<bool>("dbConnection:connectionConfigs:0:tableSettings:enableInitTable"); } }
    /// <summary>
    /// 
    /// </summary>
    public static bool encryptResult { get { return Setting.Get<bool>("cryptogram:enabled"); } }
    /// <summary>
    /// 
    /// </summary>
    public static string? crypType { get { return Setting.Get<string>("cryptogram:cryptoType"); } }
    /// <summary>
    /// 材料
    /// </summary>
    public static string MaterialRedisHeader(long id) => $"material_{id}";
    /// <summary>
    /// 广告单元
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string UnitRedisHeader(long id) => $"unit_{id}";
    /// <summary>
    /// 黑名单
    /// </summary>
    /// <param name="unitid"></param>
    /// <param name="materialid"></param>
    /// <returns></returns>
    public static string BlacklistRedisHeader() => $"blacklist";
    /// <summary>
    /// 点击
    /// </summary>
    public static string EscalationClickRedisHeader(long materialid) => $"click_{materialid}";
    /// <summary>
    /// 曝光，浏览量
    /// </summary>
    /// <param name="materialid"></param>
    /// <returns></returns>
    public static string EscalationExposureRedisHeader(long materialid) => $"exposure_{materialid}";
    /// <summary>
    /// 统计详情
    /// </summary>
    /// <returns></returns>
    public static string StatisticsRedisHeader() => $"statistics_{Guid.NewGuid().ToString().Replace("-","")}";
    /// <summary>
    /// 
    /// </summary>
    public static SystemUser user { get; set; } = new SystemUser();
    /// <summary>
    /// 
    /// </summary>
    public static string GenericJsCode { get; set; } = $"<div data-ads-id=\"广告ID\" data-ads-type=\"类型\" data-ads-load=\"false\"></div>\r\n<script src=\"js路径\"></script>";
}
