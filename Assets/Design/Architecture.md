# 架构设计说明 (Architecture)

| 项目 | 避难防线 - 词汇守卫者 |
|------|-------------------|
| 文档版本 | v1.15 |
| 最后更新 | 2026-11-07 |
| 关联文档 | [GDD.MD](./GDD.MD) v2.13、[TechStack.md](./TechStack.md) v1.16、[DevTasks.md](./DevTasks.md) v1.15、[ProjectSetup.md](./ProjectSetup.md) v1.16、[Compliance.md](./Compliance.md) v1.12 |
| 目标平台 | **Android + iOS 移动 App**（锁横屏；休闲益智类目；**全年龄英语学习**；**国内主市场：版号前灰区内测 → 版号后上市**；**纯广告变现 · 0 内购**；GDD §12 D22）；**不做微信/抖音/团结小游戏** |
| 状态 | 主程已确认，Phase 0 开工基线 |

---

## 0. 阅读指引

本文件定义**模块边界、类职责、接口契约和事件协议**。所有 Agent 会话在实现代码前**必须先读本文件**，产出代码必须严格符合此处定义的接口。

任何接口改动必须先修改本文件并走评审，再动代码。

---

## 1. 分层架构

```
┌─────────────────────────────────────────────────────┐
│                    Scenes（场景层）                   │
│  MainMenu / LevelSelect / Battle / Result / Shelter │
├─────────────────────────────────────────────────────┤
│                     UI（表现层）                     │
│  HUD / QuizPanel / WaveAlert / SupplyPackPanel      │
├─────────────────────────────────────────────────────┤
│                  Systems（业务系统层）                │
│  BattleFlow │ Wave │ Wall │ Quiz │ Reward         │
│  WordPool │ Learning │ SupplyPack │ Save            │
├─────────────────────────────────────────────────────┤
│                  Entities（游戏实体层）               │
│  ZombieBase / NormalZombie / EliteZombie / Boss     │
│  Bullet / Laser / Grenade / WallObject            │
├─────────────────────────────────────────────────────┤
│                  Core（核心/契约层）                  │
│  Interfaces / Events / Enums / EventBus / Managers  │
├─────────────────────────────────────────────────────┤
│                  Config（配置层）                     │
│  ScriptableObjects：Level / Zombie / Quiz / Weapon  │
└─────────────────────────────────────────────────────┘
```

**依赖规则**（严格单向）：
- Config ← 所有层可引用
- Core ← 所有层可引用
- Entities ← 只可引用 Core、Config
- Systems ← 可引用 Core、Config、Entities
- UI ← 可引用 Core、Systems（只读事件）
- Scenes ← 装配所有层

**禁止**：Systems 不得引用 UI，UI 不得直接改 Systems 内部状态。

---

## 2. 目录结构

```
Assets/
├── Design/
│   ├── GDD.MD
│   ├── TechStack.md
│   └── Architecture.md
├── Scripts/
│   ├── Core/
│   │   ├── Interfaces/          # IZombie, IQuizTarget 等
│   │   ├── Enums/               # ZombieType, QuizState 等
│   │   ├── Events/              # 所有 event struct
│   │   ├── EventBus.cs          # 全局事件中心
│   │   ├── Constants.cs         # 全局常量（像素/单位比例等）
│   │   └── Managers/            # GameManager, PauseManager, SaveManager
│   ├── Config/                  # ScriptableObject 定义
│   │   ├── LevelConfig.cs
│   │   ├── WaveData.cs
│   │   ├── ZombieConfig.cs
│   │   ├── QuizConfig.cs
│   │   ├── SupplyPackConfig.cs
│   │   └── WeaponConfig.cs
│   ├── Entities/                # 游戏对象
│   │   ├── Zombie/
│   │   │   ├── ZombieBase.cs
│   │   │   ├── NormalZombie.cs
│   │   │   ├── EliteZombie.cs
│   │   │   └── BossZombie.cs
│   │   ├── Wall/
│   │   ├── Building/
│   │   └── Effects/
│   ├── Systems/                 # 业务系统
│   │   ├── Battle/
│   │   │   ├── BattleFlowController.cs
│   │   │   ├── WaveController.cs
│   │   │   ├── WallManager.cs
│   │   │   └── SpawnPoint.cs
│   │   ├── Quiz/
│   │   │   ├── QuizStateMachine.cs
│   │   │   ├── QuestionLoader.cs
│   │   │   ├── AnswerInputHandler.cs
│   │   │   └── ComboTracker.cs
│   │   ├── WordPool/
│   │   │   ├── WordPoolManager.cs
│   │   │   ├── WordReplenisher.cs
│   │   │   └── WrongWordBook.cs
│   │   ├── Learning/
│   │   │   ├── WordMasteryTracker.cs
│   │   │   └── ReviewScheduler.cs
│   │   ├── Reward/
│   │   │   ├── RewardCalculator.cs
│   │   │   └── StarRatingEvaluator.cs
│   │   └── Supply/
│   │       ├── SupplyPackDrop.cs
│   │       ├── SupplyPackEffect.cs
│   │       └── SupplyPackUpgrade.cs
│   ├── UI/
│   │   ├── HUD/
│   │   ├── QuizPanel/
│   │   ├── ResultPanel/
│   │   └── Common/
│   └── Bootstrap/
│       └── BattleBootstrap.cs   # 战斗场景装配器
├── Resources/
│   ├── Levels/
│   ├── Config/
│   └── Data/
├── Scenes/
│   ├── MainMenu.unity
│   ├── LevelSelect.unity
│   ├── Battle.unity
│   └── Result.unity
└── Editor/                      # 自定义编辑器工具
```

---

## 3. 核心枚举（定义于 Core/Enums/）

```csharp
public enum ZombieType { Normal = 0, Elite = 1, Boss = 2 }

public enum QuizState {
    IDLE,
    QUESTION_LOADING,
    WAITING_ANSWER,
    CORRECT_RESOLVE,
    WRONG_RESOLVE,
    TIMEOUT_RESOLVE,
    ZOMBIE_KILLED,
    NEXT_QUESTION
}

public enum BattleState { Loading, Running, Paused, Victory, Defeat, Quit }

public enum WordMasteryLevel { Unlearned, Learning, Mastered, LongTermMastered }

public enum SupplyPackType { Ice, Slow, Lightning, Confuse, Spread, Fire, Pierce }
// 分类（GDD §12 D16）：控制类=Ice/Slow/Confuse（**不造成任何伤害**，仅延长答题窗口）；转化类=Lightning/Spread/Fire/Pierce（把一次答对的暴击扩散到多目标，伤害仍由“答对”触发）
// 同时生效上限：1 控制 + 1 转化（GDD §12 D11/D16）；PlayerData.activeSupplyPacks[] 长度 ≤ 2，控制类可跨波囤积，至多 1 个

// —— 以下枚举为玩法增味（GDD §12 D17/D18/D19/D20，详细规格见 GDD §2.6）——
// D18：机制变体。不新增 ZombieType（D3 三类原型框架不变；D23 扩种走 ZombieSpeciesConfig 数据层、不加枚举值），逐只在 LevelConfig.waves[].spawns[] 配 modifiers[]；Shielded/Sprinter/Mender=Phase3，TwinWord=Phase4
// D20：Wanderer=稀有游走奖励怪（金库怪/铁匠怪，Phase3），不新增 ZombieType；进入答题圈后停止推进、随机游走、永不攻墙、不计入通关，掉落由 ZombieConfig.dropOverride
public enum EnemyModifier { None = 0, Shielded, Sprinter, Mender, TwinWord, Wanderer }
// D18：事件波。挂在 WaveConfig.eventType；Rush/BossPressure=Phase3，Ambush=Phase4（需 ZombieController 支持反向推进）
public enum WaveEventType { None = 0, Rush, BossPressure, Ambush }
// D18：局内战术构筑。关卡开始前 3 选 1，仅本局生效、不写回永久存档；与星级/经济解耦；doctrineEnabled=false 时不弹
//   Fire→BattleModifiers.attackMultiplier=1.2；Bulwark→wallDurabilityMultiplier=1.3（守 D9，仅本局）；Logistics→掉落保底+1 与 waveIntervalSeconds-1
public enum BattleDoctrine { None = 0, Fire, Bulwark, Logistics }
// D19：本版本地轻量成就（纯单机、读本地统计一次性发奖），区别于 Phase 4+ 三会/联网/每日成就（不违反 D4）
public enum LocalAchievementCategory { Defeat, Combo, Mastery, Clear, Collection }

// D21：局内运行时加成容器（仅本局生效、不写存档，与 BattleDoctrine 同结构）
public class BattleModifiers {
    public float attackMultiplier = 1f;            // Fire（D18）
    public float wallDurabilityMultiplier = 1f;    // Bulwark（D18）
    public float swiftThresholdBonus = 0f;         // D21 M3：Mastery Meter≥60% → +0.5s（SwiftThresholdSeconds 3.0→3.5）
    public float comboWindowBonus = 0f;            // D21 M3：Mastery Meter≥80% → +0.5s（ComboWindowSeconds 4.0→4.5）
}
// D21 M3：Mastery Meter 仅动“触发节奏”（阈值/窗口），不动伤害（守 D8/D16）；伤害仍只由“答对”触发
public interface IMasteryMeterService { float Percent { get; } }   // 本局已出题中 Mastered/LongTermMastered 词占比∈[0,1]，每出题刷新

public enum QuestionType { EN2ZH, ZH2EN } // 英译中 / 中译英（TwinWord 变体的配对题面属 Phase 4，届时扩展）

public enum StarRating { Fail = 0, OneStar = 1, TwoStar = 2, ThreeStar = 3 }
```

