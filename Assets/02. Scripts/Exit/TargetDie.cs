using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDie : MonoBehaviour
{
    public ExitPortal exitPortal;

    public void Die()
    {
        Debug.Log("¿¬±¸¿ø »ç¸Á");
        if (exitPortal != null)
            exitPortal.isNpcKilled();
    }
}
