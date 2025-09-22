using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DeathStateDefinition", menuName = "ScriptableObjects/StateDefinitions/DeathStateDefinition")]
public class DeathStateDefinition : StateDefinition
{
    public float deathSignalRadius = 5f;
    public LayerMask deathSignalMask;

    public override Type StateType => typeof(DeathState);
    public override IState CreateState(Enemy enemy)
        => new DeathState(enemy, deathSignalRadius, deathSignalMask);
}
