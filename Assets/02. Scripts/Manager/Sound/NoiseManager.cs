using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseManager : Singleton<NoiseManager>
{
    [SerializeField] private ShockWave shockWavePrefab;
    [SerializeField, Range(0, 5)] private float maxScale = 5f; // 최대 크기
    [SerializeField] private LayerMask lisenerMask;

    public void EmitNoise(Vector3 position, float volume)
    {
        if(volume <= 0) return;

        float scale = volume * maxScale; 

        // 충격파 오브젝트 생성
        GameObject shockWaveObject = Instantiate(shockWavePrefab.gameObject, position, Quaternion.identity);
        shockWaveObject.transform.localScale = new Vector3(scale, scale, scale);

        NotifyListeners(position, volume);
    }

    private void NotifyListeners(Vector3 position, float volume)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, volume * maxScale, lisenerMask);
        foreach (var coll in colliders)
        {
            Enemy enemy = coll.GetComponent<Enemy>();
            enemy.HeardNoise(position);
        }
    }

}
