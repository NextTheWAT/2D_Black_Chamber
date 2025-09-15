using UnityEngine;
using Constants;


public class GameManager : Singleton<GameManager>
{
    public Transform player;

    public GamePhase CurrentPhase { get; set; } = GamePhase.Stealth;

    public bool IsCombat
    {
        get => CurrentPhase == GamePhase.Combat;
        set
        {
            CurrentPhase = value ? GamePhase.Combat : GamePhase.Stealth;
        }
    }


    protected override void Initialize()
    {
        base.Initialize();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

}
