using UnityEngine;

public class StructSoundManager : SoundManagerBase<StructSoundManager>
{
    [Header("Strcut SFX")]
    public SoundData cctvBrokenSound;
    public SoundData pickUpSound;


    [Range(0, 1)] public float structVol = 0.8f;

    public void PlayStructBrokenSound() => PlayRandom(cctvBrokenSound, structVol);
    public void PlayPickUpSound() => PlayRandom(pickUpSound, structVol);

}
