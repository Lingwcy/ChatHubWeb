# ChatHubWeb 剩余未修复 Bug 报告

> **生成日期**: 2026-03-03
> **状态**: 待修复

---

## 已修复的问题 (20+)

### 后端 (ChatHubCore)
- [x] 添加 BusinessException 类区分异常类型
- [x] 修复 ExceptionHandlingMiddleware 敏感信息泄露
- [x] 修复 AuthController 登录/注册响应泄露密码
- [x] 修复异常处理返回敏感信息
- [x] 缩短 JWT Token 过期时间 (30000分钟→120分钟)
- [x] 修复 AdminAuthController 明文密码比较
- [x] 添加 PBKDF2 密码哈希替代 SHA256
- [x] 修复 CORS 允许所有来源
- [x] 修复 SignalR 空值检查和异常处理

### 前端 (ChatHubVue)
- [x] 修复 axios error.response 空值检查
- [x] 修复 404 处理返回 rejected promise
- [x] 添加全局 Vue 错误处理
- [x] 添加全局 Promise rejection 处理
- [x] 修复 SignalR 事件监听器累积
- [x] 添加重连次数限制
- [x] 移除 UseServiceStore 持久化配置

### Admin (ChatHubAdminVue)
- [x] 修复 Token 刷新失败后跳转登录页
- [x] 添加 401 错误处理

---

## 剩余未修复问题

### 高优先级 (High)

| # | 模块 | 问题 | 文件 |
|---|------|------|------|
| 1 | 后端 | JWT 未验证 Issuer 和 Audience | `Program.cs` |
| 2 | 后端 | 缺少 Token 刷新机制 | `jwtService.cs` |
| 3 | 后端 | 缺少 Token 黑名单/注销机制 | - |
| 4 | 后端 | SendHubKey 存在安全漏洞 | `Hub.cs` |
| 5 | 后端 | SendImage/SendPublicImage 不返回结果 | `Hub.cs` |
| 6 | 后端 | 缺少在线用户过期清理机制 | `Hub.cs` |
| 7 | 前端 | Login.vue 递归 setTimeout 未清理 | `LoginView/Login.vue` |
| 8 | Admin | 页面复制粘贴错误 - 业务逻辑混乱 | `hubData/friends/*.vue`, `hubData/friendRequest/*.vue`, `hubData/onlineUser/*.vue` |
| 9 | Admin | 编辑功能未实现 | `hook.tsx` |

### 中优先级 (Medium)

| # | 模块 | 问题 | 文件 |
|---|------|------|------|
| 1 | 后端 | JWT 密钥强度不足 (已在appsettings中改为占位符) | `appsettings.json` |
| 2 | 后端 | 缺少 SignalR 连接超时配置 | `Program.cs` |
| 3 | 后端 | 私聊消息缺少确认机制 | `Hub.cs` |
| 4 | 后端 | sysOnlineUser 缺少过期时间字段 | `sysOnlineUser.cs` |
| 5 | 前端 | MessageCT.vue scrollDown setTimeout 未清理 | `MessageCT.vue` |
| 6 | 前端 | MessageCT.vue 消息列表无虚拟滚动优化 | `MessageCT.vue` |
| 7 | 前端 | main.ts 页面卸载时无清理逻辑 | `main.ts` |
| 8 | 前端 | appsetting store 计数器模式问题 | `store/index.ts` |
| 9 | Admin | 路由守卫未验证按钮级别权限 | `router/index.ts` |
| 10 | Admin | 权限指令未处理动态渲染场景 | `directives/auth/index.ts` |
| 11 | Admin | defineOptions 名称定义错误 | 各页面组件 |
| 12 | Admin | 表单验证规则与表单字段不匹配 | `rule.ts` |
| 13 | Admin | 分页参数未正确传递给 API | `hook.tsx` |
| 14 | Admin | 按钮缺少权限控制 | 各列表页面 |
| 15 | Admin | 删除操作后未检查返回结果 | `hook.tsx` |
| 16 | Admin | onlineUser 页面不应有增删改操作 | `onlineUser.vue` |
| 17 | Admin | 图片上传和重置密码功能未完整实现 | `hook.tsx` |

### 低优先级 (Low)

| # | 模块 | 问题 | 文件 |
|---|------|------|------|
| 1 | 后端 | CustomAuthorizeAttribute 为空实现 | `CustomAuthorizeAttribute.cs` |
| 2 | 后端 | jwtService 中有多余的调试输出 | `jwtService.cs` |
| 3 | 前端 | MessageCT.vue watch 未显式清理 | `MessageCT.vue` |
| 4 | 前端 | SendEditor.vue 组件卸载清理不完整 | `SendEditor.vue` |
| 5 | 前端 | 缺少全局 beforeunload 页面离开确认 | `main.ts` |
| 6 | Admin | 权限组件 Auth 未使用 v-auth 指令 | `auth.tsx` |
| 7 | Admin | hasAuth 函数未缓存权限结果 | `utils.ts` |
| 8 | Admin | 批量删除缺少二次确认 | 各列表页面 |

---

## 修复统计

- **总计发现**: 约 92 个问题
- **已修复**: 约 20+ 个问题
- **剩余未修复**: 约 40+ 个问题

---

## 后续修复建议

### 阶段一：功能增强（待定）
- 实现 Token 刷新机制
- 实现 Token 黑名单
- SignalR 安全加固

### 阶段二：功能完善
- Admin CRUD 功能完善
- 分页功能实现
- 编辑功能实现

### 阶段三：性能优化
- 虚拟滚动
- 代码重构
