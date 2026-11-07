using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 轻量序列帧切换组件（仅特效/嘲讽气泡/UI 复用，非角色骨骼动画；角色动画走 Spine2D）。
/// 真源：Architecture §15.1。属基础设施、非玩法，故 Phase 0 给最小可用实现。
/// </summary>
public class SpriteAnimationPlayer : MonoBehaviour
{
    [SerializeField] Sprite[] frames;
    [SerializeField] float fps = 12f;
    [SerializeField] bool loop = true;

    SpriteRenderer targetRenderer;
    Coroutine current;

    /// <summary>动画播放完成（非循环播放结束时触发）。</summary>
    public event Action OnComplete;

    private void Awake()
    {
        targetRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>从首帧开始播放序列帧。</summary>
    public void Play()
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(PlayRoutine());
    }

    /// <summary>停止播放。</summary>
    public void Stop()
    {
        if (current != null)
        {
            StopCoroutine(current);
            current = null;
        }
    }

    private IEnumerator PlayRoutine()
    {
        if (frames == null || frames.Length == 0) yield break;
        float interval = fps > 0f ? 1f / fps : 0f;
        do
        {
            for (int i = 0; i < frames.Length; i++)
            {
                if (targetRenderer != null) targetRenderer.sprite = frames[i];
                yield return new WaitForSeconds(interval);
            }
        } while (loop);

        current = null;
        OnComplete?.Invoke();
    }
}
