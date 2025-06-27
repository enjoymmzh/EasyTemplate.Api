using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Connections;
using EasyTemplate.Tool.Util;
using EasyTemplate.Service.Common;
using EasyTemplate.Entry.Filter;
using EasyTemplate.Tool;

namespace EasyTemplate.Entry.Common;

public static class TheRegister
{
    /// <summary>
    /// 注册服务
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder? RegistService(this WebApplicationBuilder? builder)
    {
        var configuration = builder.Configuration;

        builder.Services.AddConfiguration();

        builder.Services.AddControllers()
            .AddNewtonsoftJson();//不加该注册，api传参易报错

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        //注入
        builder.Services.AddCustomInjection();

        if (Setting.Get<bool>("component:swagger"))
        {
            builder.Services.AddSwaggerGen(options =>
            {
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //获取枚举值上的特性
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    options.SwaggerDoc(f.Name, new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = info?.Title,
                        Version = info?.Version,
                        Description = info?.Description
                    });
                });
                options.SwaggerDoc("all", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "全部"
                });
                options.DocInclusionPredicate((docName, apiDescription) =>
                {
                    if (docName != "all")
                    {
                        return apiDescription.GroupName == docName;
                    }
                    return true;
                });
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    Description = "请输入正确的Token格式：Bearer token",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //options.DocInclusionPredicate((docName, description) => true);
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });
                var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                var xmlFile = System.AppDomain.CurrentDomain.FriendlyName + ".xml";
                var xmlPath = Path.Combine(baseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        builder.Services.AddMvc(option =>
        {
            option.Filters.Add(typeof(GlobalExceptionFilter));
            //option.Filters.Add(typeof(GlobalActionFilter));
            option.Filters.Add(typeof(AuthorizeFilter));
            option.Filters.Add(typeof(ResultFilter));
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddLocalLog();

        builder.Services.AddRedis();

        builder.Services.AddSqlSugar();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Policy", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = false;
        });

        if (Setting.Get<bool>("component:dynamicController"))
            builder.Services.AddDynamicController<ServiceLocalSelectController, ServiceActionRouteFactory>();

        if (Setting.Get<bool>("component:rabbitmq"))
            builder.Services.AddRabbitMq();

        if (Setting.Get<bool>("component:signalr"))
            builder.Services.AddSignalR();

        return builder;
    }

    /// <summary>
    /// 注册app
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplicationBuilder? RegistApp(this WebApplicationBuilder? builder)
    {
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (Setting.Get<bool>("component:swagger"))
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                //遍历ApiGroupNames所有枚举值生成接口文档，Skip(1)是因为Enum第一个FieldInfo是内置的一个Int值
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //获取枚举值上的特性
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    options.SwaggerEndpoint($"/swagger/{f.Name}/swagger.json", info != null ? info.Title : f.Name);

                });
                options.SwaggerEndpoint("/swagger/all/swagger.json", "全部");
            });
        }

        //暂时不用中间件
        //app.UseMiddleware<RequestMiddleware>();
        //app.UseMiddleware<RequestLocalizationMiddleware>();

        app.UseStaticFiles(new StaticFileOptions()
        {
            ContentTypeProvider = GetFileExtensionContentTypeProvider(),
            ServeUnknownFileTypes = true,
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/resource")),//相当于真实目录
            OnPrepareResponse = (c) =>
            {
                c.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            },
            RequestPath = new PathString("/src") //src相当于别名，为了安全
        });
        app.UseSession();
        app.UseCors("Policy");

        if (Setting.Get<bool>("component:dynamicController"))
            app.UseDynamicWebApi();

        if (Setting.Get<bool>("component:signalr"))
            app.MapHub<SignalRHub>("/thehub", options =>{
                options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();

        app.MapControllers();

        app.Run();
        return builder;
    }

    private static FileExtensionContentTypeProvider GetFileExtensionContentTypeProvider()
    {
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".iec"] = "application/octet-stream";
        provider.Mappings[".patch"] = "application/octet-stream";
        provider.Mappings[".apk"] = "application/vnd.android.package-archive";
        provider.Mappings[".pem"] = "application/x-x509-user-cert";
        provider.Mappings[".gzip"] = "application/x-gzip";
        provider.Mappings[".7zip"] = "application/zip";
        provider.Mappings[".jpg2"] = "image/jp2";
        provider.Mappings[".et"] = "application/kset";
        provider.Mappings[".dps"] = "application/ksdps";
        provider.Mappings[".cdr"] = "application/x-coreldraw";
        provider.Mappings[".shtml"] = "text/html";
        provider.Mappings[".php"] = "application/x-httpd-php";
        provider.Mappings[".php3"] = "application/x-httpd-php";
        provider.Mappings[".php4"] = "application/x-httpd-php";
        provider.Mappings[".phtml"] = "application/x-httpd-php";
        provider.Mappings[".pcd"] = "image/x-photo-cd";
        provider.Mappings[".bcmap"] = "application/octet-stream";
        provider.Mappings[".properties"] = "application/octet-stream";
        provider.Mappings[".m3u8"] = "application/x-mpegURL";
        return provider;
    }
}

internal class ServiceLocalSelectController : ISelectController
{
    public bool IsController(Type type)
    {
        return type.IsPublic && type.GetCustomAttribute<DynamicControllerAttribute>() != null;
    }
}

internal class ServiceActionRouteFactory : IActionRouteFactory
{
    public string CreateActionRouteModel(string areaName, string controllerName, ActionModel action)
    {
        var controllerType = action.ActionMethod.DeclaringType;
        var serviceAttribute = controllerType.GetCustomAttribute<DynamicControllerAttribute>();

        var _controllerName = serviceAttribute.ServiceName == string.Empty ? controllerName.Replace("Service", "") : serviceAttribute.ServiceName.Replace("Service", "");

        return $"api/{_controllerName.Replace("Service", "")}/{action.ActionName.Replace("Async", "")}";
    }
}
