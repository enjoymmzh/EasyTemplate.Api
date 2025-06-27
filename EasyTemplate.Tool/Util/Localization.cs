using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using static EasyTemplate.Tool.PublicEnum;

namespace EasyTemplate.Tool.Util;

/// <summary>
/// 请求时，header中Content-Language需要设置值，且只设置一个值
/// </summary>
public static class Local
{
    private static Dictionary<string, string> _localization { get; set; } = new Dictionary<string, string>();
    private static Dictionary<string, Dictionary<string, string>> _languages { get; set; } = new Dictionary<string, Dictionary<string, string>>();

    static Local()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var files = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}Resources\\Language\\");
        foreach (var file in files)
        {
            var language = Path.GetFileNameWithoutExtension(file);
            using var fileStream = new FileStream($"{AppDomain.CurrentDomain.BaseDirectory}Resources\\Language\\{language}.csv", FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(fileStream, Encoding.GetEncoding("GB2312"));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            _localization = csv.GetRecords<LocalLanguage>().ToDictionary(x => x.Key, x => x.Value);
            _languages.Add(language, _localization);
        }
    }

    /// <summary>
    /// 支持的语言
    /// </summary>
    /// <returns></returns>
    public static List<string> SupportedLanguages() => _languages.Keys.ToList();

    /// <summary>
    /// 从csv获取本地化语言ErrorCode，会取出Request.Headers.ContentLanguage中支持的第一个语言
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string ToLocal(this string key, string lang = "zh-CN;")
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }

        if (!string.IsNullOrWhiteSpace(lang))
        {
            lang = lang.Split(';')[0];
        }
        if (_languages.ContainsKey(lang) && _localization.ContainsKey(key))
        {
            return _languages[lang][key];
        }
        return key;
    }

    /// <summary>
    /// 从csv获取本地化语言ErrorCode，会取出Request.Headers.ContentLanguage中支持的第一个语言
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string ToLocal(this string key, HttpContext context)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }

        var lang = context?.Request?.Headers?.ContentLanguage.ToString();
        lang = string.IsNullOrWhiteSpace(lang) ? "zh-CN" : lang.Split(';')[0];
        if (_languages.ContainsKey(lang) && _localization.ContainsKey(key))
        {
            return _languages[lang][key];
        }
        return key;
    }

    /// <summary>
    /// 从csv获取本地化语言，会取出Request.Headers.ContentLanguage中支持的第一个语言
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static string ToLocal(this ErrorCode code, string? lang = "zh-CN;")
    {
        lang = string.IsNullOrWhiteSpace(lang) ? "zh-CN" : lang.Split(';')[0];
        var key = Enum.GetName(typeof(ErrorCode), code);
        if (!string.IsNullOrEmpty(key) && _languages.ContainsKey(lang) && _localization.ContainsKey(key))
        {
            return _languages[lang][key];
        }
        else
        {
            var value = GetDescription(code);
            return value is null ? key : value;
        }
    }

    /// <summary>
    /// 从csv获取本地化语言，会取出Request.Headers.ContentLanguage中支持的第一个语言
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static string ToLocal(this ErrorCode code, HttpContext context)
    {
        var lang = context?.Request?.Headers?.ContentLanguage.ToString();
        lang = string.IsNullOrWhiteSpace(lang) ? "zh-CN" : lang.Split(';')[0];
        var key = Enum.GetName(typeof(ErrorCode), code);
        if (!string.IsNullOrEmpty(key) && _languages.ContainsKey(lang) && _localization.ContainsKey(key))
        {
            return _languages[lang][key];
        }
        else
        {
            var value = GetDescription(code);
            return value is null ? key : value;
        }
    }

    /// <summary>
    /// 扩展方法，获得枚举的Description
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <param name="nameInstead">当枚举值没有定义DescriptionAttribute，是否使用枚举名代替，默认是使用</param>
    /// <returns>枚举的Description</returns>
    private static string GetDescription(Enum value, bool nameInstead = true)
    {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name == null)
        {
            return null;
        }

        FieldInfo field = type.GetField(name);
        DescriptionAttribute attribute = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        if (attribute == null && nameInstead)
        {
            return name;
        }
        return attribute?.Description;
    }

    /// <summary>
    /// 获取时区id列表
    /// </summary>
    /// <returns></returns>
    public static List<string> GetTimeZones()
    {
        return TimeZoneInfo.GetSystemTimeZones().Select(x => x.Id).ToList();
    }

    /// <summary>
    /// 根据时区id获取本地时间
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static DateTime ToLocal(this DateTime time, string timezone = "China Standard Time")
    {
        var utc = TimeZoneInfo.ConvertTimeToUtc(time, TimeZoneInfo.Local);
        var local = TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById(timezone));
        return local;
    }
}

public class LocalLanguage
{
    public string Key { get; set; }
    public string Value { get; set; }
}
