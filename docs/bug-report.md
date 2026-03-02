# ChatHubWeb Bug 报告

## Task 1: 后端 Core - 异常处理审查

### 发现的问题

#### 问题 1: 异常处理中间件未区分异常类型
- **文件**: `ChatHubCore/Middleware/ExceptionHandlingMiddleware.cs`
- **严重程度**: 高
- **描述**: 当前的异常处理中间件将所有异常统一处理，不区分业务异常（如业务逻辑错误）和系统异常（如数据库连接失败、NullReferenceException）。所有异常都返回 HTTP 500 状态码和 `"system_error"` 错误码，这会导致客户端无法准确判断错误类型。
- **建议修复**:
  1. 创建自定义业务异常类（如 `BusinessException`），继承自 `Exception`
  2. 在 `HandleException` 方法中判断异常类型：
     - 如果是业务异常，返回适当的 HTTP 状态码（如 400 Bad Request）
     - 如果是系统异常，返回 500 Internal Server Error

#### 问题 2: 敏感信息泄露风险
- **文件**: `ChatHubCore/Middleware/ExceptionHandlingMiddleware.cs`
- **严重程度**: 高
- **描述**:
  1. 第 33 行使用 `ex.ToString()` 记录日志，会输出完整的堆栈信息，可能包含服务器路径、数据库连接字符串等敏感信息
  2. 第 35 行直接返回 `ex.Message`，可能暴露内部系统信息（如表名、列名、SQL 语句等）
- **建议修复**:
  1. 日志记录使用 `ex.Message` 而不是 `ex.ToString()`，或者在生产环境使用简化的日志格式
  2. 对外返回的错误消息应该是通用的，如 "服务器内部错误"，而不是原始异常消息

#### 问题 3: Controller 缺少全局异常处理（部分方法）
- **文件**: `ChatHubCore/Controllers/Font/Login/AuthController.cs`
- **严重程度**: 中
- **描述**: `AuthController.login` 方法直接调用数据库查询，没有 try-catch 保护。如果数据库异常或用户不存在导致异常，异常会直接传播到全局中间件，返回 500 错误而不是友好的业务错误消息。
- **建议修复**: 在 `login` 方法中添加 try-catch 块，处理可能的异常情况

#### 问题 4: 局部异常处理返回敏感信息
- **文件**: `ChatHubCore/Controllers/Font/Login/AuthController.cs`, `ChatHubCore/Controllers/Admin/User/UserController.cs`
- **严重程度**: 中
- **描述**: 在 catch 块中直接返回 `ex.Message`，这可能会暴露数据库错误、堆栈跟踪等敏感信息给客户端。
```csharp
catch(Exception ex)
{
    return Ok(new Response(
        code: 2,
        data: null,
        message: "系统异常" + ex.Message));  // 暴露敏感信息
}
```
- **建议修复**:
```csharp
catch(Exception ex)
{
    _logger.LogError(ex, "注册用户时发生错误");
    return Ok(new Response(
        code: 2,
        data: null,
        message: "系统异常，请稍后重试"));
}
```

#### 问题 5: 使用 Console.WriteLine 而非proper日志记录
- **文件**: `ChatHubCore/Controllers/Font/Friends/FriendsController.cs`
- **严重程度**: 低
- **描述**: 在 `FriendsController` 的 catch 块中使用 `Console.WriteLine(ex.Message)` 输出错误，而不是使用依赖注入的日志服务。这会导致错误信息无法进入日志系统，难以排查生产环境问题。
- **建议修复**: 使用 `_logger.LogError(ex, "错误描述")` 替代 `Console.WriteLine`

#### 问题 6: SignalR Hub 异常处理不足
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 中
- **描述**:
  1. `SendPrivateMsg` 方法没有 try-catch，如果发生异常会直接断开连接
  2. `SendGroupMsg` 方法有异常处理但只是记录日志后直接返回
  3. `SendImage` 方法没有任何异常处理，可能导致未处理的异常
- **建议修复**: 在 SignalR Hub 方法中添加适当的异常处理，向客户端返回友好的错误消息

#### 问题 7: 统一响应格式不一致
- **文件**: 多个 Controller
- **严重程度**: 中
- **描述**: 项目中响应格式不统一：
  - 有的返回 `Ok(new Response(code, data, message))`
  - 有的返回 `NotFound()`
  - 有的返回 `Ok(new Response(3, null, "错误信息"))` 表示错误
  - 有的返回 `Ok(new Response(2, ...))` 表示业务错误
- **建议修复**:
  1. 统一错误响应格式（定义标准的错误码和错误消息）
  2. 建议业务错误也通过 Response 对象返回，而不是使用 HTTP 状态码（如 NotFound）

#### 问题 8: 开发环境异常页面配置位置不当
- **文件**: `ChatHubCore/Program.cs`
- **严重程度**: 低
- **描述**: `UseDeveloperExceptionPage()` 在 `UseExceptionHandling()` 之后调用，这意味着在开发环境中，全局的异常处理中间件会先捕获异常，可能导致开发者异常页面无法显示详细信息。
- **建议修复**: 将 `UseDeveloperExceptionPage()` 移到 `UseExceptionHandling()` 之前，或者确保开发环境的异常处理不被全局中间件拦截

#### 问题 9: CORS 配置允许所有来源
- **文件**: `ChatHubCore/Program.cs`
- **严重程度**: 中
- **描述**: CORS 配置中包含 `"*"`（允许所有来源），这是一个安全风险
- **建议修复**: 移除 `"*"` 配置，只允许受信任的前端域名

