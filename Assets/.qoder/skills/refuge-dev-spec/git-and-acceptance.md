# Git 与验收（真源：ProjectSetup.md §5–§8）

## Conventional Commits
```
<type>(<scope>): <subject>

<body>

<footer：Refs: DevTasks.md Fx.y, GDD a.b.c>
```
- Type：`feat / fix / chore / refactor / perf / test / docs / style / ci`（Phase 0 骨架用 `chore`）
- scope：Core / Config / Battle / Quiz / WordPool / Reward / Supply / Learning / UI / Bootstrap / Editor / Test

## 分支模型（简化 Git Flow）
- `main`（保护）· `dev`（集成，每 Phase 末打 tag 合 main）· `feature/*`（从 dev 拉，活期 ≤3 天）· `hotfix/*`（从 main，回灌 main + dev）· `release/*`
- ❌ 直推 main / dev · ❌ feature 活过 3 天 · ❌ 公共分支 `--force`

## PR 自查清单
- 接口签名符合 Architecture.md（改接口已提 RFC）
- 无 magic number / FindObjectOfType / 每帧 GC
- EventBus 订阅配对 Unsubscribe
- public API 有 XML 注释
- Git commit 符合规范
- 已跑 EditMode（通过数 / 总数）、PlayMode 冒烟、手工验证

## Code Review 四道关
1. 自审（提交者，diff 逐行）→ CI
2. 自动化（编译 / 单测 / 格式）→ 阻断合并
3. 同伴评审（≥1 Approve）→ 阻断合并
4. 架构合规（主程查是否偏离 Architecture.md）→ 关键 PR 阻断
- 大 PR >500 行要求拆小；批评代码不批评人

## 三层验收
| 层 | 内容 | 时机 |
|---|---|---|
| L1 自动化 | 编译 / EditMode / 格式 | 每次提交（CI 阻断）|
| L2 会话 | DoD 逐项 + 关键路径手跑 | 每次任务（Agent 自查表）|
| L3 Phase | PlayMode / 性能达标 / 完整试玩无阻断 bug | 阶段末（主程评审）|

## DoD 模板（每任务必含 ≥5 项）
编译 0 warning / 目录正确 / public API XML 注释 / 关键方法 EditMode 单测 ≥80% / 手工跑通场景 / 无性能回退 / PR 已 Approve / commit 规范。
**Agent 会话结束前必须逐项列出 DoD 完成情况。**

## DoR（开工前确认）
需求清晰（引用 GDD 章节）/ 依赖就位 / 验收明确 / 工时 ≤2 天 / 风险列出 / 目标平台明确。

## 必测清单（本项目）
QuizStateMachine 8 态全路径 · RewardCalculator 4 星 ×3 结果=12 组 · WordPool 补充优先级 6 边界 · WaveController 5 波完整跑 · SaveManager 数据往返一致。

## 执行环境注意（本仓库已配）
- **PowerShell 5.1：提交信息必须纯 ASCII**（中文会被当 GBK 解码致乱码）；或改用 Git Bash 提交
- 推送 GitHub：走 scoped 代理 `git config http.https://github.com.proxy http://127.0.0.1:7890`（需 Clash 在线）+ GCM `credential.helper manager`（浏览器 / 设备码登录）
- 二进制资源走 LFS（`.gitattributes`）；push 时自动上传 LFS 对象
- 撤销代理 / GCM：`git config --global --unset credential.helper` / `--unset http.https://github.com.proxy`