---

## 4. 核心接口（定义于 Core/Interfaces/）

### 4.1 IZombie

```csharp
/// <summary>
/// 所有小怪兽的公共契约。答题状态机通过此接口与小怪兽交互。
/// </summary>
public interface IZombie {
    // ---- 数据 ----
    int Id { get; }                     // 唯一实例 ID（运行时生成）
    ZombieType Type { get; }
    EnemyModifier Modifier { get; }     // GDD D18：叠加机制变体（不改变 Type）；Shielded 时下方 HasActiveShield 为 true
    bool HasActiveShield { get; }       // GDD D18：有 1 层护盾时，首次 TakeDamage 只破盾、不扣 CurrentVitality（伤害被吸收，非自动掉血）
    int MaxVitality { get; }            // 活力值上限：小怪=200，大怪=600，BOSS=2000（GDD §12 D8）
    int CurrentVitality { get; }        // 当前活力值（**不叫 HP / 血量**；玩家可见文案叫“活力值”）
    bool IsReachedWall { get; }         // 是否已到达能量墙
    bool IsAlive { get; }
    Vector3 Position { get; }
    float DistanceToWall { get; }

    // ---- 行为 ----
    void TakeDamage(int amount);        // CurrentVitality -= amount（amount = 暴击伤害 = 武器攻击力×200%，GDD D8），归零触发被击败
    void Pause(float duration);         // 停顿一段时间（不影响其他小怪兽）
    void Resume();
    void Die();                         // 播放被净化动画 → 触发掉落 → 销毁

    // ---- 事件（内部） ----
    event Action<IZombie, int> OnDamaged;    // (zombie, deltaVitality)
    event Action<IZombie> OnReachedWall;
    event Action<IZombie> OnDied;
}
```

> **小怪兽没有 Heal / 回血**：答错或超时**不造成任何伤害、也不恢复活力值**（GDD §12 D8），因此不暴露 `Heal` 接口。**`IZombie` 不再持有“题数”字段**：击败所需答对次数由调用方（QuizStateMachine）按 `⌈MaxVitality ÷（武器攻击力×200%）⌉` 动态计算；基础武器（攻击 100 ⇒ 每发 200）下即小怪 1 / 大怪 3 / BOSS 10。
>
> **伤害来源铁律（GDD §12 D8/D16）**：`TakeDamage` **只能由“答对”结算路径调用**（含连击 buff 与转化类补给将这次答对扩散到多目标）；控制类补给（Ice/Slow/Confuse）**不得调用 `TakeDamage`**，只改变移动状态。
>
> **目标推进与高亮（GDD §12 D15，v2.6 修订）**：**取消旧“锁定目标答题期不前进（Pause）/恢复 30% 减速”**；`ZombieController` 使锁定与非锁定敌人**全程按各自基础速度正常推进**（世界不因答题而暂停/减速）；当前锁定答题目标（由 `IQuizTargetProvider` 返回的实例）在表现层高亮环 + 指示连线，**不新增 `IZombie` 字段**（高亮为 view 层，目标身份取自 provider）。买时间仅由控制类补给（Ice/Slow/Confuse）改变移动状态实现（D16）。
>
> **稀有游走奖励怪（GDD §12 D20）**：`Modifier == EnemyModifier.Wanderer` 时，`ZombieController` 令其进入 1/6 答题圈后**不再向 4/6 推进**（`IsReachedWall` 恒为 false、永不触发 `OnReachedWall`），在走廊 `[1/6,4/6]` 内以 `WanderSpeed=0.6 m/s` 随机游走；默认不自动入队串行答题，**玩家点击可将其置为当前锁定目标**；波次结束仍存活则 `Die()` 但不掉落（`dropOverride` 仅在答对击败时生效）；**不计入波次完成/通关判定**。不新增伤害来源（仍由“答对” `TakeDamage`）。
>
> **即时命中与 Swift（GDD §12 D17）**：答对→**当帧**调用 `TakeDamage`（取消旧“停顿 1 秒”硬等待，`ZombieController` 不再因答对阻塞位移）；受击只播 `HitStunSeconds=0.3` 硬直动画。单题用时 ≤ `SwiftThresholdSeconds` 且答对时，调用方传入的 `amount` 已乘 `SwiftDamageMultiplier=1.15`（Swift 只是“答对暴击”的一个乘数，不新增伤害来源）。
>
> **护盾与治疗变体（GDD §12 D18）**：`Shielded` 首次 `TakeDamage` 仅置 `HasActiveShield=false`、不扣活力（伤害被盾吸收，仍符合“伤害只由答对触发”）；`Mender` 的回复是**敌方机制**（每 `MenderTickSeconds=5` 秒给半径内一只友军 +武器攻击力×200% 活力），**不是玩家伤害**，不占用 `TakeDamage`（目标回复另走 `CurrentVitality` 上调，不超过 `MaxVitality`）；`Sprinter` 仅由 `ZombieController` 将移速 ×1.5。

### 4.2 IQuizTargetProvider

```csharp
/// <summary>
/// 答题状态机通过此接口向战斗系统索取"下一个待答目标的小怪兽"
/// </summary>
public interface IQuizTargetProvider {
    IZombie AcquireNextTarget();               // 返回进入答题圈的下一只小怪兽
    bool HasPendingTarget { get; }
}
```

### 4.3 IWordProvider

```csharp
/// <summary>
/// 单词来源接口，QuestionLoader 从此获取单词，支持多种来源
/// （普通关卡词库 / 7 日复习 / 错题回溯 / 词池补充）
/// </summary>
public interface IWordProvider {
    Word GetNextWord();                        // 按当前关卡策略返回下一个单词
    int RemainingCount { get; }
    int InitialPoolCount { get; }              // 用于答对率分母，固定不变
    bool ShouldReplenish { get; }
    void RemoveWord(int wordId);
    void ReturnToWrongBook(Word word);
}

/// <summary>
/// 单词定义（class，全局唯一引用）
/// 仅通过 WordDatabase SO 加载，不在运行时 new
/// </summary>
public class Word {
    public int id;
    public string english;
    public string chinese;
    public string phonetic;
    public int difficulty;                     // 1-5
    public WordMasteryLevel mastery;
    public float memoryStrength;               // D21 M1：记忆强度连续量∈[0,1]，仅表现（头顶“上膛”进度）；Phase 1 无连续量则由 mastery 四态兜底，不参与伤害/答题结算
    public string textbookKey;                 // 如 "RenJiao_G3_U5"
    public int grade;
    public int unit;
}

/// <summary>
/// 传递用的轻量引用（可选优化，高频传递时使用）
/// </summary>
public readonly struct WordRef {
    public readonly int wordId;
    public Word Resolve(IWordProvider provider) => provider.GetById(wordId);
}

/// <summary>
/// 词库服务抽象（支持 Phase 0-1 Resources，Phase 2+ Addressables）
/// </summary>
public interface IWordLibraryService {
    Task<TextbookMetadata[]> ListAvailable();     // 仅元数据
    Task<WordDatabase> LoadLibrary(string key);   // 按需加载
    void ReleaseLibrary(string key);              // 不再使用时释放
}

[Serializable]
public class TextbookMetadata {
    public string textbookKey;
    public string displayName;                   // "人教版三年级"
    public int grade;
    public string textbookVersion;               // "人教版/外研社/..."
    public int totalWordCount;
}
```

### 4.4 IQuestionPresenter

```csharp
/// <summary>
/// Quiz 系统对 UI 层的抽象：状态机不直接依赖 UI，通过接口调用
/// </summary>
public interface IQuestionPresenter {
    void ShowQuestion(WordQuestion question);
    void HideQuestion();
    void ShowCorrectAnswerFeedback(WordQuestion q);   // 答错时高亮正确选项
    void ShowTauntBubble(string text, float duration);
    void UpdateTimer(float remaining, float total);
    event Action<int> OnAnswerSubmitted;              // 玩家选择的索引 0-3
}

[Serializable]
public class WordQuestion {
    public Word target;
    public QuestionType type;
    public Word[] options;          // 4 个选项
    public int correctIndex;        // 正确选项索引
    public float timeLimit;         // 本题时限（秒）
}
```

