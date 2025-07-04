﻿using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EasyTemplate.Tool.Util;

/// <summary>
/// 缓存帮助类
/// </summary>
public class MemoryCache
{
	private static readonly Microsoft.Extensions.Caching.Memory.MemoryCache Cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());

	/// <summary>
	/// 验证缓存项是否存在
	/// </summary>
	/// <param name="key">缓存Key</param>
	/// <returns></returns>
	public static bool Exists(string key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		return Cache.TryGetValue(key, out _);
	}

	/// <summary>
	/// 添加缓存
	/// </summary>
	/// <param name="key">缓存Key</param>
	/// <param name="value">缓存Value</param>
	/// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
	/// <param name="expiressAbsoulte">绝对过期时长</param>
	/// <returns></returns>
	public static bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (value == null)
			throw new ArgumentNullException(nameof(value));

		Cache.Set(key, value.ToJson(),
			new MemoryCacheEntryOptions().SetSlidingExpiration(expiresSliding)
				.SetAbsoluteExpiration(expiressAbsoulte));
		return Exists(key);
	}

	/// <summary>
	/// 添加缓存
	/// </summary>
	/// <param name="key">缓存Key</param>
	/// <param name="value">缓存Value</param>
	/// <param name="expiresIn">缓存时长</param>
	/// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
	/// <returns></returns>
	public static bool Set(string key, object value, TimeSpan expiresIn, bool isSliding = false)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		Cache.Set(key, value.ToJson(),
			isSliding
				? new MemoryCacheEntryOptions().SetSlidingExpiration(expiresIn)
				: new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiresIn));

		return Exists(key);
	}

	/// <summary>
	/// 添加缓存
	/// </summary>
	/// <param name="key">缓存Key</param>
	/// <param name="value">缓存Value</param>
	/// <returns></returns>
	public static bool Set(string key, object value)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		Cache.Set(key, value.ToJson());

		return Exists(key);
	}

	#region 删除缓存

	/// <summary>
	/// 删除缓存
	/// </summary>
	/// <param name="key">缓存Key</param>
	/// <returns></returns>
	public static void Remove(string key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));

		Cache.Remove(key);
	}

	/// <summary>
	/// 批量删除缓存
	/// </summary>
	/// <returns></returns>
	public static void RemoveAll(IEnumerable<string> keys)
	{
		if (keys == null)
			throw new ArgumentNullException(nameof(keys));

		keys.ToList().ForEach(item => Cache.Remove(item));
	}

	#endregion 删除缓存

	#region 获取缓存

	/// <summary>
	/// 获取缓存
	/// </summary>
	/// <param name="key">缓存Key</param>
	/// <returns></returns>
	public static T Get<T>(string key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		object temp;
		if (Cache.TryGetValue(key, out temp))
		{
			return temp.ToString().ToEntity<T>();
		}
		return default(T);
	}

	/// <summary>
	/// 获取缓存
	/// </summary>
	/// <param name="key">缓存Key</param>
	/// <returns></returns>
	public static string Get(string key)
	{
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (Cache.Get(key) == null)
		{
			return string.Empty;
		}
		return Cache.Get(key).ToString();
	}

	/// <summary>
	/// 获取缓存集合
	/// </summary>
	/// <param name="keys">缓存Key集合</param>
	/// <returns></returns>
	public static IDictionary<string, object> GetAll(IEnumerable<string> keys)
	{
		if (keys == null)
			throw new ArgumentNullException(nameof(keys));

		var dict = new Dictionary<string, object>();
		keys.ToList().ForEach(item => dict.Add(item, Cache.Get(item)));
		return dict;
	}

	#endregion 获取缓存

	/// <summary>
	/// 删除所有缓存
	/// </summary>
	public static void RemoveCacheAll()
	{
		var l = GetCacheKeys();
		foreach (var s in l)
		{
			Remove(s);
		}
	}

	/// <summary>
	/// 删除匹配到的缓存
	/// </summary>
	/// <param name="pattern"></param>
	/// <returns></returns>
	public static void RemoveCacheRegex(string pattern)
	{
		IEnumerable<string> l = SearchCacheRegex(pattern);
		foreach (var s in l)
		{
			Remove(s);
		}
	}

	/// <summary>
	/// 搜索 匹配到的缓存
	/// </summary>
	/// <param name="pattern"></param>
	/// <returns></returns>
	public static IEnumerable<string> SearchCacheRegex(string pattern)
	{
		var cacheKeys = GetCacheKeys();
		var l = cacheKeys.Where(k => Regex.IsMatch(k, pattern)).ToList();
		return l.AsReadOnly();
	}

	/// <summary>
	/// 获取所有缓存键
	/// </summary>
	/// <returns></returns>
	public static List<string> GetCacheKeys()
	{
#if NET8_0
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = Cache.GetType().GetField("_coherentState", flags)?.GetValue(Cache);
            var cacheItems = entries?.GetType()?.GetProperty("EntriesCollection", flags)?.GetValue(entries) as ICollection; //entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (var item in cacheItems)
            {
                var methodInfo = item.GetType().GetProperty("Key");

                var val = methodInfo.GetValue(item);

                keys.Add(val.ToString());
            }
            return keys;
#else
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = Cache.GetType().GetField("_entries", flags).GetValue(Cache);
            var cacheItems = entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                keys.Add(cacheItem.Key.ToString());
            }
            return keys;
#endif
        }
    }