---

## Task 2: 后端 Core - 安全风险审查

### 发现的问题

#### 问题 1: 硬编码加密密钥和 IV
- **文件**: `ChatHubCore/appsettings.json`, `ChatHubCore/appsettings.Development.json`
- **严重程度**: 高
- **描述**:
  - AES 密钥 `ASEKey` 和 IV `ASEIV` 在配置文件中硬编码为 `4555794842396b484a696b4b64413865`
  - 密钥和 IV 完全相同，这是严重的安全问题
  - 密钥以明文形式存储在配置文件中
- **建议修复**:
  1. 使用环境变量或密钥管理服务存储密钥
  2. 密钥和 IV 应该不同
  3. 在生产环境中使用安全的密钥存储机制（如 Azure Key Vault、AWS KMS 等）

#### 问题 2: 密码哈希算法不安全
- **文件**: `ChatHubCore/Untils/Crypto.cs`
- **严重程度**: 高
- **描述**: 使用 SHA256 进行密码哈希，而不是更安全的 bcrypt 或 argon2。SHA256 容易受到彩虹表攻击和 GPU 暴力破解。
- **建议修复**: 使用 bcrypt、argon2 或 PBKDF2 等专门的密码哈希算法

#### 问题 3: 数据库明文密码
- **文件**: `ChatHubCore/appsettings.json`, `ChatHubCore/appsettings.Development.json`
- **严重程度**: 高
- **描述**: 数据库连接字符串包含明文密码 `password=1444707`
- **建议修复**:
  1. 使用环境变量存储数据库密码
  2. 使用 SQL Server 的集成身份验证或其他安全认证方式

#### 问题 4: JWT 密钥过弱
- **文件**: `ChatHubCore/appsettings.json`, `ChatHubCore/appsettings.Development.json`
- **严重程度**: 高
- **描述**: `SecretKey` 设置为 `kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk`，这是一个非常弱的密钥，只是重复的 'k' 字符
- **建议修复**: 使用至少 256 位（32 字节）的随机密钥，并存储在安全的位置

#### 问题 5: Admin 登录使用明文密码比较
- **文件**: `ChatHubCore/Controllers/Admin/Login/AdminAuthController.cs`
- **严重程度**: 高
- **描述**: 管理员密码以明文形式存储和比较，没有进行哈希处理。
```csharp
var res = await _db.Queryable<sysAdmin>().FirstAsync(it => it.name == loginInput.account && it.psw == loginInput.psw);
```
- **建议修复**:
  1. 对密码进行哈希处理后再存储
  2. 登录时对输入的密码进行哈希后与数据库中的哈希值比较

#### 问题 6: CryptoMiddleware 中使用相同密钥作为 Key 和 IV
- **文件**: `ChatHubCore/Middleware/CryptoMiddleware.cs`
- **严重程度**: 高
- **描述**: `Crypto.DecryptByAES(encryptedRequestBody, client.Key, client.Key)` - 使用相同的值作为 Key 和 IV，这降低了加密安全性
- **建议修复**: 分别为 Key 和 IV 使用不同的值

#### 问题 7: 登录响应返回密码哈希
- **文件**: `ChatHubCore/Controllers/Font/Login/AuthController.cs`
- **严重程度**: 中
- **描述**: 登录和注册响应中包含密码哈希值 `userPsw: user.Password`，这会暴露给客户端
- **建议修复**: 从响应中移除密码字段

#### 问题 8: JWT Token 过期时间过长
- **文件**: `ChatHubCore/Controllers/Font/Login/AuthController.cs`
- **严重程度**: 中
- **描述**: Token 过期时间设置为 30000 分钟（约 500 小时），时间过长会增加令牌被盗用的风险
- **建议修复**: 将过期时间缩短到合理范围（如 1-24 小时）

#### 问题 9: CORS 配置允许所有来源（重复，见 Task 1 问题 9）
- **文件**: `ChatHubCore/Program.cs`
- **严重程度**: 中
- **描述**: CORS 配置包含 `"*"` 允许所有来源
- **建议修复**: 移除 `"*"` 配置，只允许受信任的前端域名

#### 问题 10: 敏感数据查询缺少权限控制
- **文件**: `ChatHubCore/Controllers/Admin/User/UserController.cs`
- **严重程度**: 中
- **描述**: `QueryByUserName` 方法没有授权注解，任何人都可以查询用户信息
- **建议修复**: 添加 `[Authorize]` 属性确保只有已认证用户可以访问

#### 问题 11: SQL 查询使用 Contains 可能存在注入风险
- **文件**: `ChatHubCore/Controllers/Admin/User/UserController.cs`
- **严重程度**: 低
- **描述**: 使用 `.Contains()` 进行模糊查询，虽然 SqlSugar 有参数化处理，但应验证输入
- **建议修复**: 对输入进行验证和清理，确保只有预期的字符

---

## Task 3: 后端 Core - JWT 认证审查

### 发现的问题

#### 问题 1: 管理员登录返回错误的 HTTP 状态码
- **文件**: `ChatHubCore/Controllers/Admin/Login/AdminAuthController.cs`
- **严重程度**: 高
- **描述**:
  - 无论登录成功与否都返回 `NotFound()`，登录成功时返回的 token 根本没有发送给客户端
  - 管理员密码使用明文比较，没有进行哈希处理