### 4.5 IWall（防线数值唯一主体）

> GDD §12 D2：**避难所无血量、不可被攻击**；全场唯一失败数值就是能量墙耐久。因此**不存在第二个 `IShelter`/`IBase` 血量接口**。

```csharp
public interface IWall {
    int CurrentDurability { get; }      // 当前耐久
    int MaxDurability { get; }          // 上限 = 能量墙阶位（1 阶 100 → 5 阶 500）
    float DurabilityPercent { get; }    // 0-1，星级判定用
    bool IsBroken { get; }              // CurrentDurability <= 0，触发即失败
    void TakeDamage(int amount);
    void Repair(int amount);            // 仅急救补给站触发（战斗中不得消耗能量晶修复，GDD D2/Q8）；不溢出 MaxDurability
    event Action<int, int> OnWallChanged;   // (current, max)
    event Action OnWallBroken;
}
```

### 4.6 IRewardCalculator

```csharp
public interface IRewardCalculator {
    RewardResult Calculate(
        int normalKills,
        int eliteKills,
        int bossKills,
        BattleResult result,       // Victory / Defeat / Quit
        int correctAnswers,
        int initialWordPoolCount,
        float durabilityPercent);
}

[Serializable]
public struct RewardResult {
    public int energyCrystal;
    public int gold;
    public int weaponShard;
    public StarRating starRating;
    public float rewardMultiplier;
    public int wrongWordsAdded;
}
```

### 4.7 ISceneLoader

```csharp
/// <summary>
/// 场景加载抽象层：Phase 0-1 默认 SceneManager 实现，Phase 2+ 可切 Addressables 实现
/// 上层代码不受实现影响
/// </summary>
public interface ISceneLoader {
    UniTask LoadAsync(string sceneName, IProgress<float> progress = null);
}

// 默认实现：SceneManagerSceneLoader（基于 SceneManager.LoadSceneAsync）
// Phase 2 可新增：AddressableSceneLoader
```

### 4.8 IAdsService

```csharp
/// <summary>
/// 广告抽象服务：完全离线时返回 Mock；平台自带广告时包装接口
/// </summary>
public interface IAdsService {
    bool IsAvailable { get; }                          // 当前环境能否展示广告
    UniTask<bool> ShowRewardedAsync(string placementId); // 返回是否完成观看
    UniTask ShowInterstitialAsync(string placementId);
    bool CanShowInThisSession { get; }                 // 未成年人保护：单局频率上限（成年建议≤3/未成年≤1）
}

// 实现（目标平台 = Android + iOS 移动 App）：
// - MockAdsService（无网络/编辑器/测试）
// - PangleAdsService（穿山甲，国内 Android/iOS）
// - TencentOptimusAdsService（优量汇，国内备份）
// - AdMobAdsService（海外）：~~因放弃海外发行不再集成~~（保留接口，若回评海外再启用）
// ❌ 不再包含小游戏实现（项目已放弃微信/抖音/团结小游）
```

### 4.9 IAnalyticsService

```csharp
/// <summary>
/// 埋点服务抽象：自研实现，未来可升级 ThinkingData/Firebase
/// </summary>
public interface IAnalyticsService {
    void LogEvent(string eventName, Dictionary<string, object> properties = null);
    void LogError(string tag, string message, Exception ex = null);
    void Flush();                            // 强制写入本地日志
}

// 三层实现：
// - LocalFileSink：永远开，写 persistentDataPath
// - StructuredEventSink：游戏事件写入环形内存 buffer
// - RemoteUploadSink：默认关；开启且有网时批量上报
```

### 4.10 IWordStudyService（三会预留 · Phase 0-3 不实现）

> GDD §12 **D4 已定案**：三会（会读/会认/会写）本版不做。Phase 0 只交付**接口 + `NoOpWordStudyService`**（所有方法返回默认值、不写存档、不弹 UI），以保证后续接入不改动现有调用点。

```csharp
public enum StudyMode { Read = 0, Recognize = 1, Write = 2 }   // 会读 / 会认 / 会写

/// <summary>
/// 三会自主学习服务抽象。Phase 0-3 为 NoOp 实现；Phase 4+ 填充真实逻辑。
/// 不接入战斗流程，不影响 QuizStateMachine。
/// </summary>
public interface IWordStudyService {
    bool IsEnabled { get; }                          // NoOp 实现恒返 false
    StudyProgress GetProgress(int wordId);           // 三模式各自星级/进度，无数据时返回全零
    bool SubmitResult(int wordId, StudyMode mode, bool correct);
    event Action<WordStudyCompletedEvent> OnWordFullyStudied;   // 三会均通过（Phase 4+ 才会发布）
}

[Serializable]
public struct StudyProgress {
    public int readStars;        // 0-5
    public int recognizeStars;   // 0-5
    public int writeStars;       // 0-5
}
```

**实现约束**：
- 文件位置：`Assets/Scripts/Core/Interfaces/IWordStudyService.cs` + `Assets/Scripts/Systems/Learning/NoOpWordStudyService.cs`
- `WordMasteryLevel` 四态仍由 `LearningSystem` 维护；三会进度**不写入现有 `WordData.masteryLevel`**（避免两套语义互相覆盖），Phase 4+ 新增 `WordData.studyProgress` 子结构
- 拼写题型（`QuestionType.Spelling`）**不新增**，以保证 Phase 0-1 的四选一 UI 不回改

### 4.11 IStoreService（能量晶商店 · 0 内购）

> GDD §12 **D1 已定案**：商店保留，但**只收能量晶**，能量晶**不可充值**。**不存在 `IPurchaseService` / IAP 接口**，不引入 `com.unity.purchasing`。

```csharp
/// <summary>
/// 本地商店：仅消耗游戏内货币，无任何真实货币路径。
/// </summary>
public interface IStoreService {
    StoreCatalog GetCatalog();                       // 在售列表（词库/补给包/记忆卡）
    StoreResult Purchase(string itemId, PurchaseCurrency currency);   // 只接受 EnergyCrystal
    bool CanUnlock(string wordBankId, out UnlockBlockReason reason);  // 掌握度门槛检查
}

public enum PurchaseCurrency { EnergyCrystal }        // ← 故意只有 1 个枚举值，禁止扩展 RealMoney

public struct StoreResult {
    public bool success;
    public int costCrystal;
    public string failReason;                        // INSUFFICIENT_CRYSTAL / LOCKED_BY_MASTERY / ALREADY_OWNED
}
```

**硬约束**：
- `PurchaseCurrency` 永远不得包含 `RealMoney` / `IAP` / `AdReward` 类型；CI 加一条反射断言：`Enum.GetNames(typeof(PurchaseCurrency)).Length != 1` 即判定失败（防止有人默默加回真钱货币）
- 广告奖励只能直接发能量晶/补给包，**不得经由“购买”路径**（`IAdsService.RewardCompleted` 直接调用 `SaveManager`）；**过关必需的技能包只能来自游戏内掉落，激励视频只能发放非必需增益包，不得构成“不看广告就必输”（GDD §12 D16 / Compliance §3）**
- UI 层不得出现价格符号、充值按钮（Compliance.md §3.3）

### 4.12 IAgeVerifyService（年龄识别 + 家长同意 · GDD §12 D22 / Compliance §2 C1）

> **本版必做（国内主市场 · 全年龄）**：首次启动阻塞流程，判定未成年且无家长同意则退出。采用**国内通用做法**：手机号实名 + **运营商一键登录/号码认证**（客户端 SDK 直连运营商网关、**不自建后端**），短信验证码作备选。手机号**仅用于验证**，不进埋点白名单、不用于广告定向（Compliance §4 D4）。

```csharp
/// <summary>
/// 年龄识别与家长同意验证抽象。Phase 0 提供 Mock 骨架（弹窗 + 二次确认），
/// Phase 2-3 接入运营商号码认证 SDK（客户端直连、不自建后端）。
/// </summary>
public interface IAgeVerifyService {
    bool IsVerified { get; }                         // 是否已完成年龄/家长验证
    UniTask<AgeVerifyResult> VerifyAsync();          // 一键登录/短信验证码主流程
    bool IsMinor { get; }                            // 判定是否未成年（驱动广告降级 C5）
    bool HasParentalConsent { get; }                 // 未成年是否取得家长同意（成年恒 true）
}

public struct AgeVerifyResult {
    public bool success;
    public bool isMinor;
    public bool parentalConsent;
    public AgeVerifyMethod method;                   // CarrierOneClick / SmsCode / Mock
}

public enum AgeVerifyMethod { CarrierOneClick, SmsCode, Mock }
```

