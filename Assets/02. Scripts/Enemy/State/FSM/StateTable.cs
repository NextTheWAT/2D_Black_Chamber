using System;
using UnityEngine;
using Constants;

[CreateAssetMenu(fileName = "StateTable", menuName = "ScriptableObjects/StateTable")]
public class StateTable : ScriptableObject
{
    public StateDefinition[] definitions; // 상태 정의 배열
}
