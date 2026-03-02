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
