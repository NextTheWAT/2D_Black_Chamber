using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;


    [Header("업적 UI이미지")]
    public List<Image> achievementsImages;

    private List<bool> isUnlocked;
    private int deathCount = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        isUnlocked = new List<bool>();  // 업적 bool 값
        for (int i = 0; i < achievementsImages.Count; i++)
            isUnlocked.Add(false);
    }

    public void UnlockAchievement(int index)
    {
        if (achievementsImages == null || index < 0 || index >= achievementsImages.Count)
            return;

        if (isUnlocked[index])    // 한번해금되면 또해금안하게
            return;

        isUnlocked[index] = true;   // 업적 갱신

        achievementsImages[index].color = Color.yellow;
    }

    public bool IsUnlocked(int index)       // 외부에서 해금 함수 호출되게
    {
        if (index < 0 || index >= isUnlocked.Count)
            return false;
        return isUnlocked[index];
    }

    public void HasAllWeapon()  // 7번 모든무기 해금 업적
    {
        var inventory = WeaponInventory.Instance;
        if (inventory == null || inventory.catalog == null )
            return;

        var allWeapons = inventory.catalog.All;
        if (allWeapons == null)
            return;

        if (inventory.Owned.Count >= allWeapons.Count())
            UnlockAchievement(6);
    }

    public void AllEnemiesKilled()  // 8번 모든적 킬 클리어 업적
    {
        var mm = MissionManager.Instance;
        if (mm != null )// && 남아있는적 = 0
            UnlockAchievement(7);
    }

    public void MeleeAttackKill()   // 10번 근접 공격으로 처치업적
    {
        // 총알이 없을때 근접공격

        UnlockAchievement(9);
    }


    public void OnPlayerDied()  // 11번 죽음 업적
    {
        deathCount++;
        if (deathCount >= 10 && !isUnlocked[10])
            UnlockAchievement(10);
    }
}