- **建议修复**:
```csharp
// 修复：使用密码哈希比较
var res = await _db.Queryable<sysAdmin>().FirstAsync(it => it.name == loginInput.account && it.psw == Crypto.HashPassword(loginInput.psw));

// 修复：返回 token
return Ok(new { token = output });
```

#### 问题 2: 登录/注册响应泄露密码哈希
- **文件**: `ChatHubCore/Controllers/Font/Login/AuthController.cs`
- **严重程度**: 高
- **描述**:
  - 登录响应返回 `userPsw: user.Password`
  - 注册响应也返回密码哈希
- **建议修复**: 从响应对象中移除 `userPsw` 字段

#### 问题 3: JWT Token 过期时间过长
- **文件**: `ChatHubCore/Controllers/Font/Login/AuthController.cs`
- **严重程度**: 高
- **描述**:
  - Token 过期时间设置为 30000 分钟（约 20.8 天）
  - 注册时是 300 分钟（5小时）
- **建议修复**: 前台用户 Token 过期时间建议设置为 60-120 分钟

#### 问题 4: Admin 策略 Claim 不匹配
- **文件**: `ChatHubCore/Program.cs` 和 `ChatHubCore/Controllers/Admin/Login/AdminAuthController.cs`
- **严重程度**: 高
- **描述**:
  - Program.cs 策略要求 `RequireClaim("Admin")`
  - 但 AdminAuthController 生成的是 `new Claim("Role","admin")` (小写)
  - 这导致 AdminOnly 策略永远不会生效
- **建议修复**: 统一使用 "Admin" 或 "admin"，确保策略和生成代码一致

#### 问题 5: 缺少 Token 刷新机制
- **文件**: `ChatHubCore/Services/jwtService.cs`
- **严重程度**: 中
- **描述**: 没有实现 refresh token 机制，用户需要重新登录才能获取新 Token
- **建议修复**: 实现 refresh token 端点，支持用 refresh token 换取新的 access token

#### 问题 6: 缺少 Token 黑名单/注销机制
- **文件**: `ChatHubCore/`
- **严重程度**: 中
- **描述**: 用户 logout 后，之前签发的 Token 仍然有效，直到自然过期
- **建议修复**: 实现 Token 黑名单机制（可以存储在内存缓存或数据库中）

#### 问题 7: JWT 未验证 Issuer 和 Audience
- **文件**: `ChatHubCore/Program.cs`
- **严重程度**: 中
- **描述**: `ValidateAudience = false, ValidateIssuer = false`，没有验证 JWT 的发行者和受众
- **建议修复**: 设置具体的 Issuer 和 Audience 并进行验证

#### 问题 8: JWT 密钥强度不足
- **文件**: `ChatHubCore/appsettings.json`
- **严重程度**: 高
- **描述**: `SecretKey` 为 `kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk`，仅由重复字符 'k' 组成
- **建议修复**: 使用至少 32 字节的随机密钥，使用 base64 或十六进制编码

#### 问题 9: CustomAuthorizeAttribute 为空实现
- **文件**: `ChatHubCore/System/Attribuite/CustomAuthorizeAttribute.cs`
- **严重程度**: 低
- **描述**: 该类仅继承 `AuthorizeAttribute` 并无任何自定义逻辑，目前没有任何作用
- **建议修复**: 如果不需要可以删除此文件，或者实现自定义授权逻辑

#### 问题 10: jwtService 中有多余的调试输出
- **文件**: `ChatHubCore/Services/jwtService.cs`
- **严重程度**: 低
- **描述**: 生成 Token 后又读取并用 Console.WriteLine 打印所有 claims，这些调试代码应该移除
- **建议修复**: 删除调试代码

---

## Task 4: 后端 Core - SignalR 连接管理审查

### 发现的问题

#### 问题 1: OnConnectedAsync 缺少 Claims 空值检查
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 高
- **描述**: 在获取 JWT Claims 时没有进行空值检查。如果 JWT 中缺少 `UserName` 或 `UserId` Claim，会抛出 `NullReferenceException` 导致连接失败。
- **建议修复**: 添加空值检查和异常处理

#### 问题 2: OnDisconnectedAsync 缺少 Claims 空值检查
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 高
- **描述**: 当连接异常断开时 Context.User 可能为 null，同样会抛出异常。
- **建议修复**: 添加空值检查

#### 问题 3: OnConnectedAsync 中 int.Parse(id) 可能抛异常
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 高
- **描述**: `int.Parse(id)` 没有 try-catch 保护，如果 id 为 null 或格式不正确会抛出异常。
- **建议修复**: 使用 `int.TryParse` 进行安全转换

#### 问题 4: 缺少 SignalR 连接超时配置
- **文件**: `ChatHubCore/Program.cs`
- **严重程度**: 中
- **描述**: SignalR 配置中缺少 `HandshakeTimeout`、`ServerTimeout` 和 `KeepAliveInterval` 的设置。
- **建议修复**: 添加超时配置

#### 问题 5: SendHubKey 存在严重安全漏洞
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 高
- **描述**: 允许客户端主动推送加密密钥，这是严重的安全漏洞。攻击者可以伪造任意密钥进行中间人攻击。密钥应该由服务器端在连接时自动生成。
- **建议修复**: 删除客户端推送密钥的方法，改为服务器端自动生成

#### 问题 6: SendPrivateMsg 缺少 null 检查
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 高
- **描述**: 查询可能返回 null，直接访问 `.key` 会抛出异常。
- **建议修复**: 添加 null 检查

