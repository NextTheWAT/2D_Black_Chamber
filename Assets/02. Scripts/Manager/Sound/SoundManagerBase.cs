using UnityEngine;

public abstract class SoundManagerBase<T> : Singleton<T> where T : MonoBehaviour
{
    [Header("Sound Pool")]
    [SerializeField, Range(1, 32)] private int poolSize = 20;
    private AudioSource[] _pool;
    private int _next; // 라운드로빈 인덱스

    // ─────────────────────────────────────────────────────────────────────
    /// <summary>단일 클립 재생 (2D 원샷, 풀 사용)</summary>
    public void PlayOne(AudioClip clip, float volume = 1f)
    {
        if (!clip) return;
        var src = AcquireSource();
        if (!src) return;

        // 최종 볼륨 계산: 호출자가 준 volume × SFX 마스터
        float finalVol = Mathf.Clamp01(volume) * VolumeSettings.Sfx;
        src.PlayOneShot(clip, finalVol);
    }

    public void PlayRandom(SoundData data, float volume = 1f)
    {
        if (data == null || data.clips == null || data.clips.Length == 0) return;
        int i = Random.Range(0, data.clips.Length);
        var clip = data.clips[i];
        if (!clip) return;

        // ⬇호출자 volume × SoundData.volume × SFX 마스터
        float finalVol = Mathf.Clamp01(volume * data.volume) * VolumeSettings.Sfx;
        var src = AcquireSource();
        if (!src) return;
        src.PlayOneShot(clip, finalVol);
    }

    // ─────────────────────────────────────────────────────────────────────
    // 내부: 풀 관리
    private AudioSource AcquireSource()
    {
        EnsurePool();
        if (_pool == null || _pool.Length == 0) return null;

        // 1) 재생중이 아닌 소스 우선
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

        // 2) 모두 재생 중이면 라운드로빈으로 덮어쓰기
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
            {
                if (_pool[i] != null) Destroy(_pool[i].gameObject);
            }
        }

        int size = Mathf.Max(1, poolSize);
        _pool = new AudioSource[size];

        for (int i = 0; i < size; i++)
        {
            var go = new GameObject($"{typeof(T).Name}_SFX_{i:00}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();

            src.playOnAwake = false;
            src.loop = false;

            _pool[i] = src;
        }

        _next = 0;
    }
}
