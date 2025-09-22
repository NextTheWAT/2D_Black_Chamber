using System;
using UnityEngine;
using Constants;

[CreateAssetMenu(fileName = "StateTable", menuName = "ScriptableObjects/StateTable")]
public class StateTable : ScriptableObject
{
    public StateDefinition[] definitions; // ���� ���� �迭
}