#### 问题 7: SendGroupMsg 缺少 null 检查
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 高
- **描述**: 同样存在查询返回 null 的问题。
- **建议修复**: 添加 null 检查

#### 问题 8: SendGroupMsg 异常时没有反馈给发送者
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 中
- **描述**: 捕获异常后只记录日志并直接返回，发送者不知道消息发送失败。
- **建议修复**: 向发送者返回错误通知

#### 问题 9: SendImage 方法实现不完整
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 高
- **描述**: 只保存图片到服务器，但没有通知接收者、没有返回图片 URL、没有异常处理。
- **建议修复**: 实现完整的图片发送流程

#### 问题 10: SendPublicImage 不返回结果
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 高
- **描述**: 发送图片后没有向发送者返回图片 URL，客户端无法获取已上传图片的访问地址。
- **建议修复**: 返回图片

#### 问题 11: 缺少 URL 给发送者在线用户过期清理机制
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 中
- **描述**: 如果客户端异常断开，数据库中的记录不会被删除，造成"幽灵用户"。
- **建议修复**: 添加后台定时任务定期清理过期记录

#### 问题 12: sysOnlineUser 缺少过期时间字段
- **文件**: `ChatHubCore/System/Entity/Font/sysOnlineUser.cs`
- **严重程度**: 中
- **描述**: 实体类没有过期时间字段，无法有效判断用户是否真正离线。
- **建议修复**: 添加 `expirationTime` 字段

#### 问题 13: 缺少心跳检测机制
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 中
- **描述**: 没有实现心跳检测机制，无法及时发现断开的连接。
- **建议修复**: 实现心跳机制

#### 问题 14: 私聊消息缺少确认机制
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 中
- **描述**: 发送消息后没有等待客户端确认，发送者不知道消息是否成功送达。
- **建议修复**: 添加消息确认回调机制

#### 问题 15: OnDisconnectedAsync 删除用户可能失败
- **文件**: `ChatHubCore/Hub/Hub.cs`
- **严重程度**: 中
- **描述**: 删除用户记录时，如果记录不存在或查询失败可能导致异常。
- **建议修复**: 处理可能的异常

## Task 5: 前端 ChatHubVue - API 错误处理审查

### 发现的问题

#### 问题 1: 错误响应拦截器中访问可能不存在的属性
- **文件**: `ChatHubVue/src/common/axiosSetting.ts`
- **严重程度**: 高
- **描述**: 第89行 `const res = error.response;` - 当网络错误（如超时）时，`error.response` 可能是 undefined，后续代码在访问 `res.status` 时会导致新的错误（Cannot read property 'status' of undefined）。
- **建议修复**: 在访问 `error.response` 之前添加空值检查：
```typescript
error => {
    if (error.message.indexOf('timeout') > -1) {
        error.message = '请求超时'
    }
    if (error.message.indexOf('Network') > -1) {
        error.message = '网络错误'
    }
    const res = error.response;
    if (!res) {
        // 网络错误时没有 response，直接抛出错误
        ElMessage.error(error.message || '网络错误，请稍后重试')
        return Promise.reject(error);
    }
    // ... 后续代码
}
```

#### 问题 2: 404 处理不一致
- **文件**: `ChatHubVue/src/common/axiosSetting.ts`
- **严重程度**: 中
- **描述**: 第60-70行处理404时返回 `res` 而不是拒绝 Promise，这可能导致后续代码继续执行而不是正确处理错误。
- **建议修复**: 将 `return res;` 改为 `return Promise.reject(res);`

#### 问题 3: 缺少请求重试机制
- **文件**: `ChatHubVue/src/common/axiosSetting.ts`
- **严重程度**: 中
- **描述**: 没有实现自动重试机制，网络不稳定时容易请求失败。
- **建议修复**: 使用 axios-retry 库或在请求拦截器中实现重试逻辑

#### 问题 4: FileService 完全没有错误处理
- **文件**: `ChatHubVue/src/services/FileService.ts`
- **严重程度**: 高
- **描述**: `Upload`, `UploadAvatar`, `UpdateUserInfo` 方法都只有 `.then()` 没有 `.catch()`，API 调用失败时会静默失败，用户不知道发生了什么。
- **建议修复**: 为所有方法添加错误处理：
```typescript
public async Upload(params:UploadParmas): Promise<[boolean,any]> {
    try {
        const res = await postUploadFile(params);
        if(res.data.code == 1){
            return [true,JSON.parse(res.data.data)];
        }
        ElMessage.error(res.data.message || '上传失败');
        return [false,null]
    } catch (error) {
        ElMessage.error('上传失败，请稍后重试');
        return [false,null]
    }
}
```

#### 问题 5: GroupService 多个方法缺少错误处理
- **文件**: `ChatHubVue/src/services/GroupService.ts`
- **严重程度**: 高
- **描述**: `SearchtGroup`, `GetGroupList`, `GetGroupMemberList`, `GetGroupRequestList` 等方法缺少 `.catch()` 处理，API 调用失败时会静默失败。
- **建议修复**: 为这些方法添加错误处理，例如：
```typescript
public async GetGroupList(id:number,name:string,GroupStore:any): Promise<boolean> {
    const playload = {
        userId:id,
        xusername:name
    }
    return await getGroupList(playload).then(res => {
        if(res.data.code == 1){
            GroupStore.MyGroups = JSON.parse(res.data.data);
            return true;
        }
        ElMessage.error(res.data.message || '获取群组列表失败');
        return false
    }).catch(error => {
        ElMessage.error('获取群组列表失败，请稍后重试');
        return false
    })
}
```

