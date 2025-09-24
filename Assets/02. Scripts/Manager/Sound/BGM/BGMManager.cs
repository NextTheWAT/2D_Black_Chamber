using UnityEngine;
using System.Collections;
using Constants; // GamePhase

public class BGMManager : SoundManagerBase<BGMManager>
{
    [Header("BGM (SoundData)")]
    public SoundData stealthBGM;   // ����
    public SoundData combatBGM;    // ����

    [SerializeField] float fadeTime = 0.8f;

    AudioSource _a, _b;
    bool _useA = true;
    Coroutine _fade;

    void Awake()
    {
        // A/B �ҽ� ���� (Inspector���� outputGroup = MainMixer/BGM �Ҵ� �ʿ�)
        _a = CreateLooping("BGM_A");
        _b = CreateLooping("BGM_B");
    }

    void OnEnable()
    {
        // GameManager ���� ���� �̺�Ʈ ����
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged += OnPhaseChanged;
    }

    void Start()
    {
        // ���� �� ���� ���·� ��� ���(������ ��������)
        var gm = GameManager.Instance;
        SetPhase(gm ? gm.CurrentPhase : GamePhase.Stealth, instant: true);
    }

    void OnDisable()
    {
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged -= OnPhaseChanged;
    }

    void OnPhaseChanged(GamePhase phase) => SetPhase(phase, instant: false);

    AudioSource CreateLooping(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);

        var src = go.AddComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;   // 2D
        src.volume = 0f;         // ���̵�� �ø�
        src.outputAudioMixerGroup = outputGroup; // �ݵ�� BGM �׷�
        return src;
    }

    public void SetPhase(GamePhase phase, bool instant = false)
    {
        var data = (phase == GamePhase.Combat) ? combatBGM : stealthBGM;
        PlayData(data, instant);
    }

    void PlayData(SoundData data, bool instant)
    {
        var clip = ExtractRandomClip(data);
        if (!clip) return;

        var from = _useA ? _a : _b;
        var to = _useA ? _b : _a;
        _useA = !_useA;

        if (to.clip != clip) to.clip = clip;
        if (!to.isPlaying) to.Play();

        float target = Mathf.Clamp01(data.volume);

        if (instant)
        {
            if (from) { from.volume = 0f; if (from.isPlaying) from.Stop(); }
            to.volume = target;
            return;
        }

        if (_fade != null) StopCoroutine(_fade);
        _fade = StartCoroutine(CrossFade(from, to, target));
    }

    IEnumerator CrossFade(AudioSource from, AudioSource to, float target)
    {
        float t = 0f;
        float fromStart = from ? from.volume : 0f;
        float toStart = to.volume;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;   // �Ͻ����� �߿��� ���̵� ����
            float k = Mathf.Clamp01(t / fadeTime);
            if (from) from.volume = Mathf.Lerp(fromStart, 0f, k);
            to.volume = Mathf.Lerp(toStart, target, k);
            yield return null;
        }
        if (from && from.isPlaying) from.Stop();
        to.volume = target;
        _fade = null;
    }

    AudioClip ExtractRandomClip(SoundData data)
    {
        if (!data || data.clips == null || data.clips.Length == 0) return null;
        return data.clips[Random.Range(0, data.clips.Length)];
    }

    // �ʿ��ϸ� �ִϸ��̼� �̺�Ʈ/��ư���� ���� ȣ��
    public void EnterCombat() => SetPhase(GamePhase.Combat);
    public void EnterStealth() => SetPhase(GamePhase.Stealth);
}
