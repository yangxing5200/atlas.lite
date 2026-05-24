# Atlas.Lite

Atlas.Lite 是面向非多租户、小型后台、单体 Web API 和轻量业务系统的 .NET 8 基础框架。它聚焦单库应用底座，不包含租户库动态连接、租户初始化、tenant/store/user scope 过滤或租户 outbox dispatcher。

## 项目结构

- `src/Atlas.Lite.Core`：异常、时间/ID 抽象、分页、Result/ApiResponse。
- `src/Atlas.Lite.Infrastructure.Logging`：Serilog、日志 Options、敏感字段脱敏。
- `src/Atlas.Lite.Web`：ProblemDetails、全局异常、Swagger、CORS、JSON、Health Check。
- `src/Atlas.Lite.Data`：单 DbContext、审计、软删除、Repository、UnitOfWork。
- `src/Atlas.Lite.Security`：JWT Bearer、Token 生成、`ICurrentUser`。
- `src/Atlas.Lite.Caching`：Memory/Redis 缓存抽象。
- `src/Atlas.Lite.BackgroundTasks`：轻量周期后台任务。
- `samples/Atlas.Lite.Sample.WebApi`：最小演示应用。

## 本地启动

```powershell
dotnet restore Atlas.Lite.sln
dotnet run --project samples/Atlas.Lite.Sample.WebApi
```

启动后访问：

- `GET /health`
- Development 环境下访问 `/swagger`
- 登录：`POST /api/auth/login`，示例账号 `demo`，密码 `demo-password`

Sample 默认使用 SQLite 和 Memory Cache，不需要外部服务。

## 配置

配置根节点是 `AtlasLite`。可从 `appsettings.Template.json` 开始复制配置，详细说明见 `docs/configuration_guide.md`。

关键配置：

- `AtlasLite:Database`：`Provider` 支持 `Sqlite`、`SqlServer`、`InMemory`。
- `AtlasLite:Auth:Jwt`：JWT secret 必须至少 32 个字符。
- `AtlasLite:Cache`：`Provider` 支持 `Memory` 和 `Redis`。
- `AtlasLite:Logging`：至少启用 Console/File/Seq 中一个 sink。

## 验收命令

```powershell
powershell -ExecutionPolicy Bypass -File tools/check-line-endings.ps1
powershell -ExecutionPolicy Bypass -File tools/check-package-versions.ps1
dotnet restore Atlas.Lite.sln
dotnet build Atlas.Lite.sln --no-restore
dotnet test Atlas.Lite.sln --no-build
```
