# 通用组件库 + 界面全集（真源：UI.md §16 / §1 / §2）

## 设计原则（⑩ UI 简洁通用化）
**能复用不新建**：优先用下列通用件拼装界面；新增专项件须先证明无可复用项；同一交互模式（弹窗 / 确认 / 飞入 / 计数）全站统一；视觉克制、动效时长与颜色层级统一。

## 通用组件库（UI.md §16）
| 组件 | 规格要点 | 复用位 |
|---|---|---|
| `UIBtnPrimary/Secondary/Danger` | 圆角矩形 + 边缘发光；normal 蓝 / 按下缩放 0.95 / disabled 灰 50% | 全部 |
| `PopupRoot` | 黑底 60% + 中央卡（进 0.8→1 缩放 + 0.2s 淡入）；右上关闭 × | 03/04/08/09/12/13/15 |
| `ConfirmDialog` | 文案 + [确认][取消]；破坏性操作（重开 / 退关）必过 | 09/11 |
| `Toast` | 底部居中 1.5s 自消；同类合并 | 全部 |
| `RewardFly` | 奖励飞入资源栏；结算 / 商店 / 抽包共用 | 06/10/15/17 |
| `CountUpText` | 数字滚动 0.4s | 顶栏晶 / 结算 |
| `StarRating` | 3 星逐颗点亮 0.4s / 颗 + 音效 | 02/10 |
| `WordCard` | 词汇七项信息模板（GDD §7.3.2） | 14 / 10 错题回顾 |
| `TabBar / GridSelector` | 词库五级漏斗、档案室页签 | 05/14 |

## 界面全集 00–17（UI.md §1.2 · 场景 / 形态 / Phase）
| # | 界面 | 形态 | 场景 | 首个 Phase |
|---|---|---|---|---|
| 00 | 首启年龄 / 家长验证 | 全屏模态阻塞 | Bootstrap（入 MainMenu 前） | P1 |
| 01 | 主界面 | 全屏 | MainMenu | P1 |
| 02 | 关卡选择 | 全屏 | LevelSelect | P1 |
| 03 | 关卡详情 | Popup | LevelSelect | P1 |
| 04 | Doctrine 三选一 | Popup | LevelSelect→Battle | P3 |
| 05 | 词库选择 | 全屏模态 | WordBankSelect | P1 |
| 06 | 战斗 HUD | Gameplay Canvas | Battle | P1（D21 行 P3） |
| 07 | 答题面板 | Gameplay Canvas | Battle | P1 |
| 08 | 补给包三选一 | Popup | Battle | P2 |
| 09 | 暂停面板 | Popup | Battle | P1 |
| 10 | 战斗结算 | 全屏 | Result | P1 |
| 11 | 避难所 | 全屏 | Shelter | P2 |
| 12 | 建筑详情 / 升级 | Popup | Shelter | P2 |
| 13 | 武器升级 | Popup | Shelter | P2 |
| 14 | 词汇档案室 | Popup 组 | MainMenu | P2+ |
| 15 | 商店 | Popup | MainMenu | P2 |
| 16 | 设置 | 全屏模态 | Settings | P1 |
| 17 | 本地纪录 + 轻成就墙 | Popup | MainMenu | P3 |

未列独立场景的（档案室 / 商店 / 纪录）= MainMenu 场景内 Popup 面板（`UI_Archive.panel` / `UI_Shop.panel` / `UI_Record.panel`）。场景真源见 Architecture §10。

## 导航流（UI.md §2）
`Bootstrap → AgeVerify(00 未验证阻塞) → MainMenu(01) → LevelSelect(02) →（详情03 → Doctrine04? → 词库05）→ Battle(06/07) → Result(10)`；主界面另设词库 / 避难所 / 档案室 / 商店 / 纪录 / 设置快捷入口。
- 返回键策略：Battle=暂停；Popup=关弹窗；全屏=回上一场景；MainMenu=双击退出（Toast“再按一次退出”）。
- 场景转场：全幅淡入淡出 0.3s。

## 元素表规范（逐界面产出必用此列）
`元素 | 控件 | 数据绑定（接口 / 服务 / PlayerData）| 交互 | 状态 / 阈值表现`；HUD 类另加 **更新频率**（实时 / 每题 / 每波 / 生成时）。
