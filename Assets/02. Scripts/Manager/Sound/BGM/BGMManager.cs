using UnityEngine;
using System.Collections;
using Constants; // GamePhase (없어도 됨)

public class BGMManager : SoundManagerBase<BGMManager>
{
    [Header("BGM (SoundData)")]
    public SoundData stealthBGM;   // 잠입용
    public SoundData combatBGM;    // 난전용

    [SerializeField] float fadeTime = 0.8f;

    AudioSource _a, _b;
    bool _useA = true;
    Coroutine _fade;

    void Awake()
    {
        // A/B 소스 생성 (outputGroup은 인스펙터에서 BGM 그룹으로 할당되어 있어야 함)
        _a = CreateLooping("BGM_A");
        _b = CreateLooping("BGM_B");
    }

    void Start()
    {
        // 핵심: 시작하자마자 기본 BGM을 한 번 틀어준다.
        //    stealth가 없으면 combat으로 폴백
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
        src.outputAudioMixerGroup = outputGroup; // BGM 그룹이어야 함
        return src;
    }

    // 외부에서 Phase 바꿀 때 호출하면 됨 (원래 쓰던 API 유지)
    public void SetPhase(GamePhase phase, bool instant = false)
    {
        var data = (phase == GamePhase.Combat) ? combatBGM : stealthBGM;
        PlayData(data, instant);
    }

    // 실제 재생 로직
    void PlayData(SoundData data, bool instant)
    {
        var clip = ExtractRandomClip(data);
        if (!clip) return;

        var from = _useA ? _a : _b;
        var to = _useA ? _b : _a;
        _useA = !_useA;

        if (to.clip != clip) to.clip = clip;
        if (!to.isPlaying) to.Play();

        float target = Mathf.Clamp01(data.volume); // SoundData 내부 볼륨 사용

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
        var list = data.clips; // 배열 구조라고 가정
        if (list != null && list.Length > 0)
            return list[Random.Range(0, list.Length)];
        // 단일 clip 하나만 쓰는 구조면 여기서 data.clip 반환하도록 변경
        return null;
    }
}
