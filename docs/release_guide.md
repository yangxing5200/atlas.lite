# Atlas.Lite 发布指南

## 发布策略

Atlas.Lite 当前优先作为源码模板和仓库内基础框架维护。NuGet 包可以在 API 稳定后再发布，避免过早把 Atlas.Lite 与其它框架耦合。

推荐顺序：

1. 先发布 `atlas-lite-webapi` 本地模板。
2. 在 Sample 和真实项目中验证 API。
3. API 稳定后，再为 `src/*` 类库发布 NuGet 包。

## SemVer

- `0.x`：允许调整公开 API，但需要在发布说明中说明迁移方式。
- `1.x`：稳定 API。破坏性变更只能进入新的 major 版本。
- Patch 版本只包含兼容修复。

## Breaking Change 记录

破坏性变更必须记录：

- 影响的包或模板。
- 旧行为和新行为。
- 必需的迁移步骤。
- 是否影响配置文件。

建议新增 `CHANGELOG.md` 后按版本维护。

## 本地打包

类库打包：

```powershell
Get-ChildItem .\src -Filter *.csproj -Recurse |
  ForEach-Object { dotnet pack $_.FullName -c Release -o artifacts/packages }
```

模板安装与验证：

```powershell
dotnet new install .\templates\atlas-lite-webapi
dotnet new atlas-lite-webapi -n Demo.AtlasLite.Api -o .\artifacts\template-smoke
dotnet restore .\artifacts\template-smoke\Demo.AtlasLite.Api.csproj
dotnet build .\artifacts\template-smoke\Demo.AtlasLite.Api.csproj --no-restore
```
