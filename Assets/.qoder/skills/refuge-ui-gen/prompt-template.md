# UI 生成提示词模板（填空版 + 完整示例）

> 用法：复制 A 模板 → 逐段填 → 先读输入文档取当前版本 → 产出前对照 compliance-and-pitfalls.md 门禁。真源取值规则见 SKILL.md §0。

## A. 填空模板（六段式）
```markdown
# 任务：UI · <编号 名称>（如 07 答题面板）

## 1) 归属与形态
- 场景：<Bootstrap/MainMenu/LevelSelect/WordBankSelect/Battle/Result/Shelter/Settings>
- Canvas 层：<Background / Gameplay / Popup>；形态：<全屏 / Popup / 常驻 HUD / 模态阻塞>
- 首个 Phase：<P1/P2/P3>；真源：GDD §7.x · UI.md §<n> · Architecture §<n>

## 2) 输入文档（先读、取当前版本，勿写死版本号）
- GDD.MD §7.x（布局 / 交互语义）
- TechStack.md（UI 框架 / §5 字体 / §2 图集 / §17 适配）
- Architecture.md（§10 场景 / §4.x 接口 / QuizState / §5.1 EventBus）
- UI.md §<n>（细化元素表；下游、冲突让位真源）
- Compliance.md §2·§3·§6（若涉年龄验证 / 广告 / 文案）

## 3) 结构产出（prefab 层级 + 组件）
- 复用通用件：<UIBtn* / PopupRoot / ConfirmDialog / Toast / RewardFly / CountUpText / StarRating / WordCard / TabBar>（见 components-and-screens.md）
- TMP 字体：中文 ShelterHan / 拉丁 ShelterBaloo；符号 ic_*，禁系统 emoji
- 布局：Layout Group + 锚点，禁按 1920×1080 绝对摆死；关键读数进 SafeAreaView
- prefab 路径与命名：Assets/UI/<场景>/UI_<Name>.prefab

## 4) 数据绑定（到接口 / 服务 / 事件；逐元素：控件→绑定源→更新频率→事件）
<列元素绑定，源名以 Architecture 为准；见 data-binding.md>

## 5) 交互与状态
<按真源状态机逐项表现（如 QuizState）；反馈 / 红线见 data-binding.md §4>

## 6) 验收 DoD
- [ ] prefab 归位、Canvas 层正确、打开 DrawCall ≤30
- [ ] 横屏 + 18:9/20:9 + 刘海 SafeArea 无溢出 / 裁切
- [ ] 命中区 ≥96×96、间距 ≥24（答题键 ≥160×96）
- [ ] 绑定走接口 / 事件、无 magic number、无每帧 Find
- [ ] 动效符合 ui-baseline.md；public API XML 注释
- [ ] 涉合规界面先过 compliance 门禁

## 歧义即问：UI.md 标〔GDD 未定义 → B-xx〕处不自造，回主策 / 主程拍板
```

## B. 完整示例 · 07 答题面板 QuizPanel
```markdown
# 任务：UI · 07 答题面板（QuizPanel）

## 1) 归属与形态
- 场景 Battle；Canvas 层 **Gameplay**；形态：战场底部常驻面板（非模态）
- 真源：GDD §7.2.3（答题）· UI.md §7.3（状态机表）· Architecture `QuizState` / `IQuestionPresenter`

## 2) 输入文档
GDD §7.2 · TechStack §5（选项 Horizontal Layout）· Architecture §4.4 IQuestionPresenter / QuizState / QuizConfig · UI.md §7.3

## 3) 结构产出
- prefab：`Assets/UI/Battle/UI_QuizPanel.prefab`（挂 Gameplay 层根节点下）
- 层级：QuestionTMP（题干词面）+ TimerTMP / 环形（倒计时）+ OptionsRoot（Horizontal Layout Group ×4）
- 4 选项复用 `UIBtnSecondary`；单按钮 ≥160×96px、间距 ≥24px；TMP：英文 ShelterBaloo / 中文释义 ShelterHan
- 入场：面板上滑 0.2s（OutQuad）；答后按状态切色（绿 / 红框）

## 4) 数据绑定
- 题干 / 选项 → `IQuestionPresenter.ShowQuestion(WordQuestion)`，每题刷新
- 倒计时 → `QuizConfig`（默认 10s，设置可调 10-20s；禁硬编码）
- 目标高亮联动 → `IQuizTargetProvider`（出题期画高亮环 + 连线，纯 view，不新增 IZombie 字段）
- 状态 → 订阅 `QuizState`；答对 / 答错经 EventBus 通知战斗结算，view 不改伤害

## 5) 交互与状态（全态见 data-binding.md §3）
IDLE 收起 → QuestionGenerated 入场显词面 + 计时 → AnswerSelected 锁选项高亮 0.15s 输入冻结 → AnswerResult 对=绿框叮暴击 / 错=红框+正确项脉冲+嘲讽气泡“就这？就这！”1.5s+连击清零 → Cooldown 串接下一目标 → Timeout 视同答错不造成伤害

## 6) DoD
- [ ] prefab 归 Assets/UI/Battle/、Canvas 层 = Gameplay
- [ ] DrawCall ≤30；字体 ShelterHan / Baloo 正确
- [ ] 选项 ≥160×96 间距 ≥24；横屏 SafeArea 无裁切
- [ ] 绑定仅 IQuestionPresenter / QuizConfig / QuizState，无 GetComponent 业务类、无 magic number
- [ ] 无血腥 / 玩家受伤动作（D13）
- commit：`feat(ui): implement QuizPanel bound to QuizState & IQuestionPresenter`
```
