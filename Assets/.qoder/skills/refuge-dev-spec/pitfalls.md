# 已知坑与规避（本会话实证）

## 1. InputActions 生成类 CS0542
`error CS0542: member names cannot be the same as their enclosing type`。
- 根因：生成器外层类名 = `MakeTypeName(asset.name / 文件名)`；每个动作映射的 struct 名 = `MakeTypeName(映射名, "Actions")`（**恒定追加 "Actions"**）。当资产名 == 映射名 PascalCase + "Actions"（如资产 `GameplayActions` + 映射 `gameplay` → struct 也叫 `GameplayActions`）即冲突。
- 修复：改动作映射名使“映射名 + Actions”≠ 类名；或改 Inspector 的 **Wrapper Class Name**。**别把映射命名成会与类名撞的词。** 改后 Editor 重导入自动重生成 `.cs`。

## 2. Burst CS0234（切 Android 冷启动）
`BurstAotCompiler.cs(1434): error CS0234: 'AndroidExternalToolsSettings' does not exist in namespace 'UnityEditor.Android'`。
- 根因：Tuanjie 把 `UnityEditor.Android.Extensions.dll` 迁到 `PlaybackEngines/AndroidPlayer/`，冷启动首趟未进 Burst.Editor 引用集。
- 修复（首选）：**重启编辑器** → Android 模块重新注册 → 编译恢复（保持 Android 目标，无需切平台 / 删 Library）。备选：切 Standalone、清 Library 重导、Hub 修复、升级补丁。

## 3. Tuanjie GUID 双编码
`.meta` 存 **base64 guid**，跨文件引用（.asset / ProjectSettings）存 **hex guid**。二者引擎内部映射，**不可用 grep 字符串比对验证引用完整性**。重命名资产保留 guid → 引用不断链。分类渲染器看 `m_Script`（UniversalRendererData = 3D；Renderer2DData = 2D，看是否含 `m_HDREmulationScale` / `postProcessData`）。

## 4. SearchReplace 写 CJK 概率损坏
批量写中文后跑 CJK 扫描集回验、必要时修复：
```
膣|阀|吅|衷|出舋|木文|符和|兑底
```
Grep 命中的可疑字 → Read 回读比对 → 修正。

## 5. manifest / ProjectSettings 文本改后需重启 Editor
直接改 `Packages/manifest.json`、`ProjectSettings/*.asset` 文本后，Editor 若不重启 / 不重导，内存态会覆盖回旧值。改后重启 Editor 或 `Assets → Refresh` 生效。

## 6. 工作区外文件写入
`Packages/`、`ProjectSettings/`、工程根 `.editorconfig` 在 Assets 工作区之外，Write / SearchReplace 会被拒。用 PowerShell：
```powershell
[IO.File]::WriteAllText($p,$s,(New-Object System.Text.UTF8Encoding($false)))
```
（UTF-8 无 BOM；`.Replace()` 做 ordinal 精确替换）。

## 7. PowerShell 语法 & git 偶发
- PS 5.1 不支持 `&&`，用 `;`；不支持 bash 的 `||` / `2>/dev/null`
- git 偶发“拒绝访问”（sandbox 通道）→ 重试

## 8. changelog / 版本串纪律
文档历史行、派生旧版本串保留；每次改文档同步版本号并级联到关联文档；单一真源——别在 skill / 提示词里写死易变数值（版本号、guid、性能数字均以文档 / 实测为准）。
