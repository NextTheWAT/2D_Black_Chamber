using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Investigate,
        Return,
        Attack
    }

    public enum PatrolType
    {
        Waypoint,
        Fixed
    }

    public enum WeaponType
    {
        None,
        Pistol,
        Rifle,
    }

    public static class AnimationHash
    {
        public static readonly int MoveBlend = Animator.StringToHash("MoveBlend");
        public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        public static readonly int Hit = Animator.StringToHash("Hit");
        public static readonly int Die = Animator.StringToHash("Die");
        public static readonly int Use = Animator.StringToHash("Use");
        public static readonly int Shoot = Animator.StringToHash("Shoot");
        public static readonly int Throw = Animator.StringToHash("Throw");
        public static readonly int Punch = Animator.StringToHash("Punch");
        public static readonly int Reload = Animator.StringToHash("Reload");
        public static readonly int Switch = Animator.StringToHash("Switch");
    }

}
