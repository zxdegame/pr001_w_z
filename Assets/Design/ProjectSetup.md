# 项目启动指南 (ProjectSetup)

| 项目 | 避难防线 - 词汇守卫者 |
|------|-------------------|
| 文档版本 | v1.16 |
| 最后更新 | 2026-11-07 |
| 关联文档 | [GDD.MD](./GDD.MD) v2.13、[TechStack.md](./TechStack.md) v1.16、[Architecture.md](./Architecture.md) v1.15、[DevTasks.md](./DevTasks.md) v1.15、[Compliance.md](./Compliance.md) v1.12 |
| 目标平台 | **移动 App（Android + iOS）**；**休闲益智**类目 · **全年龄英语学习**；**国内主市场双阶段**：本版**无预算不开版号**→灰区内测（TestFlight + 官网 APK，不提审商店）→融资→申请版号→国内 iOS 中国区 + 安卓商店上市；**纯广告变现（0 内购）**；放弃海外、Steam 暂缓（详见 [Compliance.md](./Compliance.md)、GDD §12 D22） |
| 读者 | 新成员 Onboarding / 主程自查 / Agent 会话开场参考 |

---

## 0. 阅读顺序建议

新成员加入项目时，**按下列顺序**读文档 + 配置环境：

```
1. GDD.MD（了解做什么游戏）
2. TechStack.md（了解用什么做）
3. Architecture.md（了解代码结构）
4. DevTasks.md（了解阶段任务）
5. 本文 ProjectSetup.md（配置环境、开工）
6. Compliance.md（合规基线，涉及文案/广告/数据时必读）
```

**Phase 0 会话开场引用**：必须包含以上 6 份文档路径 + 版本号（当前基线：GDD v2.13 / TechStack v1.16 / Architecture v1.15 / DevTasks v1.15 / ProjectSetup v1.16 / Compliance v1.12）。

---

## 1. 快速开始（10 分钟）

如果你**急着看效果**，只跑一遍这几步：

```powershell
# 1. 克隆项目（含 LFS）
git lfs install
git clone <repo-url>
cd refuge-defense

# 2. 打开 Tuanjie Editor
# 通过 Tuanjie Hub → 打开项目 → 选择项目根

# 3. 首次打开后 Editor 自动做的事
# - Library 生成（约 3-8 分钟）
# - 恢复上次 Package 下载
# - 编译所有 C# 脚本

# 4. 打开测试场景
# Assets/Scenes/SampleScene.unity

# 5. 按 Play 按钮验证 Editor 正常
```

若第 3 步编译失败，见第 15 节 FAQ。

---

## 2. 开发环境准备

### 2.1 必装软件清单

| 软件 | 版本要求 | 用途 | 下载 |
|------|---------|------|------|
| **Tuanjie Hub** | 最新稳定 | 引擎版本管理 | 团结引擎官网 |
| **Tuanjie Editor** | 1.x（具体小版本以 `ProjectVersion.txt` 为准） | 主 IDE | Hub 内下载 |
| **Android Studio** | Hedgehog 2023.1+ | Android SDK + Gradle + 签名 | developer.android.com |
| **JDK** | OpenJDK 17（Tuanjie 默认） | Android 构建 | Adoptium |
| **Xcode**（需 Mac） | 15+ | iOS 构建与签名 | App Store |
| **CocoaPods** | 1.14+ | iOS 第三方依赖 | `brew install cocoapods` |
| **Rider / VS 2022** | Rider 2024.1+ / VS 2022 17.8+ | C# 编码 | JetBrains / Microsoft |
| **Git** | 2.40+ | 版本控制 | git-scm.com |
| **Git LFS** | 3.4+ | 大文件管理 | git-lfs.github.com |
| **7-Zip** | 任何 | 打包资源 | 7-zip.org |

**可选**：
- **Fiddler / Charles / mitmproxy**：调试广告 SDK 与埋点上报网络
- **Android 逆向工具**（apktool）：仅需看包体时
- **Docker Desktop**：跑本地 CI 测试
- ~~推送通知~~（`com.unity.mobile.notifications`）：**不引入**（单机定位：无云存档、无推送、无排行榜，GDD §12 D4）；复习提醒只用**游戏内弹窗**，不发系统通知
- ❗ **不引入 `com.unity.purchasing`**（0 内购决策，GDD §12 D1）

### 2.2 Rider / VS 插件

**Rider**：
- 内置 Unity/Tuanjie 支持，无需额外插件
- 建议启用：**Unity → External Tools → Additional Source Roots** 指向项目根

**Visual Studio 2022**：
- 必装工作负载：**Mobile development with .NET**
- 推荐扩展：**Unity Tools**、**C# Extensions**、**EditorConfig Power Tools**

### 2.3 目标设备（测试用）

| 平台 | 建议机型 | 覆盖场景 |
|------|---------|---------|
| Android 高端 | 小米 14 / Pixel 8 | 60 FPS 稳定 |
| Android 中低端 | Redmi Note 12 / 三星 A34 | 30-45 FPS 兜底 |
| iPhone | iPhone 12 及以上 | iOS 基线 |
| iPad | iPad Air 4 | 大屏适配（TechStack §17 已声明：**支持 iPhone + iPad，仅拉伸适配，不做专版 UI**） |
| PC | Windows 10/11 主流配置 | 开发主战场 |

**设备预算不够**：先用 Editor + 1 台真机，其余走 TestFlight / 云真机（WeTest）。

### 2.4 密钥管理（不入 Git）

在**项目外**的安全位置维护 `.env` 或密钥管理工具：

```
SECRETS/
├── android_release.keystore（不入 Git，仅本地/CI Vault）
├── ios_distribution.p12 + mobileprovision
├── pangle_ad_unit_id.txt（**穿山甲广告位 ID + 优量汇 ID，国内主渠道**）
├── carrier_auth_appkey.txt（**C1 年龄/家长验证：运营商号码认证/一键登录 AppKey**，客户端直连、不自建后端）
└── README.md（说明如何本地引用）
```

**引用方式**：
```csharp
// Editor 中读取，Build 时通过构建脚本注入（环境变量，不落盘）
#if DEVELOPMENT_BUILD
var appId = File.ReadAllText("../SECRETS/admob_app_id.txt");
#else
var appId = BuildConfig.ADMOB_APP_ID;  // 构建脚本从环境变量填入
#endif
```

---

## 3. 项目初始化清单（Phase 0 前必须完成）

按顺序逐项打勾：

### 3.1 仓库创建

- [ ] `git init` 空仓库或 GitHub/GitLab/Gitee 建立远程
- [ ] 从 Unity 官方模板拉 `.gitignore`：https://github.com/github/gitignore/blob/main/Unity.gitignore
- [ ] 编写 `.gitattributes` 声明 LFS 过滤（见 5.1）
- [ ] 首次提交：6 份设计文档（当前基线见 §0）+ 目录骨架 + `.editorconfig`

### 3.2 Tuanjie 项目创建

- [ ] Hub 中新建项目 → 模板选择 **2D (URP)**
- [ ] 项目路径选择 `d:\pr001\w_z` 或你的工作目录
- [ ] 打开后确认能创建 `Assets/Scripts/` 等空目录不报错
- [ ] 首次提交 `.meta` 文件到 Git

### 3.3 Package 安装

- [ ] Package Manager 中安装 `TechStack.md` 第 16 节列出的所有依赖
- [ ] UniTask 通过 OpenUPM 或 GitHub Release 安装
- [ ] DOTween Pro 通过 Asset Store 安装（或免费的 DOTween）
- [ ] `Packages/manifest.json` 提交到 Git

### 3.4 Project Settings 锁定

