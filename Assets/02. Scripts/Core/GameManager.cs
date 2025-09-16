using UnityEngine;
using Constants;


public class GameManager : Singleton<GameManager>
{
    public Transform player;

    public GamePhase CurrentPhase { get; set; } = GamePhase.Stealth;

    public event System.Action<GamePhase> OnPhaseChanged;

    public bool IsCombat
    {
        get => CurrentPhase == GamePhase.Combat;
        set
        {
            GamePhase next = value ? GamePhase.Combat : GamePhase.Stealth;
            if (CurrentPhase != next)
            {
                CurrentPhase = next;
                OnPhaseChanged?.Invoke(CurrentPhase); //�� UI ���� �̺�Ʈ ����
            }
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

}
