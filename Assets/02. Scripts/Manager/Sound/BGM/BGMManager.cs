using UnityEngine;
using System.Collections;
using Constants; // GamePhase

public class BGMManager : SoundManagerBase<BGMManager>
{
    [Header("BGM (SoundData)")]
    public SoundData titleBGM;     // Ÿ��Ʋ
    public SoundData lobbyBGM;     // �κ�
    public SoundData stealthBGM;   // ����: ����
    public SoundData combatBGM;    // ����: ����

    [SerializeField] float fadeTime = 0.8f;

    AudioSource _a, _b;
    bool _useA = true;
    Coroutine _fade;

    bool _inGameContext = false;   // UI ���°� Game�� ���� Phase �̺�Ʈ �ݿ�

    void Awake()
    {
        // A/B ���� ���̵�� �ҽ� ���� (outputGroup�� Inspector���� BGM �׷����� �Ҵ�)
        _a = CreateLooping("BGM_A");
        _b = CreateLooping("BGM_B");
    }

    void OnEnable()
    {
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged += OnPhaseChanged;
    }

    void OnDisable()
    {
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged -= OnPhaseChanged;
    }

    AudioSource CreateLooping(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var src = go.AddComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;     // 2D
        src.volume = 0f;           // ���̵�� �ø�
        src.outputAudioMixerGroup = outputGroup; // �ݵ�� BGM �׷�
        return src;
    }

    // �������������������������� UI ���ؽ�Ʈ ��ȯ ��������������������������
    public void SetUiContext(UIKey key, bool instant = false)
    {
        switch (key)
        {
            case UIKey.Title:
                _inGameContext = false;
                PlayData(titleBGM, instant);
                break;

            case UIKey.Lobby:
                _inGameContext = false;
                PlayData(lobbyBGM, instant);
                break;

            case UIKey.Game:
                _inGameContext = true;
                // ���� Phase�� ��� ����(�Ǵ� ���̵�)
                var gm = GameManager.Instance;
                var phase = gm ? gm.CurrentPhase : GamePhase.Stealth;
                SetPhase(phase, instant);
                break;

            default:
                // ���� �� �� UI ���´� �ϴ� Ÿ��Ʋ�� ����
                _inGameContext = false;
                PlayData(titleBGM, instant);
                break;
        }
    }

    // �������������������������� ���� Phase ��ȯ(����/����) ��������������������������
    void OnPhaseChanged(GamePhase phase)
    {
        if (_inGameContext) SetPhase(phase, instant: false);
    }

    public void SetPhase(GamePhase phase, bool instant = false)
    {
        var data = (phase == GamePhase.Combat) ? combatBGM : stealthBGM;
        PlayData(data, instant);
    }

    // �������������������������� ���� ��� ���� ��������������������������
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
            t += Time.unscaledDeltaTime;
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

    // �ʿ��ϸ� ��ư/�ִϸ��̼ǿ��� ���� ȣ���� ���� �޼���
    public void PlayTitle(bool instant = false) => SetUiContext(UIKey.Title, instant);
    public void PlayLobby(bool instant = false) => SetUiContext(UIKey.Lobby, instant);
    public void EnterGame(bool instant = false) => SetUiContext(UIKey.Game, instant);
}
