// MixerVolumeUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MixerVolumeUI : MonoBehaviour
{
    [Header("MainMixer 할당")]
    public AudioMixer mixer;

    [Header("슬라이더 (0~1)")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("Expose한 파라미터명")]
    public string masterParam = "MasterVolume";
    public string bgmParam = "BGMVolume";
    public string sfxParam = "SFXVolume";

    void Start()
    {
        // 초기값 동기화
        Setup(masterSlider, masterParam);
        Setup(bgmSlider, bgmParam);
        Setup(sfxSlider, sfxParam);
    }

    void Setup(Slider slider, string param)
    {
        if (!slider || mixer == null) return;

        if (mixer.GetFloat(param, out float db))
            slider.value = DbTo01(db);             // Mixer 값 → 0~1

        slider.onValueChanged.AddListener(v =>
        {
            mixer.SetFloat(param, ToDb(v));        // 0~1 → dB
        });
    }

    static float ToDb(float v01)
    {
        // 0 → -80dB(무음), 1 → 0dB
        return (v01 <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp01(v01)) * 20f;
    }

    static float DbTo01(float db)
    {
        return Mathf.Pow(10f, db / 20f);
    }
}
