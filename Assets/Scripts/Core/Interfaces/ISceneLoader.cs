using System;
using Cysharp.Threading.Tasks;

/// <summary>
/// 场景加载抽象层：Phase 0-1 默认 SceneManager 实现，Phase 2+ 可切 Addressables 实现，上层代码不受实现影响。
/// 真源：Architecture §4.7。
/// </summary>
public interface ISceneLoader
{
    /// <summary>异步加载场景，可上报进度。</summary>
    UniTask LoadAsync(string sceneName, IProgress<float> progress = null);
}