#### 问题 6: HubService SignalR 回调中的 API 调用缺乏错误处理
- **文件**: `ChatHubVue/src/services/HubService.ts`
- **严重程度**: 高
- **描述**: `ChatMethodInitial` 方法中多处 SignalR 回调里的 API 调用（如 `getMessageBox`, `getGroupList`, `getFriends`, `findFriendTree` 等）没有错误处理。当这些 API 调用失败时，会导致静默失败，用户体验不好。
- **建议修复**: 为所有回调中的 API 调用添加 `.catch()` 处理：
```typescript
this.HubConnection.on('MsgBoxFlasherReceived', () => {
    let payload = { username: this.UserInfoStore.userName, xusername: this.UserInfoStore.userName }
    getMessageBox(payload)
        .then(res => {
            if (res.data.code == 1) {
                this.MsgboxStore.$reset()
                const result = JSON.parse(res.data.data)
                for (let i = 0; i < result.length; i++) {
                    this.MsgboxStore.MsgItems.push(result[i])
                }
            }
        })
        .catch(error => {
            ElMessage.error('获取消息失败');
        })
});
```

#### 问题 7: AuthService 部分方法缺少错误处理
- **文件**: `ChatHubVue/src/services/AuthService.ts`
- **严重程度**: 中
- **描述**: `Verify()` 和 `SendAESKey()` 方法完全没有错误处理，API 调用失败时会静默失败。
- **建议修复**: 为这两个方法添加错误处理：
```typescript
public async Verify(): Promise<boolean> {
    return await getVerify().then(() => {
        return true;
    }).catch(error => {
        console.error('验证失败:', error);
        return false;
    })
}
```

#### 问题 8: FriendsService 部分方法缺少错误处理
- **文件**: `ChatHubVue/src/services/FriendsService.ts`
- **严重程度**: 中
- **描述**: `FindFriendTree` 方法没有 `.catch()` 处理。
- **建议修复**: 添加错误处理

#### 问题 9: main.ts 缺少全局 Vue 错误处理器
- **文件**: `ChatHubVue/src/main.ts`
- **严重程度**: 高
- **描述**: 没有配置 `app.config.errorHandler` 来捕获 Vue 组件中的未处理错误，也没有配置 `window.addEventListener('unhandledrejection')` 来处理未捕获的 Promise 拒绝。
- **建议修复**: 在 main.ts 中添加全局错误处理：
```typescript
// 全局 Vue 错误处理
app.config.errorHandler = (err, instance, info) => {
    console.error('Vue Error:', err);
    ElMessage.error('发生错误，请刷新页面重试');
};

// 全局 Promise rejection 处理
window.addEventListener('unhandledrejection', (event) => {
    console.error('Unhandled Promise Rejection:', event.reason);
    ElMessage.error('网络请求失败，请稍后重试');
});
```

#### 问题 10: 错误码处理不完整
- **文件**: `ChatHubVue/src/common/axiosSetting.ts`
- **严重程度**: 低
- **描述**: 响应拦截器只处理了 code 1,2,3,4,-3，没有处理其他可能的业务错误码（如 -1, -2, 5 等）。
- **建议修复**: 扩展错误码处理，或在响应拦截器中添加对未知错误码的默认处理

#### 问题 11: axios.ts 包装方法没有额外错误处理
- **文件**: `ChatHubVue/src/common/axios.ts`
- **严重程度**: 低
- **描述**: 通用的 HTTP 方法（get, post, delete, put）只是简单包装 http 调用，没有添加额外的错误处理逻辑。虽然错误会传递到调用者，但缺少统一的错误转换。
- **建议修复**: 可以考虑在包装方法中添加统一的错误转换或日志记录

---

### 总结

本次审查发现前端 API 错误处理存在以下主要问题：

1. **Axios 拦截器问题**：网络错误时可能访问 undefined 属性导致新错误；404 处理不一致
2. **Service 层普遍缺少错误处理**：多个 Service 文件中的方法没有 `.catch()` 处理，导致 API 调用失败时静默失败
3. **缺少全局错误处理**：Vue 应用没有配置全局错误处理器
4. **SignalR 回调中的错误处理缺失**：HubService 中的 SignalR 回调里调用 API 时没有错误处理

---

## Task 6: 前端 ChatHubVue - 状态管理审查

### 发现的问题

#### 问题 1: SignalR 事件监听器累积导致内存泄漏（严重）
- **文件**: `ChatHubVue/src/services/HubService.ts`
- **严重程度**: 高
- **描述**: `ChatMethodInitial()` 方法在每次连接时调用，通过 `this.HubConnection.on()` 注册了大量事件监听器，但从未调用 `.off()` 方法清理旧监听器。每次重连都会累积新的监听器，导致同一消息事件会被触发多次，内存占用持续增长。
- **建议修复**: 在重连或断开连接前，清理所有事件监听器

#### 问题 2: 重复的 onclose 事件监听器
- **文件**: `ChatHubVue/src/services/HubService.ts`
- **严重程度**: 中
- **描述**: `onclose` 事件监听器被注册了两次，会导致连接断开时显示两次"连接断开"通知。
- **建议修复**: 删除其中一个 onclose 注册

#### 问题 3: 重连逻辑无限制
- **文件**: `ChatHubVue/src/services/HubService.ts`
- **严重程度**: 中
- **描述**: 自动重连逻辑没有最大重试次数限制和退避策略。如果服务器长时间不可用，会不断尝试重连。
- **建议修复**: 添加重连次数限制和退避策略

