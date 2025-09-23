using System;
using UnityEngine;

public static class VolumeSettings
{
    public static event Action<float> OnBgmChanged;
    public static event Action<float> OnSfxChanged;

    const string KeyBgm = "vol_bgm";
    const string KeySfx = "vol_sfx";

    static float _bgm = -1f;
    static float _sfx = -1f;

    public static float Bgm
    {
        get
        {
            if (_bgm < 0f) _bgm = PlayerPrefs.GetFloat(KeyBgm, 0.8f);
            return _bgm;
        }
    }

    public static float Sfx
    {
        get
        {
            if (_sfx < 0f) _sfx = PlayerPrefs.GetFloat(KeySfx, 1.0f);
            return _sfx;
        }
    }

    public static void SetBgm(float v)
    {
        v = Mathf.Clamp01(v);
        if (Mathf.Approximately(_bgm, v)) return;
        _bgm = v;
        PlayerPrefs.SetFloat(KeyBgm, _bgm);
        OnBgmChanged?.Invoke(_bgm);
    }

    public static void SetSfx(float v)
    {
        v = Mathf.Clamp01(v);
        if (Mathf.Approximately(_sfx, v)) return;
        _sfx = v;
        PlayerPrefs.SetFloat(KeySfx, _sfx);
        OnSfxChanged?.Invoke(_sfx);
    }
}
