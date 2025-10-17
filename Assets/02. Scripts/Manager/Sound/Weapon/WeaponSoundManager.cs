using UnityEngine;

public class WeaponSoundManager : SoundManagerBase<WeaponSoundManager>
{
    [Header("Weapon SFX")]
    public SoundData pistolShootSound;
    public SoundData rifleShootSound;
    public SoundData reloadSound;
    public SoundData emptySound;

    [Range(0, 1)] public float weaponVol = 1.0f;

    public void PlayReloadSound(Vector2 pos) => PlaySFX(reloadSound, pos, weaponVol);
    public void PlayPistolShootSound(Vector2 pos) => PlaySFX(pistolShootSound, pos, weaponVol);
    public void PlayRifleShootSound(Vector2 pos) => PlaySFX(rifleShootSound, pos, weaponVol);
    public void PlayEmptySound(Vector2 pos) => PlaySFX(emptySound, pos, weaponVol);

}
