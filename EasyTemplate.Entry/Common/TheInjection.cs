namespace EasyTemplate.Entry;

/// <summary>
/// 提供基本自定义注入写法接口，此处为范例，建议新建文件写入接口及实现逻辑，建议写在实体分类中
/// </summary>
public interface IInjectionSample
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    string ExecuteFunction();
}

/// <summary>
/// 提供基本自定义注入写法实现
/// </summary>
public class InjectionSample : IInjectionSample
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string ExecuteFunction() => "注入成功";
}

/// <summary>
/// 注入api服务
/// </summary>
public static class TheInjection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    public static void AddCustomInjection(this IServiceCollection service)
    {
        //声明
        service.AddSingleton<IInjectionSample, InjectionSample>();
        service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }
}
