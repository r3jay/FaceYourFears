using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lichHandCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!gameManager.instance.playerController.stunStatusEffectActive)
        {
            gameManager.instance.playerController.stunTime = GetComponentInParent<lichHandsController>().stunTime;
            gameManager.instance.playerController.stunStatusEffectActive = true;
        }
        GetComponent<Collider>().enabled = false;
    }
}