**实现约束**：
- 文件位置：`Assets/Scripts/Core/Interfaces/IAgeVerifyService.cs` + `Assets/Scripts/Bootstrap/MockAgeVerifyService.cs`（Phase 0 骨架，弹窗+算术题/长按二次确认）
- 联网：仅走运营商号码认证网关（客户端直连），属 TechStack §17 “数据全本地”的**唯一验证白名单例外**；**不得**上传答题/掌握度等游戏数据
- `IsMinor` 结果驱动 `IAdsService` 广告分级（Compliance C5）与家长面板（C8）

### 4.13 IPlatformService（平台/防沉迷桥接 · 防沉迷后置版号阶段）

> **状态说明**：路线改为国内主市场后本版起纳入契约（此前因“海外路线不适用”仅在 ProjectSetup §11.3 定义、未入契约）。**防沉迷实名验证后置至取得版号后**接入国家新闻出版署官方系统；本版仅提供接口与 Mock，不阻塞 Phase 0-2。

```csharp
/// <summary>
/// 平台能力桥接：登录 / 防沉迷时长时段 / 合规开关。
/// 本版为骨架 + Mock；防沉迷真实接入后置至版号后。
/// </summary>
public interface IPlatformService {
    UniTask<bool> LoginAsync();                      // 国内渠道登录（版号后接入具体 SDK）
    bool IsAntiAddictionEnabled { get; }             // 无版号阶段恒 false（防沉迷后置）
    UniTask ReportToAntiAddictionAsync(string playerId, int onlineMinutes); // 版号后接入国家实名系统
    bool IsWithinAllowedPlayWindow();                // 未成年仅周五六日 20-21 点（版号后生效）
}
```

**实现约束**：
- 文件位置：`Assets/Scripts/Core/Interfaces/IPlatformService.cs` + `Assets/Scripts/Bootstrap/MockPlatformService.cs`
- `IsAntiAddictionEnabled` 本版恒 `false`；版号后接国内渠道 SDK 时置 `true`（法定时段/时长限制）
- 与 `IAgeVerifyService` 协同：年龄判定（未成年）× 防沉迷开关 决定是否启用时段限制

---

## 5. 全局事件契约（Core/Events/）

所有事件为 `struct`，实现 `IEvent` 接口，通过 `EventBus` 发布订阅。

```csharp
public interface IEvent { }

public static class EventBus {
    public static void Publish<T>(T evt) where T : IEvent;
    public static Subscription<T> Subscribe<T>(Action<T> handler) where T : IEvent;
    public static void UnsubscribeAll(object owner);    // 对象销毁时清理
}
```

### 5.1 EventBus 自研实现参考（主程确认方案）

```csharp
public static class EventBus {
    static readonly Dictionary<Type, List<HandlerEntry>> handlers = new();
    static readonly object lockObj = new();

    public static Subscription<T> Subscribe<T>(Action<T> handler, object owner = null)
        where T : IEvent {
        var type = typeof(T);
        lock (lockObj) {
            if (!handlers.TryGetValue(type, out var list)) {
                list = new List<HandlerEntry>();
                handlers[type] = list;
            }
            list.Add(new HandlerEntry(handler, owner ?? handler.Target));
        }
        return new Subscription<T>(handler, type);
    }

    public static void Publish<T>(T evt) where T : IEvent {
        if (!handlers.TryGetValue(typeof(T), out var list) || list.Count == 0) return;
        // 倒序遍历，支持回调中取消订阅
        for (int i = list.Count - 1; i >= 0; i--) {
            try { ((Action<T>)list[i].Delegate)?.Invoke(evt); }
            catch (Exception e) { Debug.LogException(e); }
        }
    }

    public static void UnsubscribeAll(object owner) {
        lock (lockObj) {
            foreach (var list in handlers.Values) {
                list.RemoveAll(h => ReferenceEquals(h.Owner, owner));
            }
        }
    }

    struct HandlerEntry {
        public readonly Delegate Delegate;
        public readonly object Owner;
        public HandlerEntry(Delegate d, object o) { Delegate = d; Owner = o; }
    }
}
```

**内存安全约定**：
- 每个 MonoBehaviour 在 `OnDestroy` 中必须调用 `EventBus.UnsubscribeAll(this)`
- Editor 模式下启动时注册 `UnsubscribeAll` 检查：订阅者所属对象已销毁 → 抛异常（仅开发环境）
- 单元测试验证：`EventBus_WhenOwnerDestroyed_NoMemoryLeak()`

### 事件清单（不可随意扩展，新增需评审）

| 事件名 | 字段 | 发布者 | 订阅者 |
|--------|------|--------|--------|
| `BattleStartEvent` | levelId, initialWordCount | BattleFlow | 所有系统 |
| `BattleEndEvent` | result, starRating, rewards | BattleFlow | UI, Save |
| `WaveStartedEvent` | waveNumber, totalZombies | WaveController | UI (瞭望台预警) |
| `WaveCompletedEvent` | waveNumber, remainingDurability | WaveController | BattleFlow |
| `ZombieSpawnedEvent` | zombieId, type, position | WaveController | Quiz |
| `ZombieEnteredTriggerEvent` | zombieId | TriggerZone | Quiz |
| `ZombieTargetLockedEvent` | zombieId | Quiz | UI |
| `ZombieDamagedEvent` | zombieId, deltaVitality, remainingVitality | Zombie | Reward, UI |
| `ZombieKilledEvent` | zombieId, type, position | Zombie | Wave, Reward |
| `ZombieReachedWallEvent` | zombieId, type | Zombie | Wall |
| `QuestionShownEvent` | word, options, correctIndex | Quiz | UI |
| `AnswerSubmittedEvent` | selectedIndex, isCorrect, word | Quiz | Learning |
| `AnswerTimeoutEvent` | word | Quiz | Learning |
| `ComboChangedEvent` | newComboCount, buffTriggered | Quiz | UI, Effects |
| `WallDamagedEvent` | deltaDurability, currentDurability | Wall | UI |
| `SupplyPackDroppedEvent` | packType, options | Supply | UI |
| `SupplyPackSelectedEvent` | packType, targetZombieFirstShot | UI | Supply |
| `SupplyPackEffectAppliedEvent` | packType, level | Supply | Effects |
| `WordPoolLowEvent` | remainingCount | WordPool | WordReplenisher |
| `WordReplenishedEvent` | addedCount, newTotal | WordReplenisher | Quiz |
| `FirstWordMasteredEvent` | wordId | Learning | Reward, UI（D21 M2 首克爆字+额外晶） |
| `WrongWordFixedEvent` | wordId | Learning | Reward, UI（D21 M2 攻克错题爆字+额外晶） |
| `PauseRequestedEvent` | isPaused | Input | Time |
| `QuitRequestedEvent` | - | Input | BattleFlow |

---

## 6. 系统职责与生命周期

### 6.1 GameManager（全局单例，跨场景）

- 负责：全局状态、场景切换、Time.timeScale 管理
- 生命周期：`RuntimeInitializeOnLoadMethod` 创建，`DontDestroyOnLoad`

### 6.2 SaveManager（全局单例）

- 负责：PlayerData、WordMastery、WrongWords 的加载与保存
- 触发保存：关卡结算后、应用退出前、每 60 秒自动保存

### 6.3 BattleFlowController（战斗场景装配器）

- 生命周期：随战斗场景加载而创建，场景卸载而销毁
- 职责：
  1. 读取 `LevelConfig` 初始化所有子系统
  2. 监听 `WallDurability <= 0` → 触发失败流程
  3. 监听 `WaveCompletedEvent(wave=5)` 且场上无小怪兽 → 触发胜利流程
  4. 优先级：失败 > 胜利
  5. 结算调用 `IRewardCalculator`，发布 `BattleEndEvent`

### 6.4 WaveController

- 职责：按 `LevelConfig.waves[5]` 刷新小怪兽
- 每波结束条件：本波所有小怪兽被击败或到达能量墙
- 波次间隔：5 秒
- 第 5 波：固定生成 1 只 BOSS + 5 只普通小怪兽

### 6.5 WallManager

- 职责：管理能量墙耐久（`IWall` 实现），接收 `ZombieReachedWallEvent` 启动攻击计时器
- 初始化：`Initialize(maxDurability)`，数值 = `LevelConfig` 读取 `PlayerData.energyWallLevel` 对应阶位（1 阶 100 … 5 阶 500）
- 攻击节奏（按小怪兽类型）：小怪 2s/-5，大怪 1.5s/-5，BOSS 1s/-10
- **耐久归零：发布 `OnWallBroken` → BattleFlowController 立即判负**（避难所无第二段判定）
- 修复：急救补给站触发 `Repair(5)` / 满 50 题 `Repair(20)`，不溢出 MaxDurability

