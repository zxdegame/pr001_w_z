using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

/// <summary>
/// ISceneLoader 的 Phase 0-1 默认实现（基于 SceneManager.LoadSceneAsync）。
/// 真源：Architecture §4.7 / §17。
/// </summary>
public class SceneManagerSceneLoader : ISceneLoader
{
    /// <summary>异步加载场景并上报进度。</summary>
    public async UniTask LoadAsync(string sceneName, IProgress<float> progress = null)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        if (op == null) return;

        while (!op.isDone)
        {
            progress?.Report(op.progress);
            await UniTask.Yield();
        }
        progress?.Report(1f);
    }
}
