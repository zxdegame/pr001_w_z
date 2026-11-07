# 开场任务提示词：模板 + 示例

对齐 `DevTasks.md` 各 Phase 的“复制以下内容到新会话”块。会话开场前先读六份文档取当前版本串（**不硬编码**）。

## A. 填空模板

```markdown
# 任务：<Phase X · 任务名>

## 项目背景
- 项目：避难防线 - 词汇守卫者（Tuanjie 2D 英语学习塔防）
- 目标：<本任务产出边界；做什么 / 不做什么>

## 输入文档（必须先读）
1. Assets/Design/GDD.MD（当前版本；重点 §12 已定案 D1–D23、§<相关小节>）
2. Assets/Design/TechStack.md（当前版本；§<...>）
3. Assets/Design/Architecture.md（当前版本；**接口不可改动**；§<...>）
4. Assets/Design/DevTasks.md（当前版本；本任务描述）
5. Assets/Design/ProjectSetup.md（当前版本；环境与协作规范）
6. Assets/Design/Compliance.md（当前版本；§2 / §3 / §6 门禁）
（版本号读各文档头部“文档版本 / 关联文档”行填写，勿写死）

## 已确认关键决策（不可更改）
<从 DevTasks.md 对应 Phase 原样搬运>

## 目标产出（严格按 Architecture.md 第 17 节 + DevTasks.md 本 Phase）
- 目录：<Assets/Scripts/...>
- 类 / 接口 / 事件 / 场景：<逐条>

## 验收标准（DoD）
- [ ] 编译通过，0 warning
- [ ] 代码放在指定目录
- [ ] 所有 public API 有 XML 注释
- [ ] 关键方法有 EditMode 单测（≥80% 覆盖）
- [ ] 手工跑通指定场景
- [ ] 无性能回退（Profiler 采样对比）
- [ ] Git commit message 符合规范

## 开工前必答（DoR）
- [ ] 需求引用 GDD 具体章节
- [ ] 依赖接口 / 类都已存在
- [ ] 工时 ≤ 2 天、风险已列出
- [ ] 目标平台明确（Editor / Device）

## 约束提醒
- 接口 / 事件改动 → 先改 Architecture.md 走评审，不得直接改代码
- 不硬编码数值（走 SO）；跨系统走 EventBus；禁每帧 Find
```

## B. 完整示例（Phase 1 · 答题状态机）

```markdown
# 任务：Phase 1 · QuizStateMachine 实现

## 项目背景
- 项目：避难防线 - 词汇守卫者（Tuanjie 2D 英语学习塔防）
- 目标：实现 8 态答题状态机与其 EditMode 测试；不做词库补充策略（属 F2.x）

## 输入文档（必须先读，取当前版本串）
- GDD.MD §4.2 答题流程 / §12 D8 伤害模型
- TechStack.md（异步 UniTask、UI 章节）
- Architecture.md（Quiz 契约：IQuizTargetProvider / IQuestionPresenter / QuizEvents / QuizState 枚举）——**接口不可改**
- DevTasks.md Phase 1 F1.4
- ProjectSetup.md（协作规范）
- Compliance.md §6（敏感词，答题文案）

## 已确认关键决策（不可更改，摘自 DevTasks）
- Word=class；答对=一次暴击，伤害=武器攻击力×200%；答错/超时与哨塔不削减活力
- 敌人 Vitality 模型（非 HP）
- 事件解耦：Quiz 通过 QuizEvents 通知 Battle/Wave，不直接引用

## 目标产出（对齐 Architecture §17）
- Assets/Scripts/Quiz/QuizStateMachine.cs（8 态状态机；时限用 UniTask）
- Assets/Scripts/Core/Events/QuizEvents.cs（若缺依契约补 struct，走 RFC）
- Assets/Tests/EditMode/QuizStateMachineTests.cs（8 态全路径）

## DoD
- [ ] 编译 0 warning；QuizStateMachine 放 Assets/Scripts/Quiz/
- [ ] public API 有 XML 注释；无 magic number（时限走 QuizConfig SO）
- [ ] 8 态转换均有 EditMode 覆盖（≥80%）
- [ ] EventBus 订阅在 OnDestroy 配对退订
- [ ] 手工跑通：Battle 场景答对触发暴击、答错触发嘲讽气泡事件
- [ ] commit：feat(quiz): implement QuizStateMachine with 8-state transitions

## DoR
- [ ] 依赖 IQuestionPresenter / WordDatabase 已存在（Phase 0 已交付）
- [ ] 工时 1.5 天；风险：状态计时与暂停交互（已列）
```

## C. 各 Phase 提示词真源
Phase 0 / 1 / 2 / 3 的“已确认关键决策 + 目标产出”以 `DevTasks.md` 对应章节为准；本文件不复制数值，只示范结构。
