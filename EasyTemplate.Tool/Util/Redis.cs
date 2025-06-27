using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTemplate.Tool.Util;

public static class Redis
{
    /// <summary>
    /// 使用缓存
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static void AddRedis(this IServiceCollection services)
    {
        //csredis的两种使用方式
        var csredis = new CSRedisClient(Global.redisconnectionString);
        services.AddSingleton(csredis);
        RedisHelper.Initialization(csredis);

        //基于redis初始化IDistributedCache
        services.AddSingleton<IDistributedCache>(new CSRedisCache(csredis));
    }

}
