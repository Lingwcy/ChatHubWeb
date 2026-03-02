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
//sqlsugar����
builder.Services.AddSqlsugar(builder.Configuration);
//��Ȩ��������
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
             OnMessageReceived�¼�����������JwtBearer�м����ʼ��������֮ǰִ�У�����������ض������Զ������ƵĻ�ȡ��ʽ��
            ����ڴ����Ǳ�׼��ʽ�����ƴ��ݷ�ʽ�����ѯ�ַ������Զ���HTTPͷ���ǳ�����
            �˴����ã��ڻ�ȡ��HUB���������ǰ��TOKENд��context���Ա�HUB��claims�����Զ�����
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
//�ִ�
builder.Services.AddRepository(builder.Configuration);

//Serilog����
var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//�ṩ�Ե�ǰ HttpContext�ķ���
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

//jwt����
builder.Services.AddScoped<JwtSecurityTokenHandler, JwtSecurityTokenHandler>();
builder.Services.AddScoped<IGroupService, GroupService>();
// CORS 配置 - 移除 "*" 允许所有来源，使用环境变量配置
var corsConfig = builder.Configuration["Cors"];
var allowedOrigins = new List<string>();
if (!string.IsNullOrEmpty(corsConfig))
{
    allowedOrigins.Add(corsConfig);
}
// 添加开发环境常用端口
allowedOrigins.AddRange(new[] { "http://localhost:5173", "http://localhost:3000", "http://localhost:8848" });
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder => builder.WithOrigins(allowedOrigins.ToArray()).AllowAnyHeader().AllowAnyMethod());
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
// http�ܵ�����
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
