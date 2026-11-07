# 技术栈说明 (TechStack)

| 项目 | 避难防线 - 词汇守卫者 |
|------|-------------------|
| 文档版本 | v1.16 |
| 最后更新 | 2026-11-07 |
| 关联文档 | [GDD.MD](./GDD.MD) v2.13、[Architecture.md](./Architecture.md) v1.15、[DevTasks.md](./DevTasks.md) v1.15、[ProjectSetup.md](./ProjectSetup.md) v1.16、[Compliance.md](./Compliance.md) v1.12 |
| 目标平台 | **移动 App（Android / iOS）· 锁定横屏**；PC Editor 仅用于开发；**已放弃微信/抖音/团结小游戏平台** |
| 发行策略 | **全年龄英语学习 · 国内主市场**：本版**无预算不开版号**→先 iOS TestFlight + 安卓官网 APK 灰区内测（不提审商店）；融资后申请版号→国内 iOS 中国区 + 安卓商店正式上市；**放弃海外**、Steam 暂缓；**纯广告变现（0 内购，全年龄可投个性化广告）**（详见 [Compliance.md](./Compliance.md) v1.12、GDD §12 D22） |
| 状态 | 主程已确认，可开工 Phase 0 |

---

## 0. 决策原则

- **全年龄 + 轻量 2D**：优先选择官方成熟方案，避免过度设计
- **主目标：移动 App**（Android + iOS，锁定横屏）；PC 仅用于开发预览；**不再考虑小游戏平台包体限制**
- **性能硬约束**：DrawCall < 30、内存 **≤ 400MB**、包体 **≤200MB**（单渠道安装包，包含运行时；**GDD §12 D23 修订，v1.16 起生效**：原内存 <150MB、包体 <80MB 随美术扩容放宽，低端机画质降级档兜底）
- **小团队友好**：所有选型必须文档充足、社区活跃、易上手
- **可回退**：Phase 0 完成后不轻易更换核心选型（会推倒重来）
- **合规前置**：**不开版号 + 纯广告变现**；家长同意弹窗 / 儿童数据最小化 / 广告儿童标记为开工即启动项（详见 [Compliance.md](./Compliance.md)）

---

## 1. 引擎与运行时

| 项目 | 选择 | 备选 | 理由 |
|------|------|------|------|
| 引擎 | **Tuanjie 1.x**（具体小版本号待项目创建时固定） | Unity 2022.3 LTS | 国内优先；若海外发行可切回 Unity，两版 API 兼容 |
| Package Registry | Tuanjie 官方 registry | Unity registry 兼容回退 | 主选 Tuanjie；若包获取失败可临时使用 Unity registry |
| 编辑器版本 | 与引擎绑定 | - | 团队统一，避免版本漂移 |
| .NET 版本 | .NET Standard 2.1 | .NET Framework | Tuanjie/Unity 官方推荐 |
| C# 版本 | C# 9 | - | 支持 record、init、模式匹配 |
| API 兼容级别 | .NET Standard 2.1 | - | 保证 Android/iOS IL2CPP 兼容 |
| Scripting Backend | **IL2CPP**（Android / iOS）；Mono（Editor） | - | iOS 强制 IL2CPP；Android 侧为统一构建链路、启动速度与混淆支持（**与国内渠道 ARMv7 无关——国内不开版号不上架**） |
| Target Architectures | Android: **ARMv7 + ARM64**（Google Play 要求同时提供 ARM64）；iOS: **ARM64** | - | Google Play / App Store 均强制含 64-bit |
| 目标渲染 | Linear | Gamma | URP 2D 需要 Linear，配合 2D 光照更真实 |

**版本锁定方式**：项目根目录 `ProjectSettings/ProjectVersion.txt` 提交 Git，禁止自动升级。

---

## 2. 渲染管线与 2D 工具链

| 项目 | 选择 | 说明 |
|------|------|------|
| 渲染管线 | **URP 14.x**（与引擎版绑定） | 2D Renderer 支持 2D 光照、后处理、Pixel Perfect |
| 2D 像素对齐 | **Pixel Perfect Camera** | 保持 Q 版精灵边缘清晰 |
| 精灵管理 | **Sprite Atlas**（V2）＋ Spine 自有 atlas | UI/Tile/特效用 Sprite Atlas V2（每模块一张，最大 2048×2048）；角色骨骼走 Spine 自有 atlas page（**D23 扩容：每骨架可多页、允许 2048×2048，见“动画”行与 DrawCall 策略**） |
| Tilemap | 用于关卡背景 | 地面/障碍物使用，Tilemap Renderer + Composite |
| 光照 | 2D Light（点光/聚光） | 词汇能量墙、子弹轨迹发光效果 |
| 动画 | **Spine2D（spine-unity 运行时）** | 角色/敌人骨骼动画统一用 Spine2D，导出**二进制** .skel.bytes + .atlas + atlas page png（不用 JSON）；序列帧/粒子仅用于 <8 帧高速特效与嘲讽气泡；UI 过渡用 DOTween（不再依赖 Unity 2D Animation PSB） |
| 后处理 | URP Volume（可选） | BOSS 出场、核弹爆发时启用 Bloom/Chromatic |
| Shader | 优先使用 URP 内置 Sprite/Unlit | 特殊效果用 Shader Graph 可视化编写 |

**DrawCall 控制策略**：
- 主 UI 图集、战斗角色图集、敌人图集（按原型/批次分页）、补给包图集；**D23 扩容：Sprite Atlas 页数不限模块单张，允许各模块 2048×2048 多张**
- **Spine 骨骼合批**：同屏敌人共享同一 Spine atlas page + 同一材质方可合批；**SkeletonDataAsset 按种独立（D23：30 种 = 30 份，P1 首批 9 份：小 5/大 3/BOSS 1）**，合批靠**同页种类共现编排**（同批关卡优先同 atlas page 的种同屏）；DrawCall < 30 硬线不变，靠关卡编排与分页保障
- 使用 Sprite Atlas 的 "Tight Packing"，减少空白
- 禁用动态合批以外的所有 batching 冲突（材质变体数 ≤ 4）

---

## 3. 物理系统

| 项目 | 选择 | 说明 |
|------|------|------|
| 物理引擎 | **Physics 2D**（内置 Box2D） | 2D 项目标配 |
| 碰撞检测 | Collider2D + Trigger | 小怪兽进入答题圈、到达能量墙均用 Trigger |
| 刚体 | Rigidbody2D（Kinematic） | 小怪兽匀速移动使用 Kinematic，避免物理抖动 |
| 射线检测 | Raycast2D | 哨塔锁敌、激光穿透判定 |
| 碰撞层 | 分层：Zombie / Wall / TriggerZone / Bullet | 通过 Physics 2D Settings 精确控制碰撞矩阵 |
| 物理材质 | 无（本游戏无弹跳/摩擦需求） | - |

**性能提示**：所有 Collider2D 关闭 Contact Modification，Trigger 使用 `OnTriggerEnter2D` 而非每帧检测。

---

## 4. 输入系统

| 项目 | 选择 | 说明 |
|------|------|------|
| 输入方案 | **New Input System**（com.unity.inputsystem） | 支持多设备重映射，PC/移动端统一 |
| 输入动作资产 | `Assets/Input/GameplayActions.inputactions` | 定义 Answer/Escape/Pause/Restart 等 Action |
| 移动端 | 触屏点击 4 个答案按钮（Canvas 上的 Button + InputSystem UI Module） | 无需虚拟摇杆 |
| PC 端 | 鼠标点击 + 数字键 1-4 + 空格暂停 + ESC 返回 | 支持快捷键 |
| 输入回调 | 使用 `@InitializeOnLoad` 的单例 `InputManager` 分发事件 | 避免每类各自订阅 |

