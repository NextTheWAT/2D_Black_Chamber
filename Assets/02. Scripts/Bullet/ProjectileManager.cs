using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager instance; 
    public static ProjectileManager Instance  //ΩÃ±€≈Ê
    { 
        get { return instance; } 
    }

    [SerializeField] private ProjectileManager[] projectileManager;
    private void Awake()
    {
        instance = this;
    }

    //public void shootbullet(RangeWeaponHandler rangeweapnGandler, Vector2 startposition, Vector2 direction)
    //{
    //    GameObject origin = ProjectilePrefabs[rangeweapnGandler.BulletIndex];
    //    GameObject obj = Instantiate(origin, startposition, Quaternion.identity);

    //    ProjectileController projectileController = obj.GetComponent<ProjectileController>();
    //    ProjectileController.init(direction, rangeweapnGandler);
    //}

}
