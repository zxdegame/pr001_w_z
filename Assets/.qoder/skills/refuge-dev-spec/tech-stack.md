# 技术栈硬约束（真源：TechStack.md）

具体参数永远以 `Assets/Design/TechStack.md` 当前版本为准；本页只做导航与固化边界，不复制易变数值。

## 引擎与语言
- 引擎 Tuanjie 1.x（基于 Unity 2022.3 LTS，禁随意升内核；`ProjectSettings/ProjectVersion.txt` 提交 git 锁定）
- .NET Standard 2.1 · C# 9（record / init / 模式匹配）
- Scripting Backend：Android / iOS = IL2CPP；Editor = Mono
- Target Architectures：Android ARMv7 + ARM64（值=3）；iOS ARM64
- Android Min SDK 24 / Target SDK 34
- 目标渲染 Linear（URP 2D 光照需要）

## 渲染与 2D
- URP 14.x（绑定引擎），**2D Renderer**；三档 Quality 全指同一 2D 管线资产（本项目 = `Refuge_2DRenderer`，guid `99898fc0…`；勿写死，以 GraphicsSettings / QualitySettings 实测为准）
- Pixel Perfect Camera · Sprite Atlas V2（每模块一张，≤2048）· Tilemap（Tilemap Renderer + Composite）· 2D Light · URP Volume 可选后处理 · Shader Graph
- 动画：**Spine2D**（spine-unity 运行时 4.2）——美术导出须用 **Spine Editor 4.2.x**，二进制 `.skel.bytes` + `.atlas` + page png（不用 JSON）；序列帧 / 粒子仅 <8 帧特效与嘲讽气泡；UI 过渡走 DOTween

## 输入 / UI / 异步 / 存档 / 事件 / DI
- 输入：New Input System（`activeInputHandler=1`；`.inputactions` 生成类见 pitfalls CS0542）
- UI：uGUI + TextMeshPro + DOTween
- 异步：UniTask（业务链不用 Coroutine；禁 `async void`）
- 存档：JsonUtility + File IO（小游戏走 `IStorageService` 平台抽象）
- 事件：自研 EventBus（约 100 行，见 Architecture §5.1）
- DI：无框架，手写单例 + Bootstrap 装配

## 场景过渡 / 广告 / 埋点（Phase 0-1 抽象占位）
- `ISceneLoader`（Phase 0-1 SceneManager 实现）
- `IAdsService`（Phase 0 `MockAdsService`）
- `IAnalyticsService`（Phase 0 本地文件 sink）

## 性能预算（硬线，D23 修订）
| 指标 | 上限 |
|---|---|
| DrawCall | < 30 |
| 内存 | ≤ 400MB |
| 单渠道包体 | ≤ 200MB |
| 同屏僵尸 | ≤ 30（含 BOSS） |
| 帧率 | 移动端 60 FPS |
| 对象池 | 僵尸 / 子弹必 Pool 化 |

## 平台 / 发行（不可擅自变更）
- 移动 App（Android + iOS，锁横屏 Landscape）；**已放弃微信 / 抖音 / 团结小游戏**
- 休闲益智类目 · 全年龄英语学习 · 国内主市场 · 版号前灰区内测 → 版号后上市
- 纯广告变现（0 内购，不引入 `com.unity.purchasing`，不定义 `IPurchaseService`）

Phase 0-1 未引入：**Addressables、Cinemachine**（勿提前拉入，Phase 2 再评估）。
