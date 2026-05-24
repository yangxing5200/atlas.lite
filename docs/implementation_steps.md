# Atlas.Lite 实现步骤

Atlas.Lite 是面向非多租户私活、小型后台、单体 Web API 和轻量业务系统的基础框架。它不继承 Atlas 的多租户模型，不包含租户库动态连接、租户初始化、租户 outbox dispatcher、store scope 等能力。

目标是做一个能快速开项目、结构清晰、依赖可控、可扩展但不过度设计的单库应用底座。

## 设计边界

保留能力：
- Web API 标准启动封装。
- 统一异常处理和 ProblemDetails。
- Options 强类型绑定和启动校验。
- Serilog 日志。
- Swagger/OpenAPI。
- 单 DbContext EF Core 数据访问。
- Repository 或 Query/Command service 基础约定。
- Auth/JWT 基础能力。
- Memory/Redis 缓存抽象。
- 轻量后台任务。
- 测试基线、CI、代码风格和换行规范。

不引入能力：
- 多租户 Global/Tenant 双库模型。
- TenantDbContextFactory 和租户连接解析。
- tenant/store/user scope 数据过滤。
- 租户开通、租户迁移调度。
- tenant outbox dispatcher。
- 多租户边界 Analyzer。
- 过早抽 NuGet 公共包。

## 阶段一：仓库和工程基线

### Step 01：仓库文本规范

目标：
- 新增 `.editorconfig`。
- 新增 `.gitattributes`。
- 约束 YAML 使用 LF，其它文本默认 CRLF。
- 明确 UTF-8、缩进、尾随空白和最终换行规则。

验收：
- 自定义换行校验通过。
- `git diff --check` 通过。
- 不引入业务代码。

### Step 02：解决方案和项目结构

目标：
- 创建 `Atlas.Lite.sln`。
- 创建标准目录：
  - `src`
  - `tests`
  - `samples`
  - `docs`
  - `tools`
- 建立 Solution Folder：
  - `1.Core`
  - `2.Data`
  - `3.Infrastructure`
  - `4.Application`
  - `5.WebApi`
  - `tests`
  - `samples`
  - `tools`

验收：
- `dotnet sln list` 能正常显示项目。
- 空 solution 可 restore。

### Step 03：中央包版本管理

目标：
- 新增 `Directory.Packages.props`。
- 新增 `Directory.Build.props`。
- 默认启用：
  - `net8.0`
  - nullable
  - implicit usings
  - deterministic build
  - latest analysis level
- 项目文件不写内联 `PackageReference Version`。

验收：
- `dotnet restore Atlas.Lite.sln` 通过。
- CI 能检查项目文件里没有内联包版本。

### Step 04：基础文档和配置模板

目标：
- 新增 `README.md`。
- 新增 `appsettings.Template.json`。
- 新增 `.env.example`。
- 新增 `docs/configuration_guide.md`。

验收：
- 新项目如何启动、如何配置数据库/Auth/缓存有明确说明。
- 模板不包含真实密钥。

## 阶段二：核心库和 Web API 底座

### Step 05：Core 基础库

项目：
- `src/Atlas.Lite.Core`

目标：
- 定义基础异常、时间抽象、ID 生成接口、分页模型、Result 或 ApiResponse 约定。
- 保持无 ASP.NET Core、无 EF Core 依赖。

验收：
- Core 可以被任何层引用。
- Core 不依赖 Infrastructure/Data/WebApi。

### Step 06：Infrastructure.Logging

项目：
- `src/Atlas.Lite.Infrastructure.Logging`

目标：
- 接入 Serilog。
- 提供日志 Options。
- 支持 Console/File/Seq 开关。
- 提供敏感字段脱敏策略。

验收：
- 缺少或错误日志配置时启动期给出明确错误。
- Sample WebApi 能输出结构化日志。

### Step 07：WebApi 启动扩展

项目：
- `src/Atlas.Lite.Web`

目标：
- 提供 `AddAtlasLiteWebApi()`。
- 提供 `UseAtlasLiteWebApi()`。
- 内置：
  - ProblemDetails
  - 全局异常处理
  - Swagger
  - CORS
  - JSON 选项
  - Health check

验收：
- Sample WebApi 的 `Program.cs` 足够薄。
- `/health` 可访问。
- Swagger 在 Development 环境可访问。

### Step 08：Options 校验

目标：
- 为 Web、Auth、Logging、Cache、Database 增加强类型 Options。
- 关键配置使用 `ValidateOnStart()`。