**注意**：Editor 中若同时启用 Old & New，需关闭 Old（`Project Settings > Player > Active Input Handling = New Only`）。

---

## 5. UI 框架

| 项目 | 选择 | 说明 |
|------|------|------|
| UI 系统 | **uGUI**（UnityEngine.UI） | 生态成熟、动画易做、性能可控 |
| 布局组件 | Vertical/Horizontal Layout Group、Grid Layout Group | 答题选项 4 按钮使用 Horizontal |
| 文本 | **TextMeshPro (TMP)** | 全部使用 TMP，禁用 legacy Text |
| 字体 | TMP SDF 字体资产，**中英分开**（字体选型唯一真源=本节；常态中英两套资产，中英混排处优先单资产+Fallback，见 §5.1） | 中文=`ShelterHan`（思源黑体子集）、拉丁(英/数字)=`ShelterBaloo`（Baloo 2 子集）；**均 SIL OFL 1.1、免费商用**；子集化属修改→按 OFL 保留名规则**重命名**；旧 DIN Alternate（商业专有）/ Roboto Mono **弃用** |
| 动画 | DOTween（Pro 免费版） | UI 弹出、缩放、飞入动画 |
| 图集 | Sprite Atlas + `Image.sprite` 引用图集内精灵 | - |
| Canvas 分层 | 3 层：Background / Gameplay / Popup | Popup Canvas 独立渲染，减少重绘 |

**不选 UIToolkit 的原因**：UIToolkit 在 2D 游戏内 UI 生态不如 uGUI 成熟，团队学习成本更高。

### 5.1 字体授权与重命名（v1.13 定案，配合项目方“**不买字体版权**”约束）

| 字体资产 | 来源字体 | 许可证 | 不买版权可用 | 处置 |
|----------|---------|--------|:---:|------|
| `ShelterHan` SDF | 思源黑体 Source Han Sans / Noto Sans CJK | **SIL OFL 1.1** | ✅ 免费 | 子集化后**重命名**（OFL 保留名要求） |
| `ShelterBaloo` SDF | Baloo 2 | **SIL OFL 1.1** | ✅ 免费 | 兼作数字字体；子集化后**重命名** |
| ~~DIN Alternate~~ | 商业 DIN 系（Monotype/Linotype 授权；macOS 自带 DIN Alternate 属 Adobe 授权版，均不可自由嵌入） | ❌ 需购买授权 | 🚫 不能用 | **弃用**，数字并入 `ShelterBaloo` |
| ~~Roboto Mono~~ | Google Fonts | Apache 2.0 | ✅ 免费 | **弃用**（避免第三套字体/材质），归一到 `ShelterBaloo` |
| 符号 / emoji（💎★☆✓✗🔒▶🔊⚙ 等） | — | — | — | **走自制 `ic_` 图标精灵（atlas_ui_main）**，禁系统 emoji 字体直显 |

- **OFL 履约四条**：① 随包附 `OFL.txt` 许可全文 + 版权声明（游戏内“关于/开源许可/素材清单”页，UI §14 已登记承接）；② 子集化/转 SDF 属修改 → **不得沿用保留字体名（RFN）**，故资产统一改名 `ShelterHan`/`ShelterBaloo`（**改名强度以各字体 OFL 头 RFN 声明为准**：Baloo 2 声明保留名“Baloo”→必改；思源黑体/Noto 若未声明保留名→改名属可选，本项目一律改名取保守合规姿态）；③ 不单独售卖字体本身；④ 留存下载来源与许可快照备审。
- **中英混排**：优先单一资产 + Fallback 链（避免逐处 `<font>` 切换致材质/DrawCall 翻倍）；数字统一 `ShelterBaloo`。
- **生僻字（单一确定机制，与包体口径不打架）**：主资产 `ShelterHan` 走 **静态裁剪**常用字（3500）；生僻字由一份 **扩展覆盖的 OFL CJK 后备资产 `ShelterHanFB`**（近全量、挂 TMP `m_FallbackFontAssetTable`）自动回退承接；**默认不采用“全量动态字体常驻”**（后者需整份 CJK 源字体随包、包体不可控）。Editor **缺字自检**需确保业务文案 100% 命中主+备字集、零豆腐块（见 Compliance.md §10）；若走“全量文案字形一次性烘图”则需文案打包前冻结。`ShelterHanFB` 同样按 OFL 改名、附许可。

---

## 6. 异步与时间控制

| 项目 | 选择 | 说明 |
|------|------|------|
| 异步方案 | **UniTask**（Cysharp 开源库） | 零 GC、支持取消、与 Coroutine 互转 |
| 状态机计时 | `await UniTask.Delay(seconds, DelayType.DeltaTime)` | 支持 Time.timeScale = 0（暂停） |
| 帧循环 | 保留 MonoBehaviour.Update 用于高频 tick（倒计时 UI 更新） | - |
| 协程使用 | 简单一次性动画可用 Coroutine；长逻辑改用 UniTask | - |
| 全局暂停 | `Time.timeScale = 0` + `PauseManager` | 暂停期间所有 UniTask.Delay 自动冻结 |

**不选 Awaitable 的原因**：UniTask 提供零 GC、取消传播、PlayerLoop 集成；在 IL2CPP 高堆内存场景下更可控，且支持 Android 后台任务取消。

---

## 7. 事件与系统解耦

| 项目 | 选择 | 说明 |
|------|------|------|
| 模块内事件 | **C# `event Action`** | 同一 System 内部（QuizStateMachine 内部状态转换） |
| 跨模块事件 | **静态 EventHub**（自研轻量类，参考架构文档） | 例如 `EventHub.Publish(new ZombieKilledEvent{...})` |
| Inspector 可视化事件 | **UnityEvent** | SO 中配置的关卡事件（如"波次结束触发对话") |
| 消息协议 | 定义 `IEvent` 接口，所有事件为 `struct`（零 GC） | 事件结构集中在 `Assets/Scripts/Core/Events/` |

**不选 MessagePipe/Zenject 的原因**：包体预算有限（≤200MB，D23 后仍含运行时），学习成本大；轻量 EventHub 已足够。

---

## 8. 依赖注入与容器

| 项目 | 选择 | 说明 |
|------|------|------|
| DI 方案 | **无框架，手写单例 + Bootstrap** | 项目规模适中，避免过度设计 |
| 全局管理器 | `RuntimeInitializeOnLoadMethod` + 单例模式 | GameManager / AudioManager / SaveManager |
| 场景级依赖 | 通过 Bootstrap MonoBehaviour 手动装配 | 在战斗场景加载时初始化 QuizStateMachine、WaveController |

**如后续架构膨胀**：可考虑迁移到 VContainer（轻量、支持 Addressable Scope）。

---

## 9. 数据持久化

| 数据类型 | 存储方案 | 位置 |
|---------|---------|------|
| 玩家进度（金币、能量晶、关卡解锁） | **JsonUtility + File IO**（application.persistentDataPath） | `save/playerdata.json` |
| 单词掌握度（生词/学习中/已掌握/长期掌握） | **JsonUtility** | `save/wordmastery.json` |
| 错题本 | **JsonUtility** | `save/wrongwords.json` |
| 7 日复习计划 | **JsonUtility**（日期戳驱动） | `save/reviewschedule.json` |
| 关卡配置 | **ScriptableObject**（不打包运行时修改） | `Resources/Levels/*.asset` |
| 单词数据表 | **CSV + 启动时解析** | `Resources/Data/words.csv` |
| 用户设置 | **PlayerPrefs**（键少、频繁读写） | 系统注册表 / Android SharedPreferences |
| 备份策略 | 每日自动备份 `.bak`，损坏时回退 | 同 persistentDataPath |

