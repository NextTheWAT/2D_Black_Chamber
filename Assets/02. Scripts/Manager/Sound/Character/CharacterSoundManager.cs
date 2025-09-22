using UnityEngine;

public class CharacterSoundManager : SoundManagerBase<CharacterSoundManager>
{
    [Header("Character SFX")]
    public SoundData runstepSound;
    public SoundData walkstepSound;
    public SoundData dieSound;
    public SoundData hitSound;


    [Range(0, 1)] public float characterVol = 0.8f;

    public void PlayRunstepSound() => PlayRandom(runstepSound, characterVol);
    public void PlayWalkstepSound() => PlayRandom(walkstepSound, characterVol);
    public void PlayDieSound() => PlayRandom(dieSound, characterVol);
    public void PlayHitSound() => PlayRandom(hitSound, characterVol);
}