**必须一次性配好并 Git 提交**：

- [ ] **Editor** → Asset Serialization = **Force Text**
- [ ] **Editor** → Version Control Mode = **Visible Meta Files**
- [ ] **Editor** → Enter Play Mode Settings：禁用 Domain Reload（提速）
- [ ] **Player** → Color Space = **Linear**
- [ ] **Player** → Scripting Backend = **IL2CPP**（Android / iOS）/ Mono（Editor）
- [ ] **Player** → API Compatibility Level = **.NET Standard 2.1**
- [ ] **Android** → Minimum/Target API Level = 24 / 34；ARMv7 + ARM64；Signing Tool for Gradle 配置就位；keystore 注入走 **Build Pipeline 资产 + 环境变量**（不写入仓库）
- [ ] **iOS** → Target iOS 13.0；Signing & Capabilities 就位；URP 2D 兼容
- [ ] **Quality** → 建立 3 档：Low / Medium / High
- [ ] **Physics 2D** → 设置 Collision Matrix 分层（Zombie/Wall/Bullet/TriggerZone）
- [ ] **URP** → 2D Renderer 数据资产已创建并激活
- [ ] **Input System** → 导入 `GameplayActions.inputactions`
- [ ] **Graphics** → 剔除不必要 Shader（IL2CPP Strip + Shader Variant 裁剪）

### 3.5 目录结构创建

严格按 `Architecture.md` 第 2 节：
```
Assets/Scripts/
├── Core/
│   ├── Interfaces/
│   ├── Enums/
│   ├── Events/
│   └── Managers/
├── Config/
├── Entities/
│   ├── Zombie/
│   ├── Wall/
│   ├── Building/
│   └── Effects/
├── Systems/
│   ├── Battle/
│   ├── Quiz/
│   ├── WordPool/
│   ├── Learning/
│   ├── Reward/
│   └── Supply/
├── UI/
└── Bootstrap/
Assets/Tests/
├── EditMode/
└── PlayMode/
Assets/Editor/
Assets/Resources/
Assets/Scenes/
Assets/Design/
```

**每个空目录放一个 `README.md`** 说明用途，避免被 Tuanjie 剔除。

### 3.6 首次提交

- [ ] 完成以上所有配置后一次提交：`chore: project init (Phase 0 baseline)`
- [ ] 打 tag `v0.0.0-baseline`

---

## 4. Editor 预设与个性化

### 4.1 推荐的 Editor 布局

保存为 `.unitypackage` 供团队导入：
- **Layout 名**：`RefugeDev`
- 面板：Hierarchy / Scene / Inspector / Console / Project / Game
- Console：勾选 **Clear on Play**、Collapse

### 4.2 Rider / VS 附加配置

- 代码风格：`.editorconfig`（提交到项目根）
- **禁止**自动格式化无关文件（Git diff 应最小化）

推荐 `.editorconfig`：
```ini
root = true

[*.cs]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true
csharp_new_line_before_open_brace = all
csharp_prefer_braces = true
dotnet_sort_system_directives_first = true
```

---

## 5. 版本控制配置

### 5.1 `.gitattributes`（LFS 配置）

```gitattributes
# Unity 特定
*.cgs           text merge=unityyamlmerge
*.unity         text merge=unityyamlmerge
*.prefab        text merge=unityyamlmerge
*.asset         text merge=unityyamlmerge
*.meta          text

# 二进制资源走 LFS
*.psd           filter=lfs diff=lfs merge=lfs -text
*.png           filter=lfs diff=lfs merge=lfs -text
*.jpg           filter=lfs diff=lfs merge=lfs -text
*.jpeg          filter=lfs diff=lfs merge=lfs -text
*.tga           filter=lfs diff=lfs merge=lfs -text
*.fbx           filter=lfs diff=lfs merge=lfs -text
*.wav           filter=lfs diff=lfs merge=lfs -text
*.mp3           filter=lfs diff=lfs merge=lfs -text
*.ogg           filter=lfs diff=lfs merge=lfs -text
*.mp4           filter=lfs diff=lfs merge=lfs -text
*.aif           filter=lfs diff=lfs merge=lfs -text
*.tif           filter=lfs diff=lfs merge=lfs -text
*.tif.*         filter=lfs diff=lfs merge=lfs -text
*.bundle        filter=lfs diff=lfs merge=lfs -text
```

### 5.2 `.gitignore`

关键条目（完整版从 Unity 官方模板获取）：
```gitignore
# Unity auto-generated
Library/
Temp/
Obj/
Build/
Builds/
Logs/
UserSettings/
MemoryCaptures/
Recordings/
Assets/AssetStoreTools*

# IDE
.vs/
.vscode/
.idea/
*.csproj
*.unityproj
*.sln
*.suo
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db

# OS
.DS_Store
Thumbs.db

# 密钥
SECRETS/
*.pem
*.key
.env

# Android
*.keystore
*.jks
local.properties
.gradle/

# iOS
Podfile.lock
Pods/
*.ipa
*.dSYM.zip
*.dSYM

# 构建产物
Builds/
*.apk
*.aab
```

### 5.3 分支模型（简化 Git Flow）

```
main ────●──────────●──────────●──  (每次 tag 对应可发布构建)
          \        /          /
            dev ──●──●──●──●──●──  (集成分支，日常合并目标)
                  |  |
            feature/quiz-state-machine
            feature/word-pool
```

**规则**：
- `main`：仅接受来自 `dev` 或 `hotfix/*` 的 PR
- `dev`：日常集成，每个 Phase 结束打 tag 合入 `main`
- `feature/*`：每人一分支，从 `dev` 拉，活期不超过 3 天
- `hotfix/*`：从 `main` 拉，修完同时回 `main` + `dev`
- `release/*`：Phase 末准备发布时创建，测试稳定后合入 `main`

**禁止**：
- ❌ 直接 push 到 `main` 或 `dev`（保护分支）
- ❌ `feature` 分支活过 3 天（拆得太大，回炉重拆）
- ❌ `git push --force` 到公共分支

### 5.4 Commit Message 规范（Conventional Commits）

```
<type>(<scope>): <subject>

<body>

<footer>
```

**类型清单**：

| Type | 何时使用 |
|------|---------|
| `feat` | 新功能 |
| `fix` | Bug 修复 |
| `chore` | 构建、工具、脚本（如 Phase 0 骨架） |
| `refactor` | 重构（不改行为） |
| `perf` | 性能优化 |
| `test` | 单元测试 |
| `docs` | 文档 |
| `style` | 代码格式（不影响逻辑） |
| `ci` | CI/CD 相关 |

**scope 建议**：Core / Config / Battle / Quiz / WordPool / Reward / Supply / Learning / UI / Bootstrap / Editor / Test

**示例**：
```
feat(quiz): implement QuizStateMachine with 8-state transitions

- Added target lock mechanism for Elite/Boss zombies
- Fixed combo timer drift under pause
- Cover 8 EditMode test cases (80% coverage)

Refs: DevTasks.md F1.4, GDD 4.2.2
```

### 5.5 PR 模板

**`.github/PULL_REQUEST_TEMPLATE.md`**：
```markdown
## 变更内容
- [ ] 简述 1
- [ ] 简述 2

## 关联任务
- DevTasks.md Phase X 的 <task_id>
- Issue #xxx（若有）

## 类型
- [ ] feat / fix / refactor / perf / docs / chore

## 是否破坏性变更
- [ ] 是（说明原因 + 迁移步骤）
- [ ] 否

## 测试
- [ ] 已跑 EditMode 单元测试：<通过数/总数>
- [ ] 已跑 PlayMode 冒烟测试：<场景>
- [ ] 已手工验证：<具体步骤>

## 自查清单
- [ ] 接口签名符合 Architecture.md（若改接口已提 RFC）
- [ ] 无 magic number / FindObjectOfType / 每帧 GC
- [ ] EventBus 订阅配对 Unsubscribe
- [ ] public API 有 XML 注释
- [ ] Git commit 符合规范
```

