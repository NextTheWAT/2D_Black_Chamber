using UnityEngine;

public abstract class SoundManagerBase<T> : Singleton<T> where T : MonoBehaviour
{
    [Header("Sound Pool")]
    [SerializeField, Range(1, 32)] private int poolSize = 20;
    private AudioSource[] _pool;
    private int _next; // ����κ� �ε���

    // ������������������������������������������������������������������������������������������������������������������������������������������
    /// <summary>���� Ŭ�� ��� (2D ����, Ǯ ���)</summary>
    public void PlayOne(AudioClip clip, float volume = 1f)
    {
        if (!clip) return;
        var src = AcquireSource();
        if (!src) return;
        src.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    /// <summary>SoundData���� ���� ���� ��� (2D ����, Ǯ ���)</summary>
    public void PlayRandom(SoundData data, float volume = 1f)
    {
        if (data == null || data.clips == null || data.clips.Length == 0) return;
        int i = Random.Range(0, data.clips.Length);
        var clip = data.clips[i];
        if (!clip) return;

        float finalVol = Mathf.Clamp01(volume * data.volume);
        var src = AcquireSource();
        if (!src) return;
        src.PlayOneShot(clip, finalVol);
    }

    // ������������������������������������������������������������������������������������������������������������������������������������������
    // ����: Ǯ ����
    private AudioSource AcquireSource()
    {
        EnsurePool();
        if (_pool == null || _pool.Length == 0) return null;

        // 1) ������� �ƴ� �ҽ� �켱
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

        // 2) ��� ��� ���̸� ����κ����� �����
        var ret = _pool[_next];
        _next = (_next + 1) % _pool.Length;
        return ret;
    }

    private void EnsurePool()
    {
        if (_pool != null && _pool.Length == Mathf.Max(1, poolSize)) return;

        // ���� Ǯ ����(������ ���� ���)
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