验收：
- 缺少连接串、JWT secret、日志 sink 配置错误时启动失败且错误明确。

## 阶段三：数据访问和认证缓存

### Step 09：单库 EF Core 数据层

项目：
- `src/Atlas.Lite.Data`

目标：
- 提供单一 `AppDbContext` 基类或示例实现方式。
- 提供审计字段自动填充。
- 提供软删除约定。
- 提供基础 repository 或 unit of work。

验收：
- 不包含任何多租户概念。
- Sample 能通过一个连接串完成 CRUD。

### Step 10：认证与授权

项目：
- `src/Atlas.Lite.Security`

目标：
- JWT Bearer 基础认证。
- Token Options 校验。
- 当前用户抽象 `ICurrentUser`。
- 常用授权策略扩展点。

验收：
- Sample 有登录接口和受保护接口。
- JWT secret 长度不足时启动失败。

### Step 11：缓存抽象

项目：
- `src/Atlas.Lite.Caching`

目标：
- 提供 `ICacheService`。
- 支持 Memory。
- 预留 Redis Provider。
- 提供缓存 key 约定。

验收：
- Memory 模式零外部依赖可运行。
- Redis 配置缺失时不误启动 Redis 模式。

### Step 12：后台任务

项目：
- `src/Atlas.Lite.BackgroundTasks`

目标：
- 提供简单 hosted service 基础设施。
- 支持定时任务 Options。
- 支持任务开关。

验收：
- Sample 可注册一个定时任务。
- 关闭配置时不会启动后台任务。

## 阶段四：示例应用和测试

### Step 13：Sample WebApi

项目：
- `samples/Atlas.Lite.Sample.WebApi`

目标：
- 演示：
  - 标准启动方式
  - CRUD
  - 登录和授权
  - 缓存
  - 健康检查
  - Swagger

验收：
- `dotnet run` 后可本地访问 Swagger。
- 不需要任何多租户配置。

### Step 14：单元测试基线

项目：
- `tests/Atlas.Lite.Core.Tests`
- `tests/Atlas.Lite.Web.Tests`
- `tests/Atlas.Lite.Data.Tests`

目标：
- 覆盖核心基础行为。
- 覆盖 Options 校验。
- 覆盖异常响应和认证基础逻辑。

验收：
- `dotnet test` 通过。
- 测试不依赖真实外部服务。

### Step 15：CI 基线

目标：
- GitHub Actions 执行：
  - 换行校验
  - 中央包版本校验
  - restore
  - build
  - test
  - 上传测试报告

验收：
- push 和 pull request 都执行。
- 失败信息能定位到换行、包版本、构建或测试问题。

## 阶段五：模板化和发布准备

### Step 16：dotnet new 模板

目标：
- 提供 `atlas-lite-webapi` 模板。
- 生成最小可运行 WebApi。
- 支持配置项目名、默认命名空间和数据库 provider。

验收：
- 本地安装模板后可生成新项目。
- 生成项目可 restore/build/run。

### Step 17：Docker 和本地开发

目标：
- Sample WebApi Dockerfile。
- 可选 docker-compose，包含 API、数据库、Redis、Seq。

验收：
- 本地 compose 可启动基础环境。
- Memory 模式不需要 compose 也能运行。

### Step 18：版本和发布策略

目标：
- 明确是否作为源码模板、NuGet 包或两者并存。
- 明确 SemVer。
- 明确 breaking change 记录方式。

验收：
- 有 `docs/release_guide.md`。
- `dotnet pack` 或模板打包路径明确。

## 推荐执行顺序

1. 先完成 Step 01 到 Step 04，保证仓库和工程基线稳定。
2. 再完成 Step 05 到 Step 08，形成 Web API 最小底座。
3. 然后完成 Step 09 到 Step 12，补齐数据、认证、缓存和后台任务。
4. 接着完成 Step 13 到 Step 15，建立示例和 CI 验收闭环。
5. 最后完成 Step 16 到 Step 18，再考虑模板化和发布。

## 和 Atlas 的关系

Atlas.Lite 不直接依赖 Atlas，也不从 Atlas 复制多租户代码。可以借鉴 Atlas 中已经验证过的实现思路，但每个能力都要重新按单库、非多租户场景收敛。

后续如果 Atlas 和 Atlas.Lite 都稳定，再考虑抽取真正通用的包，例如：
- `Atlas.Kernel`
- `Atlas.Web`
- `Atlas.Caching`
- `Atlas.Logging`

在此之前，不提前抽公共包，避免两个框架被过早耦合。
