using ChatHubApi.Controllers.AdminServices.User.Model;
using ChatHubApi.System;
using ChatHubApi.System.Entity.Font;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace ChatHubApi.Controllers.AdminServices.User;
/// <summary>
/// 用户服务
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{

    private readonly ISqlSugarClient _db;
    private readonly ILogger<UserController> _logger;

    public UserController(ISqlSugarClient db, ILogger<UserController> logger)
    {
        _db = db;
        _logger = logger;
    }


    /// <summary>
    /// 增加用户       
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// 
    [HttpPost]
    public async Task<IActionResult> Add([FromBody]AddUserInput input)
    {
        var isExist = await _db.Queryable<sysFontUser>().SingleAsync(a => a.Username == input.Username);
        if (isExist != null)
            return Ok(new Response(3, null, "找不到此数据"));

        var user = input.Adapt<sysFontUser>();

        return Ok(new Response(_db.Insertable<sysFontUser>(user).ExecuteCommand() > 0 ? 1 : 2,null,"操作成功"));

    }


    /// <summary>
    /// 通过Username模糊查询
    /// version 2.0
    /// </summary>
    /// <param name="keyword">关键词</param>
    /// <returns></returns>
    /// 
    [HttpGet]
    //[Authorize(Policy = "SelfOnly")]
    public IActionResult QueryByUserName(string targetname)
    {
        var res = _db.Queryable<sysFontUser>().First(it => it.Username.Contains(targetname));
        if (res == null) return Ok(new Response(3, null, "没有找到此用户"));
        res.Password = "这是密码";
        return Ok(new Response(1, res, "获取成功"));
    }


    /// <summary>
    /// 批量删除（通过传入name数组来搜索）      
    /// </summary>
    /// <param name="dc">需要删除的姓名（用户名）数组</param>
    /// <returns></returns>
    /// 
    [HttpDelete]
    public Task DeletesByUserName([FromBody] dynamic dc)
    {


        JsonElement json = dc;
        List<string> objs = JsonSerializer.Deserialize<List<string>>(json.ToString());
        foreach (var item in objs)
        {
            _db.Deleteable<sysFontUser>().Where(a => a.Username == item).ExecuteCommand();
        }

        return Task.CompletedTask;

    }


    /// <summary>
    /// 编辑用户       
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// 
    [HttpPut]
    public async Task<ActionResult<int>> Update(EditUserInput input)
    {
        var isExist = await _db.Queryable<sysFontUser>().SingleAsync(a => a.Username == input.Username);
        if (isExist == null) return NotFound();

        var user = input.Adapt<sysFontUser>();

        return _db.Updateable(user).WhereColumns(it => new { it.Username, }).ExecuteCommand();

    }




    /// <summary>
    /// 删除一个用户       
    /// </summary>
    /// <param name="name">删除的用户名</param>
    /// <returns></returns>
    /// 
    [HttpDelete]
    public async Task<ActionResult<int>> Delete(string name)
    {
        var isExist = await _db.Queryable<sysFontUser>().SingleAsync(a => a.Username == name);
        if (isExist == null) return NotFound();
        return _db.Deleteable<sysFontUser>().Where(a => a.Username == name).ExecuteCommand();

    }


    /// <summary>
    /// 获取所有用户 
    /// version 2.0
    /// </summary>
    /// <param name="name">用户名</param>
    /// <returns></returns>
    /// 
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SearchModel? md)
    {
        if(md != null)
        {
            var query = _db.Queryable<sysFontUser>()
            .WhereIF(!string.IsNullOrEmpty(md.username), it => it.Username.Contains(md.username))
            .WhereIF(!string.IsNullOrEmpty(md.phone), it => it.Phone.Contains(md.phone));
            var users = await query.ToListAsync();

            return Ok(new Response(1, users, "搜索成功"));
        }


        var res = _db.Queryable<sysFontUser>().ToList();
        return Ok(new Response(1, res, "获取成功"));
    }


    /// <summary>
    /// 更新头像       
    /// </summary>
    /// <returns></returns>
    /// 
    [HttpPost]
    public async Task<ActionResult<int>> HeaderImage(string ImgUrl, string Username)
    {
        var isExist = await _db.Queryable<sysFontUser>().SingleAsync(a => a.Username == Username);
        if (isExist == null) return NotFound();

        isExist.HeaderImg = ImgUrl;

        /**
         *此操作不仅需要更改此用户表的头像数据，同时需要更改：
         *关联好友表的头像数据
         *Msgbox items中的头像数据
         */
        //关联好友表
        var friendsRes = _db.Queryable<sysFriends>().Where(a => a.FriendName == Username).ToList();
        foreach (var friend in friendsRes)
        {
            friend.FriendImg = ImgUrl;
        }
        _db.Updateable(friendsRes).ExecuteCommand();

        //MsgBox items
        var MsgBoxItems = _db.Queryable<sysMsgBox>().Where(a => a.targetfont == Username).ToList();
        foreach (var item in MsgBoxItems)
        {
            item.targetImage = ImgUrl;
        }
        _db.Updateable(MsgBoxItems).ExecuteCommand();



        return _db.Updateable(isExist).WhereColumns(it => new { it.Username, }).ExecuteCommand();

    }


    /// <summary>
    /// 返回一个用户的所有数据       
    /// </summary>
    /// <param name="name">用户名</param>
    /// <returns></returns>
    /// 
    [HttpPost]
    public async Task<ActionResult<string>> Get(string name)
    {
        var res = await _db.Queryable<sysFontUser>().SingleAsync(a => a.Username == name);
        if (res == null)
        {
            return NotFound();
        }
        string json = JsonSerializer.Serialize(res, new JsonSerializerOptions()
        {
            // 整齐打印
            WriteIndented = true,
            //重新编码，解决中文乱码问题
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        });

        return json;
    }



    /// <summary>
    /// 更新用户详细数据       
    /// </summary>
    /// <returns></returns>
    /// 
    [HttpPut]
    public async Task<ActionResult<int>> AdminUpdate(UserInput user)
    {
        var isExist = await _db.Queryable<sysFontUser>().SingleAsync(a => a.Username == user.Username);
        if (isExist == null) return NotFound();
        sysFontUser res = user.Adapt<sysFontUser>();
        return _db.Updateable(res).WhereColumns(it => new { it.Username, }).ExecuteCommand();

    }

}




