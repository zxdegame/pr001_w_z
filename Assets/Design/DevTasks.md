 # 开发任务描述模板 (DevTasks)

| 项目 | 避难防线 - 词汇守卫者 |
|------|-------------------|
| 文档版本 | v1.15 |
| 最后更新 | 2026-11-07 |
| 关联文档 | [GDD.MD](./GDD.MD) v2.13、[TechStack.md](./TechStack.md) v1.16、[Architecture.md](./Architecture.md) v1.15、[ProjectSetup.md](./ProjectSetup.md) v1.16、[Compliance.md](./Compliance.md) v1.12 |
| 发行策略 | **全年龄英语学习 · 国内主市场**：本版**无预算不开版号**→灰区内测（iOS TestFlight + 安卓官网 APK，不提审商店）；融资后申请版号→国内 iOS 中国区 + 安卓商店正式上市；**纯广告变现（0 内购）**；放弃海外、Steam 暂缓（详见 Compliance.md §1、GDD §12 D22） |
| 用途 | 复制到 Qoder IDE 新会话作为开场任务描述 |

---

## 使用须知

1. **按 Phase 顺序使用**：Phase 0 → Phase 1 → Phase 2 → Phase 3
2. **Phase 0 是基线**：不完成 Phase 0 不可开始 Phase 1
3. **每次会话开场必引用**：GDD v2.13、TechStack v1.16、Architecture v1.15、DevTasks v1.15、ProjectSetup v1.16、Compliance v1.12
4. **不可自行修改架构**：任何接口变更先改 Architecture.md，主程评审后才动代码
5. **验收标准必检**：每个任务的 DoD（Definition of Done）需逐项确认
6. **合规硬约束**：任何 UI 文案、广告接入、数据上报前先核对 [Compliance.md](./Compliance.md) §2/§3/§6

---

## 📌 Phase 0：架构骨架（基线建立，必做）

### 复制以下内容到新会话

```markdown
# 任务：Phase 0 架构骨架实现

## 项目背景
- 项目：避难防线 - 词汇守卫者（Tuanjie 2D 英语学习塔防）
- 目标：搭建可编译的空骨架，不实现具体玩法逻辑

## 输入文档（必须先读）
1. Assets/Design/GDD.MD v2.13（玩法设计，**重点看 §12 已定案 D1-D23（含 D15 修订、D17/D18/D19/D20/D21 玩法增味、D22 发行路线、D23 敌人 30 种与包体 ≤200MB）与 §2.6 增强规格（含 §2.6.5 D21）；§12.2 仅剩非阻塞 R1/R2/R3；v2.9 已落十项拍板（金币归一/星级分母/武器去射速/BOSS 带 2 大怪/掉落/价格单点/连击衰减/BattleState 子态机/Spine2D/UI 通用化）**）
2. Assets/Design/TechStack.md v1.16（技术选型，全部已确认；动画方案 Spine2D；字体真源 §5/§5.1 ShelterHan/ShelterBaloo + ShelterHanFB 后备；性能预算 §0/§18：包体 ≤200MB、内存 ≤400MB）
3. Assets/Design/Architecture.md v1.15（架构契约，接口不可改动；BattleState 顶层 6 态为真源；敌人 30 种属 species 数据层、枚举不变）
4. Assets/Design/DevTasks.md v1.15（本任务描述）
5. Assets/Design/ProjectSetup.md v1.16（环境与协作规范；Build 包体梯度 §11.1）
6. Assets/Design/Compliance.md v1.12（合规基线：§1 国内主市场双阶段 / §2 未成年人保护 / §3 广告（含 §3.2 技能包广告红线）/ §6 敏感词）

## 已确认关键决策（不可更改）
- 引擎：Tuanjie 1.x
- **平台目标**：**Android + iOS 移动 App**；不做微信/抖音/团结小游戏（已放弃）
- **发行策略**：**休闲益智**类目 · **全年龄英语学习**；本版**无预算不开版号**→iOS TestFlight + 安卓官网 APK 灰区内测（不提审商店）；融资后申请版号→国内 iOS 中国区 + 安卓商店正式上市；**纯广告变现（0 内购）**；放弃海外、Steam 暂缓（Compliance.md §1、GDD §12 D22）
- 渲染：URP 2D + Pixel Perfect + Sprite Atlas V2
- 输入：New Input System
- UI：uGUI + TextMeshPro + DOTween
- 异步：UniTask
- 事件：自研 EventBus（见 Architecture.md 第 5.1 节）
- DI：无框架，手写单例 + Bootstrap
- 存档：JsonUtility + File IO
- Word 定义为 class
- Zombie 动画：角色/敌人骨骼统一用 **Spine2D**（spine-unity，AnimationState 驱动；二进制 `.skel.bytes`+`.atlas`+page png；**D23：敌人 SkeletonDataAsset 按种独立（30 份，P1 首批 9 份），atlas page 允许 2048/多页**）；`SpriteAnimationPlayer`（手写）仅用于特效/嘲讽气泡等序列帧（TechStack §2 / Architecture §15.1）
- 场景过渡：ISceneLoader 抽象，Phase 0-1 用 SceneManager 实现
- 广告：IAdsService 抽象，Phase 0 用 MockAdsService
- 埋点：IAnalyticsService 抽象，Phase 0 用本地文件 sink
- **变现（D1）**：**0 内购**；商店界面**保留**，物品**只能用能量晶购买**，**能量晶不能用真钱兑换**；不引入 `com.unity.purchasing`，不定义 `IPurchaseService`
- **防线数值（D2）**：**避难所没有血量/耐久值、不可被攻击**；全场唯一防线数值是「词汇能量墙耐久值」（`IWall`），**耐久归零即本局直接失败**
- **敌人类型（D3 · D23 修订）**：`ZombieType` 枚举仍只有 **小怪兽 / 大怪兽 / BOSS 怪** 三类原型（`ZombieType.Normal/Elite/Boss`）；**不存在快速小怪兽、坦克小怪兽、最终 BOSS**；**BOSS 怪每局仅 1 只**；**D23：每类原型下设独立骨架种类（species）共 30 种（小 15/大 10/BOSS 5），走 `ZombieSpeciesConfig` 数据层、不新增枚举值**
- **战斗伤害模型（D8）**：**答对 = 一次暴击，伤害 = 武器攻击力 × 200%**（基础武器攻击力 100 ⇒ 每发 200）；小怪兽持**活力值**（`IZombie.MaxVitality/CurrentVitality`：小怪 200/大怪 600/BOSS 2000），归零即击败；击败所需答对数 = ⌈活力÷每发伤害⌉，**武器升级会降低大怪/BOSS 所需答对数（BOSS 不恒定 10 题）**；答错/超时与哨塔均不削减活力（取代旧“固定 1/3/10 题”）
- **范围裁剪（D4）**：**三会（读/识/写）本版不做**，只保留 `IWordStudyService` 接口 + `NoOpWordStudyService` 空实现；**单机不做排行榜 / 不做云存档 / 不做推送**，用本地 `bestScore/bestCombo/bestDurabilityPercent` 替代
- **屏幕方向**：**锁定横屏**（Landscape Left），不做竖屏（GDD §12 D6）

## 目标产出（严格按 Architecture.md 第 17 节清单）

### Core 层（Assets/Scripts/Core/）
- Enums/*.cs：ZombieType, QuizState, BattleState, WordMasteryLevel,
              SupplyPackType, QuestionType, StarRating,
              PurchaseCurrency（**仅 EnergyCrystal 一个值**）, StudyMode
- Interfaces/*.cs：IZombie, IQuizTargetProvider, IWordProvider,
                   IWordLibraryService, IQuestionPresenter, IWall,
                   IRewardCalculator, ISceneLoader, IAdsService, IAnalyticsService,
                   IStoreService, IWordStudyService
- Events/*.cs：所有 event struct，实现 IEvent 接口
- EventBus.cs：自研实现（约 100 行，参考 Architecture.md 5.1 节）
- Constants.cs：PIXELS_PER_METER=100 等
- Managers/*.cs：GameManager, SaveManager, PauseManager,
                 AnalyticsManager, AdsManager（骨架）

### Config 层（Assets/Scripts/Config/）
- LevelConfig.cs（含 WaveData[] 字段，SO）
- WaveData.cs（含 [Serializable] ZombieSpawnEntry[]）
- ZombieConfig.cs
- QuizConfig.cs
- SupplyPackConfig.cs
- WeaponConfig.cs
- WordDatabase.cs（含 Word[] words 字段）

### Entities 层（Assets/Scripts/Entities/）
- Zombie/ZombieBase.cs（abstract class，实现 IZombie；字段为 `MaxVitality/CurrentVitality` + `TakeDamage(int)`，**不使用 HP/CurrentHP 命名**，GDD D8）
- Zombie/NormalZombie.cs（maxVitality=200）
- Zombie/EliteZombie.cs（maxVitality=600）
- Zombie/BossZombie.cs（maxVitality=2000）
- Systems/Learning/NoOpWordStudyService.cs（三会空实现，`IsEnabled=false`，Phase 4+ 才替换）
- Effects/SpriteAnimationPlayer.cs（骨架，仅特效/气泡帧切换；角色骨骼走 Spine2D，Phase 1 接入 spine-unity）

### Bootstrap 层（Assets/Scripts/Bootstrap/）
- BattleBootstrap.cs（装配骨架）
- SceneManagerSceneLoader.cs（Phase 0-1 默认场景加载实现）
- MockAdsService.cs（无网络时默认使用）

### 测试（Assets/Tests/EditMode/）
- EventBusTests.cs（3 个用例：发布/订阅/owner 销毁后内存清理）
- InterfacesCompileTests.cs（接口可实例化验证）

## 严格禁止
- ❌ 不实现玩法逻辑（如"答对一题如何按暴击伤害（攻击力×200%）削减活力值"的细节，留 NotImplementedException）
- ❌ 不搭建具体战斗场景 .unity（Phase 1 才做）
- ❌ 不接入实际广告 SDK（Phase 0 用 Mock）
- ❌ 不引入 Odin Inspector
- ❌ 不引入 Addressables（Phase 2 才用）
- ❌ **不引入微信/抖音小游戏 SDK**（目标已改）
- ❌ **版号前不上架任何应用商店**（华为/小米/OPPO/vivo/应用宝/App Store 中国区均强制版号；本版无版号，仅官网 APK / TestFlight 灰区分发，不提审商店）
- ❌ 本版不接入国家防沉迷实名 SDK（无版号无法接入官方验证系统，**后置至版号阶段**）；年龄/家长验证走**运营商一键登录**（`IAgeVerifyService`/`IPlatformService` 接口预留，Phase 2-3 接 SDK）
- ❌ **不接入任何支付/内购 SDK**（`com.unity.purchasing`、App Store IAP、Google Play Billing 全部禁止）；`PurchaseCurrency` 不得新增第二个枚举值
- ❌ **不实现三会（仅 `IWordStudyService` + NoOp）**；不新增 `QuestionType.Spelling`
- ❌ **不做排行榜/竞技场/好友列表相关代码**（单机，无任何联网排名能力）
- ❌ 不改 Architecture.md 中定义的接口签名

## 验收标准（Definition of Done）
- [ ] Tuanjie 打开项目可编译，0 warning
- [ ] 所有 public API 有 `/// <summary>` XML 注释
- [ ] EventBus 单元测试 3 个用例全通过
- [ ] 不违反 Architecture.md 中的接口签名
- [ ] 玩法方法保留 NotImplementedException
- [ ] **`PurchaseCurrency` 反射断言**：`Enum.GetNames(typeof(PurchaseCurrency)).Length != 1` 即判定失败（防止有人默默加回真钱货币）
- [ ] **不存在 `IShelter`/`IBase` 类血量接口**（避难所无 HP，只能有 `IWall`）
- [ ] **工程内无任何支付/内购依赖**（manifest.json 不得出现 `com.unity.purchasing`）
- [ ] **能构建 Android Hello World AAB 与 iOS 空包**（验证 Tuanjie 环境 + SDK 签名就位）
- [ ] **年龄识别 + 家长同意弹窗骨架**（Compliance.md C1 / `IAgeVerifyService`）：首次启动阻塞流程，判定未成年且无家长同意则退出（国内做法：手机号实名 + 运营商一键登录；Phase 0 用 Mock 骨架 + 算术题/长按二次确认，不自建后端）
- [ ] Git 提交：`chore: Phase 0 architecture skeleton (v1.0)`

