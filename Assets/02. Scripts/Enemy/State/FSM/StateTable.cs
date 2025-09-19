using System;
using UnityEngine;
using Constants;

[CreateAssetMenu(fileName = "StateTable", menuName = "ScriptableObjects/StateTable")]
public class StateTable : ScriptableObject
{
    public int startStateIndex; // 시작 상태 인덱스
    public StateDefinition[] definitions; // 상태 정의 배열
    public NonCombatStateType nonCombatStateType; // 비전투 상태 유형
    public CombatStateType combatStateType; // 전투 상태 유형

    public Type StartStateType => definitions[startStateIndex].StateType;
}