---

## 6. 任务拆分方法

### 6.1 分层拆分模型（回顾）

```
Epic（Phase）→ Feature → Story → Task → Subtask
```

### 6.2 拆分颗粒度

**每个 Story 必须满足 INVEST 原则**：

| 原则 | 检查方式 |
|------|---------|
| Independent | 能否独立评审、独立合并？ |
| Negotiable | 需求方允许合理调整实现方式？ |
| Valuable | 完成后能演示或跑测试？ |
| Estimable | 工时估算误差 <±20%？ |
| Small | 单个会话（≤100k tokens）能装下？ |
| Testable | 有具体验收标准？ |

### 6.3 项目已定的任务清单

见 `DevTasks.md` 各 Phase 分解。

### 6.4 Sprint 计划

**每个 Sprint = 1 周**（周二开始，下周二结束）

**Sprint 内容固定**：
- 80% 计划内任务
- 20% 应对突发/Bug

**每日站会**（15 分钟，异步可用飞书/Slack）：
- 昨天做了什么
- 今天计划做什么
- 有阻碍吗

### 6.5 任务优先级：RICE 打分

| 因子 | 说明 | 打分 |
|------|------|------|
| Reach | 覆盖多少玩家 | 0-10 |
| Impact | 对核心玩法的影响 | 0.25/0.5/1/2/3 |
| Confidence | 完成信心 | 50% / 80% / 100% |
| Effort | 人月工作量 | 0.5-5 |

**分数 = R × I × C / E**，从高到低做。

---

## 7. Code Review 流程

### 7.1 四道关（缺一不可）

| 关次 | 内容 | 责任人 | 阻断 |
|------|------|--------|------|
| 1. 自审 | 提交前 diff 逐行看 | 提交者 | ✅ CI |
| 2. 自动化 | 编译 / 单测 / 格式检查 | CI | ✅ 阻断合并 |
| 3. 同伴评审 | 至少 1 人 Approve | Reviewer | ✅ 阻断合并 |
| 4. 架构合规 | 检查是否偏离 Architecture.md | 主程 | ✅ 关键 PR 阻断 |

### 7.2 评审 Checklist（贴在 CONTRIBUTING.md）

- [ ] 目录结构符合 Architecture.md 第 2 节
- [ ] 类命名规范（接口前缀 `I`、无 Impl 后缀）
- [ ] public API 有 `///` XML 注释
- [ ] 无 magic number（都走 SO 或 Constants）
- [ ] Update 中无 FindObjectOfType / GameObject.Find
- [ ] EventBus 订阅有对应 Unsubscribe（OnDestroy）
- [ ] 修改了公共接口 → 已同步 Architecture.md（走 RFC）
- [ ] 有对应单元测试或 PlayMode 测试
- [ ] 无性能回退（Profiler 采样对比）
- [ ] 引用的第三方库符合 TechStack.md 第 16 节

### 7.3 评审礼仪

- 批评代码，不批评人
- 提问比命令好："这里如果 xxx 会怎样？" vs "改成 xxx"
- 认可好的实现（🎉 / LGTM 也要说）
- 大 PR 拆小：>500 行改动要求提交者拆分

---

## 8. 功能验收体系

### 8.1 三层验收模型

```
┌─────────────────────────────────────┐
│ L1 自动化验收（每次提交）             │  ← CI 阻断
│  · 编译通过                          │
│  · EditMode 单元测试                 │
│  · 代码格式检查                      │
├─────────────────────────────────────┤
│ L2 会话验收（每次任务）               │  ← Agent 自查表
│  · DoD 清单逐项打勾                  │
│  · 关键路径手工跑通                  │
├─────────────────────────────────────┤
│ L3 Phase 验收（每次阶段末）           │  ← 主程评审
│  · PlayMode 测试通过                 │
│  · 性能指标达标                      │
│  · 完整试玩无阻断 bug                │
└─────────────────────────────────────┘
```

### 8.2 Definition of Done 模板（每个任务必附）

```markdown
## DoD
- [ ] 编译通过，0 warning
- [ ] 代码放在指定目录（Assets/Scripts/xxx）
- [ ] 所有 public API 有 XML 注释
- [ ] 关键方法有对应 EditMode 单元测试（≥ 80% 覆盖）
- [ ] 手工跑通指定场景
- [ ] 无性能回退（Profiler 采样对比）
- [ ] PR 已被同伴 Approve
- [ ] Git commit message 符合规范
- [ ] 若涉及接口变更，Architecture.md 已同步更新
```

### 8.3 Definition of Ready（开工前）

```markdown
## DoR
- [ ] 需求清晰（引用 GDD 具体章节）
- [ ] 依赖已就位（引用的类/接口都存在）
- [ ] 验收标准明确
- [ ] 工时估算 ≤ 2 天
- [ ] 无技术风险 or 风险已列出
- [ ] 目标平台已明确（Editor / Device）
```

---

## 9. 测试策略

### 9.1 三级测试

| 级别 | 类型 | 触发时机 | 责任人 |
|------|------|---------|--------|
| **单元** | EditMode Test | 每次提交 CI 自动跑 | 任务实施者 |
| **集成** | PlayMode Test | 每日 CI + 每次 PR | 任务实施者 |
| **系统** | 完整试玩 | Phase 结束 | QA / 主程 |

### 9.2 本项目必测清单

**EditMode 单元测试**（覆盖率 ≥ 80%）：
- QuizStateMachine：8 状态全路径 + 目标锁定不切换
- RewardCalculator：4 种星级 × 3 种结果 = 12 组合
- WordPool：补充触发 6 组边界（0/5/10/11/49/50）
- WaveController：5 波完整跑
- StarRatingEvaluator：阈值边界（80%/50%/90%/70%）
- EventBus：发布/订阅/取消/内存安全
- SaveManager：数据往返一致性

**PlayMode 测试**（3 个关键流程）：
1. 答题正确流程：进入关卡 → 3 只普通小怪兽 → 全答对 → 胜利
2. 大怪击败流程：大怪出现 → 基础武器连续答对 3 次（活力 600 归零）→ 掉落奖励
3. 失败退出：能量墙耐久归零 → 结算 50% 奖励 → 存档正确（**避难所无血量，不存在第二段失败判定**）

### 9.3 冒烟测试

每次 PR 合入 dev 前跑一遍：
```
SmokeTest.ps1
├─ 启动 Battle.unity
├─ 加载 1-1 关
├─ 自动答对前 3 题
├─ 触发击败
├─ 结算面板出现
├─ UI 节点存在性断言 + 关键文本断言（见下方说明）
└─ 输出 PASS / FAIL
```

> **视觉回归说明**：本版**不做像素截图比对**（无基准图、无容差定义、美术资源未定稿，像素比对会天天误报）。
> 代之以两项低成本断言：
> 1. **节点存在性**：结算面板 / 能量墙耐久条 / 题目面板 / 4 个选项按钮 均能找到且可交互
> 2. **文本断言**：界面不得出现 `$/¥/充值/购买去广告/排行榜` 等禁用词（与 Compliance.md §6 词表共用）
>
> 像素级视觉对比待 Phase 3 美术定稿后再评估（需先定义基准图目录与容差阈值）。

### 9.4 性能测试