#### 问题 4: UseServiceStore 持久化会导致运行时错误
- **文件**: `ChatHubVue/src/store/index.ts`
- **严重程度**: 高
- **描述**: `UseServiceStore` 启用了 localStorage 持久化，但它存储的是服务实例引用，包含 SignalR 连接对象、回调函数、循环引用，无法被正确序列化。
- **建议修复**: 移除 `UseServiceStore` 的持久化配置

#### 问题 5: UseUserInformationStore 的 connection 属性被持久化
- **文件**: `ChatHubVue/src/store/index.ts`
- **严重程度**: 中
- **描述**: SignalR 连接对象被包含在持久化中，尝试序列化会导致失败。
- **建议修复**: 从持久化中排除 connection 属性

#### 问题 6: UseMsgStore 消息持久化导致存储膨胀
- **文件**: `ChatHubVue/src/store/index.ts`
- **严重程度**: 低
- **描述**: 消息历史被持久化到 localStorage，长时间使用后会导致存储空间不足。
- **建议修复**: 限制持久化的消息数量或禁用消息持久化

#### 问题 7: Main.vue 中 ChatHub 实例管理不当
- **文件**: `ChatHubVue/src/views/MainContent/Main.vue`
- **严重程度**: 中
- **描述**: 当有旧的持久化数据时，可能创建多个 SignalR 连接同时存在。
- **建议修复**: 在创建新实例前，确保先停止并清理旧实例

#### 问题 8: SignalR 回调中缺乏错误处理
- **文件**: `ChatHubVue/src/services/HubService.ts`
- **严重程度**: 中
- **描述**: 多个 SignalR 事件回调中直接调用 API 方法，但没有添加 `.catch()` 错误处理。
- **建议修复**: 为所有异步 API 调用添加错误处理

#### 问题 9: 未使用的 watch 清理
- **文件**: `ChatHubVue/src/views/MainContent/Main.vue`
- **严重程度**: 低
- **描述**: 使用 `watch` 监听连接状态变化，但没有显式清理。
- **建议修复**: 使用 `watchEffect` 的返回值或 `onBeforeUnmount` 清理

建议优先修复高严重程度的问题，特别是问题 1、4、5、6、9，以提升应用的健壮性和用户体验。

---

## Task 7: 前端 ChatHubVue - 组件通信审查

### 发现的问题

#### 问题 1: MessageCT.vue 缺少 Props 定义和组件通信耦合
- **文件**: `ChatHubVue/src/views/MainContent/HubContent/MessageCT.vue`
- **严重程度**: 中
- **描述**: 该组件完全没有定义 props 和 emits，完全依赖全局 Pinia store 进行状态管理。这导致组件与全局状态高度耦合，难以测试和复用。
- **建议修复**: 考虑重构为接收 props 并通过 emits 抛出事件的组件

#### 问题 2: MessageCT.vue scrollDown 函数中 setTimeout 未清理
- **文件**: `ChatHubVue/src/views/MainContent/HubContent/MessageCT.vue`
- **严重程度**: 中
- **描述**: `scrollDown` 函数使用了 `setTimeout`，但组件中没有在 `onBeforeUnmount` 生命周期钩子中清理这些定时器。
- **建议修复**: 保存 timer ID 并在 onBeforeUnmount 中清理

#### 问题 3: MessageCT.vue watch 未显式清理
- **文件**: `ChatHubVue/src/views/MainContent/HubContent/MessageCT.vue`
- **严重程度**: 低
- **描述**: 定义了多个 `watch`，监听 store 中的状态变化，但没有显式清理。
- **建议修复**: 在 `onBeforeUnmount` 中调用 watch 的返回值进行清理

#### 问题 4: MessageCT.vue 消息列表渲染无虚拟滚动优化
- **文件**: `ChatHubVue/src/views/MainContent/HubContent/MessageCT.vue`
- **严重程度**: 中
- **描述**: 使用 `v-for` 直接渲染所有消息，当消息数量较多时会导致性能问题。
- **建议修复**: 使用虚拟滚动库或实现分页加载

#### 问题 5: Login.vue 递归 setTimeout 未清理
- **文件**: `ChatHubVue/src/views/LoginView/Login.vue`
- **严重程度**: 高
- **描述**: `setTimeout(doLoginCheck, 1000)` 使用递归方式循环调用，但组件卸载时没有清理这个定时器。
- **建议修复**: 保存 timer ID 并在 onBeforeUnmount 中清理

#### 问题 6: HubService 事件监听器无清理机制
- **文件**: `ChatHubVue/src/services/HubService.ts`
- **严重程度**: 高
- **描述**: 通过 `this.HubConnection.on()` 注册了大量事件监听器，但没有提供清理方法。每次重连都会累积新的监听器。
- **建议修复**: 在连接断开时调用 `.off()` 清理所有事件监听器

#### 问题 7: main.ts 页面卸载时无清理逻辑
- **文件**: `ChatHubVue/src/main.ts`
- **严重程度**: 中
- **描述**: 添加了 `beforeunload` 和 `load` 事件监听器，但从未移除。
- **建议修复**: 应用卸载时移除事件监听器

#### 问题 8: SendEditor.vue 组件卸载清理不完整
- **文件**: `ChatHubVue/src/views/Compoents/SendEditor.vue`
- **严重程度**: 低
- **描述**: 有 `onBeforeUnmount` 清理编辑器实例，但可能还有其他资源没有清理。
- **建议修复**: 检查并清理所有在组件生命周期中创建的资源

