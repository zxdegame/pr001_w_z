---
name: refuge-dev-spec
description: 避难防线（词汇守卫者）Tuanjie 2D 塔防英语学习项目的开发提示词规范。用于撰写 Qoder 新会话开场任务描述、开工前对齐技术栈/架构/编码/Git/验收纪律，或当用户询问“如何给 AI 下开发任务、提示词怎么写”时自动应用。涵盖 GDD/TechStack/Architecture/DevTasks/ProjectSetup/Compliance 文档真源引用、Phase 串行约束、Core 目录与接口契约先行、Conventional Commits、三层验收与 DoD/DoR、已知坑规避。Use when starting a dev task, writing an opening task prompt, or aligning coding/Git conventions for this project.
---

# 避难防线 · 开发提示词规范 (refuge-dev-spec)

主程序用的开工提示词标准。凡在本项目（Tuanjie 2D 塔防 · 英语学习《避难防线 / 词汇守卫者》）启动新会话、撰写任务描述、或让 AI 产出代码前，按本规范构造提示词并对齐纪律。

## 何时触发
- 用户要为某 Phase / 任务写 Qoder 开场任务描述
- 开工前需对齐技术栈 / 架构 / 编码 / Git / 验收纪律
- 用户问“该怎么给 AI 下开发任务”“提示词怎么写”

## 0. 铁律（违反即返工）
1. **文档真源优先**：一切规则以 `Assets/Design/` 六份文档为准，本 skill 只做导航与固化，**不复述易变的具体数值**。
2. **接口先行**：任何接口 / 事件契约改动 → **先改 `Architecture.md` 走主程评审**，再动代码；禁止改代码绕过。
3. **不硬编码数值**：配置一律走 ScriptableObject / `Constants`；禁止 magic number、禁止每帧 `FindObjectOfType` / `GameObject.Find`。
4. **事件解耦**：跨系统不直接引用，经自研 `EventBus`；订阅必在 `OnDestroy()` 配对 `UnsubscribeAll`。
5. **Phase 串行**：Phase 0→1 **禁止并行**；Phase 2+ 接口锁定后才可并行；Phase 0 未完成不得进 Phase 1。
6. **合规门禁**：UI 文案 / 广告接入 / 数据上报前，先核 `Compliance.md` §2 / §3 / §6。
7. **Git 越权红线**：未经主程明确同意**不提交、不改全局 git、不强推公共分支（main/dev）**。

## 1. 开工提示词五段式模板
新会话开场任务描述必须含以下五段（复制填写；结构对齐 `DevTasks.md` 各 Phase 的“复制以下内容到新会话”块）：

```markdown
# 任务：<Phase X · 任务名>

## 1) 项目背景
- 项目：避难防线 - 词汇守卫者（Tuanjie 2D 英语学习塔防）
- 目标：一句话说明本任务产出边界（做什么 / 不做什么）

## 2) 输入文档（必须先读，引用当前版本）
逐条列出 GDD / TechStack / Architecture / DevTasks / ProjectSetup / Compliance 的**当前版本串**（版本如何取见下）+ 本任务重点章节。

## 3) 已确认关键决策（不可更改）
从 `DevTasks.md` 对应 Phase 的“已确认关键决策”段**原样搬运**，不要自造或改写。

## 4) 目标产出
严格对齐 `Architecture.md` 第 17 节产出清单 + `DevTasks.md` 该 Phase 分解；列明涉及的目录、类、接口、事件、场景。

## 5) 验收标准（DoD）
逐项可勾选，至少含：编译 0 warning / 代码放对目录 / public API 有 XML 注释 / 关键方法有 EditMode 单测（≥80%）/ 手工跑通指定场景 / 无性能回退。模板见 git-and-acceptance.md。

## 开工前必答（DoR）
- 需求引用了 GDD 哪节？依赖的接口是否都已存在？工时 ≤ 2 天？技术风险是否列出？目标平台 Editor/Device 是否明确？
```

### 版本如何取
**不要把版本号写死。** 会话开始时读各文档头部“文档版本”字段与“关联文档”行，取当前串。基线示意（**以文档实际为准**）：GDD / TechStack / Architecture / DevTasks / ProjectSetup / Compliance。文档更新须按版本级联同步，历史行保留。

## 2. 硬约束速查
详见 [tech-stack.md](tech-stack.md)。一句话要点：引擎 Tuanjie 1.x · .NET Standard 2.1 / C# 9 · URP 14.x **2D Renderer 全画质档统一** + Linear + Pixel Perfect + Sprite Atlas V2 · IL2CPP（Android ARMv7+ARM64 / iOS ARM64）· Min SDK 24 / Target 34 · New Input System · uGUI+TMP+DOTween · UniTask · Spine2D（运行时 4.2，美术导出须 Spine Editor 4.2.x 二进制）· JsonUtility 存档 · 无 DI 框架（手写单例 + Bootstrap）· 自研 EventBus。性能预算：DrawCall < 30、内存 ≤ 400MB、包体 ≤ 200MB、同屏 ≤ 30 僵尸、对象池必用。

## 3. 架构与编码规范
详见 [architecture.md](architecture.md)。要点：五层 `Scenes / UI / Systems / Entities / Core`；`Core/` 放接口 + 枚举 + 事件 + EventBus + Managers（只读契约层）；命名规范（接口 `I`+PascalCase、类/枚举 PascalCase、常量 SCREAMING_SNAKE_CASE、私有字段 `_camelCase`）；`Word` = class（配 `WordRef` struct）；敌人用 **Vitality 模型（非 HP）**，`TakeDamage(int)`，答对 = 一次暴击 ×200% 攻击力。

## 4. 提示词范例
详见 [prompt-template.md](prompt-template.md)：一份“填空即用”的空模板 + 一个 Phase 1 答题状态机任务的完整示例。

## 5. Git 与验收
详见 [git-and-acceptance.md](git-and-acceptance.md)。Conventional Commits（`feat/fix/chore/refactor/perf/test/docs/style/ci` · scope 取层名）· 分支 `main/dev/feature/hotfix/release` · PR 自查清单 · 四道评审关 · 三层验收 L1/L2/L3 · DoD/DoR 模板。**提交信息在 PowerShell 5.1 下必须纯 ASCII**；推送依赖 github scoped 代理（Clash `127.0.0.1:7890`）+ GCM 浏览器登录。

## 6. 已知坑（务必规避）
详见 [pitfalls.md](pitfalls.md)：InputActions 生成类 **CS0542**（映射名 + `Actions` 撞外层类名）、Burst **CS0234**（切 Android 冷启动，重启编辑器即解）、Tuanjie `.meta` 为 **base64 guid**、与引用侧 hex 不可字符串比对、SearchReplace 写 CJK 概率损坏须 Grep 扫描回验、manifest/ProjectSettings 文本改后须重启 Editor 生效、工作区外文件须用 PowerShell `[IO.File]::WriteAllText` UTF-8 无 BOM。

## 7. 会话结束前自查
- [ ] 产出符合 `Architecture.md` 接口（改接口已走 RFC）
- [ ] 无 magic number / 每帧 Find / EventBus 订阅泄漏
- [ ] public API 有 `///` XML 注释
- [ ] DoD 逐项列出完成情况
- [ ] 未越权操作 git；若需提交，已获主程明确同意