Phase 1 末起每周五自动跑：
- Profiler 采样 60 秒战斗
- DrawCall <30 检查
- 内存峰值 ≤400MB（D23 修订，原 <150MB；低端机画质降级档另行测试）
- GC / Frame <1KB

---

## 10. Bug 追踪

### 10.1 Bug 分级

| 级别 | 定义 | 响应时限 | 举例 |
|------|------|---------|------|
| 🔴 P0 | 阻断主流程 | 2 小时内 | 无法进入关卡、崩溃 |
| 🟠 P1 | 严重功能缺陷 | 当天 | 奖励计算错误、存档丢失 |
| 🟡 P2 | 一般缺陷 | 3 天内 | UI 错位、动画异常 |
| 🟢 P3 | 优化建议 | 排入下个 Sprint | 文案调整、颜色微调 |

### 10.2 Bug 报告模板

```markdown
标题：[模块] 具体现象
严重度：P0 / P1 / P2 / P3
发现人：xxx
复现步骤：
1. ...
2. ...
3. ...
预期行为：xxx
实际行为：xxx
截图/视频：见附件
环境：Tuanjie 1.2.3 / PC Editor / 小米14
关联任务：Phase X 的 T.X.Y
```

### 10.3 Bug 生命周期

```
新建 → 分派 → 修复中 → 待验证 → 关闭
                    ↓
                    ↘ 无法复现 / 重复 / 不修 → 挂起
```

---

## 11. 移动 App 构建与发行专项

### 11.1 包体控制策略（移动 App）

**目标**：单包 **≤200MB**（GDD §12 D23 修订，v1.16 起生效：突破原 <80MB 约束，因美术扩容（敌人 30 种独立骨架+全模块品质上调）200MB 以内可直接过线；安装后总内存 ≤400MB）

| Phase | APK (arm64-v8a) | IPA | 手段 |
|-------|-----------------|-----|------|
| Phase 0 | <40MB | <40MB | 骨架阶段，仅 P1 首批骨架与占位美术 |
| Phase 1 | <80MB | <80MB | 第一章 + 核心 UI + 中文 SDF 字体 + 敌人 P1 批 9 种骨架 |
| Phase 2 | <140MB | <140MB | 多章节 + 分包 + 敌人 P2 批 11 种 + 场景/UI 提质资产 |
| Phase 3 | ≤200MB | ≤200MB | 全量内容（敌人 P3 批满 30 种 + BGM 重制/VO 转正）+ 预留 CDN |

> **包体口径（GDD §12 D14 定案 · D23 修订）**：原“仅因首发内置词库导致包体 >80MB 可接受”豁免条款**作废**，词库数据全额计入 **≤200MB 唯一硬线**；首发仍按 D14 内置小学→四六级核心词 + 人教/外研社，其余教材走 DLC（唯一真源见 [TechStack.md](./TechStack.md) §9）。

**具体手段**：
- **Android**：优先出 **AAB**（Google Play 强制），国内渠道可双出 APK；开启 ProGuard 混淆 + IL2CPP Strip Engine Code
- **iOS**：App Thinning 默认开；Bitcode 已废弃不需开
- **字体**：中文 TMP 字体资产 `ShelterHan`（思源黑体子集）只保留常用 3500 字（约 1.5MB）；生僻字由 `ShelterHanFB`（近全量 OFL CJK 后备资产，仅回退用，约 5–8MB）承接，**不用全量动态字体常驻**（二者均需并入包体口径核对 ≤200MB，见 TechStack §5.1）；拉丁(英/数字) `ShelterBaloo`（Baloo 2 子集）子集化；**均 SIL OFL 1.1 免费商用·子集化后按保留名重命名**（DIN/Roboto 弃用）；随包附 `OFL.txt` 与许可快照 + 设置内“开源许可/素材清单”页（Compliance §10 / UI §14）
- **纹理**：Android 使用 **ASTC 6×6**（主）/ **ETC2**（兼容）；iOS 使用 **ASTC**；图集无压缩 PNG 不打包，预压缩到 ASTC
- **音频**：BGM .ogg（Vorbis 96k）；短音效 .wav 16bit mono 22kHz；英文语音不上包，**启动后下载到 persistentDataPath**
- **Shader**：Graphics Settings 剔除 Unused Shader Variants，Build 时自动裁剪
- **构建后验**：Build Report 存档到 `docs/build-report/{date}.html`，每周比较变化

### 11.2 内容下载与热更新（Phase 3+）

**不依赖小游戏 CDN**，采用：
- **Android App Bundle** + **Play Feature Delivery**（Google Play）
- **iOS On-Demand Resources**（App Store）
- **自研 Addressables + 国内 CDN**（腾讯 COS / 阿里云 OSS）：**版号后国内上市时启用**（本版灰区词库全内置首包，英文语音/后续 DLC 走国内 CDN）；~~Google Play Feature Delivery / iOS ODR 因放弃海外不再是主渠道~~（能力保留）
- **热更新**：不上 C# 代码热更（IL2CPP 不支持），仅上 **SO 配置 + Addressables 资源**

### 11.3 平台 API 抽象

抽象层依然保留，但实现层改为 Android/iOS：

```csharp
public interface IPlatformService {
    string GetDeviceId();                    // Android: SSAID；iOS: IDFV
    void Vibrate(VibrateType type);
    bool IsNetworkAvailable { get; }
    UniTask<LoginResult> LoginAsync();       // 预留微信/QQ/Apple 登录（Phase 4）
    UniTask ShareAsync(ShareContent content);
    void ReportToAntiAddiction(UserProfile p);// 防沉迷上报（国内版号后必接，本版后置）
}
```

**实现**：`EditorPlatformService` / `AndroidPlatformService` / `IOSPlatformService`，运行时按 `RuntimePlatform` 注入。

> **契约真源说明（v1.15）**：`IPlatformService` 与新增的 `IAgeVerifyService`（年龄/家长验证）本版起**以 [Architecture.md](./Architecture.md) §4.12/§4.13 为接口契约真源**（此前因“海外路线不适用”仅在本文定义、未入契约）；本节代码块为移动端实现签名示意。防沉迷（`ReportToAntiAddiction`/时段限制）**后置至版号阶段**接入。

### 11.4 签名与上架

| 平台 | 签名 | 上架方式 |
|------|------|---------|
| **iOS（TestFlight 灰区 → App Store 中国区）** | Apple Developer Program（年费 99 USD）+ 开发者证书 | 版号前 TestFlight 分发；版号后 Xcode Archive → Transporter → IPA 上中国区 |
| **安卓官网 APK / 蒲公英（版号前灰区）** | 自签名 keystore（本地 / CI Vault） | 版号前唯一安卓分发路径；**不提审商店**，显式告知非商店渠道风险 |
| **国内安卓商店**（华为/小米/OPPO/vivo/应用宝） | 各自渠道 SDK 提供签名 | **版号前不上**（强制版号一票否决，Compliance.md §1.1）；版号后逐项接入 |
| ~~Google Play / 海外 App Store~~ | — | **因放弃海外不再作为发行目标**（旧路线已废） |

**密钥管理**：keystore / provision 文件存 `SECRETS/` 目录（已 .gitignore），CI 从 **GitHub Secrets** / **HashiCorp Vault** 拉取。

### 11.5 构建与自动化

| 触发条件 | 动作 |
|---------|------|
| PR 合入 dev | 跑单测 + Editor 构建 Windows 包给 QA |
| **每个工作日 06:00** | 仅自动构建 **Android Debug APK**（供真机冒烟）；**不自动发 iOS Nightly** |
| **每周一次（周五）** | iOS 构建上传 TestFlight（内部测试组）；频率受 Apple 审核/上传耗时限制 |
| 打 tag rc | 自动构建 Android Release AAB + iOS Release TestFlight，产物存云存储 |
| 打 tag release | 手动确认后上传各市场 |

