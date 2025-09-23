using UnityEngine;
using System.Collections;
using Constants; // GamePhase

// SFX �Ŵ������ �� ����
public class BGMManager : SoundManagerBase<BGMManager>
{
    [Header("BGM Clips")]
    public AudioClip[] stealthClips; // ���� ���¿�
    public AudioClip[] combatClips;  // ���� ���¿�

    [Range(0, 1)] public float bgmVolume = 0.6f;
    [SerializeField] float fadeTime = 0.8f;

    AudioSource _a, _b;
    bool _useA = true;
    Coroutine _fade;

    protected override void Initialize()
    {
        base.Initialize();

        // ���ο� BGM �ҽ� 2��(���� ���̵�)
        _a = CreateSource("BGM_A");
        _b = CreateSource("BGM_B");

        // ���� 1ȸ ���� ����ȭ + ���� ��ȭ ����
        var gm = GameManager.Instance; // �̱���
        if (gm != null)
        {
            gm.OnPhaseChanged += OnPhaseChanged;           // ���°� �ٲ� ������ �ڵ� ��ȯ
            SetPhase(gm.CurrentPhase, instant: true);      // ���� ���·� ��� ��� ����
        }
    }

    AudioSource CreateSource(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var src = go.AddComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.volume = 0f;
        return src;
    }

    void OnPhaseChanged(GamePhase phase) => SetPhase(phase);

    public void SetPhase(GamePhase phase, bool instant = false)
    {
        var nextClip = PickClip(phase == GamePhase.Combat ? combatClips : stealthClips);
        if (nextClip == null) return;

        var from = _useA ? _a : _b;
        var to = _useA ? _b : _a;
        _useA = !_useA;

        to.clip = nextClip;
        to.volume = instant ? bgmVolume : 0f;
        if (!to.isPlaying) to.Play();

        if (_fade != null) StopCoroutine(_fade);
        _fade = instant ? null : StartCoroutine(CrossFade(from, to));
        if (instant && from.isPlaying) from.Stop();
    }

    IEnumerator CrossFade(AudioSource from, AudioSource to)
    {
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime; // �Ͻ��������� �ڿ������� ���̵�
            float k = Mathf.Clamp01(t / fadeTime);
            to.volume = Mathf.Lerp(0f, bgmVolume, k);
            from.volume = Mathf.Lerp(bgmVolume, 0f, k);
            yield return null;
        }
        to.volume = bgmVolume;
        if (from.isPlaying) from.Stop();
    }

    AudioClip PickClip(AudioClip[] list)
    {
        if (list == null || list.Length == 0) return null;
        return list[Random.Range(0, list.Length)];
    }
}
