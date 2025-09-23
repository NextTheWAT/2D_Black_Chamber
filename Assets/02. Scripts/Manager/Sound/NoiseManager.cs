using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseManager : Singleton<NoiseManager>
{
    [SerializeField] private ShockWave shockWavePrefab;
    [SerializeField] private LayerMask lisenerMask;

    public void EmitNoise(Vector3 position, float noiseRange)
    {
        if(noiseRange <= 0) return;

        // 충격파 오브젝트 생성
        GameObject shockWaveObject = Instantiate(shockWavePrefab.gameObject, position, Quaternion.identity);
        shockWaveObject.transform.localScale = Vector3.one * noiseRange;

        NotifyListeners(position, noiseRange);
    }

    private void NotifyListeners(Vector3 position, float noiseRange)
    {
        float radius = noiseRange * 0.5f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius, lisenerMask);
        foreach (var coll in colliders)
        {
            Enemy enemy = coll.GetComponent<Enemy>();
            enemy.HeardNoise(position);
        }
    }

}