**推荐工具**：
- **GitHub Actions** + Windows/Linux/macOS Runner 矩阵
- **fastlane**（iOS 上传 App Store / Android 上传 Play）
- **iOS TestFlight** + **安卓官网 APK / 蒲公英 / fir.im**（版号前灰区主分发渠道，不提审商店）
- ~~Google Play 内部测试轨道~~：因放弃海外不再使用

### 11.6 发行与合规（休闲益智类目 · **全年龄**，版号前灰区 → 版号后上市，纯广告变现）

> **发行决策（GDD §12 D22）**：产品定位**全年龄英语学习**；**国内主市场**双阶段——本版**无预算、不开版号**，仅做 **iOS TestFlight + 安卓官网 APK/蒲公英 灰区内测**（**不提审任何应用商店**）；积累留存/数据后**融资**→**申请版号**→**国内 iOS 中国区 + 安卓商店正式上市**。**已放弃海外发行**（App Store 海外区、Google Play 均不再作为目标），Steam/PC 暂缓。
>
> **变现边界（GDD §12 D1 已定案）**：商店界面**保留**，物品**只能用能量晶购买**；**能量晶不能用钱换**（不可充值、不可兑换）；全工程 **0 内购代码**，不引入 `com.unity.purchasing`；提审材料中不得出现 IAP 声明。**全年龄可投个性化广告、对已识别未成年降为非个性化**（版号前灰区广告收入极低、以数据验证为主）。

**开工启动项**（项目第 1 周内发起，与 Phase 0 并行）：

| 项 | 内容 | 周期 | 负责人 |
|----|------|------|--------|
| **软著** | 代码目录 + 60 页代码 + 需求说明书（**版号申请前置**，建议尽早办） | 1-2 个月 | 发行/法务 |
| **版号** | **本版不开（无预算）→ 融资后申请**；提前备齐软著/ICP/内容审查材料 | 申请 3-6 个月 | 发行/法务 |
| **防沉迷 SDK** | **后置至版号阶段**接入国家新闻出版署官方实名系统；本版仅预留 `IPlatformService`/`IAgeVerifyService`（契约见 Architecture.md §4.13） | — | 主程 |
| **隐私政策 + 用户协议** | 中文为主，含**未成年人个人信息保护专章** + 家长权利 | 2–4 周 | 法务 |
| **年龄识别 + 家长同意** | 首次启动阻塞流程；国内做法 = **手机号实名 + 运营商一键登录/号码认证**（客户端直连、不自建后端）、短信验证码备选；判定未成年且无家长同意则退出（Compliance C1 / `IAgeVerifyService`） | 1-2 周（UI + 逻辑） | 主程 |
| **广告 SDK 合规** | **国内主渠道 = 穿山甲**（`PangleAdsService`）/ 优量汇 备选；~~AdMob（海外）因放弃海外不再集成~~；全年龄默认个性化、未成年降级；频率上限 | 2 周 | 主程 |
| **内容自检** | 小怪兽题材命名已按 [Compliance.md](./Compliance.md) §6 术语表落实（三段式重命名）；剩余敏感词软化**已定案（GDD §12 D13：全量软化 + 代码标识符保留，“攻击力”按 Q5 保留）**，Phase 3 提审前执行文案替换（不阻塞 Phase 0-1） | 1–2 周 | 主策划 |
| **商店文案自检** | 商店/词库解锁/道具界面均只展示能量晶；**无 $/¥/充值入口/去广告购买/抽卡**（GDD §12 D1）；与 §6 词表共用 CI 文本断言 | 随 Phase 2 商店 UI 一并 | 主程 + 主策划 |

**风险与阶段路线（国内主市场 · 双阶段）**：
- ❗ **版号前（本版）**：不能上任何应用商店、不能开 IAP → 仅 **iOS TestFlight（≤1 万人）+ 安卓官网 APK/蒲公英** 灰区内测；**不提审商店**；广告收入极低，定位**留存/数据验证以融资**；需完整隐私政策 + 年龄/家长验证 + 未成年人保护，显式告知“非商店渠道”风险
- ❗ **无版号 → 国内商店一票否决**（华为/小米/OPPO/vivo/应用宝 + App Store 中国区均强制版号）；**教育类目**为规避版号的备选路径（Compliance §1.2，去游戏化改动量大、Phase 3 前不启动）
- ❗ **已放弃海外** ⇒ 不再投入海外本地化 / COPPA / GDPR / Google Play Families；核心中文用户在国内，与国内主市场定位自洽
- ✔ `IPlatformService`/`IAgeVerifyService` **本版起已纳入 Architecture.md 契约（§4.13/§4.12 预留实现）**（此前仅本文 §11.3 定义）；防沉迷真实接入后置版号阶段

**阶段路线**：
1. 本版灰区内测（TestFlight + 官网 APK，不提审商店）→ 验证留存/答题完成率/崩溃率（目标 3 个月，拿去融资）
2. 融资到账 → 备齐软著/ICP → **申请版号**（3-6 个月）
3. 版号后 → **国内 iOS 中国区 + 安卓商店正式上市**（分级 12+/全年龄，接入防沉迷 C6），灰度 5%→20%→100%

### 11.7 编辑器扩展（Phase 0 起建）

| 工具 | 用途 | 优先级 |
|------|------|--------|
| `LevelEditor.cs` | 可视化配置 5 波 | P0 |
| `WordImporter.cs` | CSV → SO | P0 |
| `BuildReportAnalyzer.cs` | 自动比较包体变化 | P0（新增） |
| `BalanceConfigEditor.cs` | 数值表批量导入 | P1 |
| `SpriteAtlasGenerator.cs` | 一键按目录生成图集 | P1 |
| `PerformanceProbe.cs` | Profiler 快照对比 | P2 |

---

## 12. 风险清单与应对

### 12.1 已识别关键风险

| # | 风险 | 概率 | 影响 | 应对 |
|---|------|------|------|------|
| R1 | Agent 会话跨上下文遗忘设计 | 高 | 高 | 每次开场强制读 6 份文档 |
| R2 | 接口契约漂移 | 高 | 高 | Architecture.md 唯一真源 + PR 检查 |
| R3 | 包体超 200MB（移动 App，D23 硬线；分 Phase 梯度预警见 §11.1） | 中 | 中 | Phase 1 起每周构建 Build Report，自动比较变化（新增 BuildReportAnalyzer） |
| R4 | 移动端性能不达标 | 中 | 高 | Phase 1 末真机 Profiler（需低中高三台） |
| **R5** | **儿童合规 / 隐私 / 广告合规**（无版号纯广告路线） | **高** | **高（缺一项过不了 Apple/Google 审核）** | **不开版号（决策已定）；开工即按 Compliance.md §8 时间线启动：家长同意弹窗 1 周、隐私政策 2-4 周、广告儿童标记随 SDK 接入** |
| R6 | 词库数据量爆炸 | 中 | 中 | Phase 2 Addressables 分片 + CDN |
| R7 | 单人上下文切换成本 | 高 | 中 | 每天限 2 个大任务 |
| R8 | Unity/Tuanjie 版本升级不兼容 | 低 | 高 | 项目内锁版本，Phase 结束前不升 |
| R9 | 小怪兽题材内容审查不过 | ~~中~~ **低** | 高 | **术语重命名已完成（僵尸→小怪兽/精英→大怪兽/BOSS 僵尸→BOSS 怪，Architecture 枚举保留 `ZombieType`）**；风险已转义为“商店与提审文案”→ 用 §6 词表做 CI 文本断言 + 商店截图人工复核；剩余敏感词已定案为全量软化（GDD §12 D13），Phase 3 提审前执行 |
| R10 | Android 碎片化（机型、分辨率、厂商 ROM） | 高 | 中 | 低中高三档机型 Profiler + 厂商适配 checklist |

