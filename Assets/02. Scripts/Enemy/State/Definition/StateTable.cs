using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StateTable", menuName = "ScriptableObjects/StateTable")]
public class StateTable : ScriptableObject
{
    public int startStateIndex;
    public StateDefinition[] definitions;

    public Type StartStateType => definitions[startStateIndex].StateType;
}