#### 问题 9: 组件间通信完全依赖全局 Store，缺乏事件总线
- **文件**: 整个项目
- **严重程度**: 中
- **描述**: 项目中没有使用 Vue 3 的 provide/inject 或 mitt 等事件总线机制进行组件间通信。所有状态都存储在全局 Pinia store 中。
- **建议修复**: 对于父子组件通信使用 props/emits，对于跨级组件通信使用 provide/inject

#### 问题 10: appsetting store 中的计数器使用可能导致意外触发
- **文件**: `ChatHubVue/src/store/index.ts`
- **严重程度**: 低
- **描述**: 使用计数器模式来触发 watch 响应，这种方式比较 hack。
- **建议修复**: 使用布尔标志位或时间戳替代计数器

#### 问题 11: 缺少全局 beforeunload 页面离开确认
- **文件**: `ChatHubVue/src/main.ts`
- **严重程度**: 低
- **描述**: 当用户有未发送的消息时，没有在页面关闭前提示用户确认。
- **建议修复**: 在 beforeunload 事件中添加确认逻辑

## Task 8: Admin 后台 - 权限控制审查

### 发现的问题

#### 问题 1: 路由守卫未验证按钮级别权限（meta.auths）
- **文件**: `ChatHubAdminVue/src/router/index.ts`
- **严重程度**: 高
- **描述**: 路由守卫只检查了 `meta.roles` 来验证角色权限，但没有验证 `meta.auths`（按钮级别权限）。按钮级别权限的检查只在组件内部通过 `hasAuth` 函数进行，这意味着没有权限的用户仍然可以尝试访问路由，只是在界面上看不到相关按钮。
- **建议修复**: 在路由守卫中添加对 `meta.auths` 的检查，如果用户没有按钮权限，可以隐藏相关按钮或禁用操作

#### 问题 2: 权限指令未处理动态渲染场景
- **文件**: `ChatHubAdminVue/src/directives/auth/index.ts`
- **严重程度**: 中
- **描述**: `auth` 指令只在 `mounted` 钩子中检查权限并移除无权限元素。如果组件内容是动态渲染的（如 v-if/v-for），权限检查不会重新执行。
- **建议修复**: 添加 `updated` 钩子或在 `componentUpdated` 中重新检查权限

#### 问题 3: Token 刷新失败后未强制跳转登录页
- **文件**: `ChatHubAdminVue/src/utils/http/index.ts`
- **严重程度**: 高
- **描述**: 当 access token 过期时，系统会尝试使用 refresh token 刷新。但如果 refresh token 也过期或刷新失败，系统只是暂存请求并等待，并不会自动跳转到登录页。这会导致用户看到页面静止但无法操作。
**: 在 token - **建议修复刷新失败时添加跳转登录页的逻辑：
```typescript
.catch(() => {
    PureHttp.isRefreshing = false;
    // 跳转到登录页
    router.push('/login');
});
```

#### 问题 4: 响应拦截器未处理 401 错误
- **文件**: `ChatHubAdminVue/src/utils/http/index.ts`
- **严重程度**: 中
- **描述**: 响应拦截器没有处理 401（未授权）错误。当 refresh token 也失败时，返回的是 rejected promise，但没有统一处理 401 情况。
- **建议修复**: 在响应拦截器中添加 401 错误处理：
```typescript
(error: PureHttpError) => {
    if (error.response?.status === 401) {
        removeToken();
        router.push('/login');
    }
    // ... 现有逻辑
}
```

#### 问题 5: 权限组件 Auth 未使用 v-auth 指令
- **文件**: `ChatHubAdminVue/src/components/ReAuth/src/auth.tsx`
- **严重程度**: 低
- **描述**: `Auth` 组件使用了 `hasAuth` 函数，但没有使用自定义 `v-auth` 指令。如果页面中需要同时使用组件和指令，会导致代码不一致。
- **建议修复**: 保持一致性，可以选择统一使用组件方式或指令方式

#### 问题 6: hasAuth 函数未缓存权限结果
- **文件**: `ChatHubAdminVue/src/router/utils.ts`
- **严重程度**: 低
- **描述**: `hasAuth` 函数每次调用都会从路由 meta 中获取权限列表并进行比较，没有缓存机制。在大型应用中频繁调用可能影响性能。
- **建议修复**: 添加权限缓存或在路由加载时预处理权限数据

---

## Task 9: Admin 后台 - CRUD 操作审查

### 发现的问题

#### 问题 1: 页面复制粘贴错误 - 业务逻辑混乱（严重）
- **文件**: `ChatHubAdminVue/src/views/hubData/friends/friends.vue`, `ChatHubAdminVue/src/views/hubData/friendRequest/friendRequest.vue`, `ChatHubAdminVue/src/views/hubData/onlineUser/onlineUser.vue`
- **严重程度**: 高
- **描述**: 多个页面完全复制了 user 页面的代码，导致：
  - 所有页面都使用 `useUser` hook（业务逻辑错误）
  - 页面标题错误：friendRequest 页面显示 "好友关系管理"
  - 搜索字段与实际业务不匹配（如 friendRequest 用 username 搜索）
  - 更多/操作按钮显示不相关的功能（上传头像、重置密码、分配角色）
- **建议修复**: 每个页面应该使用独立的 hook 文件，根据实际业务定义字段