**加密**：初版不加密（本地单机），后续联网版本再引入 AES。

**数据体积口径**（三者不得混用）：
- **玩家存档（动态数据）**：只存 `wordId → masteryLevel / reviewAt` 等状态位，**不存单词文本**；常驻内存 ≤ 5000 词 × ~50 字节 ≈ **250KB**，可全量加载
- **词库总量（静态资源）**：全量约 **20000 词**（§10.5），按教材+年级**拆分 SO 按需加载**，不是单次全量加载
- **词库与包体口径（§12 D14 定案 · D23 修订）**：首发包内置小学→四六级四个年级阶段核心词库 + 人教版/外研社版教材；其余教材版本走后续 DLC 资源包；**原“仅因词库数据导致包体 >80MB 可接受”豁免条款作废（D23）**，词库数据全额计入 **≤200MB 唯一硬线**

---

## 10. 配置数据（ScriptableObject）

| SO 类型 | 用途 | 位置 |
|---------|------|------|
| `LevelConfig` | 关卡配置（5 波 + 词库 + 目标）；含 `waves[].spawns[].modifiers[]`（`EnemyModifier`）、`waves[].eventType`（`WaveEventType`）、`doctrineEnabled`、`earlyChapterBonus`（GDD D18/D19） | `Resources/Levels/{Chapter}/{LevelId}.asset` |
| `ZombieConfig` | 小怪兽原型（活力值、速度 **1.0/1.2/0.7 m·s⁻¹**（`PIXELS_PER_METER=100`）、掉落）；`MenderTickSeconds=5`；D20 游走怪 `WanderSpeed=0.6`+`dropOverride`（能量晶 20–40/碎片 15–30）（GDD D17/D18/D20） | `Resources/Config/Zombies.asset` |
| `QuizConfig` | 答题参数（时限、**连击窗口 4s**、受击硬直 `HitStunSeconds=0.3`、Swift `SwiftThresholdSeconds=3.0`/`SwiftDamageMultiplier=1.15`；GDD D17） | `Resources/Config/Quiz.asset` |
| `SupplyPackConfig` | 7 种补给包（Lv1/2/3 效果）+ **分类字段（控制类/转化类）**、**掉落保底**与**跨波囤积上限**参数（GDD D16） | `Resources/Config/SupplyPacks/*.asset` |
| `GameConfig`（全局） | 局内 `BattleDoctrine` 三选一系数（`Fire` 攻×1.2/`Bulwark` 墙耐久×1.3/`Logistics` 掉落+1·波间隔-1s）、`EarlyChapterBonus=1.3`、首档建筑升级降价 200→100、`PIXELS_PER_METER=100`；目标高亮开关 `LockedTargetHighlight=true`；D20 稀有怪 `WandererSpawnChance=0.05`/`WandererMaxPerBattle=1`/`WandererWaveWhitelist={2,3,4}`；D21 学习即弹药：里程碑奖励 `FirstMasterCrystalBonus=3`/`WordFixCrystalBonus=2`、Mastery Meter 阈值 `MasterySwiftPct=0.6`/`MasteryComboPct=0.8` 与加成 `SwiftThresholdBonus=0.5`/`ComboWindowBonus=0.5`（运行时写 `BattleModifiers`，仅本局、只动节奏不动伤害）（GDD D15 v2.6/D17/D18/D19/D20/D21） | `Resources/Config/Global.asset` |
| `WeaponConfig` | 武器碎片/等级/效果 | `Resources/Config/Weapons.asset` |
| `WordDatabase` | 单词主库（**按教材+年级拆分**） | `Resources/Data/WordDatabase_{key}.asset` |

**约束**：SO 中不允许使用 `Dictionary<K,V>` 直接字段，改用 `[Serializable]` 数组 + 运行时转字典。

---

## 10.5 单词数据加载策略

### 数据规模假设
- 小学 6 年级 × 4 教材 × 10 单元 = 240 个单元包
- 每单元约 50-100 词，**全量词库约 20000 词**（含初中/高中/四六级）；§9 的 5000 词指的是**存档/常驻部分**，不是词库总量（首发内置范围 = 小学→四六级全阶段 + 人教/外研社，其余 DLC，见 §12 D14）
- 单本教材 SO 约 3-5 MB

### 分阶段加载

**Phase 0（骨架）**：
- 不加载真实数据，只预留接口 `IWordProvider` / `IWordLibraryService`

**Phase 1（玩法闭环）**：
- 按教材+年级拆分 SO，用户选定后 `Resources.Load<WordDatabase>`
- 应用启动：只加载"上次玩家选中的词库"（1 个 SO）
- 选词界面：只列出 metadata（教材名/年级/单词数量），不加载具体单词
- 卸载时机：切换词库时释放旧 SO（`Resources.UnloadAsset`）
- 内存预算：同时保留最近 3 本 = 15MB

**Phase 2+（内容扩展）**：
- 迁移至 **Addressables Label 分组**：每个教材一个 pack
- Label：`[textbook=RenJiao] [grade=3] [unit=5]`
- 支持后续 CDN 远程包（若开联网）
- 卸载：`Addressables.Release(handle)`

**接口层统一**：
```csharp
public interface IWordLibraryService {
    Task<TextbookMetadata[]> ListAvailable();      // 仅元数据，几 KB
    Task<WordDatabase> LoadLibrary(string key);    // 按需加载
    void ReleaseLibrary(string key);               // 不再使用时释放
}

// Phase 0-1: ResourcesWordLibrary
// Phase 2+: AddressableWordLibrary
// 上层代码（选词 UI、Quiz 系统）无需修改
```

**工具支持**：`Assets/Editor/WordImporter.cs`（MenuItem），策划放 CSV 到 `StreamingAssets/words/{key}.csv`，一键生成对应 WordDatabase.asset。

---

## 11. 音频

| 项目 | 选择 | 说明 |
|------|------|------|
| 音频引擎 | **Unity Audio（内置）** | 无需 FMOD，成本考虑 |
| 分组 | Music / SFX / Voice / UI 四个 AudioMixer Group | 支持音量独立设置 |
| 播放管理 | `AudioManager` 单例 + AudioPool（16 个 AudioSource） | 避免频繁 Instantiate |
| 格式 | .ogg（BGM，**128kbps 立体声·D23 品质上调全量重制**）/ .ogg（长音效 96kbps 单声道）/ .wav（短音效，16bit 44.1kHz） | 兼顾体积和质量（D23 美术品质优先，包体口径见 §9 ≤200MB） |
| 采样率 | 44.1kHz（与 Android/iOS 默认一致，避免重采样开销） | 22kHz 用于短音效 |
| 语音（英文朗读） | 每个单词一条 .mp3，`Resources/Voice/en/{wordId}.mp3` | 或后续引入 Text-to-Speech |

---

## 12. 资源管理

