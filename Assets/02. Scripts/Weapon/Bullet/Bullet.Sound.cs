// Bullet.Sound.cs
using UnityEngine;

public partial class Bullet
{
    // Bullet.cs에서 호출됨
    partial void OnBulletObstacleHit(RaycastHit2D hit)
    {
        if (hit.collider == null) return;
        if (StructSoundManager.Instance == null) return;

        int layer = hit.collider.gameObject.layer;
        Vector2 pos = hit.point;

        int wallLayer = LayerMask.NameToLayer("Wall");
        int doorLayer = LayerMask.NameToLayer("Door");
        int steelDoorLayer = LayerMask.NameToLayer("SteelDoor");

        if (layer == wallLayer)
        {
            // 벽 타격
            StructSoundManager.Instance.PlayWallAttackSound(pos);
            return;
        }

        if (layer == doorLayer)
        {
            // 일반 문 타격
            StructSoundManager.Instance.PlayDoorAttackSound(pos);
            return;
        }

        if (layer == steelDoorLayer)
        {
            // 강철문 타격
            StructSoundManager.Instance.PlaySteelDoorAttackSound(pos);
            return;
        }

        // 필요하면 기타 표면 기본 사운드 추가
        // StructSoundManager.Instance.PlayWallAttackSound(pos);
    }
}
