---
name: refuge-ui-gen
description: 避难防线（词汇守卫者）项目的 UI 生成提示词规范。用于在 Tuanjie/Unity 的 uGUI+TMP+DOTween 技术栈下撰写“生成某界面/面板”的开发提示词、拆解 UI 任务、对齐 Canvas 三层/横屏适配/字体 ShelterHan·ShelterBaloo/通用组件复用/数据绑定到 Architecture 接口/合规门禁（首启年龄验证、无账号、单货币能量晶、禁系统 emoji）。当用户要生成或实现某个界面（主界面/HUD/答题面板/结算/商店/档案室/年龄验证等）、写 UI 开场任务描述、或问 UI 该怎么下提示词时自动应用。Use when generating or specifying a UI screen, panel, or prefab for this project.
---

# 避难防线 · UI 生成提示词规范 (refuge-ui-gen)

主程序 / UI 实现用的界面生成提示词标准。凡在本项目要“做一个界面/面板/弹窗”、拆 UI 任务、或让 AI 产出 UI prefab + 脚本 + 绑定时，按本规范构造提示词并对齐纪律。配套通用开发规范见姊妹技能 `refuge-dev-spec`。

## 何时触发
- 用户要生成 / 实现某个界面或面板（00–17 任一）
- 写 UI 相关的开场任务描述 / 子任务拆分
- 用户问“UI 该怎么给 AI 下提示词 / 界面规范是什么”

## 0. 真源三分（最高优先，违反即返工）
UI.md **是 GDD §7 的下游细化文档、非基线真源**。生成任何界面前，按下述真源取值，**不得以 UI.md 数值为准**：
1. **布局 / 功能 / 交互语义** → `GDD.MD` §7（UI.md 只是细化）
2. **技术实现（框架 / 字体 / 图集 / 适配）** → `TechStack.md`（§UI / §5·§5.1 字体 / §2 图集 / §17 适配）
3. **场景 / 接口 / 状态机契约** → `Architecture.md`（§10 场景 · §4.x 接口 · `QuizState` · §2 目录 · §5.1 EventBus）
4. **数值 / 价格** → `GameConfig` 等 ScriptableObject 单点，禁硬编码
> 版本号 / guid / 具体数值一律**运行时读文档头与实测取**，不写死（见 pitfalls）。

## 1. UI 生成提示词六段式模板
```markdown
# 任务：UI · <界面编号+名>（如 07 答题面板 / 06 战斗 HUD）

## 1) 归属与形态
- 场景：<MainMenu/Battle/Result/…>；Canvas 层：<Background/Gameplay/Popup>；形态：<全屏/Popup/常驻 HUD>
- 真源：GDD §7.x.y（布局）· UI.md §<n>（细化元素表）· Architecture §<n>（状态机/接口）

## 2) 输入文档（先读，取当前版本）
GDD §7 / TechStack(UI·字体·图集·适配) / Architecture(场景·§4.x 接口·QuizState) / UI.md §<n> / Compliance §2·§3·§6（若涉广告/验证/文案）

## 3) 结构产出（prefab 层级 + 组件）
- 优先用通用件拼装（见 components-and-screens.md）：UIBtn* / PopupRoot / ConfirmDialog / Toast / RewardFly / CountUpText / StarRating / WordCard / TabBar
- TMP 文本一律 ShelterHan（中文）/ ShelterBaloo（拉丁英数）；符号走 `ic_*`，禁系统 emoji
- 布局用 Layout Group + 锚点，禁按 1920×1080 绝对摆死；顶栏关键读数进 SafeAreaView

## 4) 数据绑定（到接口/服务/事件，不引具体类实现）
逐元素列：控件 → 绑定源（IWall / PlayerData / ComboService / IMasteryMeterService / IQuizTargetProvider / IQuestionPresenter / ILevelService / IAgeVerifyService …）→ 更新频率 → 触发的 EventBus 事件
- 跨系统只经事件 / provider，view 层表现（高亮环、连线、气泡）不新增业务接口字段

## 5) 交互与状态
按真源状态机逐项给表现（如 QuizState：IDLE/QuestionGenerated/AnswerSelected/AnswerResult/Cooldown/Timeout）；答错 = 僵尸嘲讽气泡 + 连击清零 + 正确项脉冲；玩家无受伤动作、无血腥（D13）

## 6) 验收（DoD）
- [ ] prefab 归位（Assets/UI 或对应场景）· Canvas 层正确
- [ ] 字体/图集正确、打开后 DrawCall ≤30
- [ ] 横屏 + 18:9/20:9 + 刘海 SafeArea 无溢出/裁切
- [ ] 命中区 ≥96×96、按钮间距 ≥24（答题键 ≥160×96）
- [ ] 绑定走接口/事件、无 magic number、无每帧 Find
- [ ] 动效参数符合 ui-baseline.md 表；public API XML 注释
- [ ] 涉合规界面（00/家长中心/广告位）先过 compliance 门禁

## 歧义即问：UI.md 标〔GDD 未定义 → B-xx〕处，不自造，回主策划/主程拍板
```

## 2. 速查
- **技术基线**：uGUI + TextMeshPro + DOTween · Canvas 三层 `Background / Gameplay / Popup` · 设计基准 1920×1080 · Canvas Scaler `MatchWidthOrHeight=0.5` · **锁定横屏**（详见 [ui-baseline.md](ui-baseline.md)）
- **通用件 + 界面全集（00–17）** + 元素表列规 + 导航流：[components-and-screens.md](components-and-screens.md)
- **数据绑定 / 视图表现纪律 / 反馈**：[data-binding.md](data-binding.md)
- **提示词范例（填空模板 + 07 答题面板完整示例）**：[prompt-template.md](prompt-template.md)
- **合规门禁 + UI 已知坑**：[compliance-and-pitfalls.md](compliance-and-pitfalls.md)

## 3. 铁律红线（评审阻断）
- **能复用不新建**（⑩ UI 简洁通用化）：先查通用件，新增专项件须先证明无可复用项
- **单货币**：只渲染能量晶，无金币/第二货币；不引入内购
- **无账号**：界面禁出现“登录/注册/绑定账号”字样；首启手机号仅用于年龄验证（≠ 账号）
- **禁系统 emoji 直显**（豆腐块 + 版权）；符号走 `ic_*`
- **接口只读**：绑定/表现不得修改 Architecture 接口；改接口先走 RFC
- **不硬编码**：尺寸阈值/时限/价格均取 SO / Constants / 文档
