using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDie : MonoBehaviour
{
    public ExitPortal exitPortal;

    public void Die()
    {
        Debug.Log("������ ���");
        if (exitPortal != null)
            exitPortal.isNpcKilled();
    }
}
