# EasyTemplate.Api 简易Api模板

#### 介绍

简易Api模板，包含基本JWT权限功能，无需配置，上手可用
1.  能够使用动态controller，也能使用默认的controller
2.  尽可能减少封装
3.  数据表通过实体动态生成

#### 软件架构

.Net8 + Sqlsugar

#### 文件结构说明

1.  EasyTemplate.Entry：项目主入口，需要设置为启动项目
2.  EasyTemplate.Service：所有的接口写在这里，文件名以Service结束
3.  EasyTemplate.Tool：实体 + 工具类
4.  卷影复制配置在 EasyTemplate.Blazor.Web/web.config/handlerSetting 节点中，解决了发布时占用问题，但硬盘会因此被占用，介意的可以将该节点删除，或是配置enableShadowCopy为false

#### 如何新建一个接口？

例如：当前要写用户管理功能
1.  EasyTemplate.Service项目中，新建UserService.cs
2.  在public class UserService上必须使用[DynamicController]（动态控制器）[ApiGroup(ApiGroupNames.Auth)]（swagger分组）标签声明
3.  类中使用private readonly SqlSugarRepository<SystemUser> _user;声明sql连接，并通过构造函数注入
    private readonly SqlSugarRepository<SystemUser> _user;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(
        IHttpContextAccessor contextAccessor,
        SqlSugarRepository<SystemUser> user
        )
    {
        _user = user;
        _contextAccessor = contextAccessor;
    }
4.按照一般api项目中的方式编写接口即可
5.启动项目时，会根据Service后缀自动生成控制器，并将接口显示在swagger文档中
