using System.ComponentModel;

namespace EasyTemplate.Tool;

public class PublicEnum
{
    /// <summary>
    /// 通用状态枚举
    /// </summary>
    [Description("通用状态枚举")]
    public enum StatusEnum
    {
        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        Disable = 0,

        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        Enable = 1,
    }

    /// <summary>
    /// 广告素材类型枚举
    /// </summary>
    [Description("广告素材类型枚举")]
    public enum AdMaterialTypeEnum
    {
        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        Picture = 0,

        /// <summary>
        /// 视频
        /// </summary>
        [Description("视频")]
        Video = 1,
    }

    /// <summary>
    /// 密码加密枚举
    /// </summary>
    [Description("密码加密枚举")]
    public enum CryptogramEnum
    {
        /// <summary>
        /// MD5
        /// </summary>
        [Description("MD5")]
        MD5 = 0,

        /// <summary>
        /// SM2（国密）
        /// </summary>
        [Description("SM2")]
        SM2 = 1,

        /// <summary>
        /// SM4（国密）
        /// </summary>
        [Description("SM4")]
        SM4 = 2,

        /// <summary>
        /// DESC
        /// </summary>
        [Description("DESC")]
        DESC = 3,

        /// <summary>
        /// AES
        /// </summary>
        [Description("AES")]
        AES = 4,

        /// <summary>
        /// RSA
        /// </summary>
        [Description("RSA")]
        RSA = 5,

    }

    /// <summary>
    /// 菜单类型枚举
    /// </summary>
    [Description("菜单类型枚举")]
    public enum MenuTypeEnum
    {
        /// <summary>
        /// 目录
        /// </summary>
        [Description("目录")]
        Catalog = 0,

        /// <summary>
        /// 页面
        /// </summary>
        [Description("页面")]
        Page = 1,
        /// <summary>
        /// 按钮
        /// </summary>
        [Description("按钮")]
        Button = 2,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 3,
    }

    /// <summary>
    /// 系统错误码
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// 用戶名或密碼不正確
        /// </summary>
        [Description("用戶名或密碼不正確")]
        E1000,
        /// <summary>
        /// 没有启用队列
        /// </summary>
        [Description("没有启用队列")]
        E1001,
    }
}
