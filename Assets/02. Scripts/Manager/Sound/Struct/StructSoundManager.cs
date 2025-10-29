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

    [Header("Wall Attack SFX")]
    public SoundData wallAttackSound;

    [Header("Door Attack SFX")]
    public SoundData doorAttackSound;
    public SoundData steelDoorAttackSound;


    [Range(0, 1)] public float structVol = 0.8f;

    public void PlayStructBrokenSound(Vector2 pos) => PlaySFX(cctvBrokenSound, pos, structVol);
    public void PlayPickUpSound(Vector2 pos) => PlaySFX(pickUpSound, pos, structVol);
    public void PlayDoorOpenSound(Vector2 pos) => PlaySFX(doorOpenSound, pos, structVol);
    public void PlayDoorCloseSound(Vector2 pos) => PlaySFX(doorCloseSound, pos, structVol);
    public void PlaySteelDoorOpenSound(Vector2 pos) => PlaySFX(steelDoorOpenSound, pos, structVol);
    public void PlaySteelDoorCloseSound(Vector2 pos) => PlaySFX(steelDoorCloseSound, pos, structVol);

    public void PlayWallAttackSound(Vector2 pos) => PlaySFX(wallAttackSound, pos, structVol);
    public void PlayDoorAttackSound(Vector2 pos) => PlaySFX(doorAttackSound, pos, structVol);
    public void PlaySteelDoorAttackSound(Vector2 pos) => PlaySFX(steelDoorAttackSound, pos, structVol);



}
