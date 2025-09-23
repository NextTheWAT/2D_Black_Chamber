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

    // 상태 기본 볼륨(SoundData.volume)을 저장
    float _phaseBaseVolume = 1f;
    // 직전 타겟(= base × master)을 저장해 마스터 변경 시 비율 보존
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

        // BGM 마스터 볼륨 변경에 실시간 반응
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

        _phaseBaseVolume = Mathf.Clamp01(data.volume);
        float targetVolume = _phaseBaseVolume * VolumeSettings.Bgm;

        var from = _useA ? _a : _b;
        var to = _useA ? _b : _a;
        _useA = !_useA;

        // 같은 클립이면 재생 유지하고 볼륨만 조정해도 OK
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
            // 시작 시점 볼륨 초기화
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
            t += Time.unscaledDeltaTime; // 일시정지에도 부드럽게
            float k = Mathf.Clamp01(t / fadeTime);

            to.volume = Mathf.Lerp(toStart, targetVolume, k);
            from.volume = Mathf.Lerp(fromStart, 0f, k);
            yield return null;
        }

        to.volume = targetVolume;
        if (from && from.isPlaying) from.Stop();

        _lastTargetVolume = targetVolume;
    }

    // 마스터(BGM) 변경 시, 현재 볼륨을 비율 유지하며 스케일
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

    // SoundData 내부에서 랜덤 곡 하나 뽑기
    AudioClip ExtractRandomClip(SoundData data)
    {
        if (data == null) return null;

        var list = data.clips; // AudioClip[] 예상 (프로젝트 정의에 맞게 필드명만 확인)
        if (list != null && list.Length > 0)
            return list[Random.Range(0, list.Length)];

        // 단일 clip만 쓰는 구조라면:
        // return data.clip;

        return null;
    }
}
