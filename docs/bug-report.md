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