### 6.6 QuizStateMachine

- 8 状态（详见 GDD 附录），核心逻辑：
  - 目标锁定：大怪/BOSS 期间 `currentTargetZombie` 不切换
  - 三计时器：answerTimer / comboTimer / zombiePauseTimer 独立
  - 单词队列：按 `⌈MaxVitality ÷（武器攻击力×200%）⌉` 为当前目标准备单词（基础武器下小怪1/大怪3/BOSS10）；目标活力提前归零则剩余单词本题不出现

### 6.7 WordPoolManager

- 职责：维护词池，实现 `IWordProvider`
- 补充触发：`RemainingCount < 10` 时补充 10 个
- 优先级：**上一局错词** > 已掌握 > 待复习 > 未学习（依 GDD v2.6 §4.1 单词补充规则；上一局错词指本局开始前记录的上局答错单词）

### 6.8 LearningSystem

- 职责：单词掌握度跟踪（Unlearned → Learning → Mastered → LongTermMastered）
- 数据：7 日复习计划、错题本、记忆进度
- **D21 M1**：新增连续量 `Word.memoryStrength ∈ [0,1]`（头顶“上膛”进度真源，仅表现，不新增伤害来源）；Phase 1 若无连续量则以 `WordMasteryLevel` 四态兜底映射 0/0.5/0.8/1.0
- **D21 M2**：`LearningSystem` 在“答对”结算同帧判定首克新词（首次达 Mastered）/攻克错题（移出错题本）并发布 `FirstWordMasteredEvent`/`WrongWordFixedEvent`；每词首克里程碑全局一次性记于 `PlayerData.masteredRewardSet`（不阻塞、不改伤害）
- **D21 M3**：`MasteryMeterService`（实现 `IMasteryMeterService`）每出题刷新本局已出题中 Mastered/LongTermMastered 占比 → 写 `BattleModifiers.swiftThresholdBonus/comboWindowBonus`（仅本局、守 D9）
- ❗ **不包含三会子系统**（会读/会认/会写）：按 GDD §12 D4 后置 Phase 4+，Phase 0-3 仅保留 §4.10 接口与 NoOp 实现

### 6.9 SupplyPackSystem

- 掉落触发：每波 30% / 每答对 5 题必掉 / BOSS 波必掉
- 效果范围：以补给后**第一次射击的小怪兽**为中心，半径 200 像素
- 存储：`PlayerData.activeSupplyPack` 只保留 1 个持续效果槽

### 6.10 RewardCalculator

- 公式：`基础奖励 × 奖励系数`
- 基础奖励：击败小怪 ×1 + 击败大怪 ×2 + 击败 BOSS ×5（能量晶）
- 系数：★★★ 1.5 / ★★☆ 1.2 / ★☆☆ 1.0 / 失败 0.5
- 答对率：**答对题数 / 初始词池数量**（不含补充，上限 100%）

---

## 7. 关键流程时序图

### 7.1 战斗启动

```
BattleBootstrap.Awake()
  ├─→ SaveManager.LoadPlayerData()
  ├─→ GameManager.LoadLevel(levelConfig)
  ├─→ WordPoolManager.Initialize(selectedWordBank)
  ├─→ WallManager.Initialize(maxDurability: 100)   // 按 energyWallLevel 阶位取值
  ├─→ WaveController.Initialize(levelConfig.waves)
  ├─→ QuizStateMachine.Initialize(wordPool, targetProvider)
  ├─→ BattleFlowController.Start()
  └─→ EventBus.Publish(new BattleStartEvent{...})
```

### 7.2 一只大怪兽被击败（基础武器）

```
1. WaveController 生成 EliteZombie → EventBus.Publish(ZombieSpawnedEvent)
2. EliteZombie 前进 → 进入答题圈 → EventBus.Publish(ZombieEnteredTriggerEvent)
3. QuizStateMachine:
   ├─ currentTarget = zombie（锁定）
   ├─ 从 WordPool 取 3 个单词 → 生成 3 道 WordQuestion
   └─ 状态切到 QUESTION_LOADING
4. 玩家答对第 1 题 → 武器暴击 TakeDamage(攻击力×200%=200) → CurrentVitality 600→400 → NEXT_QUESTION
5. 玩家答对第 2 题 → TakeDamage(200) → CurrentVitality 400→200 → NEXT_QUESTION
6. 玩家答对第 3 题 → TakeDamage(200) → CurrentVitality 200→0 → 触发 Die()（若武器攻击力更高，单发伤害>200，则更早归零，可能只用 1-2 题）
   ├─ EventBus.Publish(ZombieDamagedEvent)
   ├─ EventBus.Publish(ZombieKilledEvent{ type=Elite })
   ├─ DropReward(energy=2, gold=10, shard=1)
   ├─ QuizStateMachine.ReleaseLock()
   └─ 状态切到 IDLE
7. WaveController 监听 ZombieKilledEvent → 检查本波剩余 → 可能触发 WaveCompletedEvent
```

### 7.3 玩家退出关卡

```
1. 玩家按 ESC → EventBus.Publish(QuitRequestedEvent)
2. BattleFlowController 收到 → 弹出确认框
3. 玩家确认 → BattleFlow 强制结束当前战斗
4. 计算奖励：
   ├─ 击败数按当前统计（小怪/大怪/BOSS）
   ├─ RewardCalculator.Calculate(..., result=Quit, ...)
   └─ 系数 = 0.5，不适用星级
5. EventBus.Publish(BattleEndEvent{ result=Quit, rewards=... })
6. SaveManager.Save() → 场景切换到 MainMenu
```

---

## 8. 数据流：单词池与答对率

```
┌─────────────────────────────────────────────────────┐
│ 玩家选择词库（教材/年级/单元）                       │
└───────────────────┬─────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────────┐
│ WordPoolManager 初始化：                             │
│   initialWordPoolCount = 从词库抽 N 个 → 50（假设）  │
│   【关键】此数值关卡开始时固定，作为答对率分母        │
└───────────────────┬─────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────────┐
│ 游戏过程中：                                          │
│   - 每答对/答错：该单词从词池移除                     │
│   - 剩余 < 10 时：补充 10 个（补充部分不计入分母）    │
│   - GameData.correctAnswersInBattle 每次答对 +1      │
└───────────────────┬─────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────────┐
│ 战斗结束：                                            │
│   答对率 = min(100%, correctAnswers / initialPool)   │
│   星级判定用答对率 + 能量墙百分比                       │
└─────────────────────────────────────────────────────┘
```

---

## 9. PlayerData 结构（存档）

```csharp
[Serializable]
public class PlayerData {
    // 资源
    public int energyCrystal;
    public int gold;
    public int weaponShard;

    // 关卡进度
    public List<string> unlockedLevelIds;
    public Dictionary<string, int> levelStarRatings;   // levelId → star (0-3)

    // 武器
    public int weaponLevel;                            // Lv.1-7（GDD §2.2.5 武器表与附录 D 碎片总量 790 口径；v1.10 主程审核修正，原误标 1-5）
    public int currentWeaponFragment;

    // 防线建筑
    public int towerLevel;                             // 1-5
    public int energyWallLevel;                        // 1-5 → 决定本局初始耐久上限（100/200/300/400/500）
    public int firstAidStationLevel;                   // 1-5
    public int watchtowerLevel;                        // 1-5

    // 本地最佳纪录（代替排行榜，仅本机；GDD §12 D4）
    public int bestScore;
    public int bestCombo;
    public int bestDurabilityPercent;

    // 补给包等级
    public Dictionary<SupplyPackType, int> supplyPackLevels;

    // 设置
    public float answerTimeLimit;                      // 10-20 秒
    public float bgmVolume;
    public float sfxVolume;
    public bool vibrationEnabled;

    // 统计
    public int totalWordsMastered;
    public int totalZombiesKilled;
    public DateTime lastPlayDate;
    public int consecutiveHighAccuracyRuns;            // 用于动态难度
}
```

---

## 10. 场景清单

| 场景 | 用途 | 主要 Mono | 加载顺序 |
|------|------|-----------|---------|
| `Bootstrap` | 启动入口 | `Bootstrapper` | 首个加载，DontDestroyOnLoad 转 MainMenu |
| `MainMenu` | 主界面 | `MainMenuUI` | - |
| `LevelSelect` | 关卡选择 | `LevelSelectUI` | 模态 |
| `WordBankSelect` | 词库选择 | `WordBankSelectUI` | 模态 |
| `Battle` | 战斗 | `BattleBootstrap` | 主玩法 |
| `Result` | 结算 | `ResultUI` | 由 Battle 转入 |
| `Shelter` | 避难所 | `ShelterUI` | 模态 |
| `Settings` | 设置 | `SettingsUI` | 模态 |

