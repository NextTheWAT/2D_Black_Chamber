using UnityEngine;
using System.Collections;
using Constants; // GamePhase (��� ��)

public class BGMManager : SoundManagerBase<BGMManager>
{
    [Header("BGM (SoundData)")]
    public SoundData stealthBGM;   // ���Կ�
    public SoundData combatBGM;    // ������

    [SerializeField] float fadeTime = 0.8f;

    AudioSource _a, _b;
    bool _useA = true;
    Coroutine _fade;

    void Awake()
    {
        // A/B �ҽ� ���� (outputGroup�� �ν����Ϳ��� BGM �׷����� �Ҵ�Ǿ� �־�� ��)
        _a = CreateLooping("BGM_A");
        _b = CreateLooping("BGM_B");
    }

    void Start()
    {
        // �ٽ�: �������ڸ��� �⺻ BGM�� �� �� Ʋ���ش�.
        //    stealth�� ������ combat���� ����
        PlayData(stealthBGM ? stealthBGM : combatBGM, instant: true);
    }

    AudioSource CreateLooping(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var src = go.AddComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;
        src.volume = 0f;
        src.outputAudioMixerGroup = outputGroup; // BGM �׷��̾�� ��
        return src;
    }

    // �ܺο��� Phase �ٲ� �� ȣ���ϸ� �� (���� ���� API ����)
    public void SetPhase(GamePhase phase, bool instant = false)
    {
        var data = (phase == GamePhase.Combat) ? combatBGM : stealthBGM;
        PlayData(data, instant);
    }

    // ���� ��� ����
    void PlayData(SoundData data, bool instant)
    {
        var clip = ExtractRandomClip(data);
        if (!clip) return;

        var from = _useA ? _a : _b;
        var to = _useA ? _b : _a;
        _useA = !_useA;

        if (to.clip != clip) to.clip = clip;
        if (!to.isPlaying) to.Play();

        float target = Mathf.Clamp01(data.volume); // SoundData ���� ���� ���

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
        if (data == null) return null;
        var list = data.clips; // �迭 ������� ����
        if (list != null && list.Length > 0)
            return list[Random.Range(0, list.Length)];
        // ���� clip �ϳ��� ���� ������ ���⼭ data.clip ��ȯ�ϵ��� ����
        return null;
    }
}
