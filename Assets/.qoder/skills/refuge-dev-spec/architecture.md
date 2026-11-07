# 架构与编码规范（真源：Architecture.md）

具体接口签名以 `Assets/Design/Architecture.md` 当前版本为准；本页固化边界与纪律。

## 分层（自上而下依赖，Core 为最底契约层）
```
Scenes → UI → Systems → Entities → Core
```
- **Core/（必建、只读契约层）**：`Interfaces/` · `Events/` · `Enums/` · `EventBus.cs` · `Managers/` · `Constants.cs`
- Config/：ScriptableObject 配置类（数值唯一来源）
- Entities/：ZombieBase 及派生、Bullet / Wall / Effects
- Systems/：BattleFlow / Wave / Wall / Quiz / Reward / WordPool / Learning / Supply / Save
- UI/：HUD / QuizPanel / ResultPanel 等
- Bootstrap/：场景装配骨架（BattleBootstrap 等）

## 接口先行纪律
- 任何接口 / 事件契约改动：**先改 Architecture.md → 主程评审（RFC）→ 再动代码**
- 开工必列：本任务依赖的接口契约清单
- 禁止 `Impl` 后缀；接口以 `I` 开头

## 命名规范
| 元素 | 规则 | 例 |
|---|---|---|
| Interface | I + PascalCase | `IZombie`、`ISceneLoader` |
| Class / Struct / Enum | PascalCase | `QuizStateMachine`、`WordRef`、`WaveType` |
| 常量 | SCREAMING_SNAKE_CASE | `MAX_ZOMBIES_PER_WAVE` |
| 私有字段 | `_camelCase` | `_currentWave` |
| 公共属性 | PascalCase | `CurrentWave` |

## 关键数据 / 玩法契约（不可擅改，源自 GDD D1–D23）
- `Word` = class（全局唯一引用）；高频传递用 `WordRef` struct
- 敌人 = **Vitality 模型**：`MaxVitality / CurrentVitality` + `TakeDamage(int)`，**不用 HP 命名**；小 200 / 大 600 / BOSS 2000
- 伤害：答对 = 一次暴击，伤害 = 武器攻击力 × 200%；答错 / 超时与哨塔均不削减活力
- `ZombieType` 仅三原型 Normal / Elite / Boss；BOSS 每局 1；30 种 species 走 `ZombieSpeciesConfig` 数据层，不新增枚举值
- 防线唯一数值 = 词汇能量墙耐久（`IWall`），归零即本局失败；避难所无血量、不可被攻击
- 三会（读 / 识 / 写）本版不做：`IWordStudyService` + `NoOpWordStudyService`（`IsEnabled=false`）
- 单机：无排行榜 / 云存档 / 推送，用本地 `bestScore / bestCombo / bestDurabilityPercent`

## EventBus 内存安全
- `Publish<T>` 倒序遍历（支持回调内取消订阅）
- 每个 MonoBehaviour 在 `OnDestroy()` 调 `EventBus.UnsubscribeAll(this)`
- Editor 模式检测“已销毁未退订”→ 抛异常
- Bootstrap 层注册全局事件（GameStart / GameEnd）；业务层只管局部
- 高频帧同步数据不走 EventBus（改 Direct Call / EventSystem）

## 编码红线（评审阻断项）
- 无 magic number（走 SO / Constants）
- `Update` 内禁 `FindObjectOfType` / `GameObject.Find`
- public API 必带 `///` XML 注释
- 跨系统禁直接引用，必事件解耦
- 复杂逻辑加行内注释说明业务意图；禁无意义注释
