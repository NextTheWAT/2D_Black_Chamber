using UnityEngine;

public class WeaponSoundManager : SoundManagerBase<WeaponSoundManager>
{
    [Header("Weapon SFX")]
    public SoundData pistolShootSound;
    public SoundData rifleShootSound;
    public SoundData reloadSound;
    public SoundData emptySound;

    [Range(0, 1)] public float weaponVol = 1.0f;

    public void PlayReloadSound() => PlayRandom(reloadSound, weaponVol);
    public void PlayPistolShootSound() => PlayRandom(pistolShootSound, weaponVol);
    public void PlayShootRifleSound() => PlayRandom(rifleShootSound, weaponVol);
    public void PlayEmptySound() => PlayRandom(emptySound, weaponVol);

}
