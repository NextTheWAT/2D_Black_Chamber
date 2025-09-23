using UnityEngine;
using System.Collections;
using Constants; // GamePhase

public class BGMManager : SoundManagerBase<BGMManager>
{
    [Header("BGM (SoundData)")]
    public SoundData stealthBGM; // 잠입
    public SoundData combatBGM;  // 난전

    [SerializeField] float fadeTime = 0.8f;

    AudioSource _a, _b;   // 교차 페이드용
    bool _useA = true;
    Coroutine _fade;

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
    }

    void OnDestroy()
    {
        var gm = GameManager.Instance;
        if (gm != null) gm.OnPhaseChanged -= OnPhaseChanged;
    }

    AudioSource CreateLoopingSource(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var src = go.AddComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.volume = 0f;
        src.spatialBlend = 0f; // 2D BGM 권장
        return src;
    }

    void OnPhaseChanged(GamePhase phase) => SetPhase(phase);

    public void SetPhase(GamePhase phase, bool instant = false)
    {
        // 상태별 SoundData 선택
        var data = (phase == GamePhase.Combat) ? combatBGM : stealthBGM;
        var nextClip = ExtractRandomClip(data);
        if (nextClip == null) return;

        float targetVolume = Mathf.Clamp01(data.volume);

        var from = _useA ? _a : _b;
        var to = _useA ? _b : _a;
        _useA = !_useA;

        // 같은 클립이면 재생 유지하고 볼륨만 조정해도 됨
        if (to.clip != nextClip) to.clip = nextClip;
        if (!to.isPlaying) to.Play();

        if (_fade != null) StopCoroutine(_fade);

        if (instant)
        {
            to.volume = targetVolume;
            if (from.isPlaying) from.Stop();
        }
        else
        {
            // 시작 시점 볼륨 초기화
            to.volume = 0f;
            _fade = StartCoroutine(CrossFade(from, to, targetVolume));
        }
    }

    IEnumerator CrossFade(AudioSource from, AudioSource to, float targetVolume)
    {
        float t = 0f;
        float fromStart = from != null ? from.volume : 0f;
        float toStart = to.volume;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime; // 일시정지에도 부드럽게
            float k = Mathf.Clamp01(t / fadeTime);

            to.volume = Mathf.Lerp(toStart, targetVolume, k);
            from.volume = Mathf.Lerp(fromStart, 0f, k);
            yield return null;
        }

        to.volume = targetVolume;
        if (from != null && from.isPlaying) from.Stop();
    }

    // SoundData 내부에서 랜덤 곡 하나 뽑기
    AudioClip ExtractRandomClip(SoundData data)
    {
        if (data == null) return null;

        // 프로젝트의 SoundData 정의에 맞게 필드명만 확인
        var list = data.clips; // AudioClip[] 예상
        if (list != null && list.Length > 0)
            return list[Random.Range(0, list.Length)];

        // 단일 clip만 쓰는 구조라면 아래 주석 사용
        // return data.clip;

        return null;
    }
}
