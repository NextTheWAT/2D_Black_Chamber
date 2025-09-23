using UnityEngine;
using System.Collections;
using Constants; // GamePhase

public class BGMManager : SoundManagerBase<BGMManager>
{
    [Header("BGM (SoundData)")]
    public SoundData stealthBGM; // ����
    public SoundData combatBGM;  // ����

    [SerializeField] float fadeTime = 0.8f;

    AudioSource _a, _b;   // ���� ���̵��
    bool _useA = true;
    Coroutine _fade;

    // ���� �⺻ ����(SoundData.volume)�� ����
    float _phaseBaseVolume = 1f;
    // ���� Ÿ��(= base �� master)�� ������ ������ ���� �� ���� ����
    float _lastTargetVolume = 1f;

    protected override void Initialize()
    {
        base.Initialize();

        _a = CreateLoopingSource("BGM_A");
        _b = CreateLoopingSource("BGM_B");

        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.OnPhaseChanged += OnPhaseChanged;
            SetPhase(gm.CurrentPhase, instant: true);
        }

        // BGM ������ ���� ���濡 �ǽð� ����
        VolumeSettings.OnBgmChanged += ApplyMasterBgm;
    }

    void OnDestroy()
    {
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged -= OnPhaseChanged;

        VolumeSettings.OnBgmChanged -= ApplyMasterBgm;
    }

    AudioSource CreateLoopingSource(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var src = go.AddComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.volume = 0f;
        src.spatialBlend = 0f; // 2D BGM ����
        return src;
    }

    void OnPhaseChanged(GamePhase phase) => SetPhase(phase);

    public void SetPhase(GamePhase phase, bool instant = false)
    {
        // ���º� SoundData ����
        var data = (phase == GamePhase.Combat) ? combatBGM : stealthBGM;
        var nextClip = ExtractRandomClip(data);
        if (nextClip == null) return;

        _phaseBaseVolume = Mathf.Clamp01(data.volume);
        float targetVolume = _phaseBaseVolume * VolumeSettings.Bgm;

        var from = _useA ? _a : _b;
        var to = _useA ? _b : _a;
        _useA = !_useA;

        // ���� Ŭ���̸� ��� �����ϰ� ������ �����ص� OK
        if (to.clip != nextClip) to.clip = nextClip;
        if (!to.isPlaying) to.Play();

        if (_fade != null) StopCoroutine(_fade);

        if (instant)
        {
            to.volume = targetVolume;
            if (from.isPlaying) from.Stop();
            _lastTargetVolume = targetVolume;
        }
        else
        {
            // ���� ���� ���� �ʱ�ȭ
            float toStart = 0f;
            to.volume = toStart;
            _fade = StartCoroutine(CrossFade(from, to, toStart, targetVolume));
        }
    }

    IEnumerator CrossFade(AudioSource from, AudioSource to, float toStart, float targetVolume)
    {
        float t = 0f;
        float fromStart = from ? from.volume : 0f;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime; // �Ͻ��������� �ε巴��
            float k = Mathf.Clamp01(t / fadeTime);

            to.volume = Mathf.Lerp(toStart, targetVolume, k);
            from.volume = Mathf.Lerp(fromStart, 0f, k);
            yield return null;
        }

        to.volume = targetVolume;
        if (from && from.isPlaying) from.Stop();

        _lastTargetVolume = targetVolume;
    }

    // ������(BGM) ���� ��, ���� ������ ���� �����ϸ� ������
    void ApplyMasterBgm(float master)
    {
        float newTarget = _phaseBaseVolume * Mathf.Clamp01(master);

        if (_a && _a.isPlaying)
        {
            float ratio = (_lastTargetVolume > 0f) ? (_a.volume / _lastTargetVolume) : 0f;
            _a.volume = newTarget * ratio;
        }
        if (_b && _b.isPlaying)
        {
            float ratio = (_lastTargetVolume > 0f) ? (_b.volume / _lastTargetVolume) : 0f;
            _b.volume = newTarget * ratio;
        }
        _lastTargetVolume = newTarget;
    }

    // SoundData ���ο��� ���� �� �ϳ� �̱�
    AudioClip ExtractRandomClip(SoundData data)
    {
        if (data == null) return null;

        var list = data.clips; // AudioClip[] ���� (������Ʈ ���ǿ� �°� �ʵ�� Ȯ��)
        if (list != null && list.Length > 0)
            return list[Random.Range(0, list.Length)];

        // ���� clip�� ���� �������:
        // return data.clip;

        return null;
    }
}
