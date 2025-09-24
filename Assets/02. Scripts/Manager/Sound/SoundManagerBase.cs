using UnityEngine;
using UnityEngine.Audio;

/// 공통 SFX 풀 + 한 개의 Mixer Group만 할당해서 사용
public abstract class SoundManagerBase<T> : Singleton<T> where T : MonoBehaviour
{
    [Header("Sound Pool")]
    [SerializeField, Range(1, 32)] private int poolSize = 20;
    private AudioSource[] _pool;
    private int _next;

    [Header("Output (1개만)")]
    [SerializeField] protected AudioMixerGroup outputGroup; // ← 각 매니저에서 SFX 또는 BGM 할당

    // ───────────────────────────────
    public void PlayOne(AudioClip clip, float volume = 1f)
    {
        if (!clip) return;
        var src = AcquireSource();
        if (!src) return;
        src.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    public void PlayRandom(SoundData data, float volume = 1f)
    {
        if (data == null || data.clips == null || data.clips.Length == 0) return;
        var clip = data.clips[Random.Range(0, data.clips.Length)];
        var src = AcquireSource();
        if (!src) return;
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
            int idx = (_next + i) % _pool.Length;
            var s = _pool[idx];
            if (!s.isPlaying)
            {
                _next = (idx + 1) % _pool.Length;
                return s;
            }
        }
        // 모두 재생중이면 라운드로빈
        var ret = _pool[_next];
        _next = (_next + 1) % _pool.Length;
        return ret;
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
        _next = 0;

        for (int i = 0; i < size; i++)
        {
            var go = new GameObject($"{typeof(T).Name}_SFX_{i:00}");
            go.transform.SetParent(transform, false);

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;                      // 2D
            src.outputAudioMixerGroup = outputGroup;    // ★ 한 줄로 라우팅 끝
            _pool[i] = src;
        }
    }
}
