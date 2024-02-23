using ChatHubApi.Authorization;
using ChatHubApi.Middleware;
using ChatHubApi.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

//Serilog集成
var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//提供对当前 HttpContext的访问
builder.Services.AddHttpContextAccessor();



//signalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
}
);

//jwt工具
builder.Services.AddScoped<JwtSecurityTokenHandler, JwtSecurityTokenHandler>();
//跨域配置
string[] urls = new[] { builder.Configuration["Cors"], "http://localhost:5173", "https://localhost:5001/","*" };
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder => builder.WithOrigins(urls).AllowAnyHeader().AllowAnyMethod());
});
var app = builder.Build();
app.UseExceptionHandling();
// http管道配置
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<construct.Web.Entry.MyHub>("/MyHub");

app.Use(async (context, next) =>
{
    Console.WriteLine("aaa");
    await next.Invoke();
});
app.Run();
