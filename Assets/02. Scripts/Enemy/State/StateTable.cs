using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using System;

[CreateAssetMenu(fileName = "StateTable", menuName = "ScriptableObjects/StateTable")]
public class StateTable : ScriptableObject
{
    public int startStateIndex;
    public StateType[] stateTypes;
}