| 项目 | 选择 | 说明 |
|------|------|------|
| 资源系统 | **Resources**（Phase 0-1）→ **Addressables**（Phase 2 起） | Phase 1 玩法先跑通，Phase 2 内容扩展时一并迁移 |
| 场景加载 | **ISceneLoader 抽象层**（默认 SceneManager，Phase 2 可切 Addressables） | 上层代码不受实现切换影响 |
| 对象池 | **Unity 内置 Object Pool**（`UnityEngine.Pool.ObjectPool<T>`） | 子弹/伤害飘字/小怪兽对象池 |
| 图集 | Sprite Atlas（V2） | 见第 2 节 |
| 内存控制 | 单词音频按加载即卸载；纹理 Max Size 按档（角色/UI 2048、BOSS 与场景背景 2048；**D23 品质上调**，低端机降级档兜底） | - |
| 迁移开关 | Phase 2 引入 Addressables 时，保留 Resources 兼容层 30 天 | 避免一次性重构风险 |

---

## 13. 版本控制与 CI/CD

| 项目 | 选择 | 说明 |
|------|------|------|
| 源码控制 | **Git + Git LFS**（大文件） | `.gitattributes` 声明 LFS 跟踪后缀（.psd/.png/.wav/.ogg/.fbx/.asset）；`.triggerfilter` 不是 Git 文件，应改为 Git LFS 配置 |
| 分支模型 | `main` / `dev` / `feature/*` | 每个大模块一条 feature 分支 |
| .gitignore | Unity 官方模板 + Library/、Temp/、Logs/、UserSettings/ | - |
| CI | **GitHub Actions**（Phase 2 起接入） | 自动跑 EditMode Test，失败阻断合并 |
| 打包 | **Unity Cloud Build** 或本地 Jenkins | Phase 3 接入 |

---

## 14. 测试

| 项目 | 选择 | 说明 |
|------|------|------|
| 单元框架 | **Unity Test Framework**（NUnit，内置） | EditMode + PlayMode 两种 |
| 强制测试范围（EditMode） | 状态机、奖励计算、单词补充、答题判定、**波次控制、星级阈值、EventBus、存档往返** | 每项 ≥ 80% 覆盖；**完整清单以 [ProjectSetup.md](./ProjectSetup.md) §9.2（7 项）为唯一真源** |
| 构造必断言（反回退护栏） | `PurchaseCurrency` 枚举值必须为 1；代码中不得出现 `IShelter`/`IBase` 血量类接口；`IZombie` 不得出现 `HP` 命名字段（伤害制用 `MaxVitality/CurrentVitality`，GDD D8） | 防止有人默默加回真钱货币 / 双血量 / 旧命名 |
| UI 测试（PlayMode） | **仅测 3 个关键流程**，使用 PageObject 模式抽象 UI 元素 | 避免脆式测试 |
| 手工测试 | 每个 Phase 结束跑一次完整关卡 | - |
| 性能测试 | Profiler + Frame Debugger，Phase 2 起每周五跑 | 目标：移动端 60 FPS 稳定 |

### PlayMode 必测 3 个关键流程
1. **答题正确流程**：进入关卡 → 3 只普通小怪兽 → 全答对 → 胜利结算
2. **大怪击败流程**：大怪出现 → 基础武器连续答对 3 题（活力 600 归零）→ 掉落奖励
3. **失败退出**：能量墙耐久归零 → 结算 50% 奖励 → 存档正确（**避难所无血量，不存在第二段失败判定**）

**PageObject 示例**：
```csharp
public class QuizPanelPage {
    public Button AnswerButton(int index) => ...;
    public TMP_Text TimerText => ...;
    public void SelectAnswer(int index) => AnswerButton(index).onClick.Invoke();
}
```

---

## 15. 编辑器工具

| 项目 | 选择 | 说明 |
|------|------|------|
| Inspector 扩展 | **Unity 内置 `[CustomEditor]` + `[SerializeReference]`** | Phase 0-1 不引入外部插件；若 Phase 2 确认痛点强烈，重新评估 Odin |
| 关卡编辑器 | **自定义 EditorWindow**（`Assets/Editor/LevelEditor/`） | 可视化配置 5 波小怪兽（数组排序需自实现） |
| 单词导入工具 | 自定义 MenuItem：CSV → WordDatabase SO | 策划批量导入，CSV 放 `StreamingAssets/words/` |
| 图集管理 | Sprite Atlas 内置 | - |
| 不推荐 | Odin 付费插件（Phase 0） | 包体与预算考虑；内置工具已够 MVP

---

## 16. 关键第三方包（Package Manager）

```json
{
  "dependencies": {
    "com.unity.render-pipelines.universal": "14.0.10",
    "com.unity.inputsystem": "1.7.0",
    "com.unity.2d.animation": "9.1.0",
    "com.unity.2d.pixel-perfect": "5.0.3",
    "com.unity.2d.sprite": "1.0.0",
    "com.unity.2d.tilemap": "1.0.0",
    "com.unity.textmeshpro": "3.0.8",
    "com.unity.test-framework": "1.3.9",
    "com.unity.addressables": "1.21.18",
    "com.unity.cinemachine": "2.10.0"
  }
}
```

> ⚠️ **manifest.json 约束**：`com.unity.addressables` 于 **Phase 2 才写入 dependencies**（版本号不含中文注释，JSON 不合法会直接报错）；Phase 0-1 不得提前引入。
> **Cinemachine（待定）**：2D 横版固定镜头若无动态追踪需求，建议从依赖表中剔除（减少包体与维护面）；保留条件 = 需要震动/跟随相机行为，Phase 1 前由主程拍板。

**外部插件**（通过 Asset Store 或 GitHub 安装）：
- **UniTask**（Cysharp）：异步（MVP 必需）
- **DOTween**（Demigiant）：动画补间（UI 弹窗）
- **TextMeshPro 字体 SDF**：TMP 已含

**明确不引入**：
- ❌ Zenject/Extenject（手写 Bootstrap 足够）
- ❌ MessagePipe（自研 EventBus 已够）
- ❌ Odin Inspector（Phase 0，内置可用）

---

## 16.5 广告 SDK 选型

**发行路径**（国内主市场 · 双阶段）：

| 发行方式 | 广告方案 | 备注 |
|---------|---------|------|
| **国内 Android/iOS App**（本版灰区 + 版号后上市，**主路线**） | **穿山甲（字节）** 激励视频（`PangleAdsService`）+ **优量汇（腾讯）** 备选/竞价 | 全年龄默认投个性化广告；对已识别未成年切非个性化/儿童安全等级；运营商号码认证 SDK 仅用于年龄/家长验证、非广告 |
| ~~海外 Android/iOS App~~（已放弃） | ~~Google AdMob / Meta Audience Network~~ | **因放弃海外发行不再集成**；若未来回评海外再按 GDPR/COPPA/ATT 重启 |

**未成年人保护约束（必做）**：
- 防沉迷实名验证：**本版后置**（无版号无法接入国家新闻出版署官方实名系统）；取得版号后接入（见 §16.7）；`IAgeVerifyService`/`IPlatformService` 本版起入 Architecture 契约预留
- 年龄/家长验证走国内通用做法：手机号实名 + **运营商一键登录/号码认证**（客户端直连、不自建后端），短信验证码备选；判定未成年且无家长同意则退出
- 广告不得诱导充值或观看；未成年用户广告降为非个性化、频率上限（单局 ≤1、单日 ≤5），**仅激励视频，禁插屏/Banner**（Compliance.md §3.2）；成年无硬上限（建议单局 ≤3）
- 过关必需技能包不得来自激励视频（GDD §12 D16 广告红线）
- 未成年用户的答题记录/掌握度**只存本地**、不上传服务端；手机号仅走验证链路、不进埋点/广告定向（Compliance §4 D4）

