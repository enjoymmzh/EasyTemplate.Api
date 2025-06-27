using EasyTemplate.Tool.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTemplate.Tool;

public static class Setting
{
    private static IConfigurationRoot? configuration;

    /// <summary>
    /// 初始化配置，仅限appsettings.json文件
    /// </summary>
    /// <returns></returns>
    /// <summary>
    /// 初始化配置，仅限appsettings.json文件
    /// </summary>
    /// <returns></returns>
    public static bool AddConfiguration(this IServiceCollection services)
    {
        configuration = configuration ?? new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("Configuration/App.json", optional: true, reloadOnChange: true)
            .AddJsonFile("Configuration/DataBase.json", optional: true, reloadOnChange: true)
            .AddJsonFile("Configuration/Task.json", optional: true, reloadOnChange: true)
            .Build();
        Log.Info("加载配置完成");
        return configuration is null;
    }

    /// <summary>
    /// 获取appsettings.json配置信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T? Get<T>(string path)
    {
        try
        {
            var value = configuration?.GetSection(path).Value;
            var result = (T)Convert.ChangeType(value, typeof(T));
            return result;
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// 获取appsettings.json配置信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static IConfigurationSection? GetSection(string path)
    {
        try
        {
            return configuration?.GetSection(path);
        }
        catch (Exception)
        {
            return default;
        }
    }
}
