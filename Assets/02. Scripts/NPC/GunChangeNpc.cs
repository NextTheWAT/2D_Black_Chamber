using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunChangeNpc : MonoBehaviour, Iinteraction
{
    public GamePhase CurrentPhase = GamePhase.Stealth;

    public void Interaction(Transform interactor)
    {
        OnPhaseChanged();
    }

    void OnPhaseChanged()
    {
        if(GameManager.Instance.CurrentPhase == GamePhase.Stealth)
        {
            GameManager.Instance.CurrentPhase = GamePhase.Combat;
        }
        else
        {
            GameManager.Instance.CurrentPhase = GamePhase.Stealth;
        }

    }
}