**实现约束**：
- 统一抽象 `IAdsService`，运行时按平台注入实现（参考 Architecture.md）
- 提供 `MockAdsService` 用于测试/无网环境
- 广告失败不阻断游戏（游戏本身完全离线可玩）

---

## 16.7 发行与合规专项

| 项 | 主路线（国内主市场 · 版号前灰区 → 版号后上市） | 备注 |
|----|---------|---------|
| **版号（ISBN）** | **本版不开（无预算）→ 融资后申请** | 版号前靠灰区内测积累数据去融资；申请周期 3-6 个月需资金 |
| **软著** | 版号申请前置（建议尽早办理） | 周期 1-2 个月；版号/商店上架需 |
| **防沉迷** | **后置版号阶段**接入国家官方实名；本版预留 `IPlatformService`/`IAgeVerifyService` | 无版号无法接入实名验证系统 |
| **隐私政策 + 用户协议** | 必需（中文为主） | 国内商店/审核硬要求 |
| **未成年人保护** | 年龄识别 + 家长同意（手机号实名/运营商一键登录）；数据仅本地；~~COPPA/GDPR-K 因放弃海外不适用~~ | 详见 Compliance.md §2 |
| **内容审查** | 小怪兽术语已落实；剩余敏感词软化见 Compliance.md §6.2 | 分级目标 **12+/全年龄**；国内版号/商店审核同样敏感 |
| **ICP 备案** | 版号后国内上架需（完全离线无后端可后置评估） | 运营商验证走三方网关、不自建后端 |

**风险提醒**：本版无版号 → **不能上国内商店、不能开放 IAP**，仅 TestFlight + 官网 APK 灰区内测（广告收入极低、定位数据/留存验证以融资）；融资→申请版号→国内商店正式上市后广告才成为主要现金流（全年龄个性化广告可拉高 eCPM）。详细渠道矩阵与应急预案见 **[Compliance.md](./Compliance.md) §1 / §7**。

---

## 16.6 数据埋点方案

**主程建议**：**自研 AnalyticsManager，三层埋点**

### 埋点分层
```
L1 本地日志（永远开）
├─ LogManager.Write(level, category, message, dict)
├─ 存 persistentDataPath/logs/{yyyyMMdd}.log
└─ 保留 7 天，超期自动清理

L2 游戏事件（结构化）
├─ LevelStart / LevelEnd
├─ AnswerSubmitted（wordId, correct, timeSpent）
├─ ComboTriggered（comboCount, buffType）
├─ SupplyPackSelected（packType, levelContext）
├─ AdShown / AdClicked / AdRewarded
└─ WordMasteryChanged（wordId, oldLevel, newLevel）

L3 上报（可选，未来开联网时启用）
├─ 每日凌晨批量打包 gzip
└─ 上传到自建/第三方 endpoint
```

**MVP 实施**：L1 + L2 必做；L3 接口预留但默认关闭。

**具体事件清单**：见 Architecture.md 第 5 节与第 18 节。

**未来可升级到**：ThinkingData（数数科技）、Firebase Analytics（仅海外），接口不变。

---

## 17. 平台适配与联网边界

### 平台适配

| 平台 | 特殊处理 |
|------|---------|
| **Android**（主目标） | 分屏/刘海屏 Canvas 用 Safe Area 组件；最低 API 24（Android 7.0）；Target API 34；架构 ARMv7 + ARM64；**版号前官网 APK/蒲公英灰区，版号后国内安卓商店**（华为/小米/OPPO/vivo/应用宝）上架；已放弃 Google Play 海外 |
| **iOS**（主目标） | Dynamic Island + 刘海屏适配；最低 iOS 13；ARM64；TestFlight 灰区内测 → 版号后 App Store 中国区；**支持 iPhone + iPad（iPad 仅拉伸适配，不做专版 UI）** |
| 屏幕方向 | **锁定横屏**（Landscape Left）；**不做竖屏**（GDD §12 D6） |
| PC Editor | 支持窗口/全屏切换，F11 快捷键；Direct3D 11；仅用于开发预览，不发行 |
| ~~WebGL~~ | **不做**（原小游戏目标已放弃） |
| ~~微信小游戏~~ | **不做**（包体/审核/持久化三项硬约束不划算） |
| ~~抖音小游戏~~ | **不做**（同微信） |
| ~~团结小游戏~~ | **不做**（同微信） |

### 联网边界（主程确认）

**完全离线可玩**：
- ✅ 主菜单、选词、战斗、结算、存档、避难所升级均不需要网络
- ✅ 无网络时游戏功能完整

**可选联网**（失败不阻断）：
- ⚠️ 广告 SDK 初始化（启动时尝试，失败则跳过广告）
- ✅ **验证白名单子口（C1 例外）**：年龄/家长验证经**运营商号码认证/一键登录网关**（客户端 SDK 直连运营商、不自建后端，仅传验证所需、不传游戏数据，短信验证码备选）；此为“数据全本地”原则的唯一联网例外（GDD §12 D22 / Compliance §2 C1、§4 D1）
- ⚠️ 埋点上报（默认关闭；若开启且失败，仅写本地）
- ❌ **云存档 / 排行榜 / 好友 / 对战 —— 永不做**（本项目定位为单机；不是"Phase 4 再考虑"，而是架构层删除能力，GDD §12 D4）
- ❌ **内购 / 订阅 / 真钱兑换 —— 永不做**（不开版号，无法上架 IAP；GDD §12 D1）

**商店与货币边界（D1）**：
- 商店界面**保留**，全部物品**只能用能量晶购买**（纯本地货币，结算与任务产出）
- **能量晶不能用钱换**：不定义 `RealMoney` 类货币，`PurchaseCurrency` 只允许 `EnergyCrystal` 一个枚举值
- **不引入 `com.unity.purchasing` / App Store IAP / Google Play Billing**；商店实现走 `IStoreService`（Architecture.md §4.11）
- 能量晶的获得与消耗完全本地可验证，无服务端下发

**实现约束**：
- Bootstrap 中先初始化游戏层，后初始化联网层
- 联网失败不抛异常，仅记录日志，提供降级方案（MockAdsService）

**分辨率策略**：设计基准 1920×1080，Canvas Scaler = Scale With Screen Size + MatchWidthOrHeight = 0.5。

---

## 18. 性能预算（Performance Budget）

| 指标 | 目标 | 检测手段 |
|------|------|---------|
| DrawCall | < 30 | Frame Debugger |
| 三角形数 | < 200k | Stats |
| SetPass | < 15 | Profiler |
| 内存 | ≤ 400MB（D23 修订，原 <150MB；低端机画质降级档兜底） | Memory Profiler |
| 包体 | ≤ 200MB（D23 唯一硬线，原 <80MB 废除；Build Report 阈值见 ProjectSetup §11.1 Phase 梯度） | Build Report |
| 帧率 | 移动端 60 FPS 稳定 | Profiler Recording |
| GC Alloc / Frame | < 1KB | Profiler GC |
| 场景加载 | < 3 秒 | Time Recording |
| 单局最大同屏小怪兽 | 30 只 | 逻辑约束 |

---

## 19. 已确认事项（Phase 0 开工基线）

以下事项主程已确认，不得在 Phase 0 会话中自行调整：

