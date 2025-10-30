using UnityEngine;

public class UISoundManager : SoundManagerBase<StructSoundManager>
{
    [Header("UI SFX")]
    public SoundData uiButtonClickSound;


    [Range(0, 1)] public float uiVol = 0.5f;

    public void PlayButtonClickSound(Vector2 pos) => PlaySFX(uiButtonClickSound, pos, uiVol);

}
