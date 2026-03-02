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