| 项 | 确认方案 |
|----|---------|
| 引擎 | **Tuanjie 1.x**（具体小版本号在项目创建时固定） |
| Odin Inspector | **不购买**，Phase 0-1 使用内置 CustomEditor |
| 联网 | **游戏本身完全离线可玩**；广告 SDK 与埋点上报（若启用）允许联网 |
| 广告 SDK | **主路线国内 App：穿山甲**（`PangleAdsService`）+ 优量汇 备选/竞价；全年龄默认个性化、未成年降级；~~Google AdMob（海外）因放弃海外不再集成~~；不提供小游戏平台选项（真源 §16.5） |
| 平台目标 | **Android + iOS 移动 App**；PC 仅开发预览；**不做微信/抖音/团结小游戏** |
| 发行策略 | **休闲益智**类目；**全年龄英语学习 · 国内主市场**；本版**无预算不开版号**→灰区内测（TestFlight + 官网 APK），融资后申请版号→国内 iOS 中国区 + 安卓商店上市；**纯广告变现（0 内购，全年龄可投个性化广告）**；**放弃海外**、Steam 暂缓；开工即启动合规项（Compliance.md §8 时间线、GDD §12 D22） |
| Addressables 时机 | **Phase 2 引入**（而非 Phase 1）；保留 Resources 兼容层 30 天 |
| 埋点方案 | **自研 AnalyticsManager**，三层埋点（本地日志/游戏事件/可选上报） |
| EventBus | **自研**，约 100 行（参见 Architecture.md） |
| Zombie 动画 | **Spine2D 骨骼动画**（spine-unity，v1.11 定案，取代旧混合方案）：单 SkeletonDataAsset + AnimationState 驱动 idle/walk/attack/hit/die；二进制导出；序列帧仅特效 |
| Word 定义 | **class**（全局唯一引用）；传递时可选用 `WordRef` struct |
| 场景过渡 | **ISceneLoader 抽象层**；Phase 0-1 默认 SceneManager；Phase 2+ 可切 Addressables |
| UI 测试 | **PlayMode + PageObject**，只测 3 个关键流程 |
| 单词数据加载 | **按教材+年级分 SO**；Phase 0-1 按需 Resources.Load；Phase 2 切 Addressables Label |
| 商店与货币（**D1**） | 商店界面**保留**，物品**只收能量晶**；**能量晶不可用钱换**；`IStoreService` 纯本地，不引入支付 SDK / `com.unity.purchasing` |
| 防线数值（**D2**） | **避难所无血量/耐久值、不可被攻击**；`IWall` 是唯一失败数值来源（`CurrentDurability/MaxDurability/IsBroken`）；耐久归零 → 本局直接失败；**不存在 `IShelter`/`IBase`** |
| 敌人类型（**D3**） | `ZombieType` 只有 **Normal / Elite / Boss** 三值；对应小怪兽 / 大怪兽 / BOSS 怪；**BOSS 怪每局仅 1 只**；无快速/坦克变体。**D23 修订**：30 种敌人（小 15/大 10/BOSS 5）均为原型下 species，每种独立 SkeletonDataAsset + `ZombieSpeciesConfig` 数据层，**不新增枚举值**；三批递进（P1 9 种/P2 11 种/P3 满 30 种） |
| 三会（**D4**） | **本版不做**；仅保留 `IWordStudyService` + `NoOpWordStudyService`；**不新增 `QuestionType.Spelling`** |
| 排行榜（**D4**） | **单机不做**；用本地 `bestScore/bestCombo/bestDurabilityPercent` 替代 |
| 屏幕方向（**D6**） | **锁定横屏**（Landscape Left），不做竖屏 |
| 战斗伤害模型（**D8**） | 答对 = 一次暴击，伤害 = `武器攻击力 × 200%`；小怪兽用 `MaxVitality/CurrentVitality`（小怪200/大怪600/BOSS2000），归零即击败；击败所需答对数 = ⌈活力÷每发伤害⌉，武器升级会减少（BOSS 不恒定 10 题）；答错/超时与哨塔均不削减活力 |
| 耐久继承（**D9**） | 能量墙耐久**不跨局继承，每局重置为满值**（= 当前阶位上限）；避难所升级只提升上限 |
| 星级指标（**D12**） | 耐久百分比与答对率**必须同时满足**（无单一主指标） |
| 敏感词软化（**D13**） | 玩家可见文案**全量软化**（击杀/消灭/击倒→击败 等，映射见 Compliance §6.1）；**代码标识符保留**（`Zombie*`/`WallDurability`/`Vitality` 不改）；Phase 3 提审前执行 |
| 首发包词库（**D14**） | 首发含小学→四六级四年级阶段核心词 + 人教/外研社；其余教材走 DLC；**D23 修订：词库 >80MB 豁免作废，全额计入 ≤200MB 唯一硬线** |
| 战场几何与目标推进（**D15**） | 敌人左起 **1/6 触发答题、4/6 停止攻墙**（主流横屏，`1m=100px`）；**v2.6 修订：锁定与非锁定全程正常速度推进（取消不前进/30% 减速）、当前锁定目标 view 层高亮（`GameConfig.LockedTargetHighlight`）；买时间仅由控制类补给实现** |
| 技能包必用策略层（**D16**） | `SupplyPackConfig` 分**控制类**(Ice/Slow/Confuse，0 伤害)/转化类(Lightning/Spread/Fire/Pierce，扩散答对暴击)；掉落保底（每关≥1控制+1转化、BOSS 波必掉 1 控制）；`PlayerData.activeSupplyPacks[]` 同时生效≤2（1控制+1转化）、控制类跨波囤积至多 1 个；过关必需包只靠掉落、不来自激励视频（Compliance §3） |
| 战斗手感与连击（**D17**） | 即时命中（答对当帧结算、取消 1s 硬停顿、`HitStunSeconds=0.3` 仅动画）；连击窗口 3→**4s**、档位 **3/5/8/12/20**；Swift `≤3s` 答对×`1.15`（`QuizConfig` 可配） |
| 敌人变体·事件波·局内构筑（**D18**） | `EnemyModifier`（Shielded/Sprinter/Mender=Phase3、TwinWord=Phase4）与 `WaveEventType`（Rush/BossPressure=Phase3、Ambush=Phase4）**仅扩展不改 `ZombieType`**；`BattleDoctrine` 开局三选一仅本局生效不写存档（D9 不破坏）；`LevelConfig.doctrineEnabled` |
| 本版轻成就与前期成长（**D19**） | 本地轻量成就 `LocalAchievementCategory`（纯单机一次性发奖，区别于 Phase 4+ 三会/联网成就）；`EarlyChapterBonus=1.3`（仅首通）、首档建筑升级 200→100（`GameConfig`） |
| 稀有游走奖励怪（**D20**） | `EnemyModifier.Wanderer` + `ZombieConfig.dropOverride`（金库怪能量晶 20–40、铁匠怪碎片 15–30）、`WanderSpeed=0.6`；刷出 `WandererSpawnChance=0.05`、限第 2–4 波、每局 ≤1（`GameConfig`/`LevelConfig`）；**不新增 `ZombieType`（守 D3）、不攻墙、不计入通关**（Phase 3） |
| 学习即弹药·认词正反馈显性化（**D21**） | M1 头顶词汇标记叠加 `Word.memoryStrength`（四态兜底 0/0.5/0.8/1.0，纯表现）；M2 首克新词/攻克错题当帧一次性晶 `FirstMasterCrystalBonus=3`/`WordFixCrystalBonus=2`（走 §4.4 奖励、非伤害）；M3 Mastery Meter≥`MasterySwiftPct=0.6`→Swift 阈值+0.5s、≥`MasteryComboPct=0.8`→连击窗口+0.5s（运行时 `BattleModifiers`，仅本局）；**只动表现/奖励/阈值节奏，不新增伤害来源**（Phase 3） |
| 发行与合规路线（**D22**） | **全年龄 + 国内主市场双阶段**（版号前灰区 TestFlight/官网 APK → 融资 → 版号 → 国内商店上市）；放弃海外、Steam 暂缓；0 内购纯广告（全年龄个性化、未成年降级）；年龄/家长验证手机号+运营商一键登录（不自建后端），防沉迷后置版号；联网边界开验证白名单子口（§16.5/§17、Compliance §1/§2/§3） |
| 敌人种类扩张与包体重建（**D23**） | 包体 **≤200MB 唯一硬线**（<80MB 废除）、内存 **≤400MB**（<150MB 废除，低端降级档兜底）；敌人 30 种独立骨架（小 15/大 10/BOSS 5）三批递进（P1 9/P2 11/P3 10）；全模块美术品质上调（场景 2048、UI 精细材质、特效帧数、BGM 重制+VO 转正；英文语音下载不上包）；图集扩容见 §2、Build 阈值梯度见 ProjectSetup §11.1、种规格见美术资源需求表 §3 |

