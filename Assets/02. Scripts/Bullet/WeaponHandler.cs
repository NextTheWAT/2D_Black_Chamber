using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WeaponHandler : MonoBehaviour
{

    [Header("attack")]
    [SerializeField] private float delay = 1f;

    public float Delay {  get => delay; set => delay = value; }

    [SerializeField] private float weaponSize = 1f;

    public float WeaponSize { get => weaponSize; set => weaponSize = value; }

    [SerializeField] private float power = 1f;

    public float Power { get => power; set => power = value; }

    [SerializeField] public float speed = 1f;

    public float Speed { get => speed; set => speed = value; }

    [SerializeField] private float attackRange = 10f;

    public float AttackRange { get => attackRange; set => attackRange = value; }

    public LayerMask target;

    private static readonly int Isattack = Animator.StringToHash("IsAttack");

    public BaseController Contoller { get; private set; }

    private Animator animator;
    private SpriteRenderer weaponRenderer;
}

