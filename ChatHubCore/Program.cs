using ChatHubApi;
using ChatHubApi.Authorization;
using ChatHubApi.Hub;
using ChatHubApi.Middleware;
using ChatHubApi.Repository;
using ChatHubApi.Services;
using ChatHubApi.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var scheme = new OpenApiSecurityScheme()
    {
        Description = "Authorization header. \r\nExample: 'Bearer 12345abcdef'",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Authorization"
        },
        Scheme = "oauth2",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    };
    c.AddSecurityDefinition("Authorization", scheme);
    var requirement = new OpenApiSecurityRequirement();
    requirement[scheme] = new List<string>();
    c.AddSecurityRequirement(requirement);
});
//sqlsugar服务
builder.Services.AddSqlsugar(builder.Configuration);
//鉴权服务配置
builder.Services.AddAuthentication(
options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}
    ).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("SecretKey") ?? "")),
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero,
        };
        options.Events = new JwtBearerEvents
        {
            /*
             OnMessageReceived事件处理程序在JwtBearer中间件开始处理请求之前执行，允许你根据特定条件自定义令牌的获取方式。
            这对于处理非标准格式的令牌传递方式（如查询字符串或自定义HTTP头）非常有用
            此处作用：在获取到HUB的请求后提前将TOKEN写入context，以便HUB对claims进行自动解析
             */
            OnMessageReceived = context =>
            {

                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/MyHub")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminOnly", o =>
    {
        o.RequireClaim("Admin");
    });
    o.AddPolicy("SelfOnly", o =>
    {
        o.Requirements.Add(new SelfCertificationRequirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, SelfCertificationRequirementHandler>();
//仓储
builder.Services.AddRepository(builder.Configuration);

//Serilog集成
var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//提供对当前 HttpContext的访问
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = 100 * 1024 * 1024; // 100 MB
    options.KeyLengthLimit = 100 * 1024 * 1024; // 100 MB
    options.MultipartHeadersCountLimit = 100 * 1024 * 1024; // 100 MB
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
});

//signalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
}
);

//jwt工具
builder.Services.AddScoped<JwtSecurityTokenHandler, JwtSecurityTokenHandler>();
builder.Services.AddScoped<IGroupService, GroupService>();
//跨域配置
string[] urls = new[] { builder.Configuration["Cors"], "http://localhost:5173", "https://localhost:5001/", "http://localhost:8848", "http://100.83.131.91:8080", "*" };
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder => builder.WithOrigins(urls).AllowAnyHeader().AllowAnyMethod());
});
var app = builder.Build();
app.Use(next => new RequestDelegate(
    async context =>
    {
        context.Request.EnableBuffering();
        await next(context);
    }));
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
// http管道配置
app.Use(async (context, next) =>
{
    await next.Invoke();
});
app.UseExceptionHandling();
app.UseHttpsRedirection();
app.UseCors();
//app.UseCrypto();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<MyHub>("/MyHub");
app.Run();
