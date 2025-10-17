using UnityEngine;

public class StructSoundManager : SoundManagerBase<StructSoundManager>
{
    [Header("Strcut SFX")]
    public SoundData cctvBrokenSound;
    public SoundData pickUpSound;

    [Header("Door SFX")]
    public SoundData doorOpenSound;
    public SoundData doorCloseSound;
    public SoundData steelDoorOpenSound;
    public SoundData steelDoorCloseSound;


    [Range(0, 1)] public float structVol = 0.8f;

    public void PlayStructBrokenSound() => PlayRandom(cctvBrokenSound, structVol);
    public void PlayPickUpSound() => PlayRandom(pickUpSound, structVol);
    public void PlayDoorOpenSound() => PlayRandom(doorOpenSound, structVol);
    public void PlayDoorCloseSound() => PlayRandom(doorCloseSound, structVol);
    public void PlaySteelDoorOpenSound() => PlayRandom(steelDoorOpenSound, structVol);
    public void PlaySteelDoorCloseSound() => PlayRandom(steelDoorCloseSound, structVol);


}