---

## 20. 变更历史

| 版本 | 日期 | 变更内容 | 责任人 |
|------|------|---------|--------|
| v0.1 | 2026-11-04 | 初稿，基于 GDD v2.0 生成 | AI 主程视角 |
| v1.0 | 2026-11-04 | 主程确认 12 项未决事项：Tuanjie/Odin不购/完全离线可玩+广告可联网/Addressables Phase 2/自研埋点与EventBus/混合动画/Word为class/ISceneLoader抽象/UI PlayMode测试/单词分SO加载 | 主程 |
| v1.1 | 2026-11-05 | 关联文档同步：新增 ProjectSetup.md 引用，关联文档版本升至 v1.1；无技术选型变更 | 主程 |
| v1.2 | 2026-11-05 | **平台方向重大调整**：放弃微信/抖音/团结小游戏（包体/持久化/合规三硬约束）→ 专注 **Android + iOS 移动 App**；发行类目确定为**休闲益智**（需版号）；新增 §16.7 发行与合规专项；§1 补充 Scripting Backend/架构；§16.5 广告重写；§17 平台适配剔除小游戏行；§19 已确认项同步 | 主程 |
| v1.3 | 2026-11-05 | **发行策略定案**：**不开版号 + 纯广告变现（0 内购）**，主渠道 iOS 海外区 + Google Play 全球区，国内安卓市场暂不上架；新增关联文档 [Compliance.md](./Compliance.md)；§0 合规前置原则重写；§16.7 国内/海外双列表改为无版号主路线表；术语软化（僵尸→小怪兽/精英僵尸→大怪兽/BOSS 僵尸→BOSS 怪）已落实 | 主程 |
| v1.4 | 2026-11-05 | **主程二审残留修复**：§16.5 儿童合规约束剔除“防沉迷必做”残留（与 §16.7 对齐，改为未来国内可选）；广告位补充“仅激励视频”硬约束；广告发行路径表重排（海外主路线优先 + 构建目标宏剔除备注）；§16 manifest.json 非法占位注释修正 + Cinemachine 待定标注；§19 广告 SDK 已确认项同步 | 主程 |
| v1.5 | 2026-11-05 | **四项定案（GDD §12 D1-D4）入技栈基线**：<br>**D1** §17 新增“商店与货币边界”（能量晶单货币、禁支付 SDK）；§19 新增已确认行<br>**D2** 全文剔除“护盾/HP”口径（`IWall`/`WallManager`/耐久）；PlayMode 失败用例注明“避难所无第二段判定”；`ZombieConfig` 字段改“需答题数”<br>**D3** §19 固化 `ZombieType` 三值 + BOSS 每局 1 只<br>**D4** §17 将“云存档/排行”从“Phase 4 考虑”改为**永不做**；三会仅保留接口 + NoOp<br>**其它修正**：§1 “ARMv64”笔误与失效理由重写；§9/§10.5 澄清 5000 词（存档）↔ 20000 词（词库全量）口径，包体冲突归入 GDD §12 Q7；§13 `.triggerfilter` → `.gitattributes`；§14 EditMode 强制范围与 ProjectSetup §9.2 （7 项）对齐并声明单一真源，新增“构造必断言”反回退护栏；§17 剔除国内渠道 APK 双出、补锁定横屏与 iPad 支持声明；“Sprint”→“Phase”；关联文档版本串升级 | 主程 |
| v1.6 | 2026-11-05 | **Q1-Q8 拍板（GDD §12 D8-D14）入技栈基线**：<br>**D8 伤害制**：`IZombie`→`MaxVitality/CurrentVitality` + `TakeDamage(攻击力×200%)`，击败所需答对数动态计算（BOSS 不恒定）；§14 断言补注伤害制字段、PlayMode 大怪用例改活力口径；`ZombieConfig` 字段“需答题数”→“活力值”<br>**D9/D12/D13/D14**：§19 新增耐久不继承、星级同时满足、全量软化（`Zombie*` 保留）、首发词库范围与包体豁免；§9/§10.5 将 Q7 待定改为 D14 定案；关联文档串同步 GDD v2.3/Architecture v1.5 等 | 主程 |
| v1.7 | 2026-11-05 | **战场几何与技能包必用策略层（GDD §12 D15/D16）入基线**：<br>**D15**：敌人左起 1/6 触发答题、4/6 停止攻墙（主流横屏 `1m=100px`）；当前锁定目标答题期间不前进、恢复推进 30% 速度（`ZombieConfig`/`QuizConfig` 可配）<br>**D16**：`SupplyPackConfig` 新增**分类字段**（控制类 Ice/Slow/Confuse，0 伤害 / 转化类 Lightning/Spread/Fire/Pierce、扩散答对暴击）+ **掉落保底**（每关≥1控制+1转化、BOSS 波必掉 1 控制）+ **跨波囤积上限**（控制类囤 1、`PlayerData.activeSupplyPacks[]`≤2）；过关必需包不来自激励视频（Compliance §3）<br>§19 新增 D15/D16 行；关联文档串同步 GDD v2.4 / Architecture v1.6 等 | 主程 |
| v1.8 | 2026-11-06 | **玩法增味（GDD §12 D17/D18/D19）入配置基线**：新增全局 `GameConfig`（`BattleDoctrine` 三选一系数/`EarlyChapterBonus=1.3`/首档建筑升级 200→100/`PIXELS_PER_METER=100`）；`QuizConfig` 增连击窗口 4s/`HitStun=0.3`/Swift 参数；`LevelConfig` 增 `spawns[].modifiers[]`/`eventType`/`doctrineEnabled`/`earlyChapterBonus`；`ZombieConfig` 速度统一 1.0/1.2/0.7 m·s⁻¹、`MenderTick=5`；§19 新增 D17/D18/D19 行；关联文档串同步 GDD v2.5 / Architecture v1.7 等 | 主策划 |
| v1.9 | 2026-11-06 | **玩法增味 II（GDD §12 修订 D15、新增 D20）入配置基线**：D15 **取消“锁定目标不前进/30% 减速”**→全程正常推进 + `GameConfig.LockedTargetHighlight`（view 层高亮）；新增 **D20** 稀有游走怪（`EnemyModifier.Wanderer`+`ZombieConfig.dropOverride`/`WanderSpeed=0.6`、`GameConfig.WandererSpawnChance=0.05`/限 2–4 波/每局≤1，不新增 `ZombieType`）；§19 D15 行改写、新增 D20 行；关联文档串同步 GDD v2.6 / Architecture v1.8 等 | 主策划 |
| v1.10 | 2026-11-06 | **认词正反馈显性化（GDD §12 D21）入配置基线**：`GameConfig` 新增 D21 参数（`FirstMasterCrystalBonus=3`/`WordFixCrystalBonus=2`、`MasterySwiftPct=0.6`/`MasteryComboPct=0.8`、`SwiftThresholdBonus=0.5`/`ComboWindowBonus=0.5`，运行时写 `BattleModifiers`）；M1 复用 `Word.memoryStrength`（Arch §4.3/§6.8）、M3 复用 `QuizConfig` 的 Swift/连击参数做本局加成；§19 新增 D21 行；关联文档串同步 GDD v2.7 / Architecture v1.10 / DevTasks v1.10 / ProjectSetup v1.11 / Compliance v1.7 | 主策划 |
| v1.11 | 2026-11-06 | **主程审核同步（GDD 逐条对照）**：技术选型与配置契约无变更（审核确认四技术文档与 GDD 定案 D1-D21 口径一致；发现的矛盾均属 GDD 内部/待拍板项，见会话报告）；基线串升至 GDD v2.8 / Architecture v1.10 / DevTasks v1.10 / ProjectSetup v1.11 / Compliance v1.7 | 主程 |
| v1.12 | 2026-11-06 | **⑩ 拍板：动画方案改 Spine2D（本表为技术选型真源）**：§2 精灵管理/动画行、DrawCall 策略、§19 Zombie 动画行均改 **Spine2D**（spine-unity）——角色/敌人骨骼导出**二进制** `.skel.bytes`+`.atlas`+atlas page png（**不用 JSON/PSB**）；`SpriteAnimationPlayer` 降级为仅特效/嘲讽气泡序列帧；图集：Sprite Atlas V2 四张 + Spine 骨骼 atlas page 三张（spine_chr/zmb/boss，≤1024、ASTC 4×4、同屏共享同页以利合批）；真源联动 Architecture §15.1 / 美术资源需求表 §1~§3；基线串升至 GDD v2.9 / Architecture v1.11 / DevTasks v1.11 / ProjectSetup v1.12 / Compliance v1.8 | 主程 |
| v1.13 | 2026-11-06 | **字体归一 + 授权/重命名（配合项目方“不买字体版权”，本表 §5 为字体选型唯一真源）**：§5 字体行重写并新增 **§5.1 字体授权与重命名**——中文 `ShelterHan`（思源黑体子集）、拉丁(英/数字) `ShelterBaloo`（Baloo 2 子集），**均 SIL OFL 1.1 免费商用**；子集化属修改→按 OFL 保留名规则**统一重命名**；**DIN Alternate（商业专有、需购买）弃用**、数字并入 `ShelterBaloo`；Roboto Mono（Apache 2.0）自愿弃用（减字体套数）；特殊符号/emoji 走自制 `ic_*` 图标精灵、禁系统 emoji 直显；生僻字走 TMP 动态字体+Fallback；联动 GDD §8.4.3（改引本表）/ Compliance 新增 §10 素材与字体授权 / 美术表 UI-26/27；基线串升至 GDD v2.10 / Architecture v1.12 / DevTasks v1.12 / ProjectSetup v1.13 / Compliance v1.9 | 主程 |
| v1.14 | 2026-11-07 | **字体评审发现落地（本表 §5/§5.1 仍为字体选型唯一真源）**：① **生僻字机制与包体口径闭环**——取消“静态裁剪 3500”与“全量动态字体”互斥表述，定为一套：静态主资产 `ShelterHan`（3500）+ **近全量 OFL CJK 后备资产 `ShelterHanFB`**（挂 `m_FallbackFontAssetTable`、约 5–8MB、仅回退用）+ Editor 缺字自检；② **许可展示页承接**——OFL.txt 全文/版权声明放“关于/开源许可/素材清单”页（UI §14 已登记）；③ **OFL 改名强度以各字体 OFL 头 RFN 声明为准**（Baloo 2 必改、思源/Noto 保守改名）；④ **DIN 出处标注放宽**（Monotype/Linotype，macOS 自带 DIN Alternate 属 Adobe 授权版）；⑤ §5 澄清“常态中英两套资产、混排优先单资产+Fallback”。联动 GDD §8.4.3 / Compliance §10 / 美术表 UI-26~28；基线串升至 GDD v2.11 / Architecture v1.13 / DevTasks v1.13 / ProjectSetup v1.14 / Compliance v1.10 | 主程 |
| v1.15 | 2026-11-07 | **发行与合规路线重大调整落地（对齐 Compliance v1.11 / GDD v2.12 D22；本表实质变更非仅串升）**：<br>**① 发行策略**（header/§19）：不开版号只海外 → **全年龄·国内主市场双阶段**（版号前 TestFlight+官网 APK 灰区、不提审商店；融资→申请版号→国内 iOS 中国区+安卓商店上市；放弃海外、Steam 暂缓）；**② §16.5 广告 SDK**：海外 AdMob 主路线 → **穿山甲（`PangleAdsService`）+ 优量汇 国内主**、AdMob/Meta 划废、全年龄默认个性化/未成年降级；**③ §16.7 发行与合规专项**重写为国内双阶段（版号本版不开→融资后申请、防沉迷后置、未成年人保护改手机号实名/运营商一键登录、分级 12+/全年龄、ICP 版号后）；**④ §17 平台适配** Android 改官网 APK/国内商店、iOS 改 TestFlight 灰区/中国区；§17 联网边界新增 **C1 验证白名单子口**（运营商号码认证、唯一联网例外）；§0 决策原则“儿童向”→“全年龄”；§19 新增 D22 行；基线串升至 GDD v2.12 / Architecture v1.14 / DevTasks v1.14 / ProjectSetup v1.15 / Compliance v1.11；派生文档同步升至 v1.4 | 主程/制作人 |
| v1.16 | 2026-11-07 | **敌人种类扩张与包体重建落地（对齐 GDD v2.13 D23；本表 §0/§18 为性能预算真源）**：<br>**① 包体/内存硬约束重写**：§0/§18 包体 <80MB→**≤200MB 唯一硬线**、内存 <150MB→**≤400MB**（低端机画质降级档兜底；Build Report 阈值梯度见 ProjectSetup §11.1）；§7 “包体紧张”理由改写<br>**② §9 词库包体口径**：D14 “仅词库 >80MB 豁免”作废，词库全额计入 200MB<br>**③ §2 图集/纹理扩容**：Spine atlas page 每骨架可多页、允许 2048；SkeletonDataAsset 30 份（按种独立，P1 首批 9 份）；合批改靠同页种类共现编排，DrawCall <30 硬线不变；§12 纹理 Max Size 改按档 2048<br>**④ §19 已确认项**：D3/D14 行修订 + 新增 D23 行；关联文档串升至 GDD v2.13 / Architecture v1.15 / DevTasks v1.15 / ProjectSetup v1.16 / Compliance v1.12；派生文档同步升至 v1.5 | 主程 |

---

> **使用说明**：
> 1. Phase 0 会话开始前，主程需先确认第 19 节所有决策
> 2. 一旦本文件敲定，任何变更必须走文档修改 + Git PR 流程
> 3. 与 Architecture.md 配套阅读，两者共同构成开发基线
