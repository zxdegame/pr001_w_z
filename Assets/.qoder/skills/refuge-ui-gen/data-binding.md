# 数据绑定与视图表现（真源：Architecture §4.x 接口 / §5.1 EventBus / `QuizState`；数值取 SO）

## 1. 绑定纪律（违反即返工）
- **只绑接口 / provider / 只读属性**：view 不 `GetComponent` 具体业务类、不直改其字段；跨系统通信走 EventBus（Architecture §5.1）。
- **接口只读**：绑定与表现**不得修改** Architecture 接口签名；缺字段先走 RFC，绝不为“UI 要显示”往契约塞字段。
- **表现层元素归 view**：高亮环、面板→目标连线、头顶气泡、嘲讽文字、光圈色等**纯表现**，由 view 订阅事件 / 读取现有属性绘制，**不新增业务接口字段**（D15 明令“不新增 `IZombie` 字段”）。
- **冲突以真源为准**：UI.md 是下游、其绑定列可能用简写。例：耐久真源为 `IWall.CurrentDurability`（Architecture §4），UI.md 表写作 `IWall.Durability/Max`——**取 Architecture 名**。
- **数值不写死**：阈值 / 时限 / 价格 / 颜色档位一律取 `GameConfig` / `QuizConfig` 等 SO 或 Constants；下表所注数字均**示例**，以 SO / 实测为准。

## 2. 常见绑定源（HUD / 面板逐元素引用此表）
| 元素 | 绑定源（真源名） | 更新频率 | 触发 / 表现 |
|---|---|---|---|
| 能量墙耐久条 | `IWall.CurrentDurability`（+ Max） | 实时 | 受击事件；>50% 蓝 / 20-50% 橙呼吸 / ≤20% 红闪+警告音（示例阈值，以 SO 为准）；4 阶起分段破损纯表现 |
| 能量晶 | `PlayerData.energyCrystal` | 实时 | CountUpText 滚动；奖励结算事件刷新 |
| 波次指示 | `WaveController.Current/Total`（契约名以 Architecture 为准） | 每波 | “Wave N” 横幅 1s；瞭望台 ≥1 级提前 5s 预示下一波构成 |
| 连击数 | `ComboService.Count`（契约名以 Architecture 为准） | 实时 | 0=隐藏；≥3 显数字随档位变色（示例 3橙/5红/8绿/12紫/20金，对齐 GDD §8.5.1）；环形进度=窗口剩余 |
| Mastery Meter | `IMasteryMeterService.Percent` | 每题 | 0-100% 熟练度条；≥60% 亮 Swift 徽章 / ≥80% 再亮连击徽章（D21 M3，示例阈值，Phase 3） |
| 敌人头顶“上膛” | `Word.memoryStrength` ∈[0,1]（D21 M1，仅表现） | 生成 + 掌握度变化 | `?? 待解锁` 灰 / `上膛中 N%` 蓝 / `满膛 ✓` 金；光圈色=词源四类（复习绿/错题红/新学蓝/新词白） |
| 锁定目标高亮 | `IQuizTargetProvider.AcquireNextTarget()` | 出题期 | 高亮环 + 面板→目标连线；游走怪可点击锁定（不新增 `IZombie` 字段） |
| 题面 / 选项 | `IQuestionPresenter.ShowQuestion(WordQuestion)` | 每题 | 驱动 `QuizState`（见 §3） |
| 场景跳转 | `ISceneLoader.LoadAsync(sceneName, IProgress<float>)` | 事件 | 转场淡入淡出 0.3s + 进度条 |
| 年龄 / 家长验证 | `IAgeVerifyService.VerifyAsync()→UniTask<AgeVerifyResult>`（`IsVerified`/`IsMinor`/`HasParentalConsent`） | 首启 | Phase 0 `MockAgeVerifyService`（弹窗 + 算术题/长按二次确认）；`method`=CarrierOneClick/SmsCode/Mock |

## 3. 答题面板 `QuizState` 全态表现（真源：Architecture `QuizState`；时限 `QuizConfig` 默认 10s、可调 10-20s）
| 状态 | 面板表现 |
|---|---|
| IDLE | 面板收起，仅战场；无待答目标 |
| QuestionGenerated | 上滑入场 0.2s；显词面（`EnglishToChinese`=显英文选中文释义 / 反向）；倒计时起 |
| AnswerSelected | 4 选项锁定、所选高亮 0.15s；输入冻结（防双击双算） |
| AnswerResult | 对=绿框 + `叮` + 当帧暴击表现（D17）；错/超时=红框 + 正确项脉冲 + 目标嘲讽气泡“就这？就这！”1.5s + 连击清零 + 错题入本（红色光圈） |
| Cooldown | 面板收起，下一目标自动串接；目标已到墙则转点击锁定 |
| Timeout | 视同答错处理，不造成伤害 |

## 4. 反馈与红线
- **Swift**：≤3s 答对时连击位左侧闪 `SWIFT!` 角标（×1.15，纯表现）。
- **连击档位特效**：数字放大消散 + 档位特效（手雷 / 激光 / 榴弹 / 核弹，对齐 GDD §8.5.1）。
- **儿童友好**：玩家无受伤动作、无血腥（D13）；对错除颜色外附 `✓/✗` 形状 + 音效差异（色盲兜底）。
- **红线**：过关必需补给包只来自掉落，**无广告获取入口**（D16 / Compliance §3）；只渲染能量晶单货币，无金币 / 内购。