---

## 11. Bootstrap 装配示例

```csharp
public class BattleBootstrap : MonoBehaviour {
    [SerializeField] private LevelConfig levelConfig;

    private void Awake() {
        // 1. 装配系统（顺序重要）
        var saveMgr = SaveManager.Instance;
        var wordPool = Setup<WordPoolManager>();
        wordPool.Initialize(levelConfig.selectedWordBank);

        var wall = Setup<WallManager>();
        wall.Initialize(maxDurability: 100);   // = energyWallLevel 对应阶位上限

        var wave = Setup<WaveController>();
        wave.Initialize(levelConfig.waves);

        var quiz = Setup<QuizStateMachine>();
        quiz.Initialize(wordPool, targetProvider: wave);

        var reward = Setup<RewardCalculator>();
        var flow = Setup<BattleFlowController>();
        flow.Initialize(levelConfig, wordPool, wall, wave, quiz, reward);

        // 2. 启动
        flow.StartBattle();
    }

    private T Setup<T>() where T : MonoBehaviour
        => new GameObject(typeof(T).Name).AddComponent<T>();
}
```

---

## 12. 单元测试策略

| 模块 | 必测点 |
|------|--------|
| QuizStateMachine | 8 状态所有转换路径、目标锁定不切换、大怪 3 题/BOSS 10 题 |
| WordPoolManager | 剩余 <10 触发补充、优先级正确、答对/答错移除 |
| RewardCalculator | 星级判定、失败 0.5、答对率封顶 100% |
| WaveController | 5 波刷新、波次切换条件、BOSS 波第 5 波 |
| WallManager | 攻速伤害表、耐久归零触发失败、修复不溢出上限 |
| StarRatingEvaluator | 3 组阈值组合的边界值 |

**测试框架**：Unity Test Framework（EditMode）
**覆盖率目标**：核心系统 ≥ 80%

---

## 13. 性能与内存规范

| 规范 | 阈值 | 检查手段 |
|------|------|---------|
| 单局最大同屏小怪兽 | 30 只 | 逻辑约束 |
| 对象池：子弹 | 100 | `ObjectPool<Bullet>` |
| 对象池：伤害飘字 | 50 | `ObjectPool<TMP_Text>` |
| GC / Frame | < 1KB | Profiler |
| DrawCall | < 30 | Frame Debugger |
| 事件订阅 | 系统销毁时 `UnsubscribeAll(this)` | 单元测试覆盖 |

**禁止**：
- 每帧 `FindObjectOfType` / `GameObject.Find`
- 每帧 `new List<>` / `new Vector3[]`
- 空 `Update()` 存在

---

## 14. 编码约定

| 项 | 约定 |
|----|------|
| 命名 | 类/方法/属性 PascalCase，字段 _camelCase 或 camelCase，常量 SCREAMING_SNAKE_CASE |
| 前缀 | 接口 `I` 开头；实现类不加后缀 |
| 事件 | 用 `Action<T>`，禁止 `public Action<T> SomeEvent;` 字段暴露 |
| 空引用 | 系统初始化时 `[RequireComponent]` 或 Awake 中显式 assert |
| 注释 | public API 必须有 `/// <summary>`；私有复杂逻辑必须有行内注释说明"为什么" |
| 数字 | 配置字段全部走 SO，代码中出现 magic number 必须提为常量 |
| 单位 | 内部用像素坐标；SO 中 `distanceInMeters` 通过 `Constants.PIXELS_PER_METER = 100` 转换 |

---

## 15. 已确认方案（主程拍板）

| 项 | 确认方案 | 理由 |
|----|---------|------|
| EventBus | **自研**（约 100 行） | 无额外依赖，支持 owner 生命周期自动清理，内存安全可控 |
| Zombie 动画 | **Spine2D 骨骼动画**（v1.11 定案，取代旧“手写+Animator 混合”） | 美术统一用 Spine2D 二进制导出，状态由 spine-unity AnimationState 驱动；同屏靠共享 atlas page 合批 |
| Word 定义 | **class**（全局唯一引用） | 字段多、拷贝成本高；错题本/7 日复习需引用同步 |
| 场景过渡 | **ISceneLoader 抽象层** | Phase 0-1 SceneManager；Phase 2+ 可切 Addressables |
| UI 测试 | **PlayMode 测 3 个关键流程** + PageObject 抽象 | 避免脆式测试；只测高价值场景 |
| 单词数据加载 | **按教材+年级分 SO** | Phase 0-1 Resources按需；Phase 2+ Addressables Label |
| 广告抽象 | **IAdsService** + 多实现 + Mock | 保证完全离线可玩；合规频率控制 |
| 埋点抽象 | **IAnalyticsService** + 三层 sink | 自研实现；接口预留未来升级 ThinkingData/Firebase |
| 变现 | **0 内购**：`IStoreService` 只收能量晶（GDD §12 D1） | 无版号不可开放 IAP；儿童向避免误扣；不引入 `com.unity.purchasing` |
| 防线数值 | **单一 `IWall` 耐久值**，避难所无血量接口（GDD §12 D2） | 消除“双 HP”歧义；失败判定只有一处，BattleFlowController 不会漏判 |
| 敌人枚举 | **`ZombieType` 固定 3 值**（Normal/Elite/Boss），变体走 `ZombieConfig`（GDD §12 D3；**D23 修订：枚举值仍仅 3 个，30 种敌人属原型下 species，走 `ZombieSpeciesConfig` 数据层 + 每种独立 SkeletonDataAsset，不新增枚举值、不新增接口**） | 防止附录表与枚举不一致；AI/战斗规则按 3 类原型分层，种只影响外观资产与数值钩子；Spine SkeletonDataAsset 按种独立（30 份，P1 首批 9 份） |
| 三会系统 | **Phase 4+**，本期仅 `IWordStudyService` + NoOp（GDD §12 D4） | 单人力下控制 MVP；不阻塞战斗主循环 |
| 排行榜 | **不做**（完全离线，本地纪录代替） | 避开儿童数据联网与隐私申报；与 TechStack §17 一致 |

### 15.1 Zombie 动画方案详情（v1.11 定案：Spine2D）

> **取代旧“手写 SpriteAnimationPlayer + Animator 混合”方案**。角色/敌人骨骼动画统一由 **Spine2D（spine-unity 官方运行时）** 驱动；美术以 Spine 工程制作、导出**二进制** `.skel.bytes` + `.atlas` + atlas page png（**不使用 JSON 导出**）。

| 动画类型 | 实现方式 | 组件 |
|---------|---------|------|
| Idle / Walk / Attack / Hurt / Die | spine-unity `SkeletonAnimation` + `AnimationState`（`SetAnimation`/Track 分层） | `SkeletonRenderer`（SpriteRenderer 模式、参与 2D 合批） |
| BOSS 阶段切换 / 技能前摇 | `AnimationState` 多 Track + 事件轨道（`IAnimationStateListener`） | SkeletonAnimation |
| 表情/装备差分（守卫者表情×3、变体换皮） | Spine **Skin** 切换 | SkeletonDataAsset skin |
| 嘲讽气泡 / 高速特效（<8 帧） | 序列帧或粒子（不进 Spine） | 轻量 `SpriteAnimationPlayer`（仅特效复用） |

**约束**：
- **SkeletonDataAsset 按种独立（GDD §12 D23 修订）**：30 种敌人（小怪 15/大怪 10/BOSS 5）每种一份骨架（含专属 idle/walk/attack/hurt/die 动作集）；三批递进交付 P1 9 份（小 5/大 3/BOSS 1）→ P2 +11 → P3 满 30；`ZombieType` 枚举仍仅 3 值，种由 `ZombieSpeciesConfig`（ScriptableObject：speciesId、Type 原型、skeleton 引用、数值/掉落钩子）绑定；`EnemyModifier` 机制叠加层不变（D18/D20）。
- **同屏合批**：同屏敌人（≤30）共享同一 atlas page + 同一材质方可合批；种扩张后靠**同页种类共现编排**（同批关卡优先同 atlas page 的种）保障 DrawCall < 30（TechStack §2）；材质变体总数 ≤4。
- **atlas page**：Spine 导出 atlas page **按档扩容（D23：允许 2048×2048、每骨架可多页；原 ≤1024 废除）**、ASTC 4×4；包体预算见 TechStack §0/§18（≤200MB 硬线）。
- `SpriteAnimationPlayer` 降级为**仅特效/UI 用的轻量帧切换组件**（Phase 0 骨架仍保留），不再用于角色本体。

