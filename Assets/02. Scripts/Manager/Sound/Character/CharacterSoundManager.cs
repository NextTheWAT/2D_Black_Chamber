using UnityEngine;

public class CharacterSoundManager : SoundManagerBase<CharacterSoundManager>
{
    [Header("Character SFX")]
    public SoundData runstepSound;
    public SoundData walkstepSound;
    public SoundData dieSound;
    public SoundData hitSound;


    [Range(0, 1)] public float characterVol = 0.8f;

    public void PlayRunstepSound(Vector2 pos) => PlaySFX(runstepSound, pos, characterVol);
    public void PlayWalkstepSound(Vector2 pos) => PlaySFX(walkstepSound, pos, characterVol);
    public void PlayDieSound(Vector2 pos) => PlaySFX(dieSound, pos, characterVol);
    public void PlayHitSound(Vector2 pos) => PlaySFX(hitSound, pos, characterVol);
}
