using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Data/Enemy Data")]
public class EnemySheetData : ScriptableObject
{
    public string enemyName;
    public int hp;
    public float speed;
    public float viewDistance;
    public float viewAngle;
    public int equipWepaon;
    public float attackRange;
    public int patrolType;
    public float patrolPauseTime;
    public float FixedPatrolAngle;
    public float suspectTime;
    public float investigateDuration;
    public float investigateRange;
}
