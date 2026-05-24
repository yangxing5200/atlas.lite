# Atlas.Lite 配置指南

Atlas.Lite 使用 `AtlasLite` 作为配置根节点。Sample 默认使用 SQLite、Memory Cache 和本地 JWT 配置，能零外部依赖启动。

## 数据库

`AtlasLite:Database:Provider` 支持 `Sqlite`、`SqlServer` 和 `InMemory`。

```json
{
  "AtlasLite": {
    "Database": {
      "Provider": "Sqlite",
      "ConnectionString": "Data Source=atlas-lite-sample.db"
    }
  }
}
```

`Sqlite` 与 `SqlServer` 必须配置连接串。`InMemory` 仅用于本地演示或测试。

## Auth/JWT

JWT 配置位于 `AtlasLite:Auth:Jwt`。

```json
{
  "AtlasLite": {
    "Auth": {
      "Jwt": {
        "Issuer": "Atlas.Lite",
        "Audience": "Atlas.Lite",
        "Secret": "replace-with-at-least-32-characters",
        "ExpirationMinutes": 60
      }
    }
  }
}
```

`Secret` 最少 32 个字符。生产环境应通过环境变量或 Secret 管理系统注入。

## 缓存

`AtlasLite:Cache:Provider` 支持 `Memory` 和 `Redis`。Memory 模式不需要外部服务。

```json
{
  "AtlasLite": {
    "Cache": {
      "Provider": "Memory",
      "Redis": {
        "ConnectionString": ""
      }
    }
  }
}
```

选择 `Redis` 时必须配置 `Redis:ConnectionString`。

## 日志

日志配置位于 `AtlasLite:Logging`，支持 Console、File、Seq。

```json
{
  "AtlasLite": {
    "Logging": {
      "MinimumLevel": "Information",
      "Console": { "Enabled": true },
      "File": { "Enabled": false, "Path": "logs/atlas-lite-.log" },
      "Seq": { "Enabled": false, "ServerUrl": "" }
    }
  }
}
```

至少启用一个 sink。启用 File 或 Seq 时必须提供对应路径或服务地址。
