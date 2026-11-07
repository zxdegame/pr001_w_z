# UI 技术基线（真源：TechStack + UI.md 头部 / §17 / §18 / §19）

## 框架与画布
- uGUI + TextMeshPro（TMP）+ DOTween（UI 过渡 / 动效唯一方案，不新建动画系统）
- **Canvas 三层**：`Background`（场景背景 / 主视觉）→ `Gameplay`（HUD、答题面板，常驻战场相关）→ `Popup`（所有模态弹窗、全屏界面之上）。新界面按语义归层，塞错层会破坏遮挡顺序与合批
- 设计基准 **1920×1080**，`Canvas Scaler = ScaleWithScreenSize`，`MatchWidthOrHeight = 0.5`
- **锁定横屏**（D6 Landscape Left），不做竖屏分支

## 字体（真源 TechStack §5 / §5.1，勿引 UI.md 旧字）
| 用途 | 字体 | 备注 |
|---|---|---|
| 中文 | `ShelterHan` | 思源黑体 OFL 子集重命名 |
| 拉丁（英 / 数字） | `ShelterBaloo` | Baloo 2 OFL 子集重命名 |
| 符号 / 图标 | 自制 `ic_*` 精灵 | **禁系统 emoji 直显**（防豆腐块 + 版权） |

旧 Roboto Mono / DIN Alternate 已废弃；TMP 字体资产属 view 层资源，随包 `OFL.txt`（设置“开源许可”页承接）。

## 图集 / DrawCall
- 主 UI 图集、HUD、Popup 各按模块图集（每模块一张 ≤2048，Tight Packing）
- 界面打开时合批自检 **DrawCall ≤ 30**（Spine 骨骼合批属敌人表现，见 TechStack §2，非 UI 面板）

## 适配（真源 TechStack §17 · UI.md §18）
- 18:9 / 20:9 多出的宽度由战场背景延展；**UI 元素锚定不拉伸**，两侧留白由 Safe Area Handler 收拢
- 刘海 / Dynamic Island：顶栏元素内缩进 `SafeAreaView`（Architecture UI/Common）；能量晶等关键读数不入危险区

## 输入与无障碍（UI.md §17）
- 移动端：触屏（InputSystem UI Module）；**答题 4 键 ≥160×96px**，任意按钮最小命中 **96×96px @1080p**，间距 **≥24px** 防误触
- PC 开发预览键（1-4 答题 / 空格暂停 / ESC 返回）**仅 Editor 调试**，不对外宣传（B-05）
- 色盲兜底：耐久 / 连击除颜色外均有数字文本；对错附加 `✓/✗` 形状 + 音效差异（儿童友好、无血腥 D13）

## 动效参数表（UI.md §19，对齐 GDD §8.5.3）
| 场景 | 动效 | 参数 |
|---|---|---|
| 界面切换 | 淡入淡出 | 0.3s |
| 弹窗 | 缩放 + 淡入 | 0.2s，OutQuad |
| 按钮 | 按下缩放 | 0.95，回弹 0.1s |
| 答对暴击 | 飘字 + 命中闪光 + HitStun | 飘字池 50；0.3s |
| 连击档位 | 连击数字放大消散 + 档位特效 | 手雷 / 激光 / 榴弹 / 核弹 |
| Swift | 角标闪现 | ≤3s 答对闪 `SWIFT!`，×1.15 |
| 奖励 | RewardFly 入资源栏 | 0.5s / 项 |
| 升级 | 光环扩散 + 等级弹出 | 0.6s |
