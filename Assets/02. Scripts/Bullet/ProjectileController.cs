using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private LayerMask levelcollisionLayer;

    private RangeWeaponHandler rangeWeaponHandler;

    private float currentDuraction;
    private Vector2 direction;
    private bool isReady;
    private Transform pivot;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer spriteRenderer;

    public bool fxOnDestroy = true;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        if (!isReady) return;

        currentDuraction = Time.deltaTime;

        if(currentDuraction > rangeWeaponHandler.Duration)
        {
            DestroyProjectile(transform.position, false);
        }

        _rigidbody.velocity = direction * rangeWeaponHandler.speed;   //Åº¼Ó
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (levelcollisionLayer.value == (levelcollisionLayer.value | (1 << collision.gameObject.layer)))
        {
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * .2f, fxOnDestroy);
        }
        else if (rangeWeaponHandler.target.value == (rangeWeaponHandler.target.value | (-1 << collision.gameObject.layer)))
        {

        }
          
    }
    public void init(Vector2 direction, RangeWeaponHandler weaponHandler)
    {
        rangeWeaponHandler = weaponHandler;

        this.direction = direction;
        currentDuraction = 0;
       // transform.localScale = Vector3.one * weaponHandler.BulletSize;   //Åº Å©±â Á¶Àý

        transform.right = this.direction;

        if(direction.x < 0)
            pivot.localRotation = Quaternion.Euler(180, 0 ,0);
        else
            pivot.localRotation = Quaternion.Euler(0, 0, 0);

        isReady = true;
    }

    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        Destroy(this.gameObject);
    }
}


