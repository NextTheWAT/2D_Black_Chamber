using UnityEngine;
using UnityEngine.Audio;

/// 공통 SFX 풀 + 한 개의 Mixer Group만 할당해서 사용
public abstract class SoundManagerBase<T> : Singleton<T> where T : MonoBehaviour
{
    [Header("Sound Pool")]
    [SerializeField, Range(1, 32)] private int poolSize = 20;
    private AudioSource[] _pool;

    [Header("Audio Source Settings")]
    [SerializeField] private float spatialBlend = 0f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 5f;

    [Header("Output (1개만)")]
    [SerializeField] protected AudioMixerGroup outputGroup; // ← 각 매니저에서 SFX 또는 BGM 할당

    // ───────────────────────────────

    public void PlayBGM(SoundData data, float volume = 1f)
    {
        if (data == null || data.clips == null || data.clips.Length == 0) return;
        var clip = data.clips[Random.Range(0, data.clips.Length)];
        var src = AcquireSource();
        if (!src) return;
        src.PlayOneShot(clip, Mathf.Clamp01(volume * Mathf.Clamp01(data.volume)));
    }


    public void PlaySFX(SoundData data, Vector2 pos, float volume = 1f)
    {
        if (data == null || data.clips == null || data.clips.Length == 0) return;
        var clip = data.clips[Random.Range(0, data.clips.Length)];
        var src = AcquireSource();
        if (!src) return;
        src.transform.position = pos;
        src.PlayOneShot(clip, Mathf.Clamp01(volume * Mathf.Clamp01(data.volume)));
    }

    // ───────────────────────────────
    private AudioSource AcquireSource()
    {
        EnsurePool();
        if (_pool == null || _pool.Length == 0) return null;

        // 재생중 아닌 소스 우선
        for (int i = 0; i < _pool.Length; i++)
        {
            var s = _pool[i];
            if (!s.isPlaying)
                return s;
        }

        // 모두 재생중이면 null 반환
        return null;
    }

    private void EnsurePool()
    {
        if (_pool != null && _pool.Length == Mathf.Max(1, poolSize)) return;

        // 기존 풀 제거(사이즈 변경 대비)
        if (_pool != null)
        {
            for (int i = 0; i < _pool.Length; i++)
                if (_pool[i] != null) Destroy(_pool[i].gameObject);
        }

        int size = Mathf.Max(1, poolSize);
        _pool = new AudioSource[size];

        for (int i = 0; i < size; i++)
        {
            var go = new GameObject($"{typeof(T).Name}_SFX_{i:00}");
            go.transform.SetParent(transform, false);

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = spatialBlend;
            src.outputAudioMixerGroup = outputGroup;    // ★ 한 줄로 라우팅 끝
            src.minDistance = minDistance;
            src.maxDistance = maxDistance;
            _pool[i] = src;
        }
    }
}
