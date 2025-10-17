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

    public void PlayStructBrokenSound(Vector2 pos) => PlaySFX(cctvBrokenSound, pos, structVol);
    public void PlayPickUpSound(Vector2 pos) => PlaySFX(pickUpSound, pos, structVol);
    public void PlayDoorOpenSound(Vector2 pos) => PlaySFX(doorOpenSound, pos, structVol);
    public void PlayDoorCloseSound(Vector2 pos) => PlaySFX(doorCloseSound, pos, structVol);
    public void PlaySteelDoorOpenSound(Vector2 pos) => PlaySFX(steelDoorOpenSound, pos, structVol);
    public void PlaySteelDoorCloseSound(Vector2 pos) => PlaySFX(steelDoorCloseSound, pos, structVol);


}