### 12.2 兜底方案

- **每周架构评审 30 分钟**：检查是否偏离 TechStack / Architecture
- **Phase 中点检查**：卡住超过 2 天，拆分或降低范围
- **接口冻结期**：Phase 1 结束起，Architecture.md 中的接口进入冻结期，改动需正式 RFC

---

## 13. 常用命令与工具

### 13.1 Git 常用命令

```bash
# 拉最新 dev
git checkout dev
git pull --rebase

# 开新任务分支
git checkout -b feature/quiz-state-machine

# 提交（Conventional Commits）
git add .
git commit -m "feat(quiz): implement 8-state machine"

# 推远程
git push -u origin feature/quiz-state-machine

# 提 PR 后同步 dev
git fetch origin
git rebase origin/dev

# 打 tag
git tag -a v1.0.0 -m "Phase 0 baseline"
git push origin v1.0.0
```

### 13.2 Tuanjie 命令行构建（移动 App）

```powershell
# Android（Windows/Mac/Linux）
& "Tuanjie\Tuanjie.exe" -batchmode -quit `
    -projectPath . `
    -buildTarget Android `
    -playerSpaceColor -il2cpp `
    -buildOutFile ./Builds/Android/release.aab `
    -logFile ./BuildLogs/android.log

# iOS（仅 macOS）
/Applications/Tuanjie\Tuanjie.app/Contents/MacOS/Tuanjie -batchmode -quit \
    -projectPath . \
    -buildTarget iOS \
    -buildOutFile ./Builds/iOS/Raw \
    -logFile ./BuildLogs/ios.log
# Xcode 后置 Archive → Transporter 上传 IPA（需 fastlane 自动化）

> ⚠️ **参数待验证**：上述 Tuanjie CLI 参数名（-il2cpp / -buildOutFile / -androidKeystoreName 等）需在 Phase 0 第 1 周用 `-batchmode -h` 与官方文档实测校准；若参数不存在，备选方案 = 编辑器菜单脚本 `BuildPipeline.BuildPlayer`（Editor 脚本入口，参数由 BuildScript.cs 统一管理）。签名信息一律环境变量注入，不落盘。

# 签名注入（从环境变量读，不写盘）
-androidKeystoreName $env:KS_NAME `
-androidKeystorePass $env:KS_PASS `
-androidKeyaliasName $env:KS_ALIAS `
-androidKeyaliasPass $env:KS_ALIAS_PASS
```

### 13.3 Git LFS 维护

```bash
# 查看当前 LFS 追踪
git lfs ls-files

# 手动清理本地 LFS 缓存
git lfs prune
```

---

## 14. FAQ 常见问题

### Q1：首次打开项目编译失败

**排查步骤**：
1. 检查 Library 是否生成中（右下角进度条）
2. Package Manager 中是否有红色感叹号 → 点击 Resolve
3. 若报 UniTask / DOTween 找不到 → 检查 manifest.json 或手动 import
4. 删 Library 目录 + Temp 目录，重开项目

### Q2：`.meta` 文件频繁冲突

**原因**：多人同时新增资源，或有人改了资源路径没提交 .meta

**解决**：
- 每次新增资源前 `git pull --rebase`
- Editor 中重命名/移动资源必须一起提交 .meta
- 冲突时优先保留 .meta 中的 GUID

### Q3：Sprite Atlas 引用丢失

**原因**：Atlas 里的源图被 LFS 未下载

**解决**：
```bash
git lfs pull
# 或 Tuanjie → Assets → Reimport All
```

### Q4：包体异常变大

**排查**：
- **Build Report**（Window → Analysis → Build Report Online 或存档 HTML）查看每个模块占比
- 常见凶手：Unity 内置 Shader 过多、某个音效忘压缩、测试资源没剔除、ASTC 没启用回退到 RGBA32
- 开启 **Strip Engine Code** + **ProGuard minify**
- 使用 Addressables 分资源包下载到 CDN

### Q5：Agent 会话产出的代码不遵守接口

**处理**：
1. 立即停止合并该分支
2. 回炉会话开场，明确列出必须遵守的接口签名
3. 若已投入工作量大，考虑**修改文档 → 评审 → 再合并**（不轻易改代码）

### Q6：跨设备文件换行符 CRLF/LF 混乱

**预防**：`.gitattributes` 中声明：
```
* text=auto eol=lf
*.cs text eol=crlf
*.bat text eol=crlf
```

---

## 15. 术语表

| 术语 | 说明 |
|------|------|
| **DoD** | Definition of Done，验收标准 |
| **DoR** | Definition of Ready，开工前提 |
| **RFC** | Request for Comments，接口变更提案 |
| **PR** | Pull Request，合并请求 |
| **LFS** | Git Large File Storage |
| **URP** | Universal Render Pipeline |
| **SO** | ScriptableObject |
| **Epic** | 大颗粒任务集合（本项目 = Phase） |
| **Story** | 用户故事，可演示的中等颗粒 |
| **Task** | 单个开发者可完成的具体任务 |
| **Sprint** | 冲刺周期，本项目 1 周 |

---

## 16. 变更历史

