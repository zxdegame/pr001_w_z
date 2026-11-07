using System;
using NUnit.Framework;

/// <summary>
/// Phase 0 契约编译/存在性测试（真源 Architecture §17）。
/// 验证 §17 接口类型存在、Mock/NoOp 骨架可实例化并满足接口，以及 PurchaseCurrency 反射断言（GDD D1）。
/// </summary>
public class InterfacesCompileTests
{
    [Test]
    public void All_Phase0_Interfaces_Are_Present()
    {
        Type[] interfaces =
        {
            typeof(IZombie), typeof(IQuizTargetProvider), typeof(IWordProvider),
            typeof(IWordLibraryService), typeof(IQuestionPresenter), typeof(IWall),
            typeof(IRewardCalculator), typeof(ISceneLoader), typeof(IAdsService),
            typeof(IAnalyticsService), typeof(IStoreService), typeof(IWordStudyService),
            typeof(IAgeVerifyService), typeof(IPlatformService), typeof(IMasteryMeterService)
        };

        foreach (var t in interfaces)
        {
            Assert.IsTrue(t.IsInterface, $"{t.Name} 应为接口");
        }
    }

    [Test]
    public void Phase0_Skeletons_Satisfy_Interfaces()
    {
        ISceneLoader sceneLoader = new SceneManagerSceneLoader();
        IAdsService ads = new MockAdsService();
        IAgeVerifyService age = new MockAgeVerifyService();
        IPlatformService platform = new MockPlatformService();
        IWordStudyService study = new NoOpWordStudyService();

        Assert.IsNotNull(sceneLoader);
        Assert.IsNotNull(ads);
        Assert.IsNotNull(age);
        Assert.IsNotNull(platform);
        Assert.IsNotNull(study);
    }

    [Test]
    public void PurchaseCurrency_Has_Exactly_One_Value()
    {
        var names = Enum.GetNames(typeof(PurchaseCurrency));

        Assert.AreEqual(1, names.Length, "PurchaseCurrency 只允许 EnergyCrystal（禁止 IAP/RealMoney，GDD D1）");
        Assert.AreEqual("EnergyCrystal", names[0]);
    }
}
