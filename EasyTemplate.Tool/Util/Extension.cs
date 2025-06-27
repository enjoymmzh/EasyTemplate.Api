using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;

namespace EasyTemplate.Tool.Util;

/// <summary>
/// 拓展方法写这里
/// 这是一个静态类
/// </summary>
public static class Extension
{
    /// <summary>
    /// 时间戳转时间
    /// </summary>
    /// <param name="datetime"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(this long timestamp)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        var dateTime = dateTimeOffset.DateTime;
        return dateTime;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ToJson(this object data) => JsonConvert.SerializeObject(data);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T? ToEntity<T>(this string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static T Map<T>(this object source)
        => source.Map<T>();

    /// <summary>
    /// 扩展方法，获得枚举的Description
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <param name="nameInstead">当枚举值没有定义DescriptionAttribute，是否使用枚举名代替，默认是使用</param>
    /// <returns>枚举的Description</returns>
    public static string GetDescription(Enum value, Boolean nameInstead = true)
    {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name == null)
        {
            return null;
        }

        FieldInfo field = type.GetField(name);
        DescriptionAttribute attribute = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        if (attribute == null && nameInstead == true)
        {
            return name;
        }
        return attribute?.Description;
    }

}