```csharp
// 角色本体动画统一走 spine-unity（具体 API 签名以 spine-unity 版本为准，Phase 0 不锁定第三方接口）：
// skeletonAnimation.state.SetAnimation(0, "walk", true);
// skeletonAnimation.state.SetAnimation(0, "die", false).AddListener(...)  // 击败表现回调

// 轻量特效帧切换（嘲讽气泡/高速特效，非角色骨骼）仍用：
// Assets/Scripts/Entities/Effects/SpriteAnimationPlayer.cs
public class SpriteAnimationPlayer : MonoBehaviour {
    [SerializeField] Sprite[] frames;
    [SerializeField] float fps = 12f;
    [SerializeField] bool loop = true;
    SpriteRenderer targetRenderer;

    public void Play();
    public void Stop();
    public event Action OnComplete;
}
```

### 15.2 单词数据加载方案

```
Phase 0：仅预留接口（不加载真实数据）
Phase 1：Resources.Load，按教材+年级分 SO，内存中保留最近 3 本
Phase 2：迁移 Addressables Label（保留 Resources 兼容层 30 天）
Phase 3：支持远程包（若开联网）
```

**内存预算**：单本 SO 3-5MB，同时保留 3 本 = 15MB 上限。

### 15.3 埋点事件清单补充（Architecture.md 第 5 节基础上新增）

| 事件名 | 字段 | 发布者 |
|--------|------|--------|
| `AnalyticsLevelStartEvent` | levelId, textbookKey, initialWordCount | BattleFlow |
| `AnalyticsLevelEndEvent` | levelId, result, starRating, durationSec, correctRate | BattleFlow |
| `AnalyticsWordAnswerEvent` | wordId, correct, timeSpentMs, comboBefore | Quiz |
| `AnalyticsComboTriggeredEvent` | comboCount, buffType | Quiz |
| `AnalyticsSupplyPackSelectedEvent` | packType, levelContext | Supply |
| `AnalyticsAdShownEvent` | placementId, success | AdsService |
| `AnalyticsAdRewardedEvent` | placementId, rewardType | AdsService |
| `AnalyticsWordMasteryChangedEvent` | wordId, oldLevel, newLevel | Learning |
| `AnalyticsErrorEvent` | tag, message, stack | 任何系统 |

---

## 16. 变更历史

| 版本 | 日期 | 变更 | 责任人 |
|------|------|------|--------|
| v0.1 | 2026-11-04 | 初稿，基于 GDD v2.0 与 TechStack v0.1 | AI 主程视角 |
| v1.0 | 2026-11-04 | 主程确认 12 项方案：Word 改 class；新增 ISceneLoader/IAdsService/IAnalyticsService 抽象；EventBus 自研实现参考；混合动画方案；单词分 SO 加载；埋点事件清单补充；Phase 0 清单更新 | 主程 |
| v1.1 | 2026-11-05 | 关联文档同步：新增 ProjectSetup.md / DevTasks.md 引用；Phase 0 清单中"Design 文档同步"从三份 v1.0 改为五份 v1.1；接口与事件契约无变更 | 主程 |
| v1.2 | 2026-11-05 | **平台方向调整**：目标定为 **Android + iOS 移动 App**，类目 **休闲益智**；§4.8 IAdsService 实现列表剔除小游戏，新增优量汇；接口/事件/枚举本身无变更 | 主程 |
| v1.3 | 2026-11-05 | 关联文档同步：新增 [Compliance.md](./Compliance.md) 引用，其余文档升至 v1.3（GDD v2.1）；术语软化已落实（僵尸→小怪兽/精英僵尸→大怪兽/BOSS 僵尸→BOSS 怪，代码标识符 `Zombie*` 保留）；接口/事件/枚举契约无变更 | 主程 |
| v1.4 | 2026-11-05 | **四大定案落入接口契约（破坏性重命名，Phase 0 尚未写代码 → 零迁移成本）**：<br>**D2** `IShield`→`IWall`、`ShieldManager`→`WallManager`、事件/字段统一重命名（`ZombieReachedWallEvent` / `OnWallBroken` / `WallDamagedEvent` / `deltaDurability`）；`IWall` 字段 `CurrentHP/MaxHP/HPPercent` → **`CurrentDurability/MaxDurability/DurabilityPercent` + `IsBroken`**，`Heal`→`Repair`；明确**不存在 IShelter/IBase 血量接口**<br>**D3/D7** `IZombie.MaxHP/CurrentHP` → **`RequiredAnswers/RemainingAnswers`**（不叫 HP）；删除 `IZombie.Heal`（答错不回复进度）；事件 `ZombieDamagedEvent` 字段同步<br>**D1** 新增 **§4.11 `IStoreService`**，`PurchaseCurrency` 只有 `EnergyCrystal` 一个枚举值（禁止扩展 RealMoney）；**不定义 `IPurchaseService`**<br>**D4** 新增 **§4.10 `IWordStudyService` + `NoOpWordStudyService`**（三会后置 Phase 4+，不新增 `QuestionType.Spelling`）；**单机不做排行榜**，`PlayerData` 新增 `bestScore/bestCombo/bestDurabilityPercent`<br>§12 测试点、§15 已确认方案、§17 Phase 0 产出清单与版本号同步 | 主程 |
| v1.5 | 2026-11-05 | **Q5 伤害制（GDD §12 D8）落契约**：`IZombie` 由 `RequiredAnswers/RemainingAnswers`（v1.4 旧）改为 **`MaxVitality/CurrentVitality`**（小怪200/大怪600/BOSS2000）；`TakeDamage(amount)` 语义改为扣减活力值（amount = 武器攻击力×200%）；击败所需答对次数由 QuizStateMachine 按 `⌈MaxVitality÷(攻击力×200%)⌉` 动态计算（BOSS 不再恒定 10 题）；`ZombieDamagedEvent` 字段 `deltaAnswers/remainingAnswers`→`deltaVitality/remainingVitality`；`IWall.Repair` 注明仅急救补给站触发、战斗中不得消耗能量晶（D2/Q8）；§7.2 大怪示例改活力递减；正文击杀/击倒软化→击败；关联文档串同步 GDD v2.3 等 | 主程 |
| v1.6 | 2026-11-05 | **战场几何与技能包必用策略层（GDD §12 D15/D16）落契约**：`SupplyPackType` 注释分类——控制类（Ice/Slow/Confuse，**不调用 `TakeDamage`**）/ 转化类（Lightning/Spread/Fire/Pierce，将答对暴击扩散多目标）；伤害来源铁律：`TakeDamage` 只能由“答对”结算调用；当前锁定目标答题期间不前进、恢复推进 30% 速度（D15，`ZombieController`+`Pause/Resume`）；同时生效上限 1 控制+1 转化、控制类跨波囤积 1 个（D11/D16，`PlayerData.activeSupplyPacks[]`）；§4.11 广告硬约束补“过关必需包不得来自激励视频”；关联文档串同步 GDD v2.4 等 | 主程 |
| v1.7 | 2026-11-06 | **玩法增味（GDD §12 D17/D18/D19）落契约**：§3 新增 `EnemyModifier`/`WaveEventType`/`BattleDoctrine`/`LocalAchievementCategory` 四枚举（仅扩展，不新增 `ZombieType`，守 D3）；`IZombie` 新增 `Modifier`/`HasActiveShield` 字段，并注释 D18 护盾（首次 TakeDamage 只破盾不扣活力）/Mender 回复（敌方机制、非玩家伤害）；D17 即时命中（答对当帧 TakeDamage、取消 1s 硬停顿、HitStun 0.3s 仅动画）与 Swift（≤ 3s 答对 ×1.15，仍是答对暴击乘数）；doctrine 仅本局生效不写存档；关联文档串同步 GDD v2.5 / TechStack v1.8 / DevTasks v1.7 / ProjectSetup v1.8 / Compliance v1.4 | 主策划 |
| v1.8 | 2026-11-06 | **玩法增味 II（GDD §12 修订 D15、新增 D20）落契约**：`EnemyModifier` 新增 `Wanderer`（稀有游走奖励怪，Phase3，不新增 `ZombieType`）；§4.1 注释改写 D15——**取消“锁定目标 Pause/30% 减速”，锁定与非锁定全程正常推进、当前锁定目标 view 层高亮环/连线（不新增 `IZombie` 字段）**；新增 D20 游走怪行为注释（进 1/6 后不推进、`IsReachedWall` 恒 false、走廊内 `WanderSpeed=0.6` 随机游走、点击置为锁定目标、不计入通关、`dropOverride`）；§6.7 词池补充优先级新增**上一局错词**；关联文档串同步 GDD v2.6 / TechStack v1.9 / DevTasks v1.8 / ProjectSetup v1.9 / Compliance v1.5 | 主策划 |
| v1.9 | 2026-11-06 | **认词正反馈显性化（GDD §12 D21）落契约**：`Word` 新增连续量 `memoryStrength∈[0,1]`（M1 头顶“上膛”进度真源，仅表现、Phase 1 四态兜底）；§3 新增 `BattleModifiers`（含 `attackMultiplier`/`wallDurabilityMultiplier`/`swiftThresholdBonus`/`comboWindowBonus`，与 D18 Doctrine 同结构、仅本局）与 `IMasteryMeterService`（M3）；事件表新增 `FirstWordMasteredEvent`/`WrongWordFixedEvent`（M2 首克/攻克里程碑，Reward+UI 订阅）；§6.8 补 D21 M1/M2/M3 实现约束（伤害仍只由“答对”触发，D8/D16 不破）；关联文档串同步 GDD v2.7 / TechStack v1.10 / DevTasks v1.10 / ProjectSetup v1.11 / Compliance v1.7 | 主策划 |
| v1.10 | 2026-11-06 | **主程审核修复（GDD 逐条对照四技术文档）**：§9 `PlayerData.weaponLevel` 注释“1-5”修正为 **Lv.1-7**（对齐 GDD §2.2.5 武器表与附录 D 碎片总量 790；接口签名不变、仅注释口径）；其余审核发现均属 GDD 侧待拍板（见会话报告），本文未改实质契约；基线串升至 GDD v2.8 / TechStack v1.11 / DevTasks v1.10 / ProjectSetup v1.11 / Compliance v1.7 | 主程 |
| v1.11 | 2026-11-06 | **⑧ BattleState 为真源 + ⑩ Spine2D 动画契约**：<br>**⑧** 拍板 BattleState 以本文 §3 顶层 6 态 {Loading/Running/Paused/Victory/Defeat/Quit} 为唯一真源（接口契约不变），GDD §10.2.2 细化 9 态降级为 Running 内部私有子状态机（不进契约/事件白名单）<br>**⑩** §15 决策表 Zombie 动画→**Spine2D**（与 TechStack v1.12 一致）；§15.1 重写为 Spine2D 二进制导出（`.skel.bytes`+`.atlas`+page png）、SkeletonDataAsset 仅 3 份、同屏合批、atlas page ≤1024、`SpriteAnimationPlayer` 降级仅特效；修复写入时“合批”一词的 CJK 概率损坏（已回验）；基线串升至 GDD v2.9 / TechStack v1.12 / DevTasks v1.11 / ProjectSetup v1.12 / Compliance v1.8 | 主程 |
| v1.12 | 2026-11-06 | **字体归一基线串同步（仅基线串升版，接口/事件契约零变更）**：主程审核确认字体选型（`ShelterHan`/`ShelterBaloo`、DIN/Roboto 弃用、符号走 `ic_*` 图标）属 TechStack §5/§5.1 选型与美术表资源范畴，不新增/不改任何接口签名与事件契约（TMP 字体资产为 view 层资源）；本文实质未改；基线串升至 GDD v2.10 / TechStack v1.13 / DevTasks v1.12 / ProjectSetup v1.13 / Compliance v1.9 | 主程 |
| v1.13 | 2026-11-07 | **字体评审发现基线串同步（仅基线串升版，接口/事件契约零变更）**：评审落地的许可展示页（UI §14）/生僻字后备资产 `ShelterHanFB`/OFL RFN 口径均归 TechStack/Compliance/UI，不涉接口契约（TMP Fallback 资产仍为 view 层）；本文实质未改；基线串升至 GDD v2.11 / TechStack v1.14 / DevTasks v1.13 / ProjectSetup v1.14 / Compliance v1.10 | 主程 |
| v1.14 | 2026-11-07 | **发行路线调整落契约（对齐 Compliance v1.11 / GDD v2.12 D22）：新增 §4.12 `IAgeVerifyService` + §4.13 `IPlatformService`**：<br>年龄/家长验证走国内通用做法（手机号实名 + 运营商一键登录、客户端直连不自建后端、短信备选），首次启动阻塞流程，`IsMinor` 驱动广告降级；`IPlatformService` 防沉迷/登录桥接，**`IsAntiAddictionEnabled` 本版恒 false、防沉迷后置版号阶段**；此前 `IPlatformService` 仅在 ProjectSetup §11.3 定义、未入契约（“海外路线不适用”理由失效），本版纳入；§4.8 `IAdsService` 注释改未成年人保护口径、AdMobAdsService 因放弃海外划废；§17 Phase 0 接口清单与 header 目标平台行同步；基线串升至 GDD v2.12 / TechStack v1.15 / DevTasks v1.14 / ProjectSetup v1.15 / Compliance v1.11 | 主程 |
| v1.15 | 2026-11-07 | **敌人种类扩张基线串同步（对齐 GDD v2.13 D23；接口/事件/枚举契约零变更）**：`ZombieType` 仍仅 Normal/Elite/Boss 3 值，`EnemyModifier` 机制叠加层不变；30 种敌人属原型下 species，由 `ZombieSpeciesConfig`（SO：speciesId/原型/骨架引用/数值钩子）数据层承载，Phase 2 随种类名录出表落契约；§15 敌人枚举行与 §15.1 约束重写（SkeletonDataAsset 按种独立 30 份、P1 首批 9 份；atlas page 允许 2048/多页；合批靠同页种类共现编排，DrawCall <30 不变）；基线串升至 GDD v2.13 / TechStack v1.16 / DevTasks v1.15 / ProjectSetup v1.16 / Compliance v1.12；派生文档同步升至 v1.5 | 主程 |

