using System;
using UnityEngine;
using Constants;

[CreateAssetMenu(fileName = "StateTable", menuName = "ScriptableObjects/StateTable")]
public class StateTable : ScriptableObject
{
    public int startStateIndex; // ���� ���� �ε���
    public StateDefinition[] definitions; // ���� ���� �迭
    public NonCombatStateType nonCombatStateType; // ������ ���� ����
    public CombatStateType combatStateType; // ���� ���� ����

    public Type StartStateType => definitions[startStateIndex].StateType;
}
