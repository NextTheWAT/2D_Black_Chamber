using UnityEngine;

public class StructSoundManager : SoundManagerBase<StructSoundManager>
{
    [Header("Strcut SFX")]
    public SoundData cctvBrokenSound;


    [Range(0, 1)] public float structVol = 0.8f;

    public void PlayStructBrokenSound() => PlayRandom(cctvBrokenSound, structVol);

}