---

## 17. Phase 0 会话可执行清单（开工基线）

新会话读取本文件 + TechStack.md + GDD.MD 后，**只输出以下内容，不实现具体玩法**：

### 必需产出
- [ ] `Assets/Scripts/Core/Enums/*.cs`  所有枚举（ZombieType / QuizState / BattleState / StarRating / SupplyPackType / QuestionType / WordMasteryLevel）
- [ ] `Assets/Scripts/Core/Interfaces/*.cs`  所有接口（IZombie / IQuizTargetProvider / IWordProvider / IWordLibraryService / IQuestionPresenter / IWall / IRewardCalculator / ISceneLoader / IAdsService / IAnalyticsService / IStoreService / IWordStudyService / IAgeVerifyService / IPlatformService）
- [ ] `Assets/Scripts/Systems/Learning/NoOpWordStudyService.cs`  三会空实现（所有方法返回默认值，保证 Phase 4+ 不回改调用点）
- [ ] `Assets/Scripts/Core/Events/*.cs`  所有 event struct + IEvent 接口
- [ ] `Assets/Scripts/Core/EventBus.cs`  自研实现 + 内存安全机制
- [ ] `Assets/Scripts/Core/Constants.cs`  常量（PIXELS_PER_METER = 100 等）
- [ ] `Assets/Scripts/Core/Managers/`  GameManager / SaveManager / PauseManager / AnalyticsManager / AdsManager 骨架
- [ ] `Assets/Scripts/Config/*.cs`  ScriptableObject 类字段（无逻辑）
- [ ] `Assets/Scripts/Entities/Zombie/ZombieBase.cs`  抽象类 + NormalZombie/EliteZombie/BossZombie 空壳（方法 `throw NotImplementedException`）
- [ ] `Assets/Scripts/Entities/Effects/SpriteAnimationPlayer.cs`  骨架
- [ ] `Assets/Scripts/Bootstrap/BattleBootstrap.cs`  装配骨架（不实现玩法）
- [ ] `Assets/Scripts/Bootstrap/SceneManagerSceneLoader.cs`  Phase 0-1 默认实现
- [ ] `Assets/Scripts/Bootstrap/MockAdsService.cs`  默认无网实现

### 测试产出
- [ ] `Assets/Tests/EditMode/EventBusTests.cs`  验证发布/订阅/取消/内存安全 3 个用例
- [ ] `Assets/Tests/EditMode/InterfacesCompileTests.cs`  验证接口可实例化

### 验收标准（Definition of Done）
- [ ] Tuanjie 打开项目可编译，0 warning
- [ ] 所有 public API 有 XML `/// <summary>` 注释
- [ ] EventBus 单元测试全部通过
- [ ] 不违反本文件接口签名
- [ ] 不实现具体玩法（玩法方法保留 NotImplementedException）
- [ ] `Assets/Design/*.md` 六份同步到基线版本（GDD v2.12 / TechStack v1.15 / Architecture v1.14 / DevTasks v1.14 / ProjectSetup v1.15 / Compliance v1.11）
- [ ] Git commit：`chore: Phase 0 architecture skeleton (v1.0)`

---

> **使用说明**：
> 1. 本文件与 TechStack.md 是 Phase 0 会话的**唯一契约**，任何代码必须遵守
> 2. 修改流程：变更接口 → 更新本文件 → 主程评审 → 再改代码
> 3. 每个 Agent 会话开场必须显式引用本文件路径与 GDD 章节号