| 版本 | 日期 | 变更 | 责任人 |
|------|------|------|--------|
| v1.0 | 2026-11-05 | 初版发布：项目初始化 / 版本控制 / 任务拆分 / 验收体系 / 小游戏专项 / FAQ | 主程 |
| v1.1 | 2026-11-05 | 同步升至 v1.1：关联文档 TechStack/Architecture/DevTasks 版本升至 v1.1；附录 C 开场引用仍为 5 份（内容一致）；无流程变更 | 主程 |
| v1.2 | 2026-11-05 | **平台方向调整**：放弃微信/抖音/团结小游戏 → **移动 App（Android + iOS）**；类目确定**休闲益智**；§2.1 软件清单剔除微信开发者工具，新增 Android Studio/Xcode/JDK/CocoaPods；§5.2 .gitignore 剔除小游戏目录，新增 Android/iOS 条目；§11 全重写为移动 App 构建与发行专项（签名/上架/热更新/合规）；§12.1 风险清单新增 R5 版号与 R9/R10；§13.2 构建命令改为 Android/iOS 实例 | 主程 |
| v1.3 | 2026-11-05 | **发行策略定案**：**不开版号 + 纯广告变现（0 内购）**，新增关联文档 [Compliance.md](./Compliance.md) v1.0 作为合规唯一真源；§11.2 国内 CDN 描述改为暂缓；§11.4 国内安卓市场标注暂不上架；§12.1 R1 改 6 份文档、R5 重写为儿童合规/隐私/广告合规；A9 改为 Compliance §8 Week 0 启动项；附录 B/C 升级为 6 份文档；术语软化已落实（僵尸→小怪兽/精英僵尸→大怪兽/BOSS 僵尸→BOSS 怪） | 主程 |
| v1.4 | 2026-11-05 | **主程二审残留修复**：§0 开场引用 5→6 份；§2.1 可选插件剔除 `com.unity.purchasing`（0 内购）；§2.4 SECRETS 示例剔除微信残留改为 keystore/证书；§3.1 首提文档 5 份 v1.1→6 份 v1.3；§3.4 “Target Scene Framework”笔误修正为 Minimum/Target API Level + keystore 注入措辞修正；§11.6 防沉迷行改“暂缓”、广告 SDK 行改 AdMob 优先；§13.2 增加 CLI 参数待验证警告与 BuildPipeline 兜底 | 主程 |
| v1.5 | 2026-11-05 | **四项定案（GDD §12 D1-D4）落入开工流程**：<br>**D1** §11.6 新增“变现边界”引言与“商店文案自检”行（商店保留 / 只能量晶 / 不可充值）<br>**D2** §9.2 PlayMode 失败用例注明“避难所无血量、不存在第二段判定”；全文标识符随 Architecture v1.4 重命名（`IWall`/`WallManager`/耐久）<br>**D3** 风险表 R9 改写：术语重命名已完成→风险转义为商店/提审文案，概率降为低<br>**D4** 附录 C 关键约束新增“不做三会/不做排行榜”；§11.5 与 §11.6 剔除蒲公英分发描述，改为 TestFlight + Play 内部测试轨道<br>**开工阻塞解除**：附录 **A9 拆为 A9-a（行政/法务）/ A9-b（敏感词文案→降级为 Phase 3 前置）/ A9-c（变现边界）**，不再形成 Phase 0 开工阻塞；A4 新增锁定横屏<br>**其它**：§9.3 截图对比（视觉回归）**降级为节点存在性 + 文本断言**（无基准图/无容差）；§11.5 每日 iOS TestFlight Nightly 改为**仅 Android 每日、iOS 每周五**；§2.3 补 iPad 支持声明与 TechStack §17 对齐；§2.4 pangle 密钥标“暂不用”；关联文档版本串升级 | 主程 |
| v1.6 | 2026-11-05 | **GDD §12 Q1-Q8 拍板转定案 D8-D14 同步开工规范**：<br>**D8** §9.2 PlayMode “大怪击倒→击败”、改为“基础武器连续答对 3 次（活力 600 归零）”；§9.3 冒烟“触发击杀→触发击败”<br>**D13** Q6 定案为全量软化：§11.6 内容自检行、风险 R9、附录 A9-b 均从“待拍板/待定稿”改为“已定案 D13（全量软化 + 代码标识符保留，“攻击力”按 Q5 保留）”<br>**D14** §11.1 包体表新增口径注：仅因首发内置词库（小学→四六级 + 人教/外研社）导致 >80MB 可接受，硬约束对非词库内容仍适用<br>基线行与关联文档版本串升至 GDD v2.3 / TechStack v1.6 / Architecture v1.5 / DevTasks v1.5 / Compliance v1.2（本文 v1.6） | 主程 |
| v1.7 | 2026-11-05 | **GDD §12 D15/D16（战场几何 + 技能包必用策略层）同步开工规范**：附录 C 关键约束补充“技能包不产生脱离答对的自动/持续掉血、伤害一律由答对触发（D8/D16）；当前锁定目标恢复推进 30% 速度（D15）”；基线行、附录 C 与关联文档版本串升至 GDD v2.4 / TechStack v1.7 / Architecture v1.6 / DevTasks v1.6 / Compliance v1.3（本文 v1.7） | 主程 |
| v1.8 | 2026-11-06 | **玩法增味（GDD §12 D17/D18/D19）同步开工规范**：附录 C 关键约束新增“不新增 `ZombieType`（变体走 `EnemyModifier`）、武器技能/转化包伤害仍由答对触发、护盾只吸收首次答对伤害、Mender 回复为敌方机制非玩家伤害、`BattleDoctrine` 仅本局不写存档”；基线行、附录 C 与关联文档版本串升至 GDD v2.5 / TechStack v1.8 / Architecture v1.7 / DevTasks v1.7 / Compliance v1.4（本文 v1.8） | 主策划 |
| v1.9 | 2026-11-06 | **玩法增味 II（GDD §12 修订 D15、新增 D20）同步开工规范**：附录 C 关键约束将 D15 改为“全程正常推进（取消 30% 减速）+ 锁定目标 view 高亮”、玩法增味行补 D20 游走怪（`EnemyModifier.Wanderer`、不新增 `ZombieType`、不攻墙不计通关）；基线行、附录 C 与关联文档版本串升至 GDD v2.6 / TechStack v1.9 / Architecture v1.8 / DevTasks v1.8 / Compliance v1.5（本文 v1.9） | 主策划 |
| v1.10 | 2026-11-06 | **认词正反馈显性化（GDD §12 D21）同步开工规范**：附录 C 关键约束新增 D21——**只做表现（M1 头顶记忆强度）/奖励（M2 首克新词、攻克错题一次性晶，游戏内产出、不涉广告）/阈值节奏（M3 Mastery Meter 只动 Swift/连击阈值、不动伤害），不新增伤害来源**；玩法增味行补 D21、输入文档与基线行/关联文档版本串升至 GDD v2.7 / TechStack v1.10 / Architecture v1.10 / DevTasks v1.10 / Compliance v1.7（本文 v1.11） | 主策划 |
| v1.11 | 2026-11-06 | **主程审核同步（基线串升版）**：审核确认开工规范与 GDD 定案无冲突；修复 DevTasks v1.9 遗留的版本串漂移（本文输入文档/基线行同步 GDD v2.8 / TechStack v1.11 / Architecture v1.10 / DevTasks v1.10 / Compliance v1.7）；同时登记 GDD 侧待拍板审核项（武器射速口径/星级答对率分母/建筑费用双轨/金币材料归一）到风险沟通清单，实质条款未变 | 主程 |
| v1.12 | 2026-11-06 | **十项拍板级联（GDD v2.9 / TechStack v1.12 / Architecture v1.11）开工规范同步**：本文实质条款未变，仅将开场基线行（L26）与附录 C 会话开场引用格式的版本串升至 GDD v2.9 / TechStack v1.12 / Architecture v1.11 / DevTasks v1.11 / ProjectSetup v1.12 / Compliance v1.8；动画/图集规范变更归 TechStack/Architecture/美术表，开工流程不变 | 主程 |
| v1.13 | 2026-11-06 | **字体归一同步开工规范（同步 GDD v2.10 / TechStack v1.13 §5.1 / Compliance v1.9 §10）**：包体优化节“字体”行改写——中文 `ShelterHan` 常用 3500 字子集 + 动态 Fallback 防生僻字、拉丁 `ShelterBaloo` 子集化，**均 OFL 免费商用·子集化后重命名**（DIN/Roboto 弃用），随包附 `OFL.txt` 与许可快照；本文其它开工条款未变；开场基线行（L26）与附录 C 会话开场引用格式版本串升至 GDD v2.10 / TechStack v1.13 / Architecture v1.12 / DevTasks v1.12 / ProjectSetup v1.13 / Compliance v1.9 | 主程 |
| v1.14 | 2026-11-07 | **字体评审发现同步开工规范（同步 GDD v2.11 / TechStack v1.14 §5.1 / Compliance v1.10 §10）**：包体节字体口径闭环——旧写“静态裁剪 3500 + 全量动态 Fallback”互斥，定为一套：静态主资产 `ShelterHan`（3500）+ 近全量 OFL CJK 后备资产 `ShelterHanFB`（挂 TMP Fallback、约 5–8MB、仅生僻字回退、计入包体口径 <80MB 核对），**默认不用全量动态字体常驻**；新增随包 `OFL.txt` + 许可快照 + 设置内“开源许可/素材清单”页（UI §14 承接）；本文其它开工条款未变；开场基线行（L26）与附录 C 会话开场引用格式版本串升至 GDD v2.11 / TechStack v1.14 / Architecture v1.13 / DevTasks v1.13 / ProjectSetup v1.14 / Compliance v1.10 | 主程 |
| v1.15 | 2026-11-07 | **发行路线调整同步开工规范（同步 GDD v2.12 D22 / Compliance v1.11 / TechStack v1.15 / Architecture v1.14）**：<br>header 目标平台行与基线行（L26）、附录 C 会话开场引用版本串全量升至 GDD v2.12 / TechStack v1.15 / Architecture v1.14 / DevTasks v1.14 / ProjectSetup v1.15 / Compliance v1.11；**§11.6 发行与合规**整节重写为**国内主市场双阶段**（本版无预算不开版号→TestFlight+官网 APK 灰区内测、不提审商店→融资→申请版号→国内 iOS 中国区+安卓商店上市；放弃海外、Steam 暂缓），“背推路线”改阶段路线；**家长同意弹窗**行改**年龄识别 + 家长同意（手机号实名 + 运营商一键登录、客户端直连不自建后端）**、**防沉迷 SDK** 改后置版号阶段、**广告 SDK** 改国内主穿山甲/AdMob 划废；§11.4 签名上架表、§11.5 内测分发（TestFlight+官网 APK/蒲公英）、§11.2 CDN（版号后国内启用）、§11.3 ReportToAntiAddiction 注释与契约真源说明、§2.4 密钥（pangle/优量汇 + 运营商认证 AppKey，去 AdMob）同步；`IPlatformService`/`IAgeVerifyService` 本版起已入 Architecture §4.12/§4.13 契约 | 主程/制作人 |
| v1.16 | 2026-11-07 | **敌人种类扩张与包体重建同步开工规范（同步 GDD v2.13 D23 / TechStack v1.16 / ProjectSetup 本表 §11.1 为 Build 阈值真源）**：<br>**§11.1 包体梯度重建**：目标 <80MB→**≤200MB 唯一硬线**；Phase 梯度 P0<40/P1<80/P2<140/P3≤200MB（旧 25/50/65/80 废除），各 Phase 手段列补敌人三批骨架（P1 9/P2 11/P3 满 30）与提质资产；D14 词库 >80MB 豁免注改写作词库全额计入 200MB；字体行包体核对口径同步 ≤200MB<br>**§9.4 内存峰值** <150MB→≤400MB（低端降级档另测）；**风险 R3** 改“包体超 200MB”；附录 C 关键约束补 D23 species 口径（不新增枚举值）<br>header/基线行（L26）/附录 C 会话开场引用串全量升至 GDD v2.13 / TechStack v1.16 / Architecture v1.15 / DevTasks v1.15 / ProjectSetup v1.16 / Compliance v1.12 | 主程 |