## 开工前请先输出以下确认
1. 你已读取的 6 份文档版本号确认
2. 你计划的目录结构与文件清单（对照 Architecture.md 第 17 节）
3. 对本任务描述的疑问（若有）
4. 确认不实现具体玩法逻辑后开始动代码

## 遇到问题的处理原则
- 接口不明确 → 提问，不要臆测
- 需要新增接口 → 先提议修改 Architecture.md，评审后再实现
- 依赖冲突 → 报告，等主程决策
```

---

## 📌 Phase 1：核心玩法闭环

### 复制以下内容到新会话（Phase 0 完成后）

```markdown
# 任务：Phase 1 核心玩法闭环实现

## 前置检查
在开工前请先读取以下文件，确认 Phase 0 骨架已就位：
- Assets/Design/GDD.MD v2.13（重点第 2、4、6 章 + §12 已定案 D1-D23，含 D15 战场几何与推进修订、D16 技能包必用、D17 战斗手感与连击、D20 稀有游走怪、D21 学习即弹药、D23 敌人 30 种 species 口径；v2.9 星级分母=本局实际首次出题数、武器去射速、BOSS 关带 2 大怪）
- Assets/Design/TechStack.md v1.16
- Assets/Design/Architecture.md v1.15（接口契约）
- Assets/Scripts/Core/**（Phase 0 已建立）

## 目标
实现"进入关卡 → 出题 → 答题 → 击败 → 结算"的完整闭环，玩法可试玩。

## 关键规则（严格按 GDD v2.13 方案 B）
- 敌人 `ZombieType` 枚举只有 3 类原型：小怪兽 / 大怪兽 / BOSS 怪（`ZombieType.Normal/Elite/Boss`）；**不存在快速小怪兽、坦克小怪兽、最终 BOSS**（GDD §12 D3 + 附录 B）；**D23：30 种敌人属原型下 species（小 15/大 10/BOSS 5），Phase 1 先用原型骨架占位，种骨架与 `ZombieSpeciesConfig` 接入在 Phase 2 会话 D（名录主策划 Phase 2 出表）**
- 每局固定 5 波小怪兽，第 5 波仅 1 只 BOSS 怪（**BOSS 怪每局唯一**）
- **伤害制（GDD §12 D8）**：答对 = 一次暴击，伤害 = 武器攻击力 × 200%（基础武器攻击力 100 ⇒ 每发 200）；答错/超时与哨塔均不削减活力
- 小怪兽：活力值 200（基础武器下答对 1 次击败）
- 大怪兽：活力值 600（基础武器下累计答对 3 次击败，3 个不同单词）
- BOSS 怪：活力值 2000（基础武器下累计答对 10 次击败，10 个不同单词）；**武器升级会降低击败所需答对数，BOSS 不恒定 10 题**
- 小怪兽从屏幕左侧向右侧移动，**进入战场左起 1/6 处触发答题、推进至 4/6 处停止并攻击能量墙**（GDD §12 D15 / 附录 D）；**D15 v2.6 修订：锁定与非锁定敌人全程按各自基础速度正常推进（取消答题期不前进/30% 减速），当前锁定目标高亮标记（view 层，见会话 D）**；买时间仅由控制类技能包提供
- **能量墙耐久上限 = `PlayerData.energyWallLevel` 对应阶位（1 阶 100 / 2 阶 200 / 3 阶 300 / 4 阶 400 / 5 阶 500）**，普通 2s/-5、大怪 1.5s/-5、BOSS 1s/-10
- **避难所没有血量/耐久值、不可被攻击**；**能量墙耐久归零 = 唯一失败判定**（不存在第二段避难所判定）
- 答题时限：默认 10 秒，仅在战斗开始前可修改
- 答错：单词进错题本并从词池移除，小怪兽停顿 1.5 秒后继续前进
- 连击：**4 秒**内答对获得连击 +1，触发 **3/5/8/12/20** buff（均由“答对”触发，属答题唯一击败；GDD §12 D17）
- **即时命中（GDD §12 D17）**：答对→当帧结算暴击、取消旧“停顿 1 秒”硬等待，受击仅 `HitStun=0.3s` 动画；Swift（单题 ≤`3.0s` 且答对→该次暴击 ×`1.15`）
- **技能包必用策略层（GDD §12 D16）**：补给包分**控制类**（Ice/Slow/Confuse，**不造成任何伤害**，只改移动状态）与**转化类**（Lightning/Spread/Fire/Pierce，把“答对”触发的暴击扩散到多目标）；伤害一律由答对触发，不得自动/持续掉血；每关掉落保底≥ 1 控制+1 转化、BOSS 波必掉 1 控制（Phase 1 先做基础掉落，完整系统见 Phase 2 会话 A）
- 单词补充：词池 <10 时补充 10 个（优先级：上一局错词>已掌握>待复习>未学习）
- 答对率 = 答对题数 / 初始词池数量（不含补充），上限 100%
- 星级：★★★ 能量墙耐久百分比>80% 且答对率>90%（1.5x）；★★☆ 耐久百分比>50% 且答对率>70%（1.2x）；★☆☆ 通关（1.0x）；失败 0.5x

## 任务分解

### P0 - 战斗系统
1. WaveController（Assets/Scripts/Systems/Battle/）
   - 读取 LevelConfig.waves 数组
   - 每波间隔 5 秒
   - 波次结束条件：本波小怪兽全部被击败或到达能量墙
   - 第 5 波固定生成 1 只 BOSS + 5 只普通小怪兽

2. WallManager（Assets/Scripts/Systems/Battle/，实现 `IWall`）
   - 管理能量墙耐久值：`MaxDurability` = `PlayerData.energyWallLevel` 阶位（1 阶默认 100）；**耐久不跨局继承，每局开始重置为满值**（GDD §12 D9）
   - 接收 ZombieReachedWallEvent，按类型启动攻击计时器
   - **耐久值 <= 0（`IsBroken`）时发布 OnWallBroken 事件 → 本局直接失败**（避难所无第二段判定）
   - 提供 `Repair(int)`：**仅急救补给站触发**恢复耐久，**不得溢出 `MaxDurability`**；**战斗中不得消耗能量晶修复能量墙**（GDD §12 D2/Q8，1 阶无能量晶应急修复）

3. BattleFlowController（Assets/Scripts/Systems/Battle/）
   - 优先级检测：失败 > 胜利
   - 胜利：当前波次==5 且 场上小怪兽==0 且 能量墙耐久>0
   - 失败：能量墙耐久<=0（唯一失败条件），触发 2 秒失败动画 + 50% 奖励结算
   - 退出：确认框 → 视为失败 → 存档

### P0 - 答题系统
1. QuizStateMachine（Assets/Scripts/Systems/Quiz/）
   - 8 状态：IDLE/QUESTION_LOADING/WAITING_ANSWER/CORRECT_RESOLVE/
              WRONG_RESOLVE/TIMEOUT_RESOLVE/ZOMBIE_KILLED/NEXT_QUESTION
   - 目标锁定：currentTargetZombie 在大怪/BOSS 期间不切换
   - 3 独立计时器：answerTimer / comboTimer / zombiePauseTimer
   - 单词队列：按 ⌈`MaxVitality` ÷（武器攻击力×200%）⌉ 为当前目标准备不同单词题目（基础武器下小怪1/大怪3/BOSS10）；目标活力提前归零则剩余单词本题不出现

2. QuestionLoader（Assets/Scripts/Systems/Quiz/）
   - 从 IWordProvider 抽单词
   - 生成 4 选项（1 正确 + 3 干扰项）
   - 支持两种题目：EN2ZH / ZH2EN

3. ComboTracker（Assets/Scripts/Systems/Quiz/）
   - 4 秒内答对连击 +1（连击窗口 `ComboWindowSeconds=4.0`，GDD §12 D17）
   - 触发 **3/5/8/12/20** buff（Phase 1 先做 3 连击：连射两枪）

### P0 - 词池系统
1. WordPoolManager（Assets/Scripts/Systems/WordPool/）
   - 实现 IWordProvider
   - 记录 initialWordPoolCount（关卡开始时固定）
   - RemainingCount < 10 触发补充
   - 补充优先级：上一局错词 > 已掌握 > 待复习 > 未学习

2. WordReplenisher（Assets/Scripts/Systems/WordPool/）
   - 从 IWordLibraryService 拉取新单词加入词池

3. WrongWordBook（Assets/Scripts/Systems/WordPool/）
   - 答错单词归档
   - 后续关卡或 7 日复习中重新出现

### P0 - 奖励结算
1. RewardCalculator（Assets/Scripts/Systems/Reward/）
   - 基础奖励 = 击败普通×1 + 击败大怪×2 + 击败BOSS×5（能量晶）
   - 系数应用：★★★1.5 / ★★☆1.2 / ★☆☆1.0 / 失败0.5
   - 失败时不适用星级判定
   - 答对率上限 100%

2. StarRatingEvaluator
   - 3 组阈值组合的边界测试

### P0 - 场景搭建
- 创建 Battle.unity 场景
- Bootstrap 装配所有系统
- 简单测试关卡（1-1 关）：5 只普通小怪兽
- UI 骨架：题目面板 + 4 选项按钮 + 能量墙耐久条 + 倒计时 + 结算面板

### P1 - 玩家数据
- PlayerData 定义（见 Architecture.md 第 9 节）
- SaveManager 实现：JsonUtility + File IO
- 自动备份 .bak，损坏回退

### P1 - 埋点接入
- AnalyticsManager 三层 sink 实现
- 在关键节点发布 Analytics*Event（参考 Architecture.md 15.3）
- 本地文件写入 persistentDataPath

## 严格禁止
- ❌ 不引入 Addressables（Phase 2 才用）
- ❌ 不实现 7 种补给包效果（Phase 2）
- ❌ 不做多关卡内容（Phase 2）
- ❌ 不做避难所升级 UI（Phase 2）
- ❌ 不做教学引导（Phase 3）
- ❌ 不实现广告（Phase 2，MockAdsService 保留）
- ❌ 不做能量晶商店 UI（Phase 2 接入 `IStoreService`）
- ❌ 不做三会练习模式（Phase 4+，Phase 1 只用 NoOp 实现）
- ❌ 不做排行榜/联机/好友系统（单机永不做）

## 性能约束
- 单局同屏小怪兽 ≤ 30 只
- DrawCall < 30
- GC / Frame < 1KB
- 帧率 60 FPS 稳定（移动端）

## 验收标准（Definition of Done）
- [ ] 编译通过，0 warning
- [ ] Battle.unity 可试玩完整流程（进入 → 答题 → 击败 → 结算）
- [ ] 5 波结构 + 第 5 波 BOSS 正常工作
- [ ] 伤害制生效：大怪活力 600（基础武器 3 次答对击败）/ BOSS 活力 2000（基础武器 10 次答对击败），武器升级减少所需答对数（字段名是 `MaxVitality/CurrentVitality`，**代码中不得出现 HP/CurrentHP/RequiredAnswers**）
- [ ] **能量墙耐久归零 → 立即失败，不存在避难所第二段判定**（PlayMode 用例）
- [ ] 单词补充机制触发正确
- [ ] 星级判定 & 奖励计算 EditMode 单元测试通过（覆盖率 ≥ 80%）
- [ ] 存档能正确保存/加载
- [ ] 埋点日志能写入本地文件
- [ ] PlayMode 测试 3 个关键流程全通过（参考 TechStack.md 第 14 节）
- [ ] Git 提交：`feat: Phase 1 core gameplay loop`
```

---

## 📌 Phase 2：内容扩展（可部分并行）

### 复制以下内容到新会话（Phase 1 完成后）

```markdown
# 任务：Phase 2 内容扩展

## 前置
Phase 1 核心玩法已跑通，本阶段扩展内容深度。

## 并行任务拆分（4 个会话，接口已锁定可并行）

### 会话 A：补给包系统（GDD §12 D16：技能包必用策略层）
1. 7 种补给包定义 + **分类字段**：**控制类**（Ice/Slow/Confuse，**不造成伤害**）/ **转化类**（Lightning/Spread/Fire/Pierce，伤害由“答对”触发、扩散多目标）
2. 掉落触发：每波 30% / 每答对 5 题 / BOSS 波必掉 1 控制 + **关卡保底（每关≥ 1 控制+1 转化）**
3. 局外空投动画（1.5 秒掉落屏幕中央）
4. 三选一 UI 面板
5. 效果范围：以获得补给后第一次射击的小怪兽为中心，半径 200 像素（控制类改“冻结/减速/击退”，转化类“扩散本次答对暴击”）
6. 叠加规则（D16）：最多 **1 控制 + 1 转化**（闪电包即时效果不占槽）；控制类可跨波囤积、至多 1 个（D11 例外）；`PlayerData.activeSupplyPacks[]` 长度 ≤ 2
7. 升级机制：Lv2/Lv3 效果定义（参考 GDD 4.2.3 表）；**转化类伤害一律由“答对”触发，火焰“灼烧”重定义为每次答对追加同源暴击，不自动掉血**
8. SupplyPackConfig SO 完善（新增 category/掉落保底/囤积上限 字段）
9. **广告红线（Compliance §3.2）**：过关必需包只能靠游戏内掉落，激励视频只发非必需增益包

### 会话 B：7 日记忆系统与词库
1. WordMasteryTracker：Unlearned → Learning → Mastered → LongTermMastered
2. ReviewScheduler：7 日轮回复习计划
3. WrongWordBook 复习/错题/挑战关入口（归**词汇档案室**，Phase 2+；不占用主线关卡列表，GDD §12 D10）
4. 词库选择 UI（教材/年级/单元/专项标签）
5. IWordLibraryService 完善（metadata 列表 + 按需加载）
6. 单词导入工具（CSV → SO）
7. 引入 Addressables，替换 Resources 加载路径
   - 保留兼容层，30 天内两种方案并存
   - 迁移 ISceneLoader 到 AddressableSceneLoader

### 会话 C：避难所建筑与关卡内容
1. 防线建筑系统：
   - 自动哨塔（5 阶升级）
   - 词汇能量墙（5 阶升级，**升级直接提升 `MaxDurability`：100/200/300/400/500**）
   - 急救补给站（5 阶升级，单局累计答对 50 恢复 20 点能量墙耐久，**不得溢出 `MaxDurability`**）
   - 预警瞭望台（5 阶升级）
2. 建筑升级 UI（避难所场景）
3. **能量晶商店 UI**（`IStoreService`，GDD §7.5.2）：
   - **全部物品只能用能量晶购买**；**不存在用真钱兑换能量晶的入口**
   - 词库解锁：双门槛（掌握度要求 + 消耗能量晶）
   - 界面**禁止出现** $/¥/价格标签/充值入口/去广告购买/抽卡（Compliance.md §3.3）
   - 纯本地实现，不引入任何支付 SDK
4. 关卡内容扩展：
   - 第一章 5 关 + 第二章 5 关 + 第三章 5 关完整配置
   - 每关 LevelConfig SO 生成
   - 难度动态适配（连续 3 局正确率>90% 提升刷怪速度）
5. 教学关卡引导（1-1 到 1-5 的新手引导流程）

### 会话 D：玩法增味（GDD §12 D15 修订 / D17/D18/D19/D20/D21，详细规格见 GDD §2.6）

> 本会话不新增 `ZombieType`、不改已有接口签名（仅用 Architecture 契约已新增的 `EnemyModifier`（含 D20 `Wanderer`）/`WaveEventType`/`BattleDoctrine`/`LocalAchievementCategory` 与 `IZombie.Modifier/HasActiveShield`，以及 D21 的运行时 `BattleModifiers`/`IMasteryMeterService`，均不改已有签名）；伤害仍全部由“答对”触发；D15 v2.6 推进/高亮与 D21 M1/M3 均为 controller+view 改动、不动接口、不新增伤害来源。

1. **D17 即时命中**：`ZombieController` 移除答对后 1s 位移阻塞→答对当帧 `TakeDamage`；新增 `HitStunSeconds=0.3` 受击硬直动画（仅表现层、不阻塞出题）；**D15 v2.6：全程正常速度推进、不减速（Phase 1 关键规则已改），不再走 30%**。
2. **D17 连击**：`QuizConfig.ComboWindowSeconds` 3.0→**4.0**；AOE 档位改 **3/5/8/12/20**（读 GDD §2.3.4 表）。
3. **D17 Swift**：出题计时器记录用时，≤`SwiftThresholdSeconds=3.0` 且答对→`amount ×SwiftDamageMultiplier=1.15`（与会心/处决加算，受单次伤害上限约束）。
4. **D18 `EnemyModifier` 应用**：`WaveController` 读 `LevelConfig.waves[].spawns[].modifiers[]`→生成时设 `Modifier`/`HasActiveShield`。
   - `Shielded`：结算层中 `HasActiveShield` 为真时首次 `TakeDamage` 只置 false、不扣活力（伤害被盾吸收）。
   - `Sprinter`：`ZombieController` 移速 ×1.5（=1.5 m/s）。
   - `Mender`：新增 `MenderBehaviour`（每 `MenderTickSeconds=5` 秒给半径 300px 内一只友军上调 `CurrentVitality`≤`MaxVitality`，**非 `TakeDamage`**）。
5. **D18 `WaveEventType`**：`WaveController` 按 `waves[].eventType` 应用 `Rush`（spawnInterval×0.6、普怪×1.5≤Top10）/ `BossPressure`（BOSS + 1 Shielded 大怪 + 1 Mender 大怪）；`Ambush` 只预留枚举（Phase 4）。
6. **D18 `BattleDoctrine`**：关卡开始前 `DoctrineSelectPanel` 三选一（仅 `doctrineEnabled=true` 弹），写入运行时 `BattleModifiers`（不持久化）：`Fire`→`attackMultiplier=1.2`；`Bulwark`→本局 `MaxDurability`×1.3（重进重置，守 D9）；`Logistics`→掉落保底+1 与 `waveIntervalSeconds-1`。
7. **D19 本地轻成就**：`LocalAchievementService`（纯本地、无联网/每日刷新）读 `PlayerData`/`GameData` 统计，`LocalAchievementCategory` 条目一次性发奖；扩展 §7.5.4 为“本地成就 + 收集进度墙”（**不做三会/服装/词汇徽章/每日任务**）。
8. **D19 前期曲线**：结算应用 `earlyChapterBonus=1.3`（仅第 1-3 章首通、replay 不叠加）；哨塔/能量墙/急救站首档升级成本 200→100。
9. **D15 v2.6 目标高亮（view）**：`BattleHUD`/`ZombieView` 依 `GameConfig.LockedTargetHighlight`（默认 true），对 `IQuizTargetProvider` 返回的当前锁定目标播放头顶高亮环 + 答题框到目标的指示连线；切换目标时迁移高亮；非锁定目标不高亮（不改 `IZombie` 签名，仅表现层）。
10. **D20 稀有游走奖励怪（与 D18 变体同批）**：`EnemyModifier.Wanderer` + `ZombieConfig.dropOverride`（金库怪能量晶 20–40 / 铁匠怪碎片 15–30）；`ZombieController` 令其进入 1/6 后停止推进、在 `[1/6,4/6]` 内 `WanderSpeed=0.6` 随机走走停停、`IsReachedWall` 恒 false 不攻墙；点击可置为锁定目标答题击败（复用 item 9 高亮）；`GameConfig.WandererSpawnChance=0.05`/限第 2–4 波/每局≤1；**不计入波次完成判定**、波次结束仍存活直接 `Die()` 不掉落。**不新增 `ZombieType`（守 D3）**。
11. **D21 M1 生词上膛可视化（纯表现）**：敌人头顶词汇标记（`ZombieView`）在现有光圈颜色外叠加该词记忆强度——读 `Word.memoryStrength`（Phase 1 无连续量则按 `WordMasteryLevel` 四态兜底 0/0.5/0.8/1.0），显示“?? 待解锁”（冷灰）/“上膛中 {percent}%”（蓝）/“满膛 ✓”（暖金）；不新增伤害来源、不改 `IZombie` 签名。
12. **D21 M2 首克新词/攻克错题即时奖励**：`LearningSystem` 在“答对”结算同帧判定——首次达 Mastered→发布 `FirstWordMasteredEvent`、从错题本移出→发布 `WrongWordFixedEvent`；`RewardCalculator` 额外一次性晶 `GameConfig.FirstMasterCrystalBonus=3`/`WordFixCrystalBonus=2`（走 §4.4 奖励、非伤害）；UI 订阅两事件播爆字+Swift 级特效+轻震；每词首克里程碑全局一次性（`PlayerData.masteredRewardSet`）。
13. **D21 M3 Mastery Meter（仅本局）**：新增 `MasteryMeterService`（实现 `IMasteryMeterService`），每出题刷新本局已出题中 Mastered/LongTermMastered 占比→≥`MasterySwiftPct=0.6` 置 `BattleModifiers.swiftThresholdBonus=+0.5`、≥`MasteryComboPct=0.8` 置 `BattleModifiers.comboWindowBonus=+0.5`；HUD 新增弹药熟练度条显示占比与生效档；**只动触发节奏（阈值/窗口）不动伤害**、仅本局生效不写存档（守 D9）。
14. **D23 敌人 species 数据层接入（P1 首批 9 种）**：新增 `ZombieSpeciesConfig`（SO：speciesId、`ZombieType` 原型、SkeletonDataAsset/atlas 引用、数值与掉落钩子可选）；`ZombieConfig`/`LevelConfig.waves[].spawns[]` 增加 `speciesId` 字段，生成时按种解析骨架、**不改 `IZombie`/`ZombieType`**；首批 9 种（小 5/大 3/BOSS 1）骨架接入后，同批关卡优先同 atlas page 的种同屏（合批编排，TechStack §2）；P2/P3 批次随种类名录表追加资产、程序侧不回改；**本会话严格禁止新增 `ZombieType` 枚举值**。

## 严格禁止
- ❌ 不改 Architecture.md 已定接口
- ❌ 不做多语言（Phase 3）
- ❌ 不做真正的广告接入（Phase 3，运营确认后）
- ❌ 不做联网/云存档
- ❌ **商店不接入任何支付/内购 SDK**（能量晶不可用真钱兑换，也不可从服务端下发）
- ❌ **不做三会（读/识/写）子系统**（仅 `IWordStudyService` NoOp；三会/服装/联网成就是 Phase 4+；但**本会话 D 实现的本地轻量成就属本版**，GDD §12 D19，与联网成就不是一回事）
- ❌ **不做排行榜**（单机；本地纪录用 `bestScore/bestCombo/bestDurabilityPercent`）

## 验收标准
- [ ] 7 种补给包全部可用，升级效果符合 GDD 表
- [ ] 能量晶商店可买且**无任何真钱入口**（目测 + 文案断言：UI 文本不包含 $/¥/充值/去广告）
- [ ] 至少 15 关完整可玩，覆盖 3 个章节
- [ ] 词库切换工作正常，按需加载生效
- [ ] Addressables 迁移完成，包体达标 P2 梯度 **≤140MB**（Build Report 阈值见 ProjectSetup §11.1，D23）
- [ ] 所有新系统有对应 EditMode 单元测试
- [ ] **会话 D**：`EnemyModifier`（Shielded/Sprinter/Mender/Wanderer）/`WaveEventType`（Rush/BossPressure）/`BattleDoctrine`（仅本局）/本地轻成就均可用；答对无停顿硬卡；连击窗口=4s；当前锁定目标高亮可见（D15 v2.6）；游走怪停止推进不攻墙、不计通关；D21：词汇头顶标记显示上膛进度（M1）、首克新词/攻克错题当帧爆字+额外晶（M2）、Mastery Meter 影响 Swift/连击阈值且不改伤害（M3）（GDD §2.6/§2.6.5）；**D23：`ZombieSpeciesConfig` 可配且 P1 首批 9 种骨架接入（`ZombieType` 枚举零新增）**
- [ ] 每周五自动跑性能报告，指标不超标
- [ ] Git 提交：`feat: Phase 2 content expansion`
```

---

## 📌 Phase 3：打磨发布

### 复制以下内容到新会话

```markdown
# 任务：Phase 3 打磨与发布准备

## 目标
视觉、音效、性能、平台适配全面打磨，进入发布测试。

## 任务清单

### 视觉与动画
1. 完整美术资源替换占位符
2. 特效打磨：粒子系统、屏幕震动、光效
3. UI 动画：DOTween 补全所有面板过渡
4. 字体本地化：中文 SDF `ShelterHan`（思源黑体子集）+ 拉丁(英/数字) SDF `ShelterBaloo`（Baloo 2 子集），**均 SIL OFL 1.1 免费商用**；子集化/转 SDF 属修改→**按 OFL 保留名规则重命名**（勿沿用 Source Han/Noto/Baloo）；**DIN Alternate（商业专有）/Roboto Mono 已弃用**；特殊符号/emoji 走 `ic_*` 图标精灵（禁系统 emoji 直显）；中文以静态裁剪主资产 `ShelterHan`（3500 常用字）+ `ShelterHanFB` 扩展字集 Fallback 资产防生僻字（**默认不用全量动态字体**，避免包体不可控，见 TechStack §5.1）+ Editor 缺字自检零豆腐块；随包附 `OFL.txt` 与许可快照（TechStack §5.1 / Compliance §10）

### 音效与语音
1. BGM 分场景配置
2. SFX 完整录制（答题/射击/击败/连击/胜利/失败）
3. 英文单词语音（**VO 转正：预录 OGG 正式录制（D23 品质上调）**；超大词库音频另走 DLC 下载不上包）
4. AudioMixer 分组与音量设置

### 广告接入
1. 根据发行渠道接入具体 SDK：
   - **海外 Android/iOS（主路线）**：**Google AdMob**（+ Meta Audience Network 备份）
   - 国内 App：**穿山甲** + **优量汇** —— **暂不接入**（不开版号 → 国内不上架；Phase 4+ 若改变发行策略再评估）
   - **不包含小游戏平台选项**
2. 广告位设计（儿童合规硬限制，Compliance.md §3.2）：
   - 失败结算"看广告复活"（可选）
   - 每日奖励翻倍
   - 绝不打扰答题过程；单局 ≤ 1 次，单日 ≤ 5 次
   - **默认关闭，家长设置中开启**
   - 海外 AdMob 必须置 `tagForChildDirectedTreatment(true)`（C5）；仅激励视频，禁插屏/Banner
3. 家长设置中可禁用广告

### 平台适配
1. **锁定横屏**（Landscape Left）：Android/iOS 均不支持竖屏，系统旋转不影响游戏（GDD §12 D6）
2. Android 刘海屏 / 分屏 Safe Area；ARMv7 + ARM64
3. iOS Dynamic Island + 刘海屏适配
4. 国内安卓渠道包签名（华为/小米/OPPO/vivo）**暂不执行**（无版号不上架）；海外仅 Google Play AAB + Play App Signing
5. PC 窗口/全屏切换（仅开发预览）

### 本地纪录（替代排行榜，D4）
1. `PlayerData.bestScore` / `bestCombo` / `bestDurabilityPercent` 写入与读取
2. 结算面板与避难所展示“历史最佳”（**纯本地，无任何联网排名**）
3. 三会（`IWordStudyService`）与服装/每日任务 **仍为 Phase 4+**，本阶段不实现；**本地轻量成就与收集进度墙（GDD §12 D19）已在 Phase 2 会话 D 实现**，本阶段仅做展示打磨

### 性能优化
1. Profiler 全项目扫描，消除 GC 尖峰
2. DrawCall 优化（图集合并、Batching 检查）
3. 内存峰值测试（长时间游玩）
4. 首屏加载优化 <3 秒

### 埋点上线
1. AnalyticsManager 上报接口接通（若运营确认联网）
2. 关键事件覆盖率验证
3. 数据看板搭建（可选）

### 发布准备
1. 图标/启动图/商店截图（**仅 Google Play + App Store**，国内五商不上架→不准备）
2. 版本号管理（Application.version）
3. 更新日志
4. 崩溃上报（海外主路线用 Firebase Crashlytics；Bugly 仅国内渠道适用，**暂不接入**）
5. **上架合规包**（按 [Compliance.md](./Compliance.md) §5 DoD 逐项）：家长同意弹窗 + 隐私政策/用户协议链接 + 家长控制面板 + 广告儿童标记（AdMob COPPA 信号）+ **开源许可/素材清单页**（设置内文本页，`OFL.txt` 全文 + 字体/SIL OFL + SDK 许可 + 素材来源；Compliance §10.2 / UI §14）
6. Beta 测试分发（**iOS TestFlight** + Google Play 内部测试轨道；蒲公英等国内分发**不使用**）

## 验收标准
- [ ] 全部美术资源到位，无占位符
- [ ] 音效/BGM 完整，音量可调
- [ ] 广告在 Android/iOS 正常工作，失败不阻断游戏
- [ ] 移动端 60 FPS 稳定，无 GC 卡顿
- [ ] **包体 ≤200MB**（单渠道 APK/AAB/IPA；D23 唯一硬线，取代原 <80MB；Build Report 阈值见 ProjectSetup §11.1）
- [ ] **家长同意弹窗 + 广告儿童标记 + 家长面板可关广告**（Compliance.md C1/C5/C8，提审前置）
- [ ] **字体/素材 OFL 许可随附可见**（设置内“开源许可/素材清单”页 + 包内 `OFL.txt`；SDF 已按 RFN 改名；生僻字缺字自检通过、零豆腐块）
- [ ] 崩溃率 <1%（Beta 期）
- [ ] Git 提交：`release: Phase 3 polish for launch`
```

---

## 📋 通用会话开场模板

若上述模板不完全匹配你的具体子任务，可用此通用格式：

```markdown
# 任务：<具体模块名或子任务>

## 输入文档（必读）
- Assets/Design/GDD.MD v2.13，重点第 X 章（§12 D15 修订/D16 战场几何与技能包必用、§2.6 D17/D18/D19/D20/D21 玩法增味、D23 敌人 30 种与包体 ≤200MB；v2.9 十项拍板：金币归一/星级分母/武器去射速/BOSS 带 2 大怪/价格单点/连击衰减/BattleState 子态机/Spine2D/UI 通用化）
- Assets/Design/TechStack.md v1.16
- Assets/Design/Architecture.md v1.15，特别是第 X 节
- Assets/Design/DevTasks.md v1.15，Phase X
- Assets/Design/ProjectSetup.md v1.16（首次开会必读协作规范）
- Assets/Design/Compliance.md v1.12（涉及文案/广告/数据时必读）

## 前置状态
<描述当前项目状态，哪些已完成>

## 目标
<一句话说明本会话要交付什么>

## 输入契约（依赖）
<本任务依赖哪些已实现的类/接口>

## 输出契约（提供）
<本任务向其他系统提供哪些 API/事件>

## 严格约束
- 不修改 Architecture.md 已定接口
- 性能：<具体要求>
- 目录：代码放在 Assets/Scripts/<具体目录>/

## 验收标准
- [ ] 编译通过 0 warning
- [ ] <具体功能可验证>
- [ ] <单元测试通过>
- [ ] <性能指标达标>
- [ ] Git commit：<建议的提交信息>

## 遇到问题的处理
- 不明确的地方先提问，不要臆测
- 需要修改接口先改 Architecture.md
- 与已定决策冲突时报告，等主程确认
```

---

## 🎯 快速决策矩阵（会话数量建议）

| Phase | 推荐会话数 | 是否可并行 | 说明 |
|-------|-----------|----------|------|
| Phase 0 | **1 个**（不并行） | ❌ | 骨架必须一人统一口径 |
| Phase 1 | **1-2 个串行** | ⚠️ | 玩法核心耦合度高，建议串行；若并行需先拆分接口 |
| Phase 2 | **4 个并行** | ✅ | 补给包/词库/关卡/玩法增味（会话 D）耦合低，可并行 |
| Phase 3 | **2-3 个并行** | ✅ | 视觉/音效/平台适配可并行；广告需单独会话 |

**并行的前提是**：Architecture.md 中的接口已经锁定，各方实现只依赖接口不依赖具体类。

---

## 📊 版本对比

| 版本 | 日期 | 变更 |
|------|------|------|
| v1.0 | 2026-11-04 | 初版发布，包含 Phase 0-3 完整任务模板 + 通用会话格式 |
| v1.1 | 2026-11-05 | 同步新增 ProjectSetup.md 引用；任务模板中 4 份 → 5 份文档；相关文档版本升至 v1.1；任务内容无变更 |
| v1.2 | 2026-11-05 | **平台方向调整**：目标定为 **Android + iOS 移动 App**，类目 **休闲益智**；剔除小游戏/WebGL 相关行；Phase 0 DoD 新增“能构建 AAB/iOS 空包”；Phase 3 广告/平台适配/发布准备重写；相关文档版本升至 v1.2 |
| v1.3 | 2026-11-05 | **发行策略定案 + 合规入场**：**不开版号、纯广告变现（0 内购）**；新增关联文档 [Compliance.md](./Compliance.md)（开场引用从 5 份 → 6 份）；Phase 0 严格禁止新增“不上架国内安卓市场/不接防沉迷 SDK”，DoD 新增“家长同意弹窗骨架”；Phase 3 广告接入/平台适配/发布准备/验收按 Compliance.md 重写（剔除版号展示与防沉迷前置）；术语软化已落实（僵尸→小怪兽/精英→大怪兽/BOSS 僵尸→BOSS 怪） |
| v1.4 | 2026-11-05 | **四项定案（GDD §12 D1-D4）落入任务模板**：<br>**D1** 商店界面保留但改为**能量晶商店（0 内购）**：Phase 0 新增 `IStoreService` + `PurchaseCurrency`，Phase 2 会话 C 新增商店 UI 任务，严格禁止/DoD 新增“不接入支付 SDK”与 `PurchaseCurrency` 单值反射断言<br>**D2** 全文剔除 `HP/MaxHP` 口径：能量墙改为 `IWall.CurrentDurability/MaxDurability/IsBroken`，耐久上限由 `energyWallLevel` 决定（100→500）；**明确避难所无血量不可被攻击**，耐久归零为唯一失败判定；急救补给站“恢复 20 HP”→“恢复 20 点耐久”<br>**D3** 敌人固定 3 类（小/大/BOSS 怪），BOSS 每局仅 1 只；`maxHP=1/3/10` → `requiredAnswers=1/3/10`；声明不存在快速/坦克小怪兽与最终 BOSS<br>**D4** 三会仅保留 `IWordStudyService` + `NoOpWordStudyService`（Phase 4+）；**单机不做排行榜**，Phase 3 新增“本地纪录”任务组；剔除国内五商商店截图与蒲公英分发；**新增锁定横屏任务**；关联文档版本串升至 GDD v2.2 / TechStack v1.5 / Architecture v1.4 / ProjectSetup v1.5 / Compliance v1.1 | 主程 |
| v1.5 | 2026-11-05 | **GDD §12 Q1-Q8 拍板转定案 D8-D14，任务模板同步伤害制**：<br>**D8 伤害制**（推翻旧“固定 1/3/10 题”）：Phase 1 关键规则/任务分解重写——答对=一次暴击（伤害=武器攻击力×200%，基础武器 100⇒每发 200），小怪/大怪/BOSS 活力值 200/600/2000，击败所需答对数=⌈活力÷每发伤害⌉，**BOSS 不恒定 10 题**；Entities 骨架 `RequiredAnswers`→`MaxVitality/CurrentVitality`；QuizSM 单词队列/DoD 改为活力制；Phase 0 严格禁止措辞同步<br>**D9** WallManager 明确“耐久不跨局继承，每局重置满值”<br>**D10** Phase 2 复习/错题/挑战关入口归**词汇档案室**<br>**D2/Q8** WallManager.Repair 新增“战斗中不得消耗能量晶修复能量墙（1 阶无应急修复）”<br>**D13** 可见文案软化：击倒/击杀→击败（目标/波次结束/奖励结算/SFX/DoD）<br>开场版本串与关联文档升至 GDD v2.3 / TechStack v1.6 / Architecture v1.5 / ProjectSetup v1.6 / Compliance v1.2 | 主程 |
| v1.6 | 2026-11-05 | **GDD §12 D15/D16（战场几何 + 技能包必用策略层）落入任务模板**：<br>**D15** Phase 1 关键规则新增“左起 1/6 触发答题、4/6 停止攻墙；当前锁定目标答题期间不前进、恢复推进 30% 速度”<br>**D16** Phase 2 会话 A 补给包系统重写为控制类/转化类分类、掉落保底、跨波囤积、同时生效≤ 2、火焰灼烧重定义、广告红线；Phase 1 关键规则新增技能包必用与“伤害一律由答对触发”<br>开场/通用模板版本串与关联文档升至 GDD v2.4 / TechStack v1.7 / Architecture v1.6 / ProjectSetup v1.7 / Compliance v1.3 | 主程 |
| v1.7 | 2026-11-06 | **玩法增味（GDD §12 D17/D18/D19）落入任务模板（新增 Phase 2 会话 D）**：<br>**D17**：Phase 1 关键规则更新为连击窗口 3→4s、档位 3/5/8/12/20、即时命中（取消答对 1s 硬等待）与 Swift；会话 D 给 `ZombieController`/`QuizConfig` 实现任务<br>**D18**：会话 D 新增 `EnemyModifier`（Shielded/Sprinter/Mender）/`WaveEventType`（Rush/BossPressure）/`BattleDoctrine`（开局三选一仅本局）实现任务；不新增 `ZombieType`（守 D3）<br>**D19**：会话 D 新增 `LocalAchievementService`（本地轻成就）与前期曲线（`earlyChapterBonus=1.3`、首档建筑 200→100）；Phase 3 本地纪录段注“三会/服装/每日任务仍 Phase 4+、本地轻成就已 Phase 2 会话 D”<br>**E 一致性**：模板内敌人速度统一 1.0/1.2/0.7 m·s⁻¹、伤害仅答对触发口径保留；Phase 2 并行会话数 3→4；开场/通用模板与关联文档升至 GDD v2.5 / TechStack v1.8 / Architecture v1.7 / ProjectSetup v1.8 / Compliance v1.4 | 主策划 |
| v1.8 | 2026-11-06 | **玩法增味 II（GDD §12 修订 D15、新增 D20）落入任务模板**：<br>**D15** Phase 1 关键规则改为“锁定与非锁定全程正常速度推进、取消不前进/30% 减速、当前锁定目标高亮（view）”；会话 D item 1 同步、新增 item 9 目标高亮（`GameConfig.LockedTargetHighlight`）<br>**D20** 会话 D 新增 item 10 `EnemyModifier.Wanderer` 稀有游走怪（金库怪掉能量晶/铁匠怪掉碎片、停止推进不攻墙、点击锁定、不计通关）；会话 D DoD 补 Wanderer+高亮验收<br>**词池优先级**：Phase 1 关键规则与 WordPoolManager 补充优先级新增**上一局错词**（上一局错词>已掌握>待复习>未学习）；开场/模板串与关联文档升至 GDD v2.6 / TechStack v1.9 / Architecture v1.8 / ProjectSetup v1.9 / Compliance v1.5 | 主策划 |
| v1.9 | 2026-11-06 | **认词正反馈显性化（GDD §12 D21）落入任务模板**：会话 D 新增 item 11 **D21 M1** 生词上膛可视化（头顶词汇标记叠加 `Word.memoryStrength`、四态兜底、纯表现）/ item 12 **D21 M2** 首克新词、攻克错题当帧奖励（`LearningSystem` 发 `FirstWordMasteredEvent`/`WrongWordFixedEvent`、`RewardCalculator` 加 `FirstMasterCrystalBonus=3`/`WordFixCrystalBonus=2`、里程碑全局一次性）/ item 13 **D21 M3** Mastery Meter（`MasteryMeterService` 每出题刷占比→`BattleModifiers.swiftThresholdBonus/comboWindowBonus`，仅本局、只动阈值不动伤害）；会话 D 标题/注与 DoD 补 D21；开场/输入/模板串升至 GDD v2.7 / TechStack v1.10 / Architecture v1.9 / ProjectSetup v1.10 / Compliance v1.6 | 主策划 |
| v1.10 | 2026-11-06 | **主程审核修复 + 基线串升版**：修正 Phase 0 输入文档串残留的旧版本号（v1.9 编辑时漏改：TechStack v1.9→v1.11、Architecture v1.8→v1.10、DevTasks v1.8→v1.10、ProjectSetup v1.9→v1.11、Compliance v1.5→v1.7）；**版本串终检另补修 Phase 1 会话模板前置检查串的同类漂移（GDD v2.7→v2.8、TechStack v1.9→v1.11、Architecture v1.8→v1.10、关键规则标题“严格按 GDD v2.7”→v2.8）**；开场/通用模板/会话 D 注与关联文档串升至 GDD v2.8 / TechStack v1.11 / Architecture v1.10 / ProjectSetup v1.11 / Compliance v1.7；任务内容无其它变更 | 主程 |
| v1.11 | 2026-11-06 | **十项拍板级联（GDD v2.9 / TechStack v1.12 / Architecture v1.11）落入任务模板**：**⑩ Spine2D**（Phase 0 关键决策 L55、Phase 1 会话 B SpriteAnimationPlayer 骨架标注仅特效）；**⑧ BattleState** Phase 0 输入串标“Architecture 顶层 6 态为真源”；**v2.9 玩法口径** Phase 0/1 输入与模板串补充星级分母=本局实际首次出题数/武器去射速/BOSS 关带 2 大怪；**① 金币=能量晶**（无第二货币）；开场必引用串/Phase 0 输入清单/Phase 1 前置检查串与关联文档串均升至 GDD v2.9 / TechStack v1.12 / Architecture v1.11 / DevTasks v1.11 / ProjectSetup v1.12 / Compliance v1.8 | 主程/主策划 |
| v1.12 | 2026-11-06 | **字体归一 + OFL 重命名落入任务模板（同步 GDD v2.10 / TechStack v1.13 §5.1 / Compliance v1.9 §10；配合“不买字体版权”）**：Phase 3 “字体本地化”任务改写——中文 `ShelterHan`、拉丁(英/数) `ShelterBaloo`（均 OFL 子集重命名），**DIN Alternate（商业专有）/Roboto Mono 弃用**，符号/emoji 走 `ic_*` 图标（禁系统 emoji 直显），中文动态字体+Fallback 防生僻字，随包附 `OFL.txt` 与许可快照；开场必引用/Phase 0、Phase 1 输入清单与模板串均升至 GDD v2.10 / TechStack v1.13 / Architecture v1.12 / DevTasks v1.12 / ProjectSetup v1.13 / Compliance v1.9 | 主程/主策划 |
| v1.13 | 2026-11-07 | **字体评审发现落入任务模板（同步 GDD v2.11 / TechStack v1.14 §5.1 / Compliance v1.10 §10）**：Phase 3 “字体本地化”任务——中文改为静态主资产 `ShelterHan`（3500）+ `ShelterHanFB` 扩展字集 Fallback 资产防生僻字（**默认不用全量动态字体**）；“上架合规包”新增 **开源许可/素材清单页**（设置内文本页，`OFL.txt`+字体/SIL OFL+SDK 许可+素材来源）；验收 DoD 新增字体/素材 OFL 许可随附可见项；开场必引用/Phase 0、Phase 1 输入清单与模板串均升至 GDD v2.11 / TechStack v1.14 / Architecture v1.13 / DevTasks v1.13 / ProjectSetup v1.14 / Compliance v1.10 | 主程/主策划 |
| v1.14 | 2026-11-07 | **发行路线调整落入任务模板（同步 GDD v2.12 D22 / Compliance v1.11 / Architecture v1.14 新增契约）**：<br>header 与开场/输入/模板版本串全量升至 GDD v2.12 / TechStack v1.15 / Architecture v1.14 / DevTasks v1.14 / ProjectSetup v1.15 / Compliance v1.11；**发行策略**（header/关键决策）改「全年龄·国内主市场双阶段：版号前灰区 TestFlight+官网 APK 不提审商店→融资→版号→国内商店上市，放弃海外、Steam 暂缓」；Phase 0 DoD「家长同意弹窗骨架」改「年龄识别 + 家长同意（`IAgeVerifyService`，手机号实名+运营商一键登录、Mock 骨架、不自建后端）」；严格禁止项「不上架国内安卓市场」改「版号前不上架任何商店」、「不接入防沉迷 SDK」补「后置版号阶段、验证走运营商一键登录（`IPlatformService` 预留）」；Compliance 输入项描述 §2 儿童合规→§2 未成年人保护 + §1 国内双阶段 | 主程/制作人 |
| v1.15 | 2026-11-07 | **敌人种类扩张与包体重建落入任务模板（同步 GDD v2.13 D23 / TechStack v1.16 / Architecture v1.15）**：<br>**D23 任务排批**：Phase 2 会话 D 新增 item 14 `ZombieSpeciesConfig` 数据层接入与 P1 首批 9 种骨架（小 5/大 3/BOSS 1；`ZombieType` 枚举零新增；P2/P3 随名录表追加不回改）；Phase 1 关键规则标“先原型骨架占位、种接入在 Phase 2”；会话 D DoD 补 species 验收项<br>**包体梯度**：Phase 2 DoD <80MB→**≤140MB（P2 梯度）**、Phase 3 DoD <80MB→**≤200MB 唯一硬线**（真源 ProjectSetup §11.1 / TechStack §18）；Phase 3 英文语音改 VO 转正预录（D23 品质上调）<br>**口径同步**：Phase 0 关键决策 D3 行与 Spine 行标 D23（SkeletonDataAsset 按种独立 30 份）；header/开场必引用/Phase 0、1 输入清单与通用模板串全量升至 GDD v2.13 / TechStack v1.16 / Architecture v1.15 / DevTasks v1.15 / ProjectSetup v1.16 / Compliance v1.12 | 主程 |

---

> **使用提醒**：
> 1. 每次开工前**先明确当前 Phase**
> 2. 复制模板内容到 Qoder IDE **Agent 模式**新会话
> 3. 会话第一条消息务必让 Agent 读取 6 份文档
> 4. 遇到偏离模板的情况，先更新本文档 + Architecture.md，再改代码
> 5. 所有会话结束要求 Agent 明确列出 **DoD 完成情况**