#### 问题 2: defineOptions 名称定义错误
- **文件**: `ChatHubAdminVue/src/views/hubData/friends/friends.vue`, `ChatHubAdminVue/src/views/hubData/friendRequest/friendRequest.vue`
- **严重程度**: 中
- **描述**: 
  - `friends.vue` 中 `defineOptions({ name: "Friends" })` - 应该是 "FriendsManage" 或类似
  - `friendRequest.vue` 中 `defineOptions({ name: "Friends" })` - 应该是 "FriendRequest"
- **建议修复**: 根据实际页面功能定义正确的名称

#### 问题 3: 表单验证规则与表单字段不匹配
- **文件**: `ChatHubAdminVue/src/views/hubData/user/utils/rule.ts`, `ChatHubAdminVue/src/views/hubData/user/form/index.vue`
- **严重程度**: 中
- **描述**: 
  - rule.ts 中定义了 `nickname` 为必填项，但 form/index.vue 中没有 nickname 字段
  - rule.ts 中定义了 `headerImg` 为必填项，但实际业务中头像可能不需要必填
- **建议修复**: 验证规则应该与实际表单字段一致

#### 问题 4: 分页参数未正确传递给 API
- **文件**: `ChatHubAdminVue/src/views/hubData/user/utils/hook.tsx`
- **严重程度**: 中
- **描述**: 
  - `pagination.total`、`pagination.pageSize`、`pagination.currentPage` 都被注释掉了
  - 搜索方法 `onSearch` 没有传递分页参数给 API
  - `handleSizeChange` 和 `handleCurrentChange` 只是打印日志，没有更新分页状态
- **建议修复**: 实现完整的分页功能：
```typescript
async function onSearch() {
    loading.value = true;
    const { data } = await useUserStoreHook().getAllUsers({
        ...toRaw(form),
        page: pagination.currentPage,
        pageSize: pagination.pageSize
    });
    // ...
}
```

#### 问题 5: 批量删除缺少二次确认
- **文件**: `ChatHubAdminVue/src/views/hubData/user/user.vue` 等
- **严重程度**: 低
- **描述**: 批量删除操作在 el-popconfirm 中有确认对话框，但没有显示具体要删除的数据数量或信息。用户只能看到"是否确认删除?"的通用提示。
- **建议修复**: 增强确认信息，显示选中的数量：
```html
<el-popconfirm :title="`是否确认删除选中的 ${selectedNum} 项数据?`" @confirm="onbatchDel">
```

#### 问题 6: 按钮缺少权限控制
- **文件**: `ChatHubAdminVue/src/views/hubData/user/user.vue` 等
- **严重程度**: 中
- **描述**: 增删改查按钮没有使用 `v-auth` 指令进行权限控制。所有登录用户都能看到并操作这些按钮。
- **建议修复**: 为按钮添加权限指令：
```html
<el-button v-auth="'user:add'" ...>新增用户</el-button>
<el-button v-auth="'user:delete'" ...>删除</el-button>
```

#### 问题 7: 编辑功能未实现
- **文件**: `ChatHubAdminVue/src/views/hubData/user/utils/hook.tsx`
- **严重程度**: 高
- **描述**: `openDialog` 函数中，当 `title !== "新增"` 时（即编辑模式），只调用了 `chores()` 而没有实际调用更新接口：
```typescript
} else {
    // 实际开发先调用修改接口，再进行下面操作
    chores();
}
```
- **建议修复**: 实现编辑接口调用

#### 问题 8: 删除操作后未检查返回结果
- **文件**: `ChatHubAdminVue/src/views/hubData/user/utils/hook.tsx`
- **严重程度**: 中
- **描述**: `handleDelete` 函数直接显示成功消息，没有检查 API 返回的 code 值：
```typescript
await useUserStoreHook().postDeleteUser(payload);
message(`您删除了用户编号为${row.id}的这条数据`, { type: "success" });
```
- **建议修复**: 检查返回结果后再显示消息：
```typescript
const res = await useUserStoreHook().postDeleteUser(payload);
if (res.data?.code === 1) {
    message(`删除成功`, { type: "success" });
    onSearch();
}
```

#### 问题 9: onlineUser 页面不应有增删改操作
- **文件**: `ChatHubAdminVue/src/views/hubData/onlineUser/onlineUser.vue`
- **严重程度**: 中
- **描述**: 在线用户列表是只读数据，不应该有新增、编辑、删除、批量删除操作。但当前页面包含了这些按钮。
- **建议修复**: 移除在线用户页面的增删改相关按钮

#### 问题 10: 图片上传和重置密码功能未完整实现
- **文件**: `ChatHubAdminVue/src/views/hubData/user/utils/hook.tsx`
- **严重程度**: 中
- **描述**: 
  - `handleUpload` 函数只是打印日志和关闭弹窗，没有实际调用上传 API
  - `handleReset` 函数只是打印密码和显示成功消息，没有实际调用重置密码 API
- **建议修复**: 实现实际的 API 调用

---

### 总结

本次审查发现 Admin 后台存在以下主要问题：

1. **权限控制问题**：路由守卫未验证按钮级别权限、Token 刷新失败处理不完善
2. **CRUD 问题**：页面代码复制粘贴错误导致业务逻辑混乱、分页功能未实现、编辑功能未完成
3. **表单验证**：验证规则与表单字段不匹配
4. **按钮权限**：缺少 v-auth 指令控制

建议优先修复高严重程度的问题，特别是问题 1（页面复制粘贴错误）和问题 7（编辑功能未实现）。