---

## 附录 A：开工前 10 项 Checklist（打印版）

按顺序逐项打勾，全部完成即可开始 Phase 0：

- [ ] **A1** Git 仓库建立 + LFS 配置 + .gitignore 就位（含 Android keystore / iOS Pods 剔除）
- [ ] **A2** Tuanjie 项目创建 + 6 份基线文档提交（含 Compliance.md）
- [ ] **A3** Package 依赖锁定（TechStack.md 第 16 节）
- [ ] **A4** Editor 设置一次性完成（Asset Serialization、Color Space、IL2CPP、ARMv7+ARM64、**锁定横屏 Landscape Left**）
- [ ] **A5** 目录结构按 Architecture.md 第 2 节建立（每目录 README.md）
- [ ] **A6** IDE 就绪（Rider / VS + 代码风格 + EditorConfig）
- [ ] **A7** Android SDK + JDK 17 + Xcode + CocoaPods 安装验证（命令行能构建 Hello World）
- [ ] **A8** 开发签名 keystore + iOS 开发者证书就位（存 SECRETS/，不入 Git）
- [ ] **A9-a** **合规启动项**（按 Compliance.md §8 Week 0，**纯行政/法务，不阻塞写代码**）：Apple Developer $99 + Google Play $25 账号注册 / 隐私政策起草启动
- [ ] **A9-b** **敏感词与文案（GDD §12 Q6 → 已定案 D13）—— 降级为 Phase 3 前置项，不阻塞 Phase 0**：术语重命名与代码标识符（`IWall`/`WallManager`/`ZombieType`）**已完成**；软化已拍板为**全量软化 + 代码标识符保留**（“攻击力”按 Q5 保留），主策划据 Compliance §6.2 执行清单于 Phase 3 提审前替换文案
- [ ] **A9-c** **变现边界确认（GDD §12 D1）**：manifest 无 `com.unity.purchasing`；商店只收能量晶；无充值入口（Phase 0 只要空实现不违规即可）
- [ ] **A10** 首次提交打 tag `v0.0.0-baseline`

---

## 附录 B：新成员 Day 1

- 上午：读 6 份文档（GDD/TechStack/Architecture/DevTasks/ProjectSetup/Compliance）
- 下午：跑一遍第 1 节"快速开始"
- 傍晚：认领一个 `good-first-issue`（如：修一个 P3 UI Bug）

---

## 附录 C：Agent 会话开场通用引用格式

```markdown
## 输入文档（必读，逐份读，不可摘要）
1. Assets/Design/GDD.MD v2.13
2. Assets/Design/TechStack.md v1.16
3. Assets/Design/Architecture.md v1.15（重点第 X 节）
4. Assets/Design/DevTasks.md v1.15（重点 Phase X 的 <task_id>）
5. Assets/Design/ProjectSetup.md v1.16（本文件）
6. Assets/Design/Compliance.md v1.12（涉及文案/广告/数据/UI 时必读）

## 关键约束
- 不修改 Architecture.md 中的接口签名
- 不引入未在 TechStack.md 第 16 节列出的第三方包（**尤其不得引入支付/内购 SDK**）
- 玩法数值只有一套：能量墙耐久（`IWall`）；**避难所无血量**；小怪兽进度用**活力值** `MaxVitality/CurrentVitality`（伤害制，GDD §12 D8），**不得引入 HP/RequiredAnswers 命名**；**技能包不产生脱离答对的自动/持续掉血，伤害一律由“答对”触发（GDD §12 D8/D16）**；**D15 v2.6：锁定与非锁定全程正常速度推进（取消不前进/30% 减速），当前锁定目标仅 view 层高亮**；**D21 学习即弹药只做表现/奖励/阈值节奏，不新增任何伤害来源（伤害仍只由答对触发）**
- **玩法增味（GDD §12 D15 修订/D17/D18/D19/D20/D21，规格 §2.6/§2.6.5）**：不新增 `ZombieType`（机制变体与稀有游走怪均走 `EnemyModifier`，守 D3；**D23：30 种敌人属 species 数据层 `ZombieSpeciesConfig`+独立骨架，同样不新增枚举值**）；武器技能与转化包伤害仍由“答对”触发；护盾只吸收首次答对的伤害（非自动掉血）、Mender 回复属敌方机制非玩家伤害；`BattleDoctrine` 仅本局生效不写存档（守 D9）；**D20 游走怪停止推进不攻墙、不计入通关、掉落走 `ZombieConfig.dropOverride`**；**D21 只做表现（M1 头顶记忆强度）/奖励（M2 首克新词、攻克错题一次性晶，游戏内产出、不涉广告）/阈值节奏（M3 Mastery Meter 只动 Swift/连击阈值、不动伤害）**
- 不做三会（仅 `IWordStudyService` + NoOp）；不做排行榜
- 遵守本文第 5 节 Git 规范
- 遵守本文第 8 节 DoD 标准
```

---

> **使用说明**：
> 1. 本文档面向所有开发者与 Agent 会话，作为项目"操作手册"
> 2. 任何流程变更须先更新本文档 → 团队评审 → 通知 Agent 会话
> 3. 与新 Agent 会话时，第 5 项引用必带本文路